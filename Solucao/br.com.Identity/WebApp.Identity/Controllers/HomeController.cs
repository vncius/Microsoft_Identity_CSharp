﻿using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Identity.Autenticacao;
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
            return  View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    // O USERCLAIMS CRIA 3 CLAINS AUTOMÁTICAMENTE
                    var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

                    // ADICIONA AS CLAIMS DO USUÁRIO EM COOKIE
                    await HttpContext.SignInAsync("Identity.Application", principal);

                    return RedirectToAction("About");
                }
                               
                ModelState.AddModelError("", "Usuário ou Senha Invalida");
            }
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

                    };

                    //user = new MyUser()
                    //{
                    //    Id = Guid.NewGuid().ToString(),
                    //    UserName = model.UserName,

                    //};

                    var result = await _userManager.CreateAsync(user, model.Password);
                }

                return View("Sucess");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
