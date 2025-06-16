using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using EPTransporte.Clases;
using EPTransporte.Models;

namespace EPTransporte.Controllers
{
    public class OperadoresController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var model = new OperadorViewModel
                {
                    Habilitado = true,
                    TransportistasDisponibles = ObtenerTransportistasDisponibles()
                };

                ViewBag.OperadoresList = GetOperadores();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la lista de operadores: " + ex.Message;
                return View(new OperadorViewModel());
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                DataRow operadorData = conexion.ObtenerOperadorPorId(id);
                if (operadorData == null) return HttpNotFound();

                var model = new OperadorViewModel
                {
                    Id = id,
                    Nombre = operadorData["NombreOPerador"].ToString(),
                    Licencia = operadorData["Licencia"]?.ToString(),
                    Habilitado = Convert.ToBoolean(operadorData["Habilitado"]),
                    IdTransportista = Convert.ToInt32(operadorData["IdTransportista"]),
                    TransportistasDisponibles = ObtenerTransportistasDisponibles()
                };

                ViewBag.OperadoresList = GetOperadores();
                return View("Index", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar el operador: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(OperadorViewModel model)
        {
            try
            {
                model.TransportistasDisponibles = ObtenerTransportistasDisponibles();
                ViewBag.OperadoresList = GetOperadores();

                if (ModelState.IsValid)
                {
                    if (string.IsNullOrWhiteSpace(model.Licencia))
                    {
                        ModelState.AddModelError("Licencia", "El número de licencia es requerido");
                        TempData["ErrorMessage"] = "El número de licencia es requerido";
                        return View("Index", model);
                    }

                    // Verificar duplicados
                    bool licenciaExiste = conexion.VerificarExistenciaLicencia(
                        model.Licencia,
                        model.Id > 0 ? model.Id : (int?)null);

                    if (licenciaExiste)
                    {
                        ModelState.AddModelError("Licencia", "Esta licencia ya está registrada");
                        TempData["ErrorMessage"] = "No se puede guardar: La licencia ya está registrada para otro operador";
                        return View("Index", model);
                    }

                    // Proceso de guardado
                    if (model.Id > 0)
                    {
                        bool actualizado = conexion.ActualizarOperador(
                            model.Id,
                            model.Nombre,
                            model.Licencia,
                            model.Habilitado,
                            model.IdTransportista);

                        if (actualizado)
                        {
                            TempData["SuccessMessage"] = "Operador actualizado correctamente";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        int id = conexion.InsertarOperador(
                            model.Nombre,
                            model.Licencia,
                            model.Habilitado,
                            model.IdTransportista);

                        if (id > 0)
                        {
                            TempData["SuccessMessage"] = "Operador creado exitosamente";
                            return RedirectToAction("Index");
                        }
                    }

                    TempData["ErrorMessage"] = "Error al guardar el operador";
                    return View("Index", model);
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor corrija los errores en el formulario";
                    return View("Index", model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al guardar operador: {ex.Message}";
                model.TransportistasDisponibles = ObtenerTransportistasDisponibles();
                ViewBag.OperadoresList = GetOperadores();
                return View("Index", model);
            }
        }

        [HttpPost]
        public JsonResult ToggleStatus(int id)
        {
            try
            {
                bool nuevoEstado = conexion.CambiarEstadoOperador(id);
                return Json(new
                {
                    success = true,
                    message = $"Operador {(nuevoEstado ? "habilitado" : "deshabilitado")} correctamente"
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

        public JsonResult ValidarLicenciaUnica(string licencia, int id = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(licencia))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                bool existe = conexion.VerificarExistenciaLicencia(licencia, id > 0 ? id : (int?)null);
                return Json(!existe, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                // En caso de error, asumimos que no existe para no bloquear al usuario
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        #region Métodos auxiliares

        private List<Operador> GetOperadores()
        {
            var operadores = new List<Operador>();
            DataTable dtOperadores = conexion.ObtenerTodosOperadores();

            foreach (DataRow row in dtOperadores.Rows)
            {
                operadores.Add(new Operador
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Nombre = row["NombreOPerador"].ToString(),
                    Licencia = row["Licencia"]?.ToString(),
                    Habilitado = Convert.ToBoolean(row["Habilitado"]),
                    IdTransportista = Convert.ToInt32(row["IdTransportista"]),
                    NombreTransportista = row["NombreTransportista"]?.ToString()
                });
            }

            return operadores;
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