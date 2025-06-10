using System.ComponentModel.DataAnnotations;

namespace EPTransporte.Models
{
    public class Sucursal
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El sitio es requerido")]
        public string SitioEP { get; set; }

        [Required(ErrorMessage = "La ubicacion es requerida")]
        public string Ubicacion { get; set; }

        public bool Habilitado { get; set; }
    }
}