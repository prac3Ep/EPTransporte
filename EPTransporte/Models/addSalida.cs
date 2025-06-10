using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EPTransporte.Models
{
    public class addSalida
    {
        [Required(ErrorMessage = "El folio es requerido")]
        public string Folio { get; set; }

        [Required(ErrorMessage = "La sucursal es requerida")]
        public string SucursalEP { get; set; }

        [Required(ErrorMessage = "El transportista es requerido")]
        [DisplayName("Transportista")]
        public int IdTransportista { get; set; }

        [DisplayName("Operador")]
        public int IdOperador { get; set; }

        [Required(ErrorMessage = "El económico es requerido")]
        [DisplayName("Económico")]
        public int IdEconomico { get; set; }

        [DisplayName("Local/Viaje")]
        public bool LocalViaje { get; set; }

        [DisplayName("Cabina")]
        public bool Cabina { get; set; }

        [DisplayName("Cargado")]
        public bool Cargado { get; set; }

        [DisplayName("Vacío")]
        public bool Vacio { get; set; }

        [DisplayName("Exportación")]
        public bool Expo { get; set; }

        [DisplayName("Caja")]
        public bool Caja { get; set; }

        [DisplayName("Número de Caja")]
        public string NumCaja { get; set; }

        [DisplayName("Número de Sello")]
        public string NumSello { get; set; }

        [Required(ErrorMessage = "El auditor es requerido")]
        public string Auditor { get; set; }

        // Propiedades para mostrar nombres (no se guardan en BD)
        [DisplayName("Transportista")]
        public string TransportistaNombre { get; set; }

        [DisplayName("Operador")]
        public string OperadorNombre { get; set; }

        [DisplayName("Económico")]
        public string EconomicoNombre { get; set; }
    }
}