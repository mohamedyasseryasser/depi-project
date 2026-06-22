using AutoMapper;
using depi__project.Models;
using depi__project.viewmodels.invoice;

namespace depi__project.mapping
{
    public class invoicemapping : Profile
    {
        public invoicemapping()
        {
            CreateMap<AddInvoiceVM, Invoice>()
                .ForMember(dest => dest.InvoiceId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Visit, opt => opt.Ignore())
                .ForMember(dest => dest.FinalAmount, opt => opt.Ignore());

            CreateMap<UpdateInvoiceVM, Invoice>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Visit, opt => opt.Ignore())
                .ForMember(dest => dest.FinalAmount, opt => opt.Ignore());

            CreateMap<Invoice, ResponseInvoiceVM>()
                .ForMember(dest => dest.VisitDate, opt => opt.MapFrom(src => src.Visit!.visitdate))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Visit!.Appoinment!.Patient!.patientname))
                .ForMember(dest => dest.PatientPhone, opt => opt.MapFrom(src => src.Visit!.Appoinment!.Patient!.phonenumber))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Visit!.Appoinment!.Doctor!.user!.UserName))
                .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.Visit!.appoinmentid));

            CreateMap<Invoice, UpdateInvoiceVM>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Visit!.Appoinment!.Patient!.patientname))
                .ForMember(dest => dest.Visits, opt => opt.Ignore());

            CreateMap<ResponseInvoiceVM, UpdateInvoiceVM>()
                .ForMember(dest => dest.Visits, opt => opt.Ignore());
        }
    }
}
