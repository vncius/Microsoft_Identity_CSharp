using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Identity.Autenticacao.Model;
using WebApp.Identity.Email;
using WebApp.Identity.Models;

namespace WebApp.Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly RoleManager<MyRole> _roleManager;
        private readonly SignInManager<MyUser> _signInManager;

        public AccountController(UserManager<MyUser> userManager, RoleManager<MyRole> roleManager, SignInManager<MyUser> signInManager)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user != null && !await _userManager.IsLockedOutAsync(user))
                {
                    if (await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        if (!await _userManager.IsEmailConfirmedAsync(user))
                        {
                            ModelState.AddModelError("", "Email invalid!");
                            return View();
                        }

                        // RESETA AS TENTATIVAS DE LOGIN APÓS LOGAR NO SISTEMA
                        await _userManager.ResetAccessFailedCountAsync(user);

                        var claimsPrincipal = crieClaimsPrincipal(user);
                        // ADICIONA AS CLAIMS DO USUÁRIO EM COOKIE
                        await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);

                        return RedirectToAction("Index", nameof(HomeController).Replace("Controller", ""));
                    }

                    // ADICIONA UMA TENTATIVA DE LOGIN COM FALHA
                    await _userManager.AccessFailedAsync(user);

                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        // ENVIAR E-MAIL SUGERINDO MUDAR SENHA
                    }
                }

                ModelState.AddModelError("", "Usuário ou Senha Invalida");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.Context.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Register()
        {
            RegisterModel registerViewModel = new RegisterModel();
            return View(registerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    user = new MyUser()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = model.UserName,
                        Email = model.UserName
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    var myRole = await _roleManager.FindByNameAsync(model.Role);
                    if (myRole != null)
                    {
                        IdentityResult roleResult = await _userManager.AddToRoleAsync(user, myRole.Name);
                    }

                    if (result.Succeeded)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationEmail = Url.Action(nameof(ConfirmEmailAddress), nameof(AccountController).Replace("Controller", ""),
                                                        new { token = token, email = user.Email }, Request.Scheme);

                        EnviarEmail.enviar(confirmationEmail, "Confirme seu e-mail acessando o link abaixo!", "Confirmação de cadastro", user.Email);
                    }
                    else
                    {
                        foreach (var erro in result.Errors)
                        {
                            ModelState.AddModelError("", erro.Description);
                        }

                        return View();
                    }
                }
                ViewData["Mensagem"] = "Confirme o cadastro acessando o link enviado ao seu e-mail!";
                
                return View("_Mensagem");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAddress(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    ViewData["Mensagem"] = "Cadastro confirmado com sucesso! <a href='/Login'>Logar no sistema</a>";
                    return View("_Mensagem");
                }

                return View("Error");
            }

            return View();
        }
                
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            return View(new ResetPasswordModel { Token = token, Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (!result.Succeeded)
                    {
                        foreach (var erro in result.Errors)
                        {
                            ModelState.AddModelError("", erro.Description);
                        }
                        return View();
                    }

                    ViewData["Mensagem"] = "Senha alterada com sucesso!";
                    return View("_Mensagem");
                }
                ModelState.AddModelError("", "Invalid Request");
            }
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetURL = Url.Action("ResetPassword", "Home",
                        new { token = token, email = model.Email }, Request.Scheme);


                    EnviarEmail.enviar(resetURL, "Acesse o link abaixo para redefinir sua senha!", "Redefinição de Senha", user.Email);
                    System.IO.File.WriteAllText("resetLink.txt", resetURL); // LINK GERADO PARA RECUPERAR PASSWORD

                    ViewData["Mensagem"] = "E-mail de redefinição de senha enviado!";
                    return View("_Mensagem");
                }
                else
                {
                    // IMPLEMENTAR VIEW USUÁRIO NÃO ENCONTRADO
                }
            }
            return View();
        }

        private ClaimsPrincipal crieClaimsPrincipal(MyUser user)
        {
            var customClaims = new List<Claim>();
            var roles = _userManager.GetRolesAsync(user).Result;

            foreach (var role in roles)
            {
                customClaims.Add(new Claim(ClaimTypes.Role, role));
                HttpContext.User.IsInRole(role);
            }

            customClaims.Add(new Claim(ClaimTypes.Name, String.IsNullOrEmpty(user.NomeCompleto) ? "Usuário" : user.NomeCompleto));
            customClaims.Add(new Claim(ClaimTypes.Email, user.Email));

            var claimsIdentity = new ClaimsIdentity(customClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}
