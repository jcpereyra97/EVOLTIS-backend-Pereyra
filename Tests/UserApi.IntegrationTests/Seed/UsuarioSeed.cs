using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApi.IntegrationTests.Seed
{
    public  class UsuarioSeed
    {
        public string Nombre { get; set; } = default!;
        public string Email { get; set; } = default!;
        public IEnumerable<DomicilioSeed>? Domicilio { get; set; }
    }
    public  class DomicilioSeed
    {
        public string Calle { get; set; } = default!;
        public string Numero { get; set; } = default!;
        public string Provincia { get; set; } = default!;
        public string Ciudad { get; set; } = default!;
    }

}
