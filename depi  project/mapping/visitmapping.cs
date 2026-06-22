using AutoMapper;
using depi__project.Models;
using depi__project.viewmodels.departmentvm;
using depi__project.viewmodels.Visit;
using depi__project.viewmodels.VisitRepo;

namespace depi__project.mapping
{
    public class visitmapping:Profile
    {
        public visitmapping() 
        {
            CreateMap<AddVisit, Visit>();

            CreateMap<Visit, ResponseVisitVM>()
                .ForMember(
                    dest => dest.ResponseAppoimentVM,
                    opt => opt.MapFrom(src => src.Appoinment)
                )
                .ForMember(
                    dest => dest.Prescription,
                    opt => opt.MapFrom(src => src.Prescription)
                )
                .ForMember(
                    dest => dest.Invoice,
                    opt => opt.MapFrom(src => src.Invoice)
                );

            CreateMap<UpdateVisitVM, ResponseVisitVM>();
        }
    }
}
