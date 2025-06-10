using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using EPTransporte.Clases;
using EPTransporte.Models;

namespace EPTransporte.Controllers
{
    public class EconomicosController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var model = new EconomicoViewModel
                {
                    Habilitado = true,
                    TransportistasDisponibles = ObtenerTransportistasDisponibles()
                };

                ViewBag.EconomicosList = GetEconomicos();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la lista de economicos: " + ex.Message;
                return View(new EconomicoViewModel());
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                DataRow economicoData = conexion.ObtenerEconomicoPorId(id);
                if (economicoData == null) return HttpNotFound();

                var model = new EconomicoViewModel
                {
                    Id = id,
                    Nombre = economicoData["Economico"].ToString(),
                    Habilitado = Convert.ToBoolean(economicoData["Habilitado"]),
                    IdTransportista = Convert.ToInt32(economicoData["IdTransportista"]),
                    TransportistasDisponibles = ObtenerTransportistasDisponibles()
                };

                ViewBag.EconomicosList = GetEconomicos();
                return View("Index", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar el economico: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(EconomicoViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar si ya existe un económico con ese nombre
                    bool existe = conexion.VerificarExistenciaEconomico(
                        model.Nombre,
                        model.Id > 0 ? model.Id : (int?)null);

                    if (existe)
                    {
                        TempData["ErrorMessage"] = "Ya existe un económico con ese nombre";
                        model.TransportistasDisponibles = ObtenerTransportistasDisponibles();
                        ViewBag.EconomicosList = GetEconomicos();
                        return View("Index", model);
                    }

                    if (model.Id > 0)
                    {
                        // Actualizar
                        bool actualizado = conexion.ActualizarEconomico(
                            model.Id,
                            model.Nombre,
                            model.Habilitado,
                            model.IdTransportista);

                        if (actualizado)
                        {
                            TempData["SuccessMessage"] = "Economico actualizado correctamente";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        // Crear nuevo
                        int id = conexion.InsertarEconomico(
                            model.Nombre,
                            model.Habilitado,
                            model.IdTransportista);

                        if (id > 0)
                        {
                            TempData["SuccessMessage"] = "Economico creado exitosamente";
                            return RedirectToAction("Index");
                        }
                    }

                    TempData["ErrorMessage"] = "No se pudo guardar el Economico";
                }

                model.TransportistasDisponibles = ObtenerTransportistasDisponibles();
                ViewBag.EconomicosList = GetEconomicos();
                return View("Index", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al guardar economico: " + ex.Message;
                model.TransportistasDisponibles = ObtenerTransportistasDisponibles();
                ViewBag.EconomicosList = GetEconomicos();
                return View("Index", model);
            }
        }

        [HttpPost]
        public JsonResult ToggleStatus(int id)
        {
            try
            {
                bool nuevoEstado = conexion.CambiarEstadoEconomico(id);
                return Json(new
                {
                    success = true,
                    message = $"Económico {(nuevoEstado ? "habilitado" : "deshabilitado")} correctamente"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        #region Métodos auxiliares

        private List<Economico> GetEconomicos()
        {
            var economicos = new List<Economico>();
            DataTable dtEconomicos = conexion.ObtenerTodosEconomicos();

            foreach (DataRow row in dtEconomicos.Rows)
            {
                economicos.Add(new Economico
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Nombre = row["Economico"].ToString(),
                    Habilitado = Convert.ToBoolean(row["Habilitado"]),
                    IdTransportista = Convert.ToInt32(row["IdTransportista"]),
                    NombreTransportista = row["NombreTransportista"]?.ToString()
                });
            }

            return economicos;
        }

        private List<Transportista> ObtenerTransportistasDisponibles()
        {
            var transportistas = new List<Transportista>();
            DataTable dtTransportistas = conexion.ObtenerTransportistas();

            foreach (DataRow row in dtTransportistas.Rows)
            {
                transportistas.Add(new Transportista
                {
                    Id = Convert.ToInt32(row["Id"]),
                    EmpresaContratista = row["EmpresaContratista"].ToString(),
                    Habilitado = Convert.ToBoolean(row["Habilitado"])
                });
            }

            return transportistas;
        }

        #endregion Métodos auxiliares
    }
}