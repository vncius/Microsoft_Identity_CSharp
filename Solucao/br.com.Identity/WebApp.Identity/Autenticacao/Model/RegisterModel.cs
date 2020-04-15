using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Identity.Enum;

namespace WebApp.Identity.Models
{
    public class RegisterModel
    {
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Perfis de usuário : ")]
        [UIHint("List")]
        public List<SelectListItem> Roles { get; set; }

        public string Role { get; set; }

        public RegisterModel()
        {
            Roles = new List<SelectListItem>();
            Roles.Add(new SelectListItem() { Value = "1", Text = EnumPerfil.Administrador.ToString() });
            Roles.Add(new SelectListItem() { Value = "2", Text = EnumPerfil.Coordenador.ToString() });
            Roles.Add(new SelectListItem() { Value = "3", Text = EnumPerfil.Professor.ToString() });
        }
    }
}
