using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Identity.Autenticacao;
using WebApp.Identity.Email;
using WebApp.Identity.Models;

namespace WebApp.Identity.Controllers
{
    //[ApiController]   VALIDA TODOS OS OBJETOS DE ENTRADA SEM PRECISAR DO ModelState -- NÃO DEVE RETORNAR VIEW
    public class HomeController : Controller
    {

        //private readonly UserManager<MyUser> _userManager;
        private readonly UserManager<MyUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<MyUser> _userClaimsPrincipalFactory;

        //public HomeController(UserManager<MyUser> userManager)
        public HomeController(UserManager<MyUser> userManager,
            IUserClaimsPrincipalFactory<MyUser> userClaimsPrincipalFactory)
        {
            _userManager = userManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Sucess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
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

                        if (await _userManager.GetTwoFactorEnabledAsync(user))
                        {
                            var validator = await _userManager.GetValidTwoFactorProvidersAsync(user);

                            if (validator.Contains("Email"))
                            {
                                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                                EnviarEmail.enviar(token, "Token de acesso a aplicação!", "Token de autenticação", user.Email);

                                await HttpContext.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, 
                                    Store2FA(user.Id, "Email"));

                                return RedirectToAction(nameof(TwoFactor));
                            }
                        }

                        // O USERCLAIMS CRIA 3 CLAINS AUTOMÁTICAMENTE
                        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

                        // ADICIONA AS CLAIMS DO USUÁRIO EM COOKIE
                        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

                        return RedirectToAction("About");
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

        public ClaimsPrincipal Store2FA(string userId, string Provider)
        {
            var identity = new ClaimsIdentity(new List<Claim> { 
                new Claim("sub", userId),
                new Claim("amr", Provider)
            }, IdentityConstants.TwoFactorUserIdScheme);

            return new ClaimsPrincipal(identity);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactor(TwoFactorModel model)
        {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.TwoFactorUserIdScheme);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Seu token expirou!");
                return View();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(result.Principal.FindFirstValue("sub"));
                if (user != null)
                {
                    var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, result.Principal.FindFirstValue("amr"), model.Token);

                    if (isValid)
                    {
                        await HttpContext.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);

                        var claimsPrincipal = await _userClaimsPrincipalFactory.CreateAsync(user);
                        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);
                        return RedirectToAction(nameof(About));
                    }

                    ModelState.AddModelError("", "Invalid Token");
                }
                ModelState.AddModelError("", "Invalid Token");
            }
            ModelState.AddModelError("", "Invalid Request");
            return View();
        }

        [HttpGet]
        public IActionResult TwoFactor()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
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

                    if (result.Succeeded)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationEmail = Url.Action("ConfirmEmailAddress", "Home",
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

                return RedirectToAction(nameof(Login));
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
                    return View(nameof(Sucess));
                }

                return View("Error");
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

                    return View(nameof(Sucess));
                }
                else
                {
                    // IMPLEMENTAR VIEW USUÁRIO NÃO ENCONTRADO
                }
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
                    return View(nameof(Sucess));
                }
                ModelState.AddModelError("", "Invalid Request");
            }
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
