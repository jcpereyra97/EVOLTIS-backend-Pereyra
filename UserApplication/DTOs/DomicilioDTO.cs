using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApplication.DTOs
{
    public record DomicilioDTO
    {
        public int UsuarioID { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Provincia{ get; set; }
        public string Ciudad { get; set; }
    }
}
