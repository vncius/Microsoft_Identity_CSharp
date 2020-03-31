using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApp.Identity.Models;

namespace WebApp.Identity.Controllers
{
    //[ApiController]   VALIDA TODOS OS OBJETOS DE ENTRADA SEM PRECISAR DO ModelState -- NÃO DEVE RETORNAR VIEW
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly UserManager<MyUser> _userManager;

        public HomeController(UserManager<MyUser> userManager)
        {
            _userManager = userManager;
        }        

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
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

                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                }

                return View("Sucess");
            }

            return View();
        }

        public async Task<IActionResult> Register()
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
