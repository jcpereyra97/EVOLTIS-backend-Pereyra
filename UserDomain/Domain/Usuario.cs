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
        /// <summary>
        /// Agrega un nuevo domicilio al usuario
        /// </summary>
        /// <param name="calle"></param>
        /// <param name="numero"></param>
        /// <param name="provincia"></param>
        /// <param name="ciudad"></param>
        /// <exception cref="Exception"></exception>
        public void AgregarDomicilio(string? calle, string? numero, string? provincia, string? ciudad)
        {
            // Validaciones
            if (!ValidateNullString([calle, numero, provincia, ciudad]))
                throw new Exception("Datos nulos para domicilio");
            // Verificar si ya existe un domicilio con los mismos datos (ignorando mayúsculas/minúsculas)
            if (ExisteDomicilio(calle,numero,provincia,ciudad))
                throw new Exception("Ya existe Domicilio para este Usuario");
            
            _domicilios.Add(new Domicilio(this, calle, numero, provincia, ciudad));
            Actualizar();
        }
        /// <summary>
        /// Valida que los strings no sean nulos o vacíos
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool ValidateNullString(string?[] value) => value.All(p => !string.IsNullOrEmpty(p));

        public void Renombrar(string name)
        {
            Nombre = name;
            Actualizar();
        }

        public void CambiarEmail(string email)
        {
            Email = email;
            Actualizar();
        }
        /// <summary>
        /// ExisteDomicilio verifica si un domicilio con los mismos datos ya existe para este usuario
        /// </summary>
        /// <param name="calle"></param>
        /// <param name="numero"></param>
        /// <param name="provincia"></param>
        /// <param name="ciudad"></param>
        /// <returns></returns>
        public bool ExisteDomicilio(string? calle, string? numero, string? provincia, string? ciudad)
            => _domicilios.Any(p => p.Calle.Equals(calle, StringComparison.OrdinalIgnoreCase) &&
                                    p.Numero.Equals(numero, StringComparison.OrdinalIgnoreCase) &&
                                    p.Provincia.Equals(provincia, StringComparison.OrdinalIgnoreCase) &&
                                    p.Ciudad.Equals(ciudad, StringComparison.OrdinalIgnoreCase));

        public Domicilio? BuscarDomicilioPorID (int domicilioID) => _domicilios.FirstOrDefault(p => p.ID == domicilioID);
        /// <summary>
        /// Elimina el usuario y todos sus domicilios. Marca como inactivo.
        /// </summary>
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
