using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using depi__project.enums;
using depi__project.Models;
using depi__project.services.interfaces;
using depi__project.viewmodels.General;
using depi__project.viewmodels.userviewmodel;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;

namespace depi__project.services.reporesity
{
    public class User:IUser
    {
        public User(Context context,
            UserManager<Aplicationuser> userManager,
            IMapper mapper,
            SignInManager<Aplicationuser> signInManager
             )
        {
            Context = context;
            UserManager = userManager;
            Mapper = mapper;
            SignInManager = signInManager;
         }
        public Context Context { get; }
        public UserManager<Aplicationuser> UserManager { get; }
        public IMapper Mapper { get; }
        public SignInManager<Aplicationuser> SignInManager { get; }
 
        //create admin
        public async Task<ResponseStatus<responseadminviewmodel>> addadmin(adminviewmodel vm)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                Admin admin=new Admin();
                ResponseStatus<responseadminviewmodel> response = new ResponseStatus<responseadminviewmodel>();
                //check already email exit or not
                bool checkresult = await emailexit(vm.Email);
                if (checkresult) 
                {
                    response.Success = false;
                    response.Message = "this email is already exit";
                    response.Errors.Add(response.Message);
                    return response;
                }
                //create user
                var user= Mapper.Map<Aplicationuser>(vm);
                user.IsActive=true; 
                var result =await UserManager.CreateAsync(user,vm.Password);
                if (!result.Succeeded)
                {
                    response.Success = false;
                    response.Message = "create user is not successed";
                    response.Errors.AddRange(result.Errors.Select(e=>e.Description));
                    return response;
                }
                else if (result.Succeeded)
                {
                    //add admin
                     admin =Mapper.Map<Admin>(vm);
                    admin.userid = user.Id;
                     admin.status = userstatus.active;
                    await Context.Admins.AddAsync(admin);
                }
                //create cookie
                var roleResult=await UserManager.AddToRoleAsync(user, "Admin");
                if (!roleResult.Succeeded)
                {
                    response.Success = false;
                    response.Message = "roleresult is failed";
                    response.Errors.AddRange(roleResult.Errors.Select(e => e.Description));
                    return response;
                }
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                 var dataresponse = Mapper.Map<responseadminviewmodel>(admin);
                response.Data = dataresponse;
                return response;
            }
            catch (Exception ex) 
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        //update admin
        public async Task<ResponseStatus<responseadminviewmodel>> updateadmin(updateadminviewmodel vm)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();

            var response = new ResponseStatus<responseadminviewmodel>();
            try
            {
                var user = await Context.Users.FindAsync(vm.userid);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = $"this user:{vm.UserName} is not found";
                    response.Errors.Add(response.Message);
                    return response;
                }
                var admin = await Context.Admins.Include(a => a.user)
                    .FirstOrDefaultAsync(a => a.userid == vm.userid);
                if (admin == null)
                {
                    response.Success = false;
                    response.Message = $"this admin:{vm.UserName} is not found";
                    response.Errors.Add(response.Message);
                    return response;
                }

                //update user
                updateuser(vm,user);
                //update admin
                if (!string.IsNullOrEmpty(vm.permissions)) admin.permissions = vm.permissions;
                admin.status = userstatus.modeficated;

                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                response.Data = Mapper.Map<responseadminviewmodel>(admin);
                response.Message = "updated admin was successfully";
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                return response;
            }
        }
        // get admin by id
        public async Task<ResponseStatus<responseadminviewmodel>> getadminbyid(int adminid)
        {
            var response = new ResponseStatus<responseadminviewmodel>();
            try
            {
                var admin = await Context.Admins.Include(a => a.user)
                    .FirstOrDefaultAsync(a => a.Adminid == adminid);

                if (admin == null)
                {
                    response.Success = false;
                    response.Message = "admin is not found";
                    response.Errors.Add(response.Message);
                    return response;
                }

                response.Data = Mapper.Map<responseadminviewmodel>(admin);
                response.Success = true;
                response.Message = "get admin is successed";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        // get all admins with pagination
        public async Task<ResponseStatus<IEnumerable<responseadminviewmodel>>> getalladmins( pagination pg , string searchterm = null )
        {
            var response = new ResponseStatus<IEnumerable<responseadminviewmodel>>();
            try
            {
                IQueryable<Admin> query = Context.Admins.Include(a => a.user);

 

                if (!string.IsNullOrEmpty(searchterm))
                {
                    searchterm = searchterm.ToLower();
                    query = query.Where(t => t.user.UserName.ToLower().Contains(searchterm) ||
                        t.permissions.ToLower().Contains(searchterm) ||
                        t.user.address.ToLower().Contains(searchterm) ||
                        t.user.Email.ToLower() == searchterm ||
                        t.user.PhoneNumber == searchterm||
                        t.user.IsActive.ToString().ToLower()==searchterm||t.status.ToString().ToLower().Contains(searchterm));
                }

                query = query.OrderByDescending(t => t.user.createdat)
                             .Skip((pg.PageNumber - 1) * pg.PageSize)
                             .Take(pg.PageSize);

                var admins = await query.ToListAsync();
                response.Data = Mapper.Map<IEnumerable<responseadminviewmodel>>(admins);
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                return response;
            }
        }
        //getallactiveadmins
        public async Task<ResponseStatus<IEnumerable<responseadminviewmodel>>> getallactiveadmins()
        {
            var response = new ResponseStatus<IEnumerable<responseadminviewmodel>>();
            try
            {
                IQueryable<Admin> query = Context.Admins.
                    Include(a => a.user)
                    .Where(a=>a.status!=userstatus.inactive&&a.user.IsActive==true);

                var admins = await query.ToListAsync();
                response.Data = Mapper.Map<IEnumerable<responseadminviewmodel>>(admins);
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        // remove admin (soft delete)
        public async Task<ResponseStatus<bool>> removeadmin(string id)
        {
            var response = new ResponseStatus<bool>();
            try
            {
                var admin = await Context.Admins.FirstOrDefaultAsync(a => a.userid == id);
                if (admin == null)
                {
                    response.Success = false;
                    response.Message = "admin is not found";
                    response.Errors.Add(response.Message);
                    response.Data = false;
                    return response;
                }

                admin.status = userstatus.inactive;

                var user = await Context.Users.FindAsync(id);
                if (user != null)
                {
                    user.IsActive = false;
                    user.updatedat = DateTime.UtcNow;
                }
                await Context.SaveChangesAsync();
                response.Data = true;
                response.Message = "admin removed successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                response.Data = false;
                return response;
            }
        }
        // get admin count
        public async Task<ResponseStatus<int>> getadmincount()
        {
            var response = new ResponseStatus<int>();
            try
            {
                response.Data = await Context.Admins.CountAsync(a => a.status != userstatus.active);
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                return response;
            }
        }
        //getadminbyuserid
        public async Task<ResponseStatus<responseadminviewmodel>> getadminbyuserid(string userid)
        {
            var response = new ResponseStatus<responseadminviewmodel>();
            try
            {
                var admin = await Context.Admins.Include(a => a.user)
                    .FirstOrDefaultAsync(a => a.userid == userid && a.status!=userstatus.inactive);

                if (admin == null)
                {
                    response.Success = false;
                    response.Message = "admin is not found";
                    response.Errors.Add(response.Message);
                    return response;
                }

                response.Data = Mapper.Map<responseadminviewmodel>(admin);
                response.Success = true;
                response.Message = "get admin is successed";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                return response;
            }
        }
        // ===================== DOCTOR =====================
        //getdoctorcount
        public async Task<ResponseStatus<int>> getadoctorcount()
        {
            var response = new ResponseStatus<int>();
            try
            {
                response.Data = await Context.Doctors.CountAsync(a => a.status != userstatus.active);
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                return response;
            }
        }
        // create doctor
        public async Task<ResponseStatus<doctorresponseviewmodel>> adddoctor(doctorviewmodel vm)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            var response = new ResponseStatus<doctorresponseviewmodel>();
            try
            {
                if (await emailexit(vm.Email))
                {
                    response.Success = false;
                    response.Message = "this email is already exit";
                    response.Errors.Add(response.Message);
                    return response;
                }

                var user = Mapper.Map<Aplicationuser>(vm);
                user.IsActive = true;

                var result = await UserManager.CreateAsync(user, vm.Password);
                if (!result.Succeeded)
                {
                    response.Success = false;
                    response.Message = "create user is not successed";
                    response.Errors.AddRange(result.Errors.Select(e => e.Description));
                    return response;
                }
                //check exit department or not 
                var dept = await Context.Departments.FindAsync(vm.DepartmentId);
                if (dept == null)
                {
                    response.Success = false;
                    response.Message = "this department is not found";
                    response.Errors.Add(response.Message);
                    return response;
                }
                var doctor = Mapper.Map<Doctor>(vm);
                doctor.userid = user.Id;
                doctor.DepartmentId = vm.DepartmentId;
                doctor.status = userstatus.active;

                await Context.Doctors.AddAsync(doctor);

                var roleResult = await UserManager.AddToRoleAsync(user, "Doctor");
                if (!roleResult.Succeeded)
                {
                    response.Success = false;
                    response.Message = "roleresult is failed";
                    response.Errors.AddRange(roleResult.Errors.Select(e => e.Description));
                    return response;
                }

                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
 
                response.Data = Mapper.Map<doctorresponseviewmodel>(doctor);
                response.Message = "doctor created successfully";
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                return response;
            }
        }
        //update doctor
        public async Task<ResponseStatus<doctorresponseviewmodel>> updatedoctor(updatedoctorviewmodel vm)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();

            var response =new ResponseStatus<doctorresponseviewmodel>(); 
            try
            {
                //check user
                var user = await Context.Users.FindAsync(vm.userid);
                if (user == null) 
                {
                    response.Success = false;
                    response.Message = $"this user:{vm.userid} is not found";
                    response.Errors.Add(response.Message);
                    return response;
                }
                //check doctor is exit or not
                var doctor = await Context.Doctors.
                    Include(d=>d.Department)
                    .Include(d=>d.user).
                    FirstOrDefaultAsync(d=>d.DoctorId==vm.DoctorId&&d.userid==vm.userid);
                if (doctor == null) 
                {
                   response.Success=false;
                    response.Message = $"this doctor:{vm.DoctorId} is not found";
                    response.Errors.Add(response.Message);
                    return response;
                }
                //update user 
                updateuser(vm,user);
                //update doctor
                var dept = await Context.Departments.FirstOrDefaultAsync(d=>d.DepartmentId==vm.DepartmentId);
                if (dept == null)
                {
                    response.Success = false;
                    response.Message = $"this department:{vm.DepartmentId} is not found";
                    response.Errors.Add(response.Message);
                    return response;
                }
                doctor.hiredate = vm.hiredate;
                doctor.status = userstatus.modeficated;
                doctor.salary = vm.salary;
                doctor.Specialization = vm.Specialization;
                doctor.DepartmentId = vm.DepartmentId;
                //save in database
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
               response.Success = true;
                response.Data = Mapper.Map<doctorresponseviewmodel>(doctor);
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                return response;
            }
        }
        //getdoctorbyid
        public async Task<ResponseStatus<doctorresponseviewmodel>> getdoctorbyid(int doctorid)
        {
            var response = new ResponseStatus<doctorresponseviewmodel>();
            try
            {
                //check doctor
                var doctor = await Context.Doctors
                    .Include(d=>d.user)
                    .FirstOrDefaultAsync(d=>d.DoctorId==doctorid);
                if (doctor==null)
                {
                    response.Success = false;
                    response.Message = "doctor is not found";
                    response.Errors.Add(response.Message);
                    return response;
                }
                response.Success= true;
                response.Data=Mapper.Map<doctorresponseviewmodel>(doctor);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                return response;
            }
        }
        //getalldoctors
        public async Task<ResponseStatus<IEnumerable<doctorresponseviewmodel>>> getalldoctors(pagination pg, string searchterm = null)
        {
            var response = new ResponseStatus<IEnumerable<doctorresponseviewmodel>>();
            try
            {
               
                IQueryable<Doctor> query = Context.Doctors.
                    Include(a => a.user).
                    Include(a=>a.Department);
                 if (!string.IsNullOrEmpty(searchterm))
                {
                    searchterm = searchterm.ToLower();
                    query = query.Where(t => t.user.UserName.ToLower().Contains(searchterm) ||
                        t.Specialization.ToLower().Contains(searchterm) ||
                        t.user.address.ToLower().Contains(searchterm) ||
                        t.user.Email.ToLower() == searchterm ||
                        t.user.PhoneNumber == searchterm||
                        t.Department.Name.ToLower().Contains(searchterm)||
                        t.status.ToString().ToLower().Contains(searchterm)
                        ||t.user.IsActive.ToString().ToLower()==searchterm 
                        );
                }

                query = query.OrderByDescending(t => t.user.createdat)
                             .Skip((pg.PageNumber - 1) * pg.PageSize)
                             .Take(pg.PageSize);

                var doctors = await query.ToListAsync();
 
                response.Data = Mapper.Map<IEnumerable<doctorresponseviewmodel>>(doctors);
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                return response;
            }
        }
        //removedoctor
        public async Task<ResponseStatus<bool>> removedoctor(string id)
        {
            var response = new ResponseStatus<bool>();
            try
            {
                var doctor = await Context.Doctors.FirstOrDefaultAsync(a => a.userid == id);
                if (doctor == null)
                {
                    response.Success = false;
                    response.Message = "doctor is not found";
                    response.Errors.Add(response.Message);
                    response.Data = false;
                    return response;
                }

                doctor.status = userstatus.inactive;

                var user = await Context.Users.FindAsync(id);
                if (user != null)
                {
                    user.IsActive = false;
                    
                    user.updatedat = DateTime.UtcNow;
                }
                await Context.SaveChangesAsync();
                response.Data = true;
                response.Message = "doctor removed successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                response.Data = false;
                return response;
            }
        }

        public async Task<IEnumerable<Doctor>> getdoctors()
        {
            return await Context.Doctors.Include(d=>d.user).ToListAsync();
        }
        // ===================== RECEPTIONIST =====================
        // create receptionist
        public async Task<ResponseStatus<responsereceptionistviewmodel>> addresceptionist(resceptionistviewmodel vm)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            var response = new ResponseStatus<responsereceptionistviewmodel>();
            try
            {
                if (await emailexit(vm.Email))
                {
                    response.Success = false;
                    response.Message = "this email is already exit";
                    response.Errors.Add(response.Message);
                    return response;
                }

                var user = Mapper.Map<Aplicationuser>(vm);
                user.IsActive = true;

                var result = await UserManager.CreateAsync(user, vm.Password);
                if (!result.Succeeded)
                {
                    response.Success = false;
                    response.Message = "create user is not successed";
                    response.Errors.AddRange(result.Errors.Select(e => e.Description));
                    return response;
                }
                var resptionist = Mapper.Map<resptionist>(vm);
                resptionist.userid = user.Id;
                resptionist.status = userstatus.active;
                await Context.resptionists.AddAsync(resptionist);
                var roleResult = await UserManager.AddToRoleAsync(user, "Receptionist");
                if (!roleResult.Succeeded)
                {
                    response.Success = false;
                    response.Message = "roleresult is failed";
                    response.Errors.AddRange(roleResult.Errors.Select(e => e.Description));
                    return response;
                }

                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                                   
                response.Data = Mapper.Map<responsereceptionistviewmodel>(resptionist);
                response.Message = "receptionist created successfully";
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                return response;
            }
        }
        //update receptionist
        public async Task<ResponseStatus<responsereceptionistviewmodel>> updatereceptionist(updatereceptionistviewmodel vm)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();

            var response =new ResponseStatus<responsereceptionistviewmodel>();
            try
            {
                //check user
                var user = await Context.Users.FindAsync(vm.userid);
                if (user == null) 
                {
                    response.Success = false;
                    response.Message = $"this user id:{vm.userid} is not found";
                    response.Errors.Add(response.Message);
                    return response;
                }
                //update user
                updateuser(vm,user);
                //check receptionist exit or not
                var receptionist = await Context.resptionists.
                    Include(r=>r.user)
                    .FirstOrDefaultAsync(r=>r.userid==vm.userid);
                if (receptionist == null)
                {
                    response.Success=false;
                    response.Message = $"this receptionise employee:{vm.receptionid} is not found";
                    response.Errors.Add(response.Message);
                    return response;
                }
                //update receptionist
                receptionist.hiredate = vm.hiredate;
                receptionist.salary = vm.salary;
                receptionist.status = userstatus.modeficated;
                receptionist.user.updatedat = DateTime.UtcNow;
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
                //return success

                response.Success = true;
                response.Message = "updated receptionist is successfully";
                response.Data = Mapper.Map<responsereceptionistviewmodel>(receptionist);
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(response.Message);
                return response;
            }
        }
        //remove receptionist
        public async Task<ResponseStatus<bool>> removereceptionist(string userid)
        {
            var response=new ResponseStatus<bool>();
            try
            {
                //check user 
                var user=await Context.Users.FindAsync(userid);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = $"this user id:{userid} is not found";
                    response.Errors.Add(response.Message);
                    return response;
                }
                //update user
                user.IsActive = false;
                user.updatedat = DateTime.UtcNow;
                //receptionist
                var receptionist = await Context.resptionists.
                    Include(r=>r.user).
                    FirstOrDefaultAsync(r=>r.userid==userid);
                if (receptionist!=null)
                {
                    receptionist.status = userstatus.inactive;
                }
                await Context.SaveChangesAsync();
                response.Success = true;
                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(response.Message);
                return response;
            }
        }
        //getallreceptionist
        public async Task<ResponseStatus<IEnumerable<responsereceptionistviewmodel>>> getallreceptionists(pagination pg, string searchterm = null)
        {
            var response = new ResponseStatus<IEnumerable<responsereceptionistviewmodel>>();
            try
            {
                IQueryable<resptionist> query = Context.resptionists.
                    Include(a => a.user)
                   ;

                if (!string.IsNullOrEmpty(searchterm))
                {
                    searchterm = searchterm.ToLower();
                    query = query.Where(t => t.user.UserName.ToLower().Contains(searchterm) ||
                        t.user.address.ToLower().Contains(searchterm) ||
                        t.user.Email.ToLower() == searchterm ||
                        t.user.PhoneNumber == searchterm ||
                        t.status.ToString().ToLower().Contains(searchterm)
                        ||t.user.IsActive.ToString().ToLower()==searchterm);
                }

                query = query.OrderByDescending(t => t.hiredate).OrderByDescending(t=>t.salary)
                             .Skip((pg.PageNumber - 1) * pg.PageSize)
                             .Take(pg.PageSize);

                var receptionists = await query.ToListAsync();
                response.Data = Mapper.Map<IEnumerable<responsereceptionistviewmodel>>(receptionists);
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
                return response;
            }
        }
        public async Task<ResponseStatus<responsereceptionistviewmodel>> getreceptionistbyid(int id)
        {
            var response = new ResponseStatus<responsereceptionistviewmodel>();
            try
            {
                var receptionist = await Context.resptionists.
                    Include(r=>r.user).FirstOrDefaultAsync(r=>r.resptionistid==id);
                if (receptionist == null)
                {
                    response.Success = false;
                    response.Message = $"this receptionist id:{id} is not found";
                    response.Errors.Add(response.Message);
                    return response;
                }
                response.Success = true;
                response.Data = Mapper.Map<responsereceptionistviewmodel>(receptionist);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(response.Message);
                return response;
            }
        }
        public async Task<IEnumerable<resptionist>> getreceptionists()
        {
            return await Context.resptionists.Include(r=>r.user).ToListAsync();
        }
        //update user
        private void updateuser(updateuserviewmodel vm,Aplicationuser user)
        {
            if (user==null||vm==null)
            {
                throw new Exception("this data invalid");
            }
            if (!string.IsNullOrEmpty(vm.address)) user.address = vm.address;
            if (vm.Gender.HasValue) user.Gender = vm.Gender.Value;
            if (!string.IsNullOrEmpty(vm.UserName)) user.UserName = vm.UserName;
            if (!string.IsNullOrEmpty(vm.Email)) user.Email = vm.Email;
            if (!string.IsNullOrEmpty(vm.PhoneNumber)) user.PhoneNumber = vm.PhoneNumber;
            if (vm.IsActive!=null) user.IsActive = vm.IsActive;
            user.updatedat = DateTime.UtcNow;
        }
        private async Task<bool> emailexit(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            return user != null;
        }

    }
}
