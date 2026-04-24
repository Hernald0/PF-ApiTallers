using System.Collections.Generic;
using UTNApiTalleres.Model;

namespace UTNApiTalleres.Application.DTOs
{
    public class AccesoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }

        public  string Agrupador { get; set; }

        public List<Permiso>?  Permisos { get; set; }

    }

    public class MenuGrupoDTO
    {
        public string Agrupador { get; set; }
        public List<AccesoDTO> Accesos { get; set; }
    }

}
