using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using EPTransporte.Clases;
using EPTransporte.Models;

namespace EPTransporte.Controllers
{
    public class EntradaController : Controller
    {

        public ActionResult Index()
        {
            List<DatosEntrada> oEntrada = new List<DatosEntrada>();
            oEntrada = Clases.conexion.ObtenerEntradasPendientes();
            return View(oEntrada);
        }

        [HttpPost]
        public ActionResult BorrarEntrada(int id)
        {
            conexion.EliminarEntrada(id);

            return Content("1");
        }

        [HttpGet]
        public ActionResult VistaReporte()
        {
            List<DatosEntradaReporte> oEntrada = new List<DatosEntradaReporte>();
            ViewData["EntradaReporte"] = oEntrada;
            return View();
        }

        [HttpPost]
        public ActionResult VistaReporte(reporteFechas fecha)
        {
            if (!ModelState.IsValid)
            {
                List<DatosEntradaReporte> oEntrada = new List<DatosEntradaReporte>();
                ViewData["EntradaReporte"] = oEntrada;

                return View(fecha);
            }
            else
            {
                DataTable dt = new DataTable("excel");
                dt.Columns.AddRange(new DataColumn[3] {
                                        new DataColumn("Folio"),
                                        new DataColumn("Sucursal"),
                                        new DataColumn("Transportista")});

                List<DatosEntradaReporte> oEntrada = new List<DatosEntradaReporte>();
                oEntrada = conexion.ObtenerVistaReporteEntrada(fecha.FechaInicio.ToString("yyyy-MM-dd"), fecha.FechaFinal.ToString("yyyy-MM-dd"));
                ViewData["EntradaReporte"] = oEntrada;

                ViewBag.Fecha1 = fecha.FechaInicio.ToString("yyyy-MM-dd");
                ViewBag.Fecha2 = fecha.FechaFinal.ToString("yyyy-MM-dd");

                return View();
            }
        }

        [HttpGet]
        public ActionResult Export(string lafecha1, string fecha2)
        {
            DataTable dt = new DataTable("excel");
            dt.Columns.AddRange(new DataColumn[11] {
        new DataColumn("No.Entrada"),
        new DataColumn("Sucursal"),
        new DataColumn("Transportista"),
        new DataColumn("Operador"),
        new DataColumn("Economico"),
        new DataColumn("Cargado"),
        new DataColumn("Vacio"),
        new DataColumn("Botando"),
        new DataColumn("Numero de caja"),
        new DataColumn("Numero de sello"),
        new DataColumn("Fecha de Entrada")
    });

            DataTable myDT = conexion.ObtenerReporteExcelEntrada(lafecha1, fecha2);

            foreach (DataRow dr in myDT.Rows)
            {
                // Convertir valores booleanos a palomitas/tachitas
                string cargado = dr[5].ToString() == "Sí" ? "✓" : "✗";
                string vacio = dr[6].ToString() == "Sí" ? "✓" : "✗";
                string botando = dr[7].ToString() == "Sí" ? "✓" : "✗";

                dt.Rows.Add(
                    dr[0],  // id
                    dr[1],  // Sucursal
                    dr[2],  // Transportista
                    dr[3],  // Operador
                    dr[4],  // Economico
                    cargado,  // Cargado
                    vacio,  // Vacio
                    botando,  // Caja
                    dr[8], // Numero de caja
                    dr[9], // Numero de sello
                    dr[10]  // Fecha de Entrada
                );
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt);

                // Ajustar el ancho de las columnas automáticamente
                ws.Columns().AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Reporte_Trafico_Entradas_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                }
            }
        }
    }
}