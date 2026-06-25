using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering    ;
using depi__project.Models;
using depi__project.services.interfaces;
using depi__project.viewmodels.General;
using depi__project.viewmodels.userviewmodel;
using System.Threading.Tasks;

namespace depi__project.Controllers
{
    public class UserController : Controller
    {
        public UserController(IUser userservice,IDepartment departmentserivce)
        {
            Userservice = userservice;
            Departmentserivce = departmentserivce;
        }
        public IUser Userservice { get; }
        public IDepartment Departmentserivce { get; }
        public IActionResult AddAdmin()
        {
            return View();
        }
        public async Task<IActionResult> SaveAddAdmin(adminviewmodel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("AddAdmin",vm);
            }
            try
            {
                var result=await Userservice.addadmin(vm);
                if (!result.Success)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    TempData["errormessage"] = result.Message ?? "add admin is failed";
                    return View("AddAdmin", vm);
                }
                else
                {
                    TempData["successed"] = result.Message ?? "add admin is successed";
                    return RedirectToAction("AdminDetails", new { adminid = result.Data.adminid});
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong");
                return View("AddAdmin", vm);
            }
        }
        public async Task<IActionResult> AdminDetails(int adminid)
        {
            ResponseStatus<responseadminviewmodel> response =await Userservice.getadminbyid(adminid);
            if (response.Success)
            {
                return View(response.Data);
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Admin not found.";
                return RedirectToAction(nameof(ListAdmins));
            }
        }
        public async Task<IActionResult> ListAdmins(int pageNumber = 1, int pageSize = 10, string searchTerm = null)
        {
            var pagination = new pagination { PageNumber = pageNumber, PageSize = pageSize };
         
            ResponseStatus<IEnumerable<responseadminviewmodel>> response = await Userservice.getalladmins(pagination, searchTerm);
            if (response.Success)
            {
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.SearchTerm = searchTerm;
                return View(response.Data);
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to retrieve admins.";
                return View(new List<responseadminviewmodel>());
            }
        }
        // GET: User/UpdateAdmin/{adminId}
        public async Task<IActionResult> UpdateAdmin(int adminId)
        {
             var response = await Userservice.getadminbyid(adminId);
            if (response.Success) 
            {
                var updateVm = new updateadminviewmodel
                {
                    userid = response.Data.Id,
                    UserName = response.Data.UserName,
                    Email = response.Data.Email,
                    PhoneNumber = response.Data.PhoneNumber,
                    address = response.Data.address,
                    Gender = response.Data.Gender,
                    permissions = response.Data.permissions
                    ,IsActive= response.Data.IsActive
                };
                return View(updateVm);
            }
            else
            {
                TempData["errormessage"] = response.Message ??"this admin is not found";
                return RedirectToAction(nameof(ListAdmins));
            }
        }
        public async Task<IActionResult> SaveUpdateAdmin(updateadminviewmodel vm) 
        {
            var response=new ResponseStatus<responseadminviewmodel>();  
            if (!ModelState.IsValid) 
            {
                return View("UpdateAdmin",vm);
            }
            var updatedadmin = await Userservice.updateadmin(vm);
            if (!updatedadmin.Success) 
            {
                TempData["errormessage"] = response.Message??"updated is failed";
                foreach (var error in updatedadmin.Errors)
                {
                    ModelState.AddModelError(string.Empty,error);
                }
            }
            else
            {
                TempData["successfully"] =response.Message?? "this updated is successfully";
                response.Data = updatedadmin.Data;
                return RedirectToAction(nameof(AdminDetails),new { adminid=updatedadmin.Data.adminid});
            }
            return View("UpdateAdmin",vm);
        }
        //remove admin
        public async Task<ActionResult> RemoveAdmin(string Id) 
        {
            ResponseStatus<bool> response = await Userservice.removeadmin(Id);
            if (response.Success)
            {
                TempData["SuccessMessage"] = response.Message ?? "Admin removed successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to remove admin.";
            }
            return RedirectToAction(nameof(ListAdmins));
        }
        // Doctor Methods (Placeholders)
        public async Task<IActionResult> AddDoctor()
        {
            var vm = new doctorviewmodel();
            await LoadDepartments(vm);
            return View(vm);
        }
        public async Task<IActionResult> SaveAddDoctor(doctorviewmodel vm)
        {
           
            if(ModelState.IsValid) 
            {
                ResponseStatus<doctorresponseviewmodel> response = await Userservice.adddoctor(vm);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = response.Message ?? "Doctor added successfully!";
                    return RedirectToAction(nameof(DoctorDetails), new { doctorId = response.Data.DoctorId });
                }
                else
                {
                    foreach (var error in response.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    TempData["ErrorMessage"] = response.Message ?? "Failed to add doctor.";
                }
            }
            await LoadDepartments(vm);
            return View("AddDoctor",vm);
        }
        private async Task LoadDepartments(doctorviewmodel vm)
        {
            var depts = await Departmentserivce.getalldept();

            vm.deptids = depts.Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.Name
            }).ToList();
        }
        public async Task<IActionResult> DoctorDetails(int doctorId)
        {
            ResponseStatus<doctorresponseviewmodel> response = await Userservice.getdoctorbyid(doctorId);
            if (response.Success)
            {
                return View(response.Data);
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Doctor not found.";
                return RedirectToAction(nameof(ListDoctors));
            }
        }

        public async Task<IActionResult> ListDoctors(int pageNumber = 1, int pageSize = 10, string searchTerm = null)
        {
            var pagination = new pagination { PageNumber = pageNumber, PageSize = pageSize };
            ResponseStatus<IEnumerable<doctorresponseviewmodel>> response = await Userservice.getalldoctors(pagination, searchTerm);
            if (response.Success)
            {
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.SearchTerm = searchTerm;
                return View(response.Data);
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to retrieve doctors.";
                return View(new List<doctorresponseviewmodel>());
            }
        }

        public async Task<IActionResult> UpdateDoctor(int doctorId)
        {
            var depts = await Departmentserivce.getalldept();

            ResponseStatus<doctorresponseviewmodel> response = await Userservice.getdoctorbyid(doctorId);
            if (response.Success)
            {
                var updateVm = new updatedoctorviewmodel
                {
                    userid = response.Data.Id,
                    UserName = response.Data.UserName,
                    Email = response.Data.Email,
                    PhoneNumber = response.Data.PhoneNumber,
                    address = response.Data.address,
                    Gender = response.Data.Gender,
                    DoctorId = response.Data.DoctorId,
                    Specialization = response.Data.Specialization,
                    hiredate = response.Data.hiredate,
                    salary = response.Data.salary,
                    
                    DepartmentId = response.Data.DepartmentId  ,
                    deptids = depts.Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.Name
                    }).ToList()
                
            }
            ;
                return View(updateVm);
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Doctor not found for update.";
                return RedirectToAction(nameof(ListDoctors));
            }
        }
        public async Task<IActionResult> SaveUpdateDoctor(updatedoctorviewmodel vm)
        {
            if (ModelState.IsValid)
            {
                ResponseStatus<doctorresponseviewmodel> response = await Userservice.updatedoctor(vm);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = response.Message ?? "Doctor updated successfully!";
                    return RedirectToAction(nameof(DoctorDetails), new { doctorId = response.Data.DoctorId });
                }
                else
                {
                    foreach (var error in response.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    TempData["ErrorMessage"] = response.Message ?? "Failed to update doctor.";
                }
            }
            var depts = await Departmentserivce.getalldept();

            vm.deptids = depts.Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.Name
            }).ToList();
            return View(vm);
        }
        public async Task<IActionResult> RemoveDoctor(string id)
        {
            ResponseStatus<bool> response = await Userservice.removedoctor(id);
            if (response.Success)
            {
                TempData["SuccessMessage"] = response.Message ?? "Doctor removed successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to remove doctor.";
            }
            return RedirectToAction(nameof(ListDoctors));
        }
        //receptionist
        public async Task<IActionResult> AddReceptionist()
        {
            return View();
        }
        public async Task<IActionResult> SaveAddReceptionist(resceptionistviewmodel vm)
        {
            if (ModelState.IsValid)
            {
                var response = await Userservice.addresceptionist(vm);
                if (response.Success)
                {
                    TempData["successs"] = response.Message ?? "add receptionist is successfully";
                    return RedirectToAction(nameof(ReceptionistDetails), new { id = response.Data.resptionistid });
                }
                else
                {
                    TempData["errormessage"] = response.Message ?? "add receptionist is failed";
                    foreach (var error in response.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
            }
            return View("AddReceptionist", vm);

        }

        public async Task<IActionResult> ReceptionistDetails(int id)
        {
            ResponseStatus<responsereceptionistviewmodel> response = await Userservice.getreceptionistbyid(id);
            if (response.Success)
            {
                return View(response.Data);
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Receptionist not found.";
                return RedirectToAction(nameof(ListReceptionists));
            }
        }

        public async Task<IActionResult> ListReceptionists(int pageNumber = 1, int pageSize = 10, string searchTerm = null)
        {
            var pagination = new pagination { PageNumber = pageNumber, PageSize = pageSize };
            ResponseStatus<IEnumerable<responsereceptionistviewmodel>> response = await Userservice.getallreceptionists(pagination, searchTerm);
            if (response.Success)
            {
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.SearchTerm = searchTerm;
                return View(response.Data);
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to retrieve receptionists.";
                return View(new List<responsereceptionistviewmodel>());
            }
        }

        public async Task<IActionResult> UpdateReceptionist(int id)
        {
            var response = await Userservice.getreceptionistbyid(id);
            if (response.Success)
            {
                var updateVm = new updatereceptionistviewmodel
                {
                    receptionid=response.Data.resptionistid,
                    hiredate=response.Data.hiredate,
                    salary=response.Data.salary,
                    userid = response.Data.Id,
                    UserName = response.Data.UserName,
                    Email = response.Data.Email,
                    PhoneNumber = response.Data.PhoneNumber,
                    address = response.Data.address,
                    Gender = response.Data.Gender,
                    IsActive = response.Data.IsActive
                };
                return View(updateVm);
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Receptionist not found for update.";
                return RedirectToAction(nameof(ListReceptionists));
            }
        }

        public async Task<IActionResult> SaveUpdateReceptionist(updatereceptionistviewmodel vm)
        {
            if (ModelState.IsValid)
            {
                var response = await Userservice.updatereceptionist(vm);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = response.Message ?? "Receptionist updated successfully!";
                    return RedirectToAction(nameof(ReceptionistDetails), new { id = response.Data.resptionistid });
                }
                else
                {
                    foreach (var error in response.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    TempData["ErrorMessage"] = response.Message ?? "Failed to update receptionist.";
                }
            }
            return View("UpdateReceptionist", vm);
        }

        public async Task<IActionResult> RemoveReceptionist(string id)
        {
            ResponseStatus<bool> response = await Userservice.removereceptionist(id);
            if (response.Success)
            {
                TempData["SuccessMessage"] = response.Message ?? "Receptionist removed successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Receptionist not found.";
            }
            return RedirectToAction(nameof(ListReceptionists));
        }

        public IActionResult ListUsers()
        {
            return View();
        }

    }
}
