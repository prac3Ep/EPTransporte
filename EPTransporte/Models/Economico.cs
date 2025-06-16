using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPTransporte.Models
{
    // Modelo Operador
    public class Economico
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "No puede contener solo espacios en blanco")]
        public string Nombre { get; set; }

        public bool Habilitado { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un transportista")]
        public int IdTransportista { get; set; }

        public string NombreTransportista { get; set; }
    }

    // Modelo para la vista de creación/edición
    public class EconomicoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "No puede contener solo espacios en blanco")]

        public string Nombre { get; set; }

        public bool Habilitado { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un transportista")]
        public int IdTransportista { get; set; }

        public List<Transportista> TransportistasDisponibles { get; set; }
    }
}