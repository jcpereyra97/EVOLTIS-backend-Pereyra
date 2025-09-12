using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDomain.DTOs
{
    public record UsuarioDTO
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
       
    }
}
