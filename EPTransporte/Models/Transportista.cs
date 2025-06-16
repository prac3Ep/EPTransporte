using System.ComponentModel.DataAnnotations;

namespace EPTransporte.Models
{
    public class Transportista
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "No puede contener solo espacios en blanco")]

        public string EmpresaContratista { get; set; }

        public bool Habilitado { get; set; }
    }
}