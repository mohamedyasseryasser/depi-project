using depi__project.viewmodels.Authviewmodel;
using depi__project.viewmodels.General;

namespace depi__project.services.interfaces
{
    public interface IAuth
    {
        Task<ResponseStatus<string>> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<ResponseStatus<string>> ChangePasswordAsync(ChangePasswordViewModel model);
    }
}
