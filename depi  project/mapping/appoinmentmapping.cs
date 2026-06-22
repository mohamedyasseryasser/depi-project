using AutoMapper;
using depi__project.Models;
using depi__project.viewmodels.Appoinment;

namespace depi__project.mapping
{
    public class appoinmentmapping:Profile
    {
        public appoinmentmapping()
        {
            CreateMap<AddAppoinmentVM, Appoinment>()

                // Ignore Primary Key
                .ForMember(dest => dest.appoimentid,
                    opt => opt.Ignore())

             

                // Ignore Navigation Properties
                .ForMember(dest => dest.Doctor,
                    opt => opt.Ignore())

                .ForMember(dest => dest.Patient,
                    opt => opt.Ignore())

                .ForMember(dest => dest.resptionist,
                    opt => opt.Ignore())

                .ForMember(dest => dest.Visit,
                    opt => opt.Ignore());

            // Appoinment -> ResponseAppoimentVM
            CreateMap<Appoinment, ResponseAppoimentVM>()

                .ForMember(dest => dest.Patient,
                    opt => opt.MapFrom(src => src.Patient))

                .ForMember(dest => dest.updateat,
                    opt => opt.MapFrom(src => src.updateat ?? DateTime.Now))

                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Doctor != null && src.Doctor.user != null ? src.Doctor.user.UserName : string.Empty))

                .ForMember(dest => dest.ReceptionistName,
                    opt => opt.MapFrom(src => src.resptionist != null && src.resptionist.user != null ? src.resptionist.user.UserName : string.Empty))

                .ForMember(dest => dest.Visit,
                    opt => opt.MapFrom(src => src.Visit));
        }

    }
}
