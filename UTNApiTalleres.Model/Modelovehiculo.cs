﻿using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Modelovehiculo
    {
        public int Id { get; set; }
        public int IdMarca { get; set; }
        public string Nombre { get; set; }

        public virtual Marcavehiculo IdMarcaNavigation { get; set; }
    }
}
