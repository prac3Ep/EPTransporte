using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using EPTransporte.Clases;
using EPTransporte.Models;


namespace EPTransporte.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            int metaMensual = 200;
            int pasesEsteMes = ObtenerConteoPases(InicioMes(), FinMes());
            ViewBag.ProgresoMes = Math.Min(100, (int)((double)pasesEsteMes / metaMensual * 100));

            try
            {
                ViewBag.PasesHoy = ObtenerConteoPases(DateTime.Today, DateTime.Today);
                ViewBag.PasesSemana = ObtenerConteoPases(InicioSemana(), FinSemana());
                ViewBag.PasesMes = ObtenerConteoPases(InicioMes(), FinMes());


                ViewBag.SalidasPendientes = ObtenerSalidasPendientes();
                ViewBag.EntradasHoy = ObtenerEntradasHoy(DateTime.Today, DateTime.Today);

                ViewBag.UltimasSalidas = conexion.ObtenerUltimasSalidas(3);
                ViewBag.UltimasEntradas = conexion.ObtenerUltimasEntradas(3);

            }
            catch (Exception ex)
            {
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

        private int ObtenerEntradasNormalesHoy(DateTime fechaInicio, DateTime fechaFin)
        {
            string query = "SELECT COUNT(*) FROM Entradas WHERE FechaEntrada BETWEEN @FechaInicio AND @FechaFin";
            return Convert.ToInt32(conexion.EjecutarEscalar(query, new System.Data.SqlClient.SqlParameter("@FechaInicio", fechaInicio),
                                                          new System.Data.SqlClient.SqlParameter("@FechaFin", fechaFin.AddDays(1).AddSeconds(-1))));
        }

        private int ObtenerEntradasGeneralesHoy(DateTime fechaInicio, DateTime fechaFin)
        {
            string query = "SELECT COUNT(*) FROM EntradasGenerales WHERE FechaEntrada BETWEEN @FechaInicio AND @FechaFin";
            return Convert.ToInt32(conexion.EjecutarEscalar(query, new System.Data.SqlClient.SqlParameter("@FechaInicio", fechaInicio),
                                                          new System.Data.SqlClient.SqlParameter("@FechaFin", fechaFin.AddDays(1).AddSeconds(-1))));
        }

        private int ObtenerEntradasHoy(DateTime fechaInicio, DateTime fechaFin)
        {
            int entradasNormales = ObtenerEntradasNormalesHoy(fechaInicio, fechaFin);
            int entradasGenerales = ObtenerEntradasGeneralesHoy(fechaInicio, fechaFin);
            return entradasNormales + entradasGenerales;
        }

        private int ObtenerSalidasPendientes()
        {
            string query = "SELECT COUNT(*) FROM Salidas WHERE FechaCreacion = FechaSalida";
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