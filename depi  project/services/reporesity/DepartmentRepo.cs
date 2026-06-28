using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using depi__project.Models;
using depi__project.services.interfaces;
using depi__project.viewmodels.departmentvm;
using depi__project.viewmodels.General;
using System.Threading.Tasks;

namespace depi__project.services.reporesity
{
    public class DepartmentRepo:IDepartment
    {
        public DepartmentRepo(Context context,IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }
        public Context Context { get; }
        public IMapper Mapper { get; }

        public async Task<List<Department>> getalldept()
        {
            return await Context.Departments.Where(d=>d.isactive==true).ToListAsync();
        }
        public async Task<ResponseStatus<IEnumerable<ResponseDeptVm>>> GetAllAsync(pagination pg, string phone = null, bool? isactive = null, string name = null, int? departmentid = null)
        {
            var response = new ResponseStatus<IEnumerable<ResponseDeptVm>>();
            try
            {
                IQueryable<Department> query = Context.Departments.AsNoTracking()
                    .Include(d => d.user)
                    .Include(d => d.doctors);

                if (!string.IsNullOrEmpty(phone))
                    query = query.Where(d => d.Phone.Contains(phone));

                if (isactive.HasValue)
                    query = query.Where(d => d.isactive == isactive.Value);

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(d => d.Name.Contains(name));

                if (departmentid.HasValue)
                    query = query.Where(d => d.DepartmentId == departmentid.Value);

                query = query.OrderByDescending(t => t.createdat)
                                         .Skip((pg.PageNumber - 1) * pg.PageSize)
                                         .Take(pg.PageSize);
                var departments = await query.ToListAsync();
                response.Success = true;
                response.Data = Mapper.Map<IEnumerable<ResponseDeptVm>>(departments);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "exit wrong";
                response.Errors.Add(response.Message);
                return response;
            }
        }      
        public async Task<ResponseStatus<IEnumerable<ResponseDeptVm>>> GetActiveDepartmentsAsync()
        {
            var response = new ResponseStatus<IEnumerable<ResponseDeptVm>>();
            try 
            {
                var depts= await Context.Departments.
                    Include(d=>d.user).
                    Include(d=>d.doctors)
                .Where(d => d.isactive==true)
                .ToListAsync();

                response.Success = true;
                response.Data = Mapper.Map<IEnumerable<ResponseDeptVm>>(depts);
                return response;
            }
            catch (Exception ex) 
            {
                response.Success = false;
                response.Message = "exit wrong";
                response.Errors.Add(response.Message);
                return response;
            }
        
        }

        public async Task<ResponseStatus<ResponseDeptVm>> GetByIdAsync(int id)
        {
            var response = new ResponseStatus<ResponseDeptVm>();
            try
            {
                var department= await Context.Departments
            .Include(d => d.user)
            .Include(d=>d.doctors)
            .FirstOrDefaultAsync(d => d.DepartmentId == id);
                if (department == null)
                {
                    response.Success = false;
                    response.Message = "Not Found";
                    return response;
                }
                response.Success = true;
                response.Data = Mapper.Map<ResponseDeptVm>(department);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "exit wrong";
                response.Errors.Add(response.Message);
                return response;
            }
       
        }

        public async Task<ResponseStatus<ResponseDeptVm>> GetByIdWithDoctorsAsync(int id)
        {
            var response=new ResponseStatus<ResponseDeptVm>();
            try
            {
                var department= await Context.Departments
                .Include(d => d.doctors)
                    .ThenInclude(doc => doc.user)
                .Include(d => d.user)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);
                if (department == null)
                {
                    response.Success = false;
                    response.Message = "Not Found";
                    return response;
                }
                response.Success = true;
                response.Data = Mapper.Map<ResponseDeptVm>(department);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "exit wrong";
                response.Errors.Add(response.Message);
                return response;
            }

        }

        public async Task<ResponseStatus<ResponseDeptVm>> AddAsync(AddDeptVm vm)
        {
            var response= new ResponseStatus<ResponseDeptVm>();
            try
            {
                var department = Mapper.Map<Department>(vm);
                if (department==null)
                {
                    response.Success=false;
                    response.Message ="failed in mapping";
                    response.Errors.Add(response.Message);
                    return response;
                }
                if (string.IsNullOrWhiteSpace(department.userid))
                {
                    department.userid = await Context.Users.Select(u => u.Id).FirstOrDefaultAsync();
                    if (string.IsNullOrWhiteSpace(department.userid))
                    {
                        response.Success = false;
                        response.Message = "Department must be linked to an existing user.";
                        response.Errors.Add(response.Message);
                        return response;
                    }
                }
                await Context.AddAsync(department);
                await Context.SaveChangesAsync();
                response.Success = true;
                response.Data = Mapper.Map<ResponseDeptVm>(department);
                return response;
            }
            catch (Exception ex) 
            {
                response.Success = false;
                response.Message = "exit wrong";
                response.Errors.Add(response.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponseDeptVm>> UpdateAsync(UpdateDertVm vm)
        {
            var response =new ResponseStatus<ResponseDeptVm>();
            try
            {
                //check department
                var existing = await Context.Departments.FindAsync(vm.DepartmentId);
                if (existing != null)
                {
                    existing.Name = vm.Name;
                    existing.FloorNumber = vm.FloorNumber;
                    existing.Phone = vm.Phone;
                    existing.description = vm.description;
                    existing.isactive = vm.isactive;
                    existing.updatedat = DateTime.Now;
                    await Context.SaveChangesAsync();
                    response.Success = true;
                    response.Data = Mapper.Map<ResponseDeptVm>(existing);
                    return response;
                }
                response.Success= false;
                response.Message = "not found";
                response.Errors.Add(response.Message);
                return response;
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "exit wrong";
                response.Errors.Add(response.Message);
                return response;
            }
         
            }

        public async Task<ResponseStatus<bool>> DeleteAsync(int id)
        {
            var response=new ResponseStatus<bool>();
            var department = await Context.Departments.FindAsync(id);
            if (department == null)
            {
                response.Success = false;
                response.Message ="not found";
                response.Errors.Add(response.Message);
                response.Data = false;
                return response;    
            }
            department.isactive = false;
            department.updatedat = DateTime.UtcNow;
            await Context.SaveChangesAsync();
            response.Success= true;
            response.Data = true;
            return response;
        }
        public async Task<ResponseStatus<bool>> ExistsAsync(int id)
        {
            var response = new ResponseStatus<bool>();
            try
            {
                var department = await Context.Departments.AnyAsync(e => e.DepartmentId == id&&e.isactive==true);
                if (department)
                {
                    response.Success = true;
                    response.Data= true;
                    return response;
                }
                response.Success= false;
                response.Message = "not found";
                response.Errors.Add(response.Message);
                response.Data = false;
                return response;
            }
            catch (Exception ex) {
                response.Success = false;
                response.Message = "exit wrong";
                response.Errors.Add(response.Message);
                response.Data = false;
                return response;
            }
        }
        public async Task<IEnumerable<Department>> SearchAsync(string term)
        {
            return await Context.Departments
                .Where(d => d.Name.Contains(term) || d.description.Contains(term) || d.Phone.Contains(term))
                .ToListAsync();
        }
        public async Task<int> GetCountAsync()
        {
            return await Context.Departments.CountAsync();
        }
    }
}
