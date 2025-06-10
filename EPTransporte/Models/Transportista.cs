using System.ComponentModel.DataAnnotations;

namespace EPTransporte.Models
{
    public class Transportista
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string EmpresaContratista { get; set; }

        public bool Habilitado { get; set; }
    }
}