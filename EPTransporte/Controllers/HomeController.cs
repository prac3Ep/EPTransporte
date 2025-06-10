using System;
using System.Web.Mvc;
using EPTransporte.Clases;

namespace EPTransporte.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            int metaMensual = 200;
            int pasesEsteMes = ObtenerConteoPases(InicioMes(), FinMes());
            ViewBag.ProgresoMes = Math.Min(100, (int)((double)pasesEsteMes / metaMensual * 100));
            ViewBag.ProgresoMes = Math.Min(100, (int)((double)pasesEsteMes / metaMensual * 100));

            try
            {
                // Obtener estadísticas
                ViewBag.PasesHoy = ObtenerConteoPases(DateTime.Today, DateTime.Today);
                ViewBag.PasesSemana = ObtenerConteoPases(InicioSemana(), FinSemana());
                ViewBag.PasesMes = ObtenerConteoPases(InicioMes(), FinMes());

                // Obtener transportistas y vehículos activos
                ViewBag.TransportistasActivos = ObtenerTransportistasActivos();
                ViewBag.VehiculosDisponibles = ObtenerVehiculosDisponibles();
            }
            catch (Exception ex)
            {
                // Registrar el error pero no impedir que la vista se muestre
                System.Diagnostics.Trace.TraceError($"Error al cargar datos del dashboard: {ex.Message}");
            }

            return View();
        }

        private int ObtenerConteoPases(DateTime fechaInicio, DateTime fechaFin)
        {
            string query = "SELECT COUNT(*) FROM Salidas WHERE FechaCreacion BETWEEN @FechaInicio AND @FechaFin";
            return Convert.ToInt32(conexion.EjecutarEscalar(query, new System.Data.SqlClient.SqlParameter("@FechaInicio", fechaInicio),
                                                          new System.Data.SqlClient.SqlParameter("@FechaFin", fechaFin.AddDays(1).AddSeconds(-1))));
        }

        private int ObtenerTransportistasActivos()
        {
            string query = "SELECT COUNT(*) FROM Transportistas WHERE Habilitado = 1";
            return Convert.ToInt32(conexion.EjecutarEscalar(query));
        }

        private int ObtenerVehiculosDisponibles()
        {
            string query = "SELECT COUNT(*) FROM Economicos WHERE Habilitado = 1";
            return Convert.ToInt32(conexion.EjecutarEscalar(query));
        }

        private DateTime InicioSemana()
        {
            DateTime today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            return today.AddDays(-1 * diff).Date;
        }

        private DateTime FinSemana()
        {
            return InicioSemana().AddDays(6);
        }

        private DateTime InicioMes()
        {
            DateTime today = DateTime.Today;
            return new DateTime(today.Year, today.Month, 1);
        }

        private DateTime FinMes()
        {
            DateTime today = DateTime.Today;
            return new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}