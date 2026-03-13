using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiTalleres.Models.Enum
{
    public enum EstadoOrden
    {
        ADiagnosticar = 1,
        EsperaConfirmacion  = 2,
        AEjecutar = 3,
        EnEjecucion = 4,
        Finalizado = 5,
        CanceladoCliente = 6,
        CanceladoTaller = 7,

    }
}


 