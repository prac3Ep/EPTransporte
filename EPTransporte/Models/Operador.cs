using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EPTransporte.Models
{
    // Modelo Operador
    public class Operador
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "No puede contener solo espacios en blanco")]

        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe ingresar una licencia")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "No puede contener solo espacios en blanco")]

        public string Licencia { get; set; }

        public bool Habilitado { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un transportista")]
        public int IdTransportista { get; set; }

        public string NombreTransportista { get; set; }
    }

    public class OperadorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El número de licencia es requerido")]
        [StringLength(50, ErrorMessage = "La licencia no puede exceder 50 caracteres")]
        [Remote("ValidarLicenciaUnica", "Operadores", AdditionalFields = "Id",
                ErrorMessage = "Esta licencia ya está registrada")]
        public string Licencia { get; set; }

        public bool Habilitado { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un transportista")]
        [Display(Name = "Transportista")]
        public int IdTransportista { get; set; }

        public List<Transportista> TransportistasDisponibles { get; set; }
    }
}