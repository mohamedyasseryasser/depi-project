using AutoMapper;
using Microsoft.EntityFrameworkCore;
using depi__project.Models;
using depi__project.services.interfaces;
using depi__project.viewmodels.category;
using depi__project.viewmodels.General;

namespace depi__project.services.reporesity
{
    public class CategoryRepository : ICategoryRepository
    {
        public CategoryRepository(Context context, IMapper mapper)
        {
            Context = context;
            Mapper  = mapper;
        }

        public Context Context { get; }
        public IMapper Mapper  { get; }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await Context.categories
                .Where(c => c.isactive)
                .ToListAsync();
        }

        public async Task<ResponseStatus<IEnumerable<ResponseCategoryVM>>> GetAllAsync(
            pagination pg, bool? isactive = null, string name = null)
        {
            var response = new ResponseStatus<IEnumerable<ResponseCategoryVM>>();
            try
            {
                IQueryable<Category> query = Context.categories
                    .AsNoTracking()
                    .Include(c => c.user)
                    .Include(c => c.medicines);

                if (isactive.HasValue)
                    query = query.Where(c => c.isactive == isactive.Value);

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(c => c.cat_name.Contains(name));

                query = query
                    .OrderByDescending(m => m.cat_id)
                    .Skip((pg.PageNumber - 1) * pg.PageSize)
                    .Take(pg.PageSize);

                var categories = await query.ToListAsync();

                response.Success = true;
                response.Data    = Mapper.Map<IEnumerable<ResponseCategoryVM>>(categories);
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

        public async Task<ResponseStatus<ResponseCategoryVM>> GetByIdAsync(int id)
        {
            var response = new ResponseStatus<ResponseCategoryVM>();
            try
            {
                var category = await Context.categories
                    .Include(c => c.user)
                    .Include(c => c.medicines)
                    .FirstOrDefaultAsync(c => c.cat_id == id);

                if (category == null)
                {
                    response.Success = false;
                    response.Message = "Category not found";
                    return response;
                }

                response.Success = true;
                response.Data    = Mapper.Map<ResponseCategoryVM>(category);
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

        public async Task<ResponseStatus<ResponseCategoryVM>> AddAsync(AddCategoryVM vm)
        {
            var response = new ResponseStatus<ResponseCategoryVM>(); 
            try
            {
                var exists = await Context.categories
                    .AnyAsync(c => c.cat_name.ToLower() == vm.cat_name.ToLower());

                if (exists)
                {
                    response.Success = false;
                    response.Message = "Category with this name already exists";
                    response.Errors.Add(response.Message);
                    return response;
                }

                var category = Mapper.Map<Category>(vm);
                category.isactive = true;

                if (string.IsNullOrWhiteSpace(category.user_id))
                {
                    category.user_id = await Context.Users
                        .Select(u => u.Id)
                        .FirstOrDefaultAsync();

                    if (string.IsNullOrWhiteSpace(category.user_id))
                    {
                        response.Success = false;
                        response.Message = "Category must be linked to an existing user.";
                        response.Errors.Add(response.Message);
                        return response;
                    }
                }

                await Context.categories.AddAsync(category);
                await Context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Category added successfully";
                response.Data = Mapper.Map<ResponseCategoryVM>(category);
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

        public async Task<ResponseStatus<ResponseCategoryVM>> UpdateAsync(UpdateCategoryVM vm)
        {
            var response = new ResponseStatus<ResponseCategoryVM>();
            try
            {
                var existing = await Context.categories.FindAsync(vm.cat_id);
                if (existing == null)
                {
                    response.Success = false;
                    response.Message = "Category not found";
                    return response;
                }

                existing.cat_name        = vm.cat_name;
                existing.cat_description = vm.cat_description;
                existing.isactive        = vm.isactive;
                existing.user_id         = vm.user_id;

                await Context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Category updated successfully";
                response.Data    = Mapper.Map<ResponseCategoryVM>(existing);
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
    }
}
