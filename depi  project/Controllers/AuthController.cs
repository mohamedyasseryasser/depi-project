using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
 using depi__project.Models;
using depi__project.services.interfaces;
using depi__project.viewmodels.Authviewmodel;

namespace depi__project.Controllers
{
   
     public class AuthController : Controller
    {
        public IAuth _authService { get; }
        public ILogger<AuthController> _logger { get; }

        public AuthController(IAuth authservice,ILogger<AuthController> logger)
        {
            _authService = authservice;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Login()
        {
            
            _logger.LogInformation("Login page requested");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login attempt (ModelState invalid)");

                return View(model);
            }
            _logger.LogInformation($"try login username is{model.UserName} and password is{model.Password}");

            var result = await _authService.LoginAsync(model);
            if (result.Success)
            {
                _logger.LogInformation("User logged in successfully: {username}", model.UserName);
                return RedirectToAction("Index", "Home");
            }
            _logger.LogWarning("Failed login attempt for user: {username} - Reason: {Message}", model.UserName, result.Message);
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {

            _logger.LogInformation("User logging out");

            await _authService.LogoutAsync();

            _logger.LogInformation("User logged out successfully");

            return RedirectToAction("Login", "Auth");
        }
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            _logger.LogInformation("ChangePassword page requested");

            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid ChangePassword request");

                return View(model);
            }
            _logger.LogInformation("ChangePassword attempt");

            var result = await _authService.ChangePasswordAsync(model);
            if (result.Success)
            {
                _logger.LogInformation("Password changed successfully");

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index", "Home");
            }
            _logger.LogWarning("Failed ChangePassword attempt - Reason: {Message}", result.Message);


            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }
    }
}
