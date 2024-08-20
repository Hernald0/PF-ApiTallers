using System;
using System.Collections.Generic;
using System.Text;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Model
{
    public class ClienteVehiculoViewModel
    {
        public int IdCliente { get; set; }
        public  Vehiculo Vehiculo { get; set; }
    }

}
