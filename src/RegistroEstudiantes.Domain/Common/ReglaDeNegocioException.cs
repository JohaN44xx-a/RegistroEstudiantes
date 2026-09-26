using System;
using System.Collections.Generic;
using System.Text;

namespace RegistroEstudiantes.Domain.Common
{
    public class ReglaDeNegocioException : Exception
    {
        public ReglaDeNegocioException(string mensaje) : base(mensaje) { }
    }
}
