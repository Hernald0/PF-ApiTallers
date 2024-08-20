using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{
    public interface IConfiguracionDao
    {

        //Crear el nuevo objeto en la base
        Task<object> getDatos(string nombreVistaPopUp, string parametro );
            
    }
}
