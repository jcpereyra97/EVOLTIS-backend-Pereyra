using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace UserDomain.Domain
{
    public class Domicilio
    {
        public int ID { get; private set; }
        public int UsuarioID { get; private set; }
        public string Calle { get; private set; }
        public string Numero { get; private set; }
        public string Provincia { get; private set; }
        public string Ciudad { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public DateTime FechaUltimaActualizacion { get; private set; }
        public bool Activo { get; private set; }
        public Usuario Usuario { get; private set; } = default!;
        public Domicilio() {}
        public Domicilio(Usuario usuario, string calle, string numero, string provincia, string ciudad)
        {
            Usuario = usuario;
            Calle = calle;
            Numero = numero;
            Provincia = provincia;
            Ciudad = ciudad;
            FechaCreacion = DateTime.UtcNow;
        }

        public void ActualizarCalle(string calle)
        {
            Calle = calle;
        }

        public void ActualizarProvincia(string provincia)
        {
            Provincia = provincia;
        }

        public void ActualizarCiudad(string ciudad)
        {
            Ciudad = ciudad;
        }

        public void ActualizarNumero(string numero)
        {
            Numero = numero;
        }

        public void EliminarDomicilio() { Activo = false; Actualizar(); }

        private void Actualizar() => FechaUltimaActualizacion = DateTime.UtcNow;

    }
}
