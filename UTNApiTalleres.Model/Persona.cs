using System;
using System.Collections.Generic;

#nullable disable

namespace WebApiTalleres.Models
{
    public partial class Persona
    {     
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string RazonSocial { get; set; }
        public string Apellido { get; set; }
        public DateTime? FecNacimiento { get; set; }
        public int? IdLocalidad { get; set; }
        public string Barrio { get; set; }
        public string Direccion { get; set; }
        public short? NroDireccion { get; set; }
        public string Dpto { get; set; }
        public int? Piso { get; set; }
        public string Telcelular { get; set; }
        public string Telfijo { get; set; }
        public string Email { get; set; }
        public int? IdTipoIdentificador { get; set; }
        public int? NroIdentificacion { get; set; }
        public string TipoPersona { get; set; }
        public int? IdGenero { get; set; }
        public string Ocupacion { get; set; }
        public int? IdEstadoCivil { get; set; }
        public DateTime FechaAlta { get; set; }
        public string UsrAlta { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string UsrBaja { get; set; }
        public DateTime? FechaMod { get; set; }
        public string UsrMod { get; set; }
        
    }
}
