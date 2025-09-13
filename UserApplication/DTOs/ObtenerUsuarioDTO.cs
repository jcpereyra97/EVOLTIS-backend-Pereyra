using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApplication.DTOs
{
    public record ObtenerUsuarioDTO
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public DateTime FechaCreacion { get; set; }
        public IEnumerable<ObtenerDomicilioDTO> Domicilios { get; set; }
        

    }
}
