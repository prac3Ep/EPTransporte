using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using EPTransporte.Clases;
using EPTransporte.Models;

namespace EPTransporte.Controllers
{
    public class UsuariosController : Controller
    {
        public ActionResult Index()
        {
            ModelState.Clear();
            try
            {
                var model = new UsuarioViewModel
                {
                    Activo = true,
                    SucursalesDisponibles = ObtenerSucursalesDisponibles()
                };

                ViewBag.UsuariosList = GetUsuarios();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la lista de usuarios: " + ex.Message;
                return View(new UsuarioViewModel());
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                DataRow usuarioData = conexion.ObtenerUsuarioPorId(id);
                if (usuarioData == null)
                {
                    TempData["ErrorMessage"] = "Usuario no encontrado";
                    return RedirectToAction("Index");
                }

                var model = new UsuarioViewModel
                {
                    Id = id,
                    Nombre = usuarioData["nombre"].ToString(),
                    UserName = usuarioData["usuario"].ToString(),
                    Activo = Convert.ToBoolean(usuarioData["activo"]),
                    SucursalesDisponibles = ObtenerSucursalesDisponibles(),
                    SucursalesSeleccionadas = conexion.ObtenerSucursalesUsuario(id)
                };

                ViewBag.UsuariosList = GetUsuarios();
                return View("Index", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar el usuario: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(UsuarioViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validación adicional para contraseña en nuevos usuarios
                    if (model.Id == 0 && string.IsNullOrEmpty(model.Password))
                    {
                        TempData["ErrorMessage"] = "Debe ingresar una contraseña para crear un nuevo usuario";
                        model.SucursalesDisponibles = ObtenerSucursalesDisponibles();
                        ViewBag.UsuariosList = GetUsuarios();
                        return View("Index", model);
                    }

                    if (model.Id > 0)
                    {
                        // Obtener el usuario actual para comparar el nombre de usuario
                        DataRow usuarioActual = conexion.ObtenerUsuarioPorId(model.Id);
                        if (usuarioActual == null)
                        {
                            TempData["ErrorMessage"] = "Usuario no encontrado";
                            return RedirectToAction("Index");
                        }

                        string usuarioActualNombre = usuarioActual["usuario"].ToString();

                        // Si el nombre de usuario ha cambiado, verificar si ya existe
                        if (!string.Equals(model.UserName, usuarioActualNombre, StringComparison.OrdinalIgnoreCase))
                        {
                            bool usuarioExiste = conexion.VerificarExistenciaUsuario(model.UserName);
                            if (usuarioExiste)
                            {
                                TempData["ErrorMessage"] = "El nombre de usuario ya está en uso";
                                model.SucursalesDisponibles = ObtenerSucursalesDisponibles();
                                ViewBag.UsuariosList = GetUsuarios();
                                return View("Index", model);
                            }
                        }

                        bool usuarioActualizado = conexion.ActualizarUsuario(
                            model.Id,
                            model.Nombre,
                            model.UserName,
                            string.IsNullOrEmpty(model.Password) ? null : model.Password,
                            model.Activo);

                        if (!usuarioActualizado)
                        {
                            TempData["ErrorMessage"] = "No se pudo actualizar la información básica del usuario";
                            model.SucursalesDisponibles = ObtenerSucursalesDisponibles();
                            ViewBag.UsuariosList = GetUsuarios();
                            return View("Index", model);
                        }

                        if (!conexion.LimpiarSucursalesUsuario(model.Id))
                        {
                            TempData["ErrorMessage"] = "No se pudieron eliminar las sucursales actuales";
                            model.SucursalesDisponibles = ObtenerSucursalesDisponibles();
                            ViewBag.UsuariosList = GetUsuarios();
                            return View("Index", model);
                        }

                        if (model.SucursalesSeleccionadas != null && model.SucursalesSeleccionadas.Any())
                        {
                            foreach (var idSucursal in model.SucursalesSeleccionadas)
                            {
                                if (!conexion.AsignarSucursalAUsuario(model.Id, idSucursal))
                                {
                                    TempData["ErrorMessage"] = "Error al asignar sucursales al usuario";
                                    model.SucursalesDisponibles = ObtenerSucursalesDisponibles();
                                    ViewBag.UsuariosList = GetUsuarios();
                                    return View("Index", model);
                                }
                            }
                        }

                        TempData["SuccessMessage"] = "Usuario actualizado correctamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Validación para nuevo usuario
                        bool usuarioExiste = conexion.VerificarExistenciaUsuario(model.UserName);
                        if (usuarioExiste)
                        {
                            TempData["ErrorMessage"] = "El nombre de usuario ya está en uso";
                            model.SucursalesDisponibles = ObtenerSucursalesDisponibles();
                            ViewBag.UsuariosList = GetUsuarios();
                            return View("Index", model);
                        }

                        if (string.IsNullOrEmpty(model.Password))
                        {
                            TempData["ErrorMessage"] = "La contraseña es requerida para nuevos usuarios";
                            model.SucursalesDisponibles = ObtenerSucursalesDisponibles();
                            ViewBag.UsuariosList = GetUsuarios();
                            return View("Index", model);
                        }

                        int idUsuario = conexion.InsertarUsuario(
                            model.Nombre,
                            model.UserName,
                            model.Password);

                        if (idUsuario > 0)
                        {
                            // Asignar sucursales
                            if (model.SucursalesSeleccionadas != null && model.SucursalesSeleccionadas.Any())
                            {
                                foreach (var idSucursal in model.SucursalesSeleccionadas)
                                {
                                    if (!conexion.AsignarSucursalAUsuario(idUsuario, idSucursal))
                                    {
                                        System.Diagnostics.Trace.TraceError($"Error al asignar sucursal {idSucursal}");
                                    }
                                }
                            }

                            TempData["SuccessMessage"] = "Usuario creado exitosamente";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "No se pudo crear el usuario";
                        }
                    }
                }
                else
                {
                    // Recolectar todos los mensajes de validación del ModelState
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();

                }

                model.SucursalesDisponibles = ObtenerSucursalesDisponibles();
                ViewBag.UsuariosList = GetUsuarios();
                return View("Index", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error inesperado al guardar el usuario: " +
                                         (ex.InnerException?.Message ?? ex.Message);

                model.SucursalesDisponibles = ObtenerSucursalesDisponibles();
                ViewBag.UsuariosList = GetUsuarios();
                return View("Index", model);
            }
        }

        [HttpPost]
        public JsonResult ToggleStatus(int id)
        {
            try
            {
                string resultado = conexion.CambiarEstadoUsuario(id);
                return Json(new { success = true, message = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al cambiar estado: " + ex.Message });
            }
        }

        public class ValidatePasswordAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (UsuarioViewModel)validationContext.ObjectInstance;

                // Solo validar si es nuevo usuario o si se dio una contraseña
                if (model.Id == 0 || !string.IsNullOrEmpty(value as string))
                {
                    if (model.Id == 0 && string.IsNullOrEmpty(value as string))
                    {
                        return new ValidationResult("La contraseña es requerida para nuevos usuarios");
                    }

                    if (!string.IsNullOrEmpty(value as string) && value.ToString().Length < 6)
                    {
                        return new ValidationResult("La contraseña debe tener al menos 6 caracteres");
                    }
                }

                return ValidationResult.Success;
            }
        }

        public class ValidateConfirmPasswordAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (UsuarioViewModel)validationContext.ObjectInstance;

                // Solo validar si se proporcionó una contraseña
                if (!string.IsNullOrEmpty(model.Password))
                {
                    if (string.IsNullOrEmpty(value as string))
                    {
                        return new ValidationResult("Por favor confirme la contraseña");
                    }

                    if (value.ToString() != model.Password)
                    {
                        return new ValidationResult("Las contraseñas no coinciden");
                    }
                }

                return ValidationResult.Success;
            }
        }

        #region Métodos auxiliares

        private List<Usuario> GetUsuarios()
        {
            var usuarios = new List<Usuario>();
            DataTable dtUsuarios = conexion.ObtenerTodosUsuarios();

            foreach (DataRow row in dtUsuarios.Rows)
            {
                var usuario = new Usuario
                {
                    Id = Convert.ToInt32(row["id"]),
                    Nombre = row["nombre"].ToString(),
                    UserName = row["usuario"].ToString(),
                    FechaCreacion = Convert.ToDateTime(row["fechacreacion"]),
                    Activo = Convert.ToBoolean(row["activo"])
                };

                var sucursalesIds = conexion.ObtenerSucursalesUsuario(usuario.Id);
                usuario.Sucursales = ObtenerSucursalesPorIds(sucursalesIds);
                usuarios.Add(usuario);
            }

            return usuarios;
        }

        private List<Sucursal> ObtenerSucursalesPorIds(List<int> ids)
        {
            var sucursales = new List<Sucursal>();
            DataTable dtSucursales = conexion.ObtenerSucursales();

            foreach (DataRow row in dtSucursales.Rows)
            {
                int id = Convert.ToInt32(row["id"]);
                if (ids.Contains(id))
                {
                    sucursales.Add(new Sucursal
                    {
                        Id = id,
                        SitioEP = row["SitioEP"].ToString(),
                        Ubicacion = row["Ubicacion"].ToString(),
                        Habilitado = Convert.ToBoolean(row["habilitado"])
                    });
                }
            }
            return sucursales;
        }

        private List<Sucursal> ObtenerSucursalesDisponibles()
        {
            var sucursales = new List<Sucursal>();
            DataTable dtSucursales = conexion.ObtenerSucursalesDisponibles();

            foreach (DataRow row in dtSucursales.Rows)
            {
                sucursales.Add(new Sucursal
                {
                    Id = Convert.ToInt32(row["id"]),
                    SitioEP = row["SitioEP"].ToString(),
                    Ubicacion = row["Ubicacion"].ToString(),
                    Habilitado = Convert.ToBoolean(row["habilitado"])
                });
            }

            return sucursales.Where(s => s.Habilitado).ToList();
        }

        #endregion Métodos auxiliares
    }
}