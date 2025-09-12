using System;
using System.Collections.Generic;
using System.Linq;
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
        public Usuario Usuario { get; private set; } = default!;
        public Domicilio() {}
        public Domicilio(int usuarioID, string calle, string numero, string provincia, string ciudad)
        {
            UsuarioID = usuarioID;
            Calle = calle;
            Numero = numero;
            Provincia = provincia;
            Ciudad = ciudad;
            FechaCreacion = DateTime.UtcNow;
        }

        public void UpdateCalle(string calle)
        {
            Calle = calle;
        }

        public void UpdateProvincia(string provincia)
        {
            Provincia = provincia;
        }

        public void UpdateCiudad(string ciudad)
        {
            Ciudad = ciudad;
        }

    }
}
