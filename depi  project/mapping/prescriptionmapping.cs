using AutoMapper;
using depi__project.Models;
using depi__project.viewmodels.prescription;
using depi__project.viewmodels.prescriptionitems;

namespace depi__project.mapping
{
    public class prescriptionmapping:Profile
    {
        public prescriptionmapping()
        {
            // AddPrescriptionVM to Prescription
            CreateMap<AddPrescriptionVM, Prescription>()
                .ForMember(dest => dest.prescriptionid, opt => opt.Ignore())
                .ForMember(dest => dest.items, opt => opt.Ignore());

            // Prescription to ResponsePrescriptionVM
            CreateMap<Prescription, ResponsePrescriptionVM>()
                .ForMember(dest => dest.patientname, opt => opt.MapFrom(src => src.Visit.Appoinment.Patient.patientname))
                .ForMember(dest => dest.phonenumber, opt => opt.MapFrom(src => src.Visit.Appoinment.Patient.phonenumber))
                .ForMember(dest => dest.prescriptionitems, opt => opt.MapFrom(src => src.items));

            // UpdatePrescriptionvm to Prescription
            CreateMap<UpdatePrescriptionvm, Prescription>()
                .ForMember(dest => dest.items, opt => opt.Ignore());

            // Prescription to UpdatePrescriptionvm
            CreateMap<Prescription, UpdatePrescriptionvm>()
                .ForMember(dest => dest.items, opt => opt.MapFrom(src => src.items));

            // AddPrescriptionItemVM to Prescriptionitems
            CreateMap<AddPrescriptionItemVM, Prescriptionitems>()
                .ForMember(dest => dest.prescriptionitemid, opt => opt.Ignore())
                .ForMember(dest => dest.Medicine, opt => opt.Ignore());

            // Prescriptionitems to ResponseRescriptionitemVM
            CreateMap<Prescriptionitems, ResponseRescriptionitemVM>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Medicine.Name));

            // UpdatePrescriptionitemvm to Prescriptionitems
            CreateMap<UpdatePrescriptionitemvm, Prescriptionitems>()
                .ForMember(dest => dest.Medicine, opt => opt.Ignore());

            // Prescriptionitems to UpdatePrescriptionitemvm
            CreateMap<Prescriptionitems, UpdatePrescriptionitemvm>();
        }
    }
}
