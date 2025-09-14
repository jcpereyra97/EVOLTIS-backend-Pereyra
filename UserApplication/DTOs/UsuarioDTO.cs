using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApplication.DTOs
{
    public record UsuarioDTO
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
       
    }


    public class UserValidator : AbstractValidator<UsuarioDTO>
    {
        public UserValidator()
        {
            RuleFor(p => p.Nombre)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Nombre obligatorio")
                .MinimumLength(2).MaximumLength(100).WithMessage("No puede estar vacio entre 2-100 caracteres");
            RuleFor(p => p.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email obligatorio")
                .EmailAddress().Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .WithMessage("Direccion de correo electronico invalida");
               
        }
    }
}
