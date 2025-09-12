using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDomain.Domain
{
    public class Usuario
    {
        public int ID { get; private set; }
        public string Nombre { get; private set; }
        public string Email { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        public Usuario(string nombre, string email)
        {
            Nombre = nombre;
            Email = email;
            FechaCreacion = DateTime.UtcNow;
        }

        public void UpdateName(string name)
        {
            Nombre = name;
        }

        public void UpdateEmail(string email)
        {
            Email = email;
        }
    }
}
