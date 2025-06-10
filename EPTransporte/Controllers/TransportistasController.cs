using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using EPTransporte.Clases;
using EPTransporte.Models;

namespace EPTransporte.Controllers
{
    public class TransportistasController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var model = new Transportista
                {
                    Habilitado = true
                };

                ViewBag.TransportistasList = GetTransportistas();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la lista de transportistas: " + ex.Message;
                return View(new Transportista());
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                DataRow row = conexion.ObtenerTodosTransportistas()
                    .AsEnumerable()
                    .FirstOrDefault(r => r.Field<int>("Id") == id);

                if (row == null)
                {
                    TempData["ErrorMessage"] = "Transportista no encontrado";
                    return RedirectToAction("Index");
                }

                var model = new Transportista
                {
                    Id = id,
                    EmpresaContratista = row["EmpresaContratista"].ToString(),
                    Habilitado = Convert.ToBoolean(row["Habilitado"])
                };

                ViewBag.TransportistasList = GetTransportistas();
                return View("Index", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar el transportista: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Transportista model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar si el transportista ya existe
                    bool existe = conexion.VerificarExistenciaTransportista(
                        model.EmpresaContratista,
                        model.Id > 0 ? model.Id : (int?)null);

                    if (existe)
                    {
                        TempData["ErrorMessage"] = "El transportista ya existe";
                        ViewBag.TransportistasList = GetTransportistas();
                        return View("Index", model);
                    }

                    if (model.Id > 0)
                    {
                        bool actualizado = conexion.ActualizarTransportista(
                            model.Id,
                            model.EmpresaContratista,
                            model.Habilitado);

                        if (actualizado)
                        {
                            TempData["SuccessMessage"] = "Transportista actualizado correctamente";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        int id = conexion.InsertarTransportista(
                            model.EmpresaContratista,
                            model.Habilitado);

                        if (id > 0)
                        {
                            TempData["SuccessMessage"] = "Transportista creado exitosamente";
                            return RedirectToAction("Index");
                        }
                    }

                    TempData["ErrorMessage"] = "No se pudo guardar el transportista";
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor corrija los errores en el formulario.";
                }

                ViewBag.TransportistasList = GetTransportistas();
                return View("Index", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al guardar el transportista: {ex.Message}";
                if (ex.InnerException != null)
                {
                    TempData["ErrorMessage"] += $"<br/>Detalles: {ex.InnerException.Message}";
                }

                ViewBag.TransportistasList = GetTransportistas();
                return View("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleEstado(int id)
        {
            try
            {
                bool nuevoEstado = conexion.CambiarEstadoTransportista(id);

                return Json(new
                {
                    success = true,
                    nuevoEstado = nuevoEstado,
                    message = $"Estado cambiado a {(nuevoEstado ? "Activo" : "Inactivo")}"
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "Error interno al procesar la solicitud"
                });
            }
        }

        #region Métodos auxiliares

        private List<Transportista> GetTransportistas()
        {
            var transportistas = new List<Transportista>();
            DataTable dt = conexion.ObtenerTodosTransportistas();

            foreach (DataRow row in dt.Rows)
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