using AutoMapper;
using depi__project.Models;
using depi__project.viewmodels.medicine;

namespace depi__project.mapping
{
    public class MedicineMapping : Profile
    {
        public MedicineMapping()
        {
            CreateMap<Medicine, ResponseMedicineVM>()
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.category != null ? src.category.cat_name : null));

            CreateMap<AddMedicineVM, Medicine>()
                .ForMember(dest => dest.medicineId,        opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,         opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted,         opt => opt.Ignore())
                .ForMember(dest => dest.category,          opt => opt.Ignore())
                .ForMember(dest => dest.user,              opt => opt.Ignore())
                .ForMember(dest => dest.AddedBy,           opt => opt.Ignore())
                .ForMember(dest => dest.CategoryName,      opt => opt.Ignore())
                .ForMember(dest => dest.Prescriptionitems, opt => opt.Ignore());

            CreateMap<ResponseMedicineVM, UpdateMedicineVM>()
                .ForMember(dest => dest.Categories, opt => opt.Ignore());
        }
    }
}
