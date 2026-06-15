using AutoMapper;
using depi__project.Models;
using depi__project.viewmodels.Patient;

namespace depi__project.mapping
{
    public class Patientmapping:Profile
    {
       public Patientmapping()
        {
            CreateMap<AddPatientVM, Patient>();
            CreateMap<Patient, ResponsePatientVM>();
            CreateMap<UpdatePatientVM, ResponsePatientVM>();

        }
    }
}
