using System;
using System.ComponentModel.DataAnnotations;

namespace EPTransporte.Models
{
    public class reporteFechas
    {
        [Required]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required]
        [Display(Name = "Fecha Final")]
        public DateTime FechaFinal { get; set; }
    }
}