using System;
using System.Collections.Generic;

namespace EPTransporte.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
        public List<Sucursal> Sucursales { get; set; } = new List<Sucursal>();
    }
}