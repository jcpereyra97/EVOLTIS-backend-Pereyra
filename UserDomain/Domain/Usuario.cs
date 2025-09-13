using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UserDomain.Domain
{
    public class Usuario
    {
        public int ID { get; private set; }
        public string Nombre { get; private set; }
        public string Email { get; private set; }
        public DateTime FechaCreacion { get; private set; } = DateTime.Now.AddDays(-1);
        private readonly List<Domicilio> _domicilios = new(); 
        public IReadOnlyCollection<Domicilio> Domicilios => _domicilios;
        public Usuario() {}
       
        
        public Usuario(string nombre, string email)
        {

            if (string.IsNullOrEmpty(nombre) || nombre.Trim().Length is < 2 or 100)
                throw new ArgumentOutOfRangeException(nameof(nombre),"2-100 caracteres");            
            Nombre = nombre.Trim();
            Email = email;
            FechaCreacion = DateTime.UtcNow;
        }

        public void AgregarDomicilio(string calle, string numero, string provincia, string ciudad)
        {
            if (_domicilios.Any(p => p.Calle.ToUpper().Equals(calle.ToUpper()) && p.Numero.ToUpper().Equals(numero.ToUpper())))
                throw new InvalidOperationException("Ya existe Domicilio para este Usuario");
            
            var domicilio = new Domicilio(this,calle,numero,provincia,ciudad);
            _domicilios.Add(domicilio);
        }

        public void Renombrar(string name)
        {
            Nombre = name;
        }

        public void CambiarEmail(string email)
        {
            Email = email;
        }
    }
}
