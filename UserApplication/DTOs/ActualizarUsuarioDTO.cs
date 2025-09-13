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
}
