using System;
using System.Collections.Generic;
using System.Text;

namespace UTNApiTalleres.Model
{
    public class DiaLaboral
    {
       
            public int Id { get; set; }
            public string Dia_semana { get; set; }
            public TimeSpan Hora_inicio { get; set; }
            public TimeSpan Hora_fin { get; set; }
            public bool es_excepcion { get; set; }
        
    }
}
