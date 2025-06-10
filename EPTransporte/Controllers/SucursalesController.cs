using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using EPTransporte.Clases;
using EPTransporte.Models;

namespace EPTransporte.Controllers
{
    public class SucursalesController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var model = new Sucursal
                {
                    Habilitado = true
                };

                ViewBag.SucursalesList = GetSucursales();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la lista de sucursales: " + ex.Message;
                return View(new Sucursal());
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                DataRow row = conexion.ObtenerTodasSucursales()
                    .AsEnumerable()
                    .FirstOrDefault(r => r.Field<int>("id") == id);

                if (row == null)
                {
                    TempData["ErrorMessage"] = "Sucursal no encontrada";
                    return RedirectToAction("Index");
                }

                var model = new Sucursal
                {
                    Id = id,
                    SitioEP = row["SitioEP"].ToString(),
                    Ubicacion = row["Ubicacion"]?.ToString(),
                    Habilitado = Convert.ToBoolean(row["habilitado"])
                };

                ViewBag.SucursalesList = GetSucursales();
                return View("Index", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la sucursal: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Sucursal model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Id > 0) // Si es una edición
                    {
                        // Obtener la sucursal actual para comparar el SitioEP
                        DataRow sucursalActual = conexion.ObtenerSucursalPorId(model.Id);
                        if (sucursalActual == null)
                        {
                            TempData["ErrorMessage"] = "Sucursal no encontrada";
                            return RedirectToAction("Index");
                        }

                        string sitioEPActual = sucursalActual["SitioEP"].ToString();

                        // Si el SitioEP ha cambiado, verificar si ya existe
                        if (!string.Equals(model.SitioEP, sitioEPActual, StringComparison.OrdinalIgnoreCase))
                        {
                            bool sitioEPExiste = conexion.VerificarExistenciaSitioEP(model.SitioEP);
                            if (sitioEPExiste)
                            {
                                TempData["ErrorMessage"] = "El Sitio EP ya está registrado";
                                ViewBag.SucursalesList = GetSucursales();
                                return View("Index", model);
                            }
                        }

                        bool actualizado = conexion.ActualizarSucursal(
                            model.Id,
                            model.SitioEP,
                            model.Ubicacion,
                            model.Habilitado);

                        if (actualizado)
                        {
                            TempData["SuccessMessage"] = "Sucursal actualizada correctamente";
                            return RedirectToAction("Index");
                        }
                    }
                    else // Si es una nueva sucursal
                    {
                        bool sitioEPExiste = conexion.VerificarExistenciaSitioEP(model.SitioEP);
                        if (sitioEPExiste)
                        {
                            TempData["ErrorMessage"] = "El Sitio EP ya está registrado";
                            ViewBag.SucursalesList = GetSucursales();
                            return View("Index", model);
                        }

                        int id = conexion.InsertarSucursal(
                            model.SitioEP,
                            model.Ubicacion,
                            model.Habilitado);

                        if (id > 0)
                        {
                            TempData["SuccessMessage"] = "Sucursal creada exitosamente";
                            return RedirectToAction("Index");
                        }
                    }

                    TempData["ErrorMessage"] = "No se pudo guardar la sucursal";
                }
                else
                {
                    TempData["ErrorMessage"] = "Por favor corrija los errores en el formulario.";
                }

                ViewBag.SucursalesList = GetSucursales();
                return View("Index", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al guardar la sucursal: {ex.Message}";
                if (ex.InnerException != null)
                {
                    TempData["ErrorMessage"] += $"<br/>Detalles: {ex.InnerException.Message}";
                }

                ViewBag.SucursalesList = GetSucursales();
                return View("Index", model);
            }
        }

        [HttpPost]
        public JsonResult ToggleEstado(int id)
        {
            try
            {
                return Json(new
                {
                    success = true,
                    message = conexion.CambiarEstadoSucursal(id)
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

        private List<Sucursal> GetSucursales()
        {
            var sucursales = new List<Sucursal>();
            DataTable dt = conexion.ObtenerTodasSucursales();

            foreach (DataRow row in dt.Rows)
            {
                sucursales.Add(new Sucursal
                {
                    Id = Convert.ToInt32(row["id"]),
                    SitioEP = row["SitioEP"].ToString(),
                    Ubicacion = row["Ubicacion"]?.ToString(),
                    Habilitado = Convert.ToBoolean(row["habilitado"])
                });
            }

            return sucursales;
        }

        #endregion Métodos auxiliares
    }
}