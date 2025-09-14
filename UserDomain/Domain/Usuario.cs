using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
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

        private readonly List<Domicilio> _domicilios = new(); 
        public IReadOnlyCollection<Domicilio> Domicilios => _domicilios;

        public DateTime FechaUltimaActualizacion { get; private set; }
        public bool Activo { get; private set; } = true;
        public Usuario() {}
       
        
        public Usuario(string nombre, string email)
        {

            if (string.IsNullOrEmpty(nombre) || nombre.Trim().Length is < 2 or 100)
                throw new ArgumentOutOfRangeException(nameof(nombre),"2-100 caracteres");            
            Nombre = nombre.Trim();
            Email = email;
            FechaCreacion = DateTime.UtcNow;
        }

        public void AgregarDomicilio(string? calle, string? numero, string? provincia, string? ciudad)
        {
            if (!ValidateNullString([calle, numero, provincia, ciudad]))
                throw new InvalidOperationException("Datos nulos para domicilio");

            if (ExisteDomicilio(calle,numero,provincia,ciudad))
                throw new InvalidOperationException("Ya existe Domicilio para este Usuario");
            
            _domicilios.Add(new Domicilio(this, calle, numero, provincia, ciudad));
        }

        private bool ValidateNullString(string?[] value) => value.All(p => !string.IsNullOrEmpty(p));

        public void Renombrar(string name)
        {
            Nombre = name;
        }

        public void CambiarEmail(string email)
        {
            Email = email;
        }

        public bool ExisteDomicilio(string? calle, string? numero, string? provincia, string? ciudad)
            => _domicilios.Any(p => p.Calle.Equals(calle, StringComparison.OrdinalIgnoreCase) &&
                                    p.Numero.Equals(numero, StringComparison.OrdinalIgnoreCase) &&
                                    p.Provincia.Equals(provincia, StringComparison.OrdinalIgnoreCase) &&
                                    p.Ciudad.Equals(ciudad, StringComparison.OrdinalIgnoreCase));

        public Domicilio? BuscarDomicilioPorID (int domicilioID) => _domicilios.FirstOrDefault(p => p.ID == domicilioID);

        public void EliminarUsuario() 
        { 
            Activo = false; 
            if(_domicilios.Any())
            {
                foreach (var d in _domicilios)
                {
                    d.EliminarDomicilio();
                }
            }
            Actualizar();
        }
        
        private void Actualizar() => FechaUltimaActualizacion = DateTime.UtcNow;
    }
}
