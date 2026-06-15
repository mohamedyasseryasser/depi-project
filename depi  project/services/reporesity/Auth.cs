using Microsoft.AspNetCore.Identity;
using depi__project.Models;
using depi__project.services.interfaces;
using depi__project.viewmodels;
using depi__project.viewmodels.Authviewmodel;
using depi__project.viewmodels.General;

namespace depi__project.services.reporesity
{
    public class Auth:IAuth
    {
        public Auth(Context context,UserManager<Aplicationuser> userManager,SignInManager<Aplicationuser> signInManager)
        {
            Context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public Context Context { get; }
        public UserManager<Aplicationuser> _userManager { get; }
        public SignInManager<Aplicationuser> _signInManager { get; }

        public async Task<ResponseStatus<string>> LoginAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return new ResponseStatus<string> { Success = false, Message = "Invalid username or password" };
            }

            var checkpassword=await _userManager.CheckPasswordAsync(user, model.Password);
            if (!checkpassword)
            {
                return new ResponseStatus<string> { Success = false, Message = "Invalid username or password" };
            }
            if (!user.IsActive) 
            {
                return new ResponseStatus<string> { Success = false, Message = "your blocked from system" };

            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                return new ResponseStatus<string> { Success = true, Message = "Login successful" };
            }
            return new ResponseStatus<string> { Success = false,Message = "Invalid email or password" , };
        }
        public async Task LogoutAsync()
        {

            await _signInManager.SignOutAsync(); 
           
        }
        public async Task<ResponseStatus<string>> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new ResponseStatus<string> { Success = false, Message = "User not found with this email" };
            }
            if (!user.IsActive)
            {
                return new ResponseStatus<string> { Success = false, Message = "your blocked from system" };

            }
            var result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
            if (result.Succeeded)
            {
                return new ResponseStatus<string> { Success = true, Message = "Password changed successfully" };
            }
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new ResponseStatus<string> { Success = false, Message = errors };
        }
    }
}
