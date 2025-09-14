using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApplication.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public string Resource { get; }
        public object? Key { get; }

        public NotFoundException(string resource, object? key = null, string? message = null)
            : base(message ?? $"{resource} no encontrado.")
        {
            Resource = resource;
            Key = key;
        }
    }
}
