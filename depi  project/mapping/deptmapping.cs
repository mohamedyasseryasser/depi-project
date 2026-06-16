using AutoMapper;
using depi__project.Models;
using depi__project.viewmodels.departmentvm;

namespace depi__project.mapping
{
    public class deptmapping:Profile
    {
        public deptmapping() 
        {
            CreateMap<Department, ResponseDeptVm>().
                ForMember(dest => dest.UserName,
                          opt => opt.MapFrom(src => src.user.UserName))
               .ForMember(dest => dest.DoctorCount,
                          opt=>opt.MapFrom(src=>src.doctors.Count));

            CreateMap<AddDeptVm, Department>()
               .ForMember(dest => dest.createdat,
                          opt => opt.MapFrom(src => DateTime.Now))
               .ForMember(dest => dest.updatedat,
                          opt => opt.Ignore()) 
               .ForMember(dest => dest.DepartmentId,
                          opt => opt.Ignore()); 

            CreateMap<ResponseDeptVm, UpdateDertVm>();
        }
    }
}
