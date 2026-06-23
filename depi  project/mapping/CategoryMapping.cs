using AutoMapper;
using depi__project.Models;
using depi__project.viewmodels.category;

namespace depi__project.mapping
{
    public class CategoryMapping : Profile
    {
        public CategoryMapping()
        {
            CreateMap<Category, ResponseCategoryVM>()
                .ForMember(dest => dest.AddedBy,
                           opt => opt.MapFrom(src => src.user != null ? src.user.UserName : null));

            CreateMap<AddCategoryVM, Category>()
                .ForMember(dest => dest.cat_id,   opt => opt.Ignore())
                .ForMember(dest => dest.isactive,  opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.medicines, opt => opt.Ignore())
                .ForMember(dest => dest.user,      opt => opt.Ignore())
                .ForMember(dest => dest.AddedBy,   opt => opt.Ignore());

            CreateMap<ResponseCategoryVM, UpdateCategoryVM>();

            CreateMap<Category, UpdateCategoryVM>();
        }
    }
}
