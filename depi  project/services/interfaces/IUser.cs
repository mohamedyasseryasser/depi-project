using depi__project.Models;
using depi__project.viewmodels.General;
using depi__project.viewmodels.userviewmodel;

namespace depi__project.services.interfaces
{
    public interface IUser
    {
        // Admin Methods
      public  Task<ResponseStatus<responseadminviewmodel>> addadmin(adminviewmodel vm);
       public  Task<ResponseStatus<responseadminviewmodel>> updateadmin(updateadminviewmodel vm);
       public Task<ResponseStatus<responseadminviewmodel>> getadminbyid(int adminid);
       public Task<ResponseStatus<IEnumerable<responseadminviewmodel>>> getalladmins(pagination pg, string searchterm = null);
      public  Task<ResponseStatus<bool>> removeadmin(string Id);
       public Task<ResponseStatus<int>> getadmincount();
        public Task<ResponseStatus<responseadminviewmodel>> getadminbyuserid(string userid);
        public Task<ResponseStatus<IEnumerable<responseadminviewmodel>>> getallactiveadmins();

        // Doctor Methods
        public  Task<ResponseStatus<int>> getadoctorcount();

        public Task<IEnumerable<Doctor>> getdoctors();
        public Task<ResponseStatus<doctorresponseviewmodel>> adddoctor(doctorviewmodel vm);
    public    Task<ResponseStatus<doctorresponseviewmodel>> updatedoctor(updatedoctorviewmodel vm);
     public   Task<ResponseStatus<doctorresponseviewmodel>> getdoctorbyid(int doctorid);
     public   Task<ResponseStatus<IEnumerable<doctorresponseviewmodel>>> getalldoctors(pagination pg, string searchterm = null);
      public  Task<ResponseStatus<bool>> removedoctor(string id);

        // Receptionist Methods
        public Task<IEnumerable<resptionist>> getreceptionists();

        public Task<ResponseStatus<responsereceptionistviewmodel>> addresceptionist(resceptionistviewmodel vm);

        public  Task<ResponseStatus<responsereceptionistviewmodel>> updatereceptionist(updatereceptionistviewmodel vm);

        public  Task<ResponseStatus<bool>> removereceptionist(string userid);
        public Task<ResponseStatus<responsereceptionistviewmodel>> getreceptionistbyid(int id);
        public  Task<ResponseStatus<IEnumerable<responsereceptionistviewmodel>>> getallreceptionists(pagination pg, string searchterm = null);
    }
}
