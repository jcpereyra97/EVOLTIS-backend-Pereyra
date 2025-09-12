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

            if (string.IsNullOrEmpty(nombre) || nombre.Trim().Length is < 2 or 100)
                throw new ArgumentOutOfRangeException(nameof(nombre),"2-100 caracteres");            
            Nombre = nombre.Trim();
            Email = email;
            FechaCreacion = DateTime.UtcNow;
        }



        public void UpdateName(string name)
        {
            Nombre = name;
        }

        public void CambiarEmail(string email)
        {
            Email = email;
        }
    }
}
