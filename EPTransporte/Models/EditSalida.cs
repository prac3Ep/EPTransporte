using System;
using System.ComponentModel.DataAnnotations;

namespace EPTransporte.Models
{
    public class EditSalida
    {
        public Int64 IdSalida { get; set; }

        [Required(ErrorMessage = "El folio es requerido")]
        public string Folio { get; set; }

        [Required(ErrorMessage = "La sucursal es requerida")]
        [Display(Name = "Sucursal EP")]
        public string SucursalEP { get; set; }

        [Required(ErrorMessage = "El transportista es requerido")]
        [Display(Name = "Transportista")]
        public string Transportista { get; set; }

        [Required(ErrorMessage = "El operador es requerido")]
        [Display(Name = "Operador")]
        public string Operador { get; set; }

        [Required(ErrorMessage = "El económico es requerido")]
        [Display(Name = "Económico")]
        public string Economico { get; set; }

        [Display(Name = "Local/Viaje")]
        public bool LocalViaje { get; set; }

        [Display(Name = "Cabina")]
        public bool Cabina { get; set; }

        [Display(Name = "Cargado")]
        public bool Cargado { get; set; }

        [Display(Name = "Vacío")]
        public bool Vacio { get; set; }

        [Display(Name = "Exportación")]
        public bool Expo { get; set; }

        [Display(Name = "Caja")]
        public bool Caja { get; set; }

        [Display(Name = "Número de Caja")]
        public string NumCaja { get; set; }

        [Display(Name = "Número de Sello")]
        public string NumSello { get; set; }

        [Required(ErrorMessage = "El auditor es requerido")]
        public string Auditor { get; set; }
    }
}