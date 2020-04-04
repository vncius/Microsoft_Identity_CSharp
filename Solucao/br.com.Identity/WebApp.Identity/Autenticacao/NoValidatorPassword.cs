using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Identity.Autenticacao
{
    public class NoValidatorPassword<TUser> : IPasswordValidator<TUser> where TUser : class
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            var username = await manager.GetUserNameAsync(user);

            if (username == password)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "A senha não pode ser o usuário" });
            }
            if (password.Contains("password"))
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "A senha não pode ser password" });
            }

            return IdentityResult.Success;

        }
    }
}
