using System.ComponentModel.DataAnnotations;

namespace EPTransporte.Models
{
    public class Sucursal
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El sitio es requerido")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "No puede contener solo espacios en blanco")]
        public string SitioEP { get; set; }

        [Required(ErrorMessage = "La ubicacion es requerida")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "No puede contener solo espacios en blanco")]

        public string Ubicacion { get; set; }

        public bool Habilitado { get; set; }
    }
}