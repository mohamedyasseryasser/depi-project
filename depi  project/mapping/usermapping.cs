using AutoMapper;
using depi__project.Models;
using depi__project.viewmodels.userviewmodel;

namespace depi__project.mapping
{
    public class usermapping : Profile
    {
        public usermapping()
        {// 🔥 ViewModel ➜ Identity User
            CreateMap<userviewmodel, Aplicationuser>(MemberList.None)
      .ForMember(dest => dest.UserName,
                 opt => opt.MapFrom(src => src.UserName))
      .ForMember(dest => dest.Email,
                 opt => opt.MapFrom(src => src.Email))
      .ForMember(dest => dest.PhoneNumber,
                 opt => opt.MapFrom(src => src.PhoneNumber))
      .ForMember(dest => dest.address,
                 opt => opt.MapFrom(src => src.address))
      .ForMember(dest => dest.Gender,
                 opt => opt.MapFrom(src => src.Gender))
      .ForMember(dest => dest.createdat,
                 opt => opt.MapFrom(_ => DateTime.Now))
      .ForMember(dest => dest.updatedat,
                 opt => opt.MapFrom(_ => DateTime.Now))
      .ForMember(dest => dest.IsActive,
      opt => opt.MapFrom(src => src.IsActive));

            
            // 🔥 ViewModel ➜ Admin
            CreateMap<adminviewmodel, Admin>()
                .ForMember(dest => dest.permissions,
                           opt => opt.MapFrom(src => src.permissions))
                          
                .ForMember(dest => dest.userid,
                           opt => opt.Ignore());
            // viewmodel-> doctor
            CreateMap<doctorviewmodel, Doctor>()
                .ForMember(dest => dest.DepartmentId,
                opt => opt.MapFrom(src => src.DepartmentId)).
                ForMember(dest => dest.Specialization,
                opt => opt.MapFrom(src => src.Specialization)).ForMember(dest => dest.hiredate,
                opt => opt.MapFrom(src => src.hiredate)).ForMember(dest => dest.salary,
                opt => opt.MapFrom(src => src.salary)) 
                .ForMember(dest => dest.userid,
                           opt => opt.Ignore());
            // 🔥 ViewModel ➜ resecptionist
            CreateMap<resceptionistviewmodel, resptionist>()
                .ForMember(dest => dest.userid,
                           opt => opt.Ignore()).ForMember(dest => dest.salary,
                opt => opt.MapFrom(src => src.salary)).
                ForMember(dest => dest.hiredate,
                opt => opt.MapFrom(src => src.hiredate));












            CreateMap<Admin, responseadminviewmodel>()
    // Admin data
    .ForMember(dest => dest.adminid,
               opt => opt.MapFrom(src => src.Adminid)) 
    .ForMember(dest => dest.status,
                           opt => opt.MapFrom(src => src.status))
    .ForMember(dest => dest.permissions,
               opt => opt.MapFrom(src => src.permissions))

    // User data (🔥 مهم جدًا)
    .ForMember(dest => dest.Id,
               opt => opt.MapFrom(src => src.user.Id))
    .ForMember(dest => dest.UserName,
               opt => opt.MapFrom(src => src.user.UserName))
    .ForMember(dest => dest.Email,
               opt => opt.MapFrom(src => src.user.Email))
    .ForMember(dest => dest.PhoneNumber,
               opt => opt.MapFrom(src => src.user.PhoneNumber))
    .ForMember(dest => dest.address,
               opt => opt.MapFrom(src => src.user.address))
    .ForMember(dest => dest.Gender,
               opt => opt.MapFrom(src => src.user.Gender))
    .ForMember(dest => dest.IsActive,
               opt => opt.MapFrom(src => src.user.IsActive))
    .ForMember(dest => dest.createdat,
               opt => opt.MapFrom(src => src.user.createdat))
    .ForMember(dest => dest.updatedat,
               opt => opt.MapFrom(src => src.user.updatedat));



            //from doctor to doctorresponseviewmodel
            CreateMap<Doctor, doctorresponseviewmodel>()
// Admin data
.ForMember(dest => dest.deptname,
   opt => opt.MapFrom(src => src.Department.Name))
.ForMember(dest => dest.DepartmentId,
               opt => opt.MapFrom(src => src.DepartmentId))
.ForMember(dest => dest.DoctorId,
   opt => opt.MapFrom(src => src.DoctorId))
.ForMember(dest => dest.status,
   opt => opt.MapFrom(src => src.status))
.ForMember(dest => dest.salary,
               opt => opt.MapFrom(src => src.salary))
.ForMember(dest => dest.hiredate,
   opt => opt.MapFrom(src => src.hiredate))
.ForMember(dest => dest.Specialization,
   opt => opt.MapFrom(src => src.Specialization))
// User data (🔥 مهم جدًا)
.ForMember(dest => dest.Id,
   opt => opt.MapFrom(src => src.user.Id))
.ForMember(dest => dest.UserName,
   opt => opt.MapFrom(src => src.user.UserName))
.ForMember(dest => dest.Email,
   opt => opt.MapFrom(src => src.user.Email))
.ForMember(dest => dest.PhoneNumber,
   opt => opt.MapFrom(src => src.user.PhoneNumber))
.ForMember(dest => dest.address,
   opt => opt.MapFrom(src => src.user.address))
.ForMember(dest => dest.Gender,
   opt => opt.MapFrom(src => src.user.Gender))
.ForMember(dest => dest.IsActive,
   opt => opt.MapFrom(src => src.user.IsActive))
.ForMember(dest => dest.createdat,
   opt => opt.MapFrom(src => src.user.createdat))
.ForMember(dest => dest.updatedat,
   opt => opt.MapFrom(src => src.user.updatedat));





            //from receptionist to responsereceptionistviewmodel
            CreateMap<resptionist,responsereceptionistviewmodel>()
// Admin data
.ForMember(dest => dest.resptionistid,
   opt => opt.MapFrom(src => src.resptionistid))
.ForMember(dest => dest.salary,
               opt => opt.MapFrom(src => src.salary))
.ForMember(dest => dest.status,
   opt => opt.MapFrom(src => src.status))
.ForMember(dest => dest.hiredate,
   opt => opt.MapFrom(src => src.hiredate))
 
// User data (🔥 مهم جدًا)
.ForMember(dest => dest.Id,
   opt => opt.MapFrom(src => src.user.Id))
.ForMember(dest => dest.UserName,
   opt => opt.MapFrom(src => src.user.UserName))
.ForMember(dest => dest.Email,
   opt => opt.MapFrom(src => src.user.Email))
.ForMember(dest => dest.PhoneNumber,
   opt => opt.MapFrom(src => src.user.PhoneNumber))
.ForMember(dest => dest.address,
   opt => opt.MapFrom(src => src.user.address))
.ForMember(dest => dest.Gender,
   opt => opt.MapFrom(src => src.user.Gender))
.ForMember(dest => dest.IsActive,
   opt => opt.MapFrom(src => src.user.IsActive))
.ForMember(dest => dest.createdat,
   opt => opt.MapFrom(src => src.user.createdat))
.ForMember(dest => dest.updatedat,
   opt => opt.MapFrom(src => src.user.updatedat));



        }
        }
    }