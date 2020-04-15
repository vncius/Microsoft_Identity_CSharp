using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Identity.Enum;
using WebApp.Identity.Models;

namespace WebApp.Identity.Autenticacao
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<MyUser>
    {
        public MyUserClaimsPrincipalFactory(UserManager<MyUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {

        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(MyUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            identity.AddClaim(new Claim(ClaimTypes.Role, user.Email));

            return identity;
        }
    }
}
