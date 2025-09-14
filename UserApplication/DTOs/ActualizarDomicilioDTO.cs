using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApplication.DTOs
{
    public class ActualizarDomicilioDTO
    {
        public int? ID { get;  set ; }
        public string? Calle { get; set; }
        public string? Numero { get; set; }
        public string? Provincia { get; set; }
        public string? Ciudad { get; set; }

       
    }

    public class ActualizarDomicilioDTOValidator : AbstractValidator<ActualizarDomicilioDTO>
    {
        public ActualizarDomicilioDTOValidator()
        {
            RuleFor(p => p.ID).GreaterThan(0).When(p => p.ID.HasValue).WithMessage("El ID debe ser mayor a 0");

            When(p => p.ID.HasValue, () =>
            {
                RuleFor(p => p.Calle).NotEmpty().WithMessage("Ciudad no puede estar vacio cuando se envia ID")
                    .MinimumLength(2).MaximumLength(50).WithMessage("Debe tener entre 2-50 caracteres");


                RuleFor(p => p.Numero).NotEmpty().WithMessage("Numero no puede estar vacio cuando se envia ID")
                    .MinimumLength(2).MaximumLength(50).WithMessage("Debe tener entre 2-50 caracteres");


                RuleFor(p => p.Provincia).NotEmpty().WithMessage("Provincia no puede estar vacio cuando se envia ID")
                    .MinimumLength(2).MaximumLength(100).WithMessage("Debe tener entre 2-100 caracteres");


                RuleFor(p => p.Ciudad).NotEmpty().WithMessage("Ciudad no puede estar vacio cuando se envia ID")
                    .MinimumLength(2).MaximumLength(50).WithMessage("Debe tener entre 2-50 caracteres");

            });
        }
    }
}
