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
        public bool Activo { get; private set; } = true;
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
            Actualizar();
        }

        public void ActualizarProvincia(string provincia)
        {
            Provincia = provincia;
            Actualizar();
        }

        public void ActualizarCiudad(string ciudad)
        {
            Ciudad = ciudad;
            Actualizar();
        }

        public void ActualizarNumero(string numero)
        {
            Numero = numero;
            Actualizar();
        }

        public void EliminarDomicilio() { Activo = false; Actualizar(); }

        private void Actualizar() => FechaUltimaActualizacion = DateTime.UtcNow;

    }
}
