using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using EPTransporte.Clases;
using EPTransporte.Models;

namespace EPTransporte.Controllers
{
    public class SalidaController : Controller
    {
        [HttpGet]
        public ActionResult EditarSalida(int id)
        {
            var oSalida = conexion.ObtenerSalidaEditar(id);

            if (oSalida == null)
            {
                return HttpNotFound();
            }

            // Cargar dropdowns
            var sucursalesData = conexion.ObtenerSucursalesDisponibles();
            ViewBag.Sucursales = new SelectList(
                sucursalesData.AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["SitioEP"].ToString(),
                    Text = row["SitioEP"].ToString()
                }),
                "Value", "Text", oSalida.SucursalEP);

            var transportistasData = conexion.ObtenerTransportistas();
            ViewBag.Transportistas = new SelectList(
                transportistasData.AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["EmpresaContratista"].ToString(),
                    Text = row["EmpresaContratista"].ToString()
                }),
                "Value", "Text", oSalida.Transportista);

            var auditoresData = conexion.ObtenerUsuarios();
            ViewBag.Auditores = new SelectList(
                auditoresData.AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["Nombre"].ToString(),
                    Text = row["Nombre"].ToString()
                }),
                "Value", "Text", oSalida.Auditor);

            // Cargar operadores y económicos del transportista actual
            if (!string.IsNullOrEmpty(oSalida.Transportista))
            {
                var operadoresData = conexion.ObtenerOperadoresPorTransportistaNombre(oSalida.Transportista);
                ViewBag.Operadores = new SelectList(
                    operadoresData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["NombreOperador"].ToString(),
                        Text = row["NombreOperador"].ToString()
                    }),
                    "Value", "Text", oSalida.Operador);

                var economicosData = conexion.ObtenerEconomicosPorTransportistaNombre(oSalida.Transportista);
                ViewBag.Economicos = new SelectList(
                    economicosData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["Economico"].ToString(),
                        Text = row["Economico"].ToString()
                    }),
                    "Value", "Text", oSalida.Economico);
            }

            return View(oSalida);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarEditar(EditSalida model)
        {
            if (!ModelState.IsValid)
            {
                // Reconstruir dropdowns si hay error de validación
                var sucursalesData = conexion.ObtenerSucursalesDisponibles();
                ViewBag.Sucursales = new SelectList(
                    sucursalesData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["SitioEP"].ToString(),
                        Text = row["SitioEP"].ToString()
                    }),
                    "Value", "Text", model.SucursalEP);

                var transportistasData = conexion.ObtenerTransportistas();
                ViewBag.Transportistas = new SelectList(
                    transportistasData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["EmpresaContratista"].ToString(),
                        Text = row["EmpresaContratista"].ToString()
                    }),
                    "Value", "Text", model.Transportista);

                if (!string.IsNullOrEmpty(model.Transportista))
                {
                    var operadoresData = conexion.ObtenerOperadoresPorTransportistaNombre(model.Transportista);
                    ViewBag.Operadores = new SelectList(
                        operadoresData.AsEnumerable().Select(row => new SelectListItem
                        {
                            Value = row["NombreOperador"].ToString(),
                            Text = row["NombreOperador"].ToString()
                        }),
                        "Value", "Text", model.Operador);

                    var economicosData = conexion.ObtenerEconomicosPorTransportistaNombre(model.Transportista);
                    ViewBag.Economicos = new SelectList(
                        economicosData.AsEnumerable().Select(row => new SelectListItem
                        {
                            Value = row["Economico"].ToString(),
                            Text = row["Economico"].ToString()
                        }),
                        "Value", "Text", model.Economico);
                }

                return View(model);
            }

            try
            {
                conexion.guardarEtitar(model);
                TempData["SuccessMessage"] = "Salida actualizada correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar la salida: " + ex.Message);

                // Reconstruir dropdowns en caso de error
                var sucursalesData = conexion.ObtenerSucursalesDisponibles();
                ViewBag.Sucursales = new SelectList(
                    sucursalesData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["SitioEP"].ToString(),
                        Text = row["SitioEP"].ToString()
                    }),
                    "Value", "Text", model.SucursalEP);

                var transportistasData = conexion.ObtenerTransportistas();
                ViewBag.Transportistas = new SelectList(
                    transportistasData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["EmpresaContratista"].ToString(),
                        Text = row["EmpresaContratista"].ToString()
                    }),
                    "Value", "Text", model.Transportista);

                return View(model);
            }
        }

        [HttpGet]
        public JsonResult GetOperadoresByTransportistaNombre(string transportistaNombre)
        {
            try
            {
                if (string.IsNullOrEmpty(transportistaNombre))
                {
                    return Json(new { error = "Nombre de transportista inválido" }, JsonRequestBehavior.AllowGet);
                }

                var operadores = Clases.conexion.ObtenerOperadoresPorTransportistaNombre(transportistaNombre);

                var operadoresList = operadores.AsEnumerable().Select(row => new
                {
                    Nombre = row.Field<string>("NombreOperador"),
                    Licencia = row.Field<string>("Licencia") ?? string.Empty
                }).ToList();

                return Json(operadoresList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error en GetOperadoresByTransportistaNombre: {ex}");
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetEconomicosByTransportistaNombre(string transportistaNombre)
        {
            try
            {
                if (string.IsNullOrEmpty(transportistaNombre))
                {
                    return Json(new { error = "Nombre de transportista inválido" }, JsonRequestBehavior.AllowGet);
                }

                var economicos = Clases.conexion.ObtenerEconomicosPorTransportistaNombre(transportistaNombre);

                var economicosList = economicos.AsEnumerable().Select(row => new
                {
                    Nombre = row.Field<string>("Economico")
                }).ToList();

                return Json(economicosList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error en GetEconomicosByTransportistaNombre: {ex}");
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult addSalida()
        {
            var model = new addSalida();
            model.Folio = Clases.conexion.ObtenerFolio();

            // Inicialización de valores booleanos
            model.LocalViaje = false;
            model.Cabina = false;
            model.Cargado = false;
            model.Vacio = false;
            model.Expo = false;
            model.Caja = false;

            // Cargar dropdowns
            var sucursalesData = conexion.ObtenerSucursalesDisponibles();
            ViewBag.Sucursales = new SelectList(
                sucursalesData.AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["SitioEP"].ToString(),
                    Text = row["SitioEP"].ToString()
                }),
                "Value", "Text");

            var transportistasData = conexion.ObtenerTransportistas();
            ViewBag.Transportistas = new SelectList(
                transportistasData.AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["Id"].ToString(),
                    Text = row["EmpresaContratista"].ToString()
                }),
                "Value", "Text");

            var auditoresData = conexion.ObtenerUsuarios();
            ViewBag.Auditores = new SelectList(
                auditoresData.AsEnumerable().Select(row => new SelectListItem
                {
                    Value = row["Nombre"].ToString(), // Usar el nombre como valor
                    Text = row["Nombre"].ToString()
                }),
                "Value", "Text");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addSalida(addSalida model)
        {
            if (!ModelState.IsValid)
            {
                // Reconstruir los dropdowns completamente
                var sucursalesData = conexion.ObtenerSucursalesDisponibles();
                ViewBag.Sucursales = new SelectList(
                    sucursalesData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["SitioEP"].ToString(),
                        Text = row["SitioEP"].ToString()
                    }),
                    "Value", "Text", model.SucursalEP);

                var transportistasData = conexion.ObtenerTransportistas();
                ViewBag.Transportistas = new SelectList(
                    transportistasData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["Id"].ToString(),
                        Text = row["EmpresaContratista"].ToString()
                    }),
                    "Value", "Text", model.IdTransportista);

                var auditoresData = conexion.ObtenerUsuarios();
                ViewBag.Auditores = new SelectList(
                    auditoresData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["Nombre"].ToString(),
                        Text = row["Nombre"].ToString()
                    }),
                    "Value", "Text", model.Auditor);

                if (model.IdTransportista > 0)
                {
                    var operadoresData = conexion.ObtenerOperadoresPorTransportista(model.IdTransportista);
                    ViewBag.Operadores = new SelectList(
                        operadoresData.AsEnumerable().Select(row => new SelectListItem
                        {
                            Value = row["Id"].ToString(),
                            Text = row["NombreOperador"].ToString()
                        }),
                        "Value", "Text", model.IdOperador);

                    var economicosData = conexion.ObtenerEconomicosPorTransportista(model.IdTransportista);
                    ViewBag.Economicos = new SelectList(
                        economicosData.AsEnumerable().Select(row => new SelectListItem
                        {
                            Value = row["Id"].ToString(),
                            Text = row["Economico"].ToString()
                        }),
                        "Value", "Text", model.IdEconomico);
                }

                return View(model);
            }

            try
            {
                var transportista = conexion.ObtenerTransportistaPorId(model.IdTransportista);
                var operador = conexion.ObtenerOperadorPorId(model.IdOperador);
                var economico = conexion.ObtenerEconomicoPorId(model.IdEconomico);

                // Validar si se obtuvo correctamente cada fila
                if (transportista == null || operador == null || economico == null)
                {
                    ModelState.AddModelError("", "No se pudo encontrar uno de los datos seleccionados. Verifica los IDs de transportista, operador o económico.");

                    // Volver a cargar dropdowns
                    var sucursalesData = conexion.ObtenerSucursalesDisponibles();
                    ViewBag.Sucursales = new SelectList(
                        sucursalesData.AsEnumerable().Select(row => new SelectListItem
                        {
                            Value = row["SitioEP"].ToString(),
                            Text = row["SitioEP"].ToString()
                        }), "Value", "Text", model.SucursalEP);

                    var transportistasData = conexion.ObtenerTransportistas();
                    ViewBag.Transportistas = new SelectList(
                        transportistasData.AsEnumerable().Select(row => new SelectListItem
                        {
                            Value = row["Id"].ToString(),
                            Text = row["EmpresaContratista"].ToString()
                        }), "Value", "Text", model.IdTransportista);

                    return View(model);
                }

                conexion.GuardarSalida(new
                {
                    Folio = model.Folio,
                    SucursalEP = model.SucursalEP,
                    IdTransportista = model.IdTransportista,
                    IdOperador = model.IdOperador,
                    IdEconomico = model.IdEconomico,
                    LocalViaje = model.LocalViaje,
                    Cabina = model.Cabina,
                    Cargado = model.Cargado,
                    Vacio = model.Vacio,
                    Expo = model.Expo,
                    Caja = model.Caja,
                    NumCaja = model.NumCaja,
                    NumSello = model.NumSello,
                    Auditor = model.Auditor,
                    TransportistaNombre = transportista["EmpresaContratista"].ToString(),
                    OperadorNombre = operador["NombreOperador"].ToString(),
                    EconomicoNombre = economico["Economico"].ToString()
                });
                TempData["SuccessMessage"] = "Salida creada correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar la salida: " + ex.Message);

                var sucursalesData = conexion.ObtenerSucursalesDisponibles();
                ViewBag.Sucursales = new SelectList(
                    sucursalesData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["SitioEP"].ToString(),
                        Text = row["SitioEP"].ToString()
                    }),
                    "Value", "Text", model.SucursalEP);

                var transportistasData = conexion.ObtenerTransportistas();
                ViewBag.Transportistas = new SelectList(
                    transportistasData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["Id"].ToString(),
                        Text = row["EmpresaContratista"].ToString()
                    }),
                    "Value", "Text", model.IdTransportista);

                var auditoresData = conexion.ObtenerUsuarios();
                ViewBag.Auditores = new SelectList(
                    auditoresData.AsEnumerable().Select(row => new SelectListItem
                    {
                        Value = row["Nombre"].ToString(),
                        Text = row["Nombre"].ToString()
                    }),
                    "Value", "Text", model.Auditor);

                return View(model);
            }
        }

        // Métodos AJAX para cargar operadores y economicos
        [HttpGet]
        public JsonResult GetOperadoresByTransportista(int idTransportista)
        {
            try
            {
                if (idTransportista <= 0)
                {
                    return Json(new { error = "ID de transportista inválido" }, JsonRequestBehavior.AllowGet);
                }

                var operadores = Clases.conexion.ObtenerOperadoresPorTransportista(idTransportista);

                // Convertir DataTable
                var operadoresList = operadores.AsEnumerable().Select(row => new
                {
                    Id = row.Field<int>("Id"),
                    NombreOperador = row.Field<string>("NombreOperador"),
                    Licencia = row.Field<string>("Licencia") ?? string.Empty
                }).ToList();

                return Json(operadoresList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetEconomicosByTransportista(int idTransportista)
        {
            try
            {
                if (idTransportista <= 0)
                {
                    return Json(new { error = "ID de transportista inválido" }, JsonRequestBehavior.AllowGet);
                }

                var economicos = Clases.conexion.ObtenerEconomicosPorTransportista(idTransportista);

                var economicosList = economicos.AsEnumerable().Select(row => new
                {
                    Id = row.Field<int>("Id"),
                    Economico = row.Field<string>("Economico")
                }).ToList();

                return Json(economicosList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Index()
        {
            List<DatosSalida> oSalida = new List<DatosSalida>();
            oSalida = Clases.conexion.ObtenerSalidasPendientes();
            return View(oSalida);
        }

        [HttpPost]
        public ActionResult BorrarSalida(int id)
        {
            conexion.EliminarSalida(id);

            return Content("1");
        }

        [HttpGet]
        public ActionResult VistaReporte()
        {
            List<DatosSalidaReporte> oSalida = new List<DatosSalidaReporte>();
            ViewData["SalidaReporte"] = oSalida;
            return View();
        }

        [HttpPost]
        public ActionResult VistaReporte(reporteFechas fecha)
        {
            if (!ModelState.IsValid)
            {
                List<DatosSalidaReporte> oSalida = new List<DatosSalidaReporte>();
                ViewData["SalidaReporte"] = oSalida;

                return View(fecha);
            }
            else
            {
                DataTable dt = new DataTable("excel");
                dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Folio"),
                                        new DataColumn("Sucursal"),
                                        new DataColumn("Transportista")});

                List<DatosSalidaReporte> oSalida = new List<DatosSalidaReporte>();
                oSalida = conexion.ObtenerVistaReporte(fecha.FechaInicio.ToString("yyyy-MM-dd"), fecha.FechaFinal.ToString("yyyy-MM-dd"));
                ViewData["SalidaReporte"] = oSalida;

                ViewBag.Fecha1 = fecha.FechaInicio.ToString("yyyy-MM-dd");
                ViewBag.Fecha2 = fecha.FechaFinal.ToString("yyyy-MM-dd");

                return View();
            }
        }

        [HttpGet]
        public ActionResult Export(string lafecha1, string fecha2)
        {
            DataTable dt = new DataTable("excel");
            dt.Columns.AddRange(new DataColumn[16] {
        new DataColumn("Folio"),
        new DataColumn("Sucursal"),
        new DataColumn("Transportista"),
        new DataColumn("Operador"),
        new DataColumn("Economico"),
        new DataColumn("Local/Viaje"),
        new DataColumn("Cabina"),
        new DataColumn("Cargado"),
        new DataColumn("Vacio"),
        new DataColumn("Exportacion"),
        new DataColumn("Caja"),
        new DataColumn("Numero de caja"),
        new DataColumn("Numero de sello"),
        new DataColumn("Auditor"),
        new DataColumn("Fecha de captura"),
        new DataColumn("Fecha de Salida")
    });

            DataTable myDT = conexion.ObtenerReporteExcel(lafecha1, fecha2);

            foreach (DataRow dr in myDT.Rows)
            {
                // Convertir valores booleanos a palomitas/tachitas
                string localViaje = dr[5].ToString() == "Sí" ? "✓" : "✗";
                string cabina = dr[6].ToString() == "Sí" ? "✓" : "✗";
                string cargado = dr[7].ToString() == "Sí" ? "✓" : "✗";
                string vacio = dr[8].ToString() == "Sí" ? "✓" : "✗";
                string exportacion = dr[9].ToString() == "Sí" ? "✓" : "✗";
                string caja = dr[10].ToString() == "Sí" ? "✓" : "✗";

                dt.Rows.Add(
                    dr[0],  // Folio
                    dr[1],  // Sucursal
                    dr[2],  // Transportista
                    dr[3],  // Operador
                    dr[4],  // Economico
                    localViaje,  // Local/Viaje
                    cabina,  // Cabina
                    cargado,  // Cargado
                    vacio,  // Vacio
                    exportacion,  // Exportacion
                    caja,  // Caja
                    dr[11], // Numero de caja
                    dr[12], // Numero de sello
                    dr[13], // Auditor
                    dr[14], // Fecha de captura
                    dr[15]  // Fecha de Salida
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
                        $"Reporte_Trafico_Salida_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                }
            }
        }
    }
}