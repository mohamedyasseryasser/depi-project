using AutoMapper;
using depi__project.Models;
using depi__project.viewmodels.prescription;
using depi__project.viewmodels.prescriptionitems;

namespace depi__project.mapping
{
    public class prescriptionmapping : Profile
    {
        public prescriptionmapping()
        {
            CreateMap<AddPrescriptionVM, Prescription>()
                .ForMember(dest => dest.prescriptionid, opt => opt.Ignore())
                .ForMember(dest => dest.items, opt => opt.Ignore())
                .ForMember(dest => dest.Visit, opt => opt.Ignore());

            CreateMap<Prescription, ResponsePrescriptionVM>()
                .ForMember(dest => dest.patientname, opt => opt.MapFrom(src => src.Visit!.Appoinment!.Patient!.patientname))
                .ForMember(dest => dest.phonenumber, opt => opt.MapFrom(src => src.Visit!.Appoinment!.Patient!.phonenumber))
                .ForMember(dest => dest.patientid, opt => opt.MapFrom(src => src.Visit!.Appoinment!.patientid))
                .ForMember(dest => dest.doctorid, opt => opt.MapFrom(src => src.Visit!.Appoinment!.doctorid))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Visit!.Appoinment!.Doctor!.user!.UserName))
                .ForMember(dest => dest.VisitDate, opt => opt.MapFrom(src => src.Visit!.visitdate))
                .ForMember(dest => dest.prescriptionitems, opt => opt.MapFrom(src => src.items));

            CreateMap<UpdatePrescriptionvm, Prescription>()
                .ForMember(dest => dest.items, opt => opt.Ignore())
                .ForMember(dest => dest.Visit, opt => opt.Ignore());

            CreateMap<Prescription, UpdatePrescriptionvm>()
                .ForMember(dest => dest.items, opt => opt.MapFrom(src => src.items))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Visit!.Appoinment!.Patient!.patientname));

            CreateMap<AddPrescriptionItemVM, Prescriptionitems>()
                .ForMember(dest => dest.prescriptionitemid, opt => opt.Ignore())
                .ForMember(dest => dest.prescriptionid, opt => opt.Ignore())
                .ForMember(dest => dest.notes, opt => opt.MapFrom(src => src.Instructions))
                .ForMember(dest => dest.Medicine, opt => opt.Ignore())
                .ForMember(dest => dest.Prescription, opt => opt.Ignore());

            CreateMap<Prescriptionitems, ResponseRescriptionitemVM>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Medicine!.Name))
                .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.notes));

            CreateMap<UpdatePrescriptionitemvm, Prescriptionitems>()
                .ForMember(dest => dest.notes, opt => opt.MapFrom(src => src.Instructions))
                .ForMember(dest => dest.Medicine, opt => opt.Ignore())
                .ForMember(dest => dest.Prescription, opt => opt.Ignore());

            CreateMap<Prescriptionitems, UpdatePrescriptionitemvm>()
                .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.notes));
        }
    }
}
