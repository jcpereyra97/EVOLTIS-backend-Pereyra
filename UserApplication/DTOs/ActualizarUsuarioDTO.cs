using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApplication.DTOs
{
    public class ActualizarUsuarioDTO
    {
        public int ID { get; private set; }
        public string? Nombre{ get; set; }
        public string? Email { get; set; }
        public ActualizarDomicilioDTO? Domicilio { get; set; }
        public void SetID(int id)
        {
            ID = id;
        }
    }


    public class ActualizarUsuarioDTOValidator : AbstractValidator<ActualizarUsuarioDTO>
    {
        public ActualizarUsuarioDTOValidator()
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
