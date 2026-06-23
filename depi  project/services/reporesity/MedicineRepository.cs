using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using depi__project.Models;
using depi__project.services.interfaces;
using depi__project.viewmodels.medicine;
using depi__project.viewmodels.General;

namespace depi__project.services.reporesity
{
    public class MedicineRepository : IMedicineRepository
    {
        public MedicineRepository(Context context, IMapper mapper)
        {
            Context = context;
            Mapper  = mapper;
        }

        public Context Context { get; }
        public IMapper Mapper  { get; }

        public async Task<ResponseStatus<IEnumerable<ResponseMedicineVM>>> GetAllAsync(
            pagination pg, bool? isDeleted = null, string name = null, int? cat_id = null)
        {
            var response = new ResponseStatus<IEnumerable<ResponseMedicineVM>>();
            try
            {
                IQueryable<Medicine> query = Context.Medicines
                    .AsNoTracking()
                    .Include(m => m.category)
                    .Include(m => m.user);

                if (isDeleted.HasValue)
                    query = query.Where(m => m.IsDeleted == isDeleted.Value);

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(m => m.Name.Contains(name));

                if (cat_id.HasValue)
                    query = query.Where(m => m.cat_id == cat_id.Value);

                query = query
                    .OrderByDescending(m => m.medicineId)
                    .Skip((pg.PageNumber - 1) * pg.PageSize)
                    .Take(pg.PageSize);

                var medicines = await query.ToListAsync();

                response.Success = true;
                response.Data    = Mapper.Map<IEnumerable<ResponseMedicineVM>>(medicines);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Something went wrong";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponseMedicineVM>> GetByIdAsync(int id)
        {
            var response = new ResponseStatus<ResponseMedicineVM>();
            try
            {
                var medicine = await Context.Medicines
                    .Include(m => m.category)
                    .Include(m => m.user)
                    .FirstOrDefaultAsync(m => m.medicineId == id);

                if (medicine == null)
                {
                    response.Success = false;
                    response.Message = "Medicine not found";
                    return response;
                }

                response.Success = true;
                response.Data    = Mapper.Map<ResponseMedicineVM>(medicine);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Something went wrong";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponseMedicineVM>> AddAsync(AddMedicineVM vm)
        {
            var response = new ResponseStatus<ResponseMedicineVM>(); 
            try
            {
                var exists = await Context.Medicines
                    .AnyAsync(m => m.Name.ToLower() == vm.Name.ToLower() && !m.IsDeleted);

                if (exists)
                {
                    response.Success = false;
                    response.Message = "Medicine with this name already exists";
                    response.Errors.Add(response.Message);
                    return response;
                }

                var medicine = Mapper.Map<Medicine>(vm);
                medicine.CreatedAt = DateTime.Now;
                medicine.IsDeleted = false;
                medicine.AddedBy = await Context.Users
                                            .Where(u => u.Id == medicine.user_id)
                                            .Select(u => u.UserName)
                                            .FirstOrDefaultAsync() ?? "Admin";
                medicine.CategoryName = await Context.categories
                                            .Where(c => c.cat_id == medicine.cat_id)
                                            .Select(c => c.cat_name)
                                            .FirstOrDefaultAsync() ?? "";

                if (string.IsNullOrWhiteSpace(medicine.user_id))
                {
                    medicine.user_id = await Context.Users
                        .Select(u => u.Id)
                        .FirstOrDefaultAsync();

                    if (string.IsNullOrWhiteSpace(medicine.user_id))
                    {
                        response.Success = false;
                        response.Message = "Medicine must be linked to an existing user.";
                        response.Errors.Add(response.Message);
                        return response;
                    }
                }

                await Context.Medicines.AddAsync(medicine);
                await Context.SaveChangesAsync();

                await Context.Entry(medicine).Reference(m => m.category).LoadAsync();

                response.Success = true;
                response.Message = "Medicine added successfully";
                response.Data = Mapper.Map<ResponseMedicineVM>(medicine);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Something went wrong";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ResponseStatus<ResponseMedicineVM>> UpdateAsync(UpdateMedicineVM vm)
        {
            var response = new ResponseStatus<ResponseMedicineVM>();
            try
            {
                var existing = await Context.Medicines.FindAsync(vm.medicineId);
                if (existing == null)
                {
                    response.Success = false;
                    response.Message = "Medicine not found";
                    return response;
                }

                existing.Name          = vm.Name;
                existing.StockQuantity = vm.StockQuantity;
                existing.SupplierName  = vm.SupplierName;
                existing.UnitPrice     = vm.UnitPrice;
                existing.RecordLevel   = vm.RecordLevel;
                existing.ExpiryDate    = vm.ExpiryDate;
                existing.cat_id        = vm.cat_id;
                existing.IsDeleted     = vm.IsDeleted;

                await Context.SaveChangesAsync();

                await Context.Entry(existing).Reference(m => m.category).LoadAsync();

                response.Success = true;
                response.Message = "Medicine updated successfully";
                response.Data    = Mapper.Map<ResponseMedicineVM>(existing);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Something went wrong";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetCategoriesSelectListAsync()
        {
            return await Context.categories
                .Where(c => c.isactive)
                .Select(c => new SelectListItem
                {
                    Value = c.cat_id.ToString(),
                    Text  = c.cat_name
                })
                .ToListAsync();
        }
    }
}
