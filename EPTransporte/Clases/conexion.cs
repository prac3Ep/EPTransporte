using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using EPTransporte.Models;

namespace EPTransporte.Clases
{
    public class conexion
    {
        //private static string sConn = @"Server=localhost\SQLEXPRESS01;Database=TransporteEPTest;User ID=sa;Password=DesarrolloEP1*;TrustServerCertificate=True;";

        private static string sConn = @"Data Source = 192.168.50.11; Initial Catalog=TransporteEPTest; Persist Security Info=True;User ID=sa;Password=DesarrolloEP1*";//@np4jr#10!

        // private static string sConn = @"Data Source= 192.168.50.168,1433;Network Library=DBMSSOCN;Initial Catalog=myDataBase;User ID=sa;Password=@np4jr#10!;Connect Timeout=10";

        //private static string sConn = @"Server=192.168.50.168;Database=TransporteEP;Trusted_Connection=True;";

        private static SqlCommand comm;
        private static SqlConnection conn;
        private static DataTable dt;
        private static SqlDataAdapter da;



        // Dentro de tu clase conexion
        internal static List<DatosSalida> ObtenerUltimasSalidas(int cantidad)
        {
            var salidas = new List<DatosSalida>();
            DataTable dt = new DataTable();
            SqlConnection conn = null;
            SqlCommand comm = null;
            SqlDataAdapter da = null;

            try
            {
                conn = new SqlConnection(sConn);
                comm = new SqlCommand("Sel_Ultimas_Salidas", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@Cantidad", cantidad);

                da = new SqlDataAdapter(comm);
                conn.Open();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    salidas.Add(new DatosSalida
                    {
                        IdPaseSalida = Convert.ToInt32(dr["IdSalida"]),
                        Folio = dr["Folio"].ToString(),
                        Transportista = dr["Transportista"].ToString(),  // Cambiado de TransportistaNombre
                        Economico = dr["Economico"].ToString()         // Cambiado de EconomicoNombre
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error al obtener últimas salidas: {ex.Message}");
            }
            finally
            {
                if (da != null) da.Dispose();
                if (comm != null) comm.Dispose();
                if (conn != null && conn.State == ConnectionState.Open) conn.Close();
            }

            return salidas;
        }

        internal static List<DatosEntrada> ObtenerUltimasEntradas(int cantidad)
        {
            var entradas = new List<DatosEntrada>();
            DataTable dt = new DataTable();
            SqlConnection conn = null;
            SqlCommand comm = null;
            SqlDataAdapter da = null;

            try
            {
                conn = new SqlConnection(sConn);
                comm = new SqlCommand("Sel_Ultimas_Entradas", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@Cantidad", cantidad);

                da = new SqlDataAdapter(comm);
                conn.Open();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    entradas.Add(new DatosEntrada
                    {
                        IdPaseEntrada = Convert.ToInt32(dr["IdEntrada"]),
                        Transportista = dr["TransportistaNombre"].ToString(),
                        Economico = dr["EconomicoNombre"].ToString(),
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error al obtener últimas entradas: {ex.Message}");
            }
            finally
            {
                if (da != null) da.Dispose();
                if (comm != null) comm.Dispose();
                if (conn != null && conn.State == ConnectionState.Open) conn.Close();
            }

            return entradas;
        }

        internal static bool VerificarExistenciaEconomico(string nombreEconomico, int? idExcluir = null)
        {
            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Sel_Economico_Por_Nombre", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@NombreEconomico", nombreEconomico);

                    if (idExcluir.HasValue)
                    {
                        comm.Parameters.AddWithValue("@IdExcluir", idExcluir.Value);
                    }
                    else
                    {
                        comm.Parameters.AddWithValue("@IdExcluir", DBNull.Value);
                    }

                    conn.Open();
                    var result = comm.ExecuteScalar();

                    return result != null;
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Trace.TraceError($"Error SQL al verificar económico: {sqlEx.Message}");
                throw new Exception("Error al verificar la existencia del económico");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error al verificar económico: {ex.Message}");
                throw new Exception("Error al verificar la existencia del económico");
            }
        }

        internal static bool VerificarExistenciaLicencia(string licencia, int? idExcluir = null)
        {
            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Sel_Operador_Por_Licencia", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Licencia", licencia);

                    if (idExcluir.HasValue)
                    {
                        comm.Parameters.AddWithValue("@IdExcluir", idExcluir.Value);
                    }
                    else
                    {
                        comm.Parameters.AddWithValue("@IdExcluir", DBNull.Value);
                    }

                    conn.Open();
                    var result = comm.ExecuteScalar();

                    return result != null;
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Trace.TraceError($"Error SQL al verificar licencia: {sqlEx.Message}");
                throw new Exception("Error al verificar la existencia de la licencia");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error al verificar licencia: {ex.Message}");
                throw new Exception("Error al verificar la existencia de la licencia");
            }
        }

        internal static bool VerificarExistenciaTransportista(string empresaContratista, int? idExcluir = null)
        {
            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Sel_Transportista_Por_Nombre", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@EmpresaContratista", empresaContratista);

                    if (idExcluir.HasValue)
                    {
                        comm.Parameters.AddWithValue("@IdExcluir", idExcluir.Value);
                    }
                    else
                    {
                        comm.Parameters.AddWithValue("@IdExcluir", DBNull.Value);
                    }

                    conn.Open();
                    var result = comm.ExecuteScalar();

                    return result != null;
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Trace.TraceError($"Error SQL al verificar transportista: {sqlEx.Message}");
                throw new Exception("Error al verificar la existencia del transportista");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error al verificar transportista: {ex.Message}");
                throw new Exception("Error al verificar la existencia del transportista");
            }
        }

        internal static DataRow ObtenerSucursalPorId(int id)
        {
            DataTable dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Sucursal_Por_Id", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);

                    return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error al obtener sucursal por ID: {ex.Message}");
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        internal static bool VerificarExistenciaSitioEP(string SitioEP)
        {
            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Sel_SitioEP_Por_SitioEP", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("SitioEP", SitioEP);

                    conn.Open();
                    var result = comm.ExecuteScalar();

                    return result != null;
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Trace.TraceError($"Error SQL al verificar SitioEP: {sqlEx.Message}");
                throw new Exception("Error al verificar la existencia del SitioEP");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error al verificar SitioEP: {ex.Message}");
                throw new Exception("Error al verificar la existencia del SitioEP");
            }
        }

        internal static bool VerificarExistenciaUsuario(string nombreUsuario)
        {
            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Sel_Usuario_Por_Usuario", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@usuario", nombreUsuario);

                    conn.Open();
                    var result = comm.ExecuteScalar();

                    return result != null;
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Trace.TraceError($"Error SQL al verificar usuario: {sqlEx.Message}");
                throw new Exception("Error al verificar la existencia del usuario");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error al verificar usuario: {ex.Message}");
                throw new Exception("Error al verificar la existencia del usuario");
            }
        }

        internal static DataTable ObtenerOperadoresPorTransportistaNombre(string transportistaNombre)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Sel_Operadores_Por_Transportista_Nombre", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@TransportistaNombre", transportistaNombre);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener operadores por transportista: " + ex.Message);
            }
            return dt;
        }

        internal static DataTable ObtenerEconomicosPorTransportistaNombre(string transportistaNombre)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Sel_Economicos_Por_Transportista_Nombre", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@TransportistaNombre", transportistaNombre);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener economicos por transportista: " + ex.Message);
            }
            return dt;
        }

        internal static void GuardarSalida(dynamic oSalida)
        {
            try
            {
                using (comm = new SqlCommand("Ins_Salida", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;

                    comm.Parameters.AddWithValue("@SucursalEP", oSalida.SucursalEP);
                    comm.Parameters.AddWithValue("@IdTransportista", oSalida.IdTransportista);
                    comm.Parameters.AddWithValue("@IdOperador", oSalida.IdOperador);
                    comm.Parameters.AddWithValue("@IdEconomico", oSalida.IdEconomico);
                    comm.Parameters.AddWithValue("@LocalViaje", oSalida.LocalViaje);
                    comm.Parameters.AddWithValue("@Cabina", oSalida.Cabina);
                    comm.Parameters.AddWithValue("@Cargado", oSalida.Cargado);
                    comm.Parameters.AddWithValue("@Vacio", oSalida.Vacio);
                    comm.Parameters.AddWithValue("@Exp", oSalida.Expo);
                    comm.Parameters.AddWithValue("@Caja", oSalida.Caja);
                    comm.Parameters.AddWithValue("@NumCaja", oSalida.NumCaja ?? (object)DBNull.Value);
                    comm.Parameters.AddWithValue("@NumSello", oSalida.NumSello ?? (object)DBNull.Value);
                    comm.Parameters.AddWithValue("@Auditor", oSalida.Auditor);
                    var partesFolio = oSalida.Folio?.Split('-');
                    comm.Parameters.AddWithValue("@Folio", (partesFolio != null && partesFolio.Length > 1) ? partesFolio[1] : oSalida.Folio);
                    comm.Parameters.AddWithValue("@TransportistaNombre", oSalida.TransportistaNombre);
                    comm.Parameters.AddWithValue("@OperadorNombre", oSalida.OperadorNombre);
                    comm.Parameters.AddWithValue("@EconomicoNombre", oSalida.EconomicoNombre);

                    conn.Open();
                    comm.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar la salida: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        internal static DataTable ObtenerOperadoresPorTransportista(int idTransportista)
        {
            DataTable dt = new DataTable();
            SqlConnection conn = null;
            SqlCommand comm = null;
            SqlDataAdapter da = null;

            try
            {
                conn = new SqlConnection(sConn);
                comm = new SqlCommand("Sel_Operadores_Por_Transportista", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@IdTransportista", idTransportista);

                da = new SqlDataAdapter(comm);

                conn.Open();
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener operadores: " + ex.Message);
            }
            finally
            {
                // Cerrar en orden inverso a la apertura
                if (da != null)
                    da.Dispose();

                if (comm != null)
                    comm.Dispose();

                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return dt;
        }

        internal static DataTable ObtenerEconomicosPorTransportista(int idTransportista)
        {
            DataTable dt = new DataTable();
            SqlConnection conn = null;
            SqlCommand comm = null;
            SqlDataAdapter da = null;

            try
            {
                conn = new SqlConnection(sConn);
                comm = new SqlCommand("Sel_Economicos_Por_Transportista", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@IdTransportista", idTransportista);

                da = new SqlDataAdapter(comm);

                conn.Open();
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener economicos: " + ex.Message);
            }
            finally
            {
                if (da != null)
                    da.Dispose();

                if (comm != null)
                    comm.Dispose();

                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return dt;
        }

        internal static DataTable ObtenerTodosOperadores()
        {
            DataTable dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Todos_Operadores", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener operadores: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dt;
        }

        internal static DataTable ObtenerTodosEconomicos()
        {
            DataTable dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Todos_Economicos", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener economicos: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dt;
        }

        // Obtener operador por ID
        internal static DataRow ObtenerOperadorPorId(int id)
        {
            DataTable dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_OperadorPorId", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);

                    return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener operador: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        internal static DataRow ObtenerEconomicoPorId(int id)
        {
            DataTable dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_EconomicoPorId", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);

                    return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener economico: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        // Insertar operador
        internal static int InsertarOperador(string nombre, string licencia, bool habilitado, int idTransportista)
        {
            int id = -1;
            try
            {
                using (comm = new SqlCommand("Ins_Operador", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@NombreOperador", nombre);
                    comm.Parameters.AddWithValue("@Licencia", string.IsNullOrEmpty(licencia) ? DBNull.Value : (object)licencia);
                    comm.Parameters.AddWithValue("@Habilitado", habilitado);
                    comm.Parameters.AddWithValue("@IdTransportista", idTransportista);

                    conn.Open();
                    var result = comm.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                        id = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar operador: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return id;
        }

        internal static int InsertarEconomico(string nombre, bool habilitado, int idTransportista)
        {
            int id = -1;
            try
            {
                using (comm = new SqlCommand("Ins_Economico", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Economico", nombre);
                    comm.Parameters.AddWithValue("@Habilitado", habilitado);
                    comm.Parameters.AddWithValue("@IdTransportista", idTransportista);

                    conn.Open();
                    var result = comm.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                        id = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar economico: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return id;
        }

        // Actualizar operador
        internal static bool ActualizarOperador(int id, string nombre, string licencia, bool habilitado, int idTransportista)
        {
            try
            {
                using (comm = new SqlCommand("Upd_Operador", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);
                    comm.Parameters.AddWithValue("@NombreOperador", nombre);
                    comm.Parameters.AddWithValue("@Licencia", string.IsNullOrEmpty(licencia) ? DBNull.Value : (object)licencia);
                    comm.Parameters.AddWithValue("@Habilitado", habilitado);
                    comm.Parameters.AddWithValue("@IdTransportista", idTransportista);

                    conn.Open();
                    comm.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar operador: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        internal static bool ActualizarEconomico(int id, string nombre, bool habilitado, int idTransportista)
        {
            try
            {
                using (comm = new SqlCommand("Upd_Economico", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);
                    comm.Parameters.AddWithValue("@Economico", nombre);
                    comm.Parameters.AddWithValue("@Habilitado", habilitado);
                    comm.Parameters.AddWithValue("@IdTransportista", idTransportista);

                    conn.Open();
                    comm.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar economico: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        // Cambiar estado de operador
        internal static bool CambiarEstadoOperador(int id)
        {
            try
            {
                using (comm = new SqlCommand("Toggle_Operador", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    var result = comm.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                        return Convert.ToBoolean(result);
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cambiar estado de operador: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        internal static bool CambiarEstadoEconomico(int id)
        {
            try
            {
                using (comm = new SqlCommand("Toggle_Economico", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    var result = comm.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                        return Convert.ToBoolean(result);
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cambiar estado de economico: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        internal static DataTable ObtenerTransportistas()
        {
            DataTable dt = new DataTable();

            try
            {
                using (comm = new SqlCommand("Sel_Transportistas", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error al obtener transportistas: {sqlEx.Message}");
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return dt;
        }

        internal static DataTable ObtenerTodosTransportistas()
        {
            DataTable dt = new DataTable();

            try
            {
                using (comm = new SqlCommand("Sel_Todos_Transportistas", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error al obtener transportistas: {sqlEx.Message}");
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return dt;
        }

        // Obtener transportista por ID
        internal static DataRow ObtenerTransportistaPorId(int id)
        {
            DataTable dt = new DataTable();

            try
            {
                using (comm = new SqlCommand("Sel_TransportistaPorId", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(comm))
                    {
                        da.Fill(dt);
                    }

                    return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error al obtener transportista: {sqlEx.Message}");
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        internal static int InsertarTransportista(string empresaContratista, bool habilitado)
        {
            int id = -1;

            try
            {
                using (comm = new SqlCommand("Ins_Transportista", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@EmpresaContratista", empresaContratista);
                    comm.Parameters.AddWithValue("@Habilitado", habilitado);

                    conn.Open();
                    var result = comm.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        id = Convert.ToInt32(result);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error al insertar transportista: {sqlEx.Message}");
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return id;
        }

        // Actualizar transportista
        internal static bool ActualizarTransportista(int id, string empresaContratista, bool habilitado)
        {
            try
            {
                using (comm = new SqlCommand("Upd_Transportista", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);
                    comm.Parameters.AddWithValue("@EmpresaContratista", empresaContratista);
                    comm.Parameters.AddWithValue("@Habilitado", habilitado);

                    conn.Open();
                    comm.ExecuteNonQuery();
                    return true; // Asumir éxito si no hay excepción
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error al actualizar transportista: {sqlEx.Message}");
                return false;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        // Cambiar estado (toggle)
        internal static bool CambiarEstadoTransportista(int id)
        {
            try
            {
                using (comm = new SqlCommand("Toggle_Transportista", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    var result = comm.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToBoolean(result);
                    }
                    return false;
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error al cambiar estado: {sqlEx.Message}");
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public static DataRow ObtenerUsuarioPorId(int id)
        {
            DataRow usuario = null;
            DataTable dt = new DataTable();

            try
            {
                using (SqlCommand comm = new SqlCommand("Sel_Usuario_Por_Id", new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        usuario = dt.Rows[0];
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error al obtener usuario por ID: {ex.Message}");
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();

                dt.Dispose();
            }

            return usuario;
        }

        internal static bool AsignarSucursalAUsuario(int idUsuario, int idSucursal)
        {
            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Ins_Sucursal_A_Usuario", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@idUsuario", idUsuario);
                    comm.Parameters.AddWithValue("@idSucursal", idSucursal);

                    // Agregar parámetro de retorno
                    var returnParam = comm.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    conn.Open();
                    comm.ExecuteNonQuery();

                    int result = (int)returnParam.Value;

                    return result > 0;
                }
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 2627) // Violación de clave duplicada
            {
                return true; // Considerar como éxito si ya existe
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static bool LimpiarSucursalesUsuario(int id)
        {
            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Limpiar_Sucursales_Usuario", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@id", id);

                    conn.Open();
                    return comm.ExecuteNonQuery() >= 0; // Puede devolver 0 si no había sucursales
                }
            }
            catch
            {
                return false;
            }
        }

        internal static List<int> ObtenerSucursalesUsuario(int idUsuario)
        {
            var sucursales = new List<int>();
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(sConn))
            {
                try
                {
                    using (SqlCommand comm = new SqlCommand("Sel_Sucursales_Usuario", conn))
                    {
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.Parameters.AddWithValue("@idUsuario", idUsuario);

                        conn.Open();
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sucursales.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError($"Error al obtener sucursales del usuario: {ex.Message}");
                    throw;
                }
            }

            return sucursales;
        }

        internal static int InsertarUsuario(string nombre, string usuario, string password)
        {
            int id = -1;
            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Ins_Usuario", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;

                    comm.Parameters.AddWithValue("@nombre", nombre ?? (object)DBNull.Value);
                    comm.Parameters.AddWithValue("@usuario", usuario ?? (object)DBNull.Value);
                    comm.Parameters.AddWithValue("@password", password ?? (object)DBNull.Value);

                    conn.Open();
                    var result = comm.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        id = Convert.ToInt32(result);
                        Debug.WriteLine($"Usuario insertado correctamente. ID: {id}");
                    }
                    else
                    {
                        Debug.WriteLine("El procedimiento no devolvió un ID válido");
                        throw new Exception("No se recibió un ID válido del procedimiento almacenado");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Debug.WriteLine($"SQL Error (Número {sqlEx.Number}): {sqlEx.Message}");
                Debug.WriteLine($"Procedimiento: {sqlEx.Procedure}, Línea: {sqlEx.LineNumber}");

                if (sqlEx.Number == 50000)
                {
                    throw new Exception(sqlEx.Message);
                }
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error general: {ex.Message}");
                throw;
            }
            return id;
        }

        internal static bool ActualizarUsuario(int id, string nombre, string usuario, string password, bool activo)
        {
            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Upd_Usuario", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;

                    comm.Parameters.AddWithValue("@id", id);
                    comm.Parameters.AddWithValue("@nombre", nombre);
                    comm.Parameters.AddWithValue("@usuario", usuario);

                    if (string.IsNullOrEmpty(password))
                    {
                        System.Diagnostics.Trace.TraceInformation("Actualizando sin cambiar password");
                        comm.Parameters.Add("@password", SqlDbType.VarChar, 100).Value = DBNull.Value;
                    }
                    else
                    {
                        System.Diagnostics.Trace.TraceInformation("Actualizando con nuevo password");
                        comm.Parameters.AddWithValue("@password", password);
                    }

                    comm.Parameters.AddWithValue("@activo", activo);

                    conn.Open();
                    int rowsAffected = comm.ExecuteNonQuery();

                    System.Diagnostics.Trace.TraceInformation($"Filas afectadas: {rowsAffected}");
                    return rowsAffected > 0;
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Trace.TraceError($"ERROR SQL [Número {sqlEx.Number}]: {sqlEx.Message}");
                System.Diagnostics.Trace.TraceError($"Procedimiento: {sqlEx.Procedure}, Línea: {sqlEx.LineNumber}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error general: {ex.Message}");
                return false;
            }
        }

        internal static DataTable ObtenerUsuarios()
        {
            DataTable dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Usuarios", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener usuarios: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dt;
        }

        internal static DataTable ObtenerTodosUsuarios()
        {
            DataTable dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Todos_Usuarios", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener usuarios: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dt;
        }

        internal static string CambiarEstadoUsuario(int id)
        {
            try
            {
                using (comm = new SqlCommand("Toggle_Usuario", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@id", id);

                    conn.Open();
                    var result = comm.ExecuteScalar();
                    return result?.ToString() ?? "Estado cambiado";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cambiar estado de usuario: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        internal static int InsertarSucursal(string sitioEP, string ubicacion, bool habilitado)
        {
            int id = -1;

            try
            {
                using (comm = new SqlCommand("Ins_Sucursal", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;

                    comm.Parameters.AddWithValue("@SitioEP", sitioEP);
                    comm.Parameters.AddWithValue("@Ubicacion", string.IsNullOrEmpty(ubicacion) ? DBNull.Value : (object)ubicacion);
                    comm.Parameters.AddWithValue("@Habilitado", habilitado);

                    conn.Open();
                    var result = comm.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        id = Convert.ToInt32(result);
                        System.Diagnostics.Debug.WriteLine($"Sucursal insertada correctamente. ID: {id}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("El procedimiento no devolvió un ID válido");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error al insertar sucursal: {sqlEx.Number} - {sqlEx.Message}");
                throw new Exception($"Error de base de datos: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error general al insertar sucursal: {ex.Message}");
                throw new Exception("Error al intentar guardar la sucursal", ex);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return id;
        }

        internal static DataTable ObtenerSucursales()
        {
            DataTable dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Sucursales", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                comm.Dispose();
                dt.Dispose();
                throw new Exception("Error al obtener sucursales: " + ex.Message);
            }
            return dt;
        }

        internal static DataTable ObtenerTodasSucursales()
        {
            DataTable dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Todas_Sucursales", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                comm.Dispose();
                dt.Dispose();
                throw new Exception("Error al obtener sucursales: " + ex.Message);
            }
            return dt;
        }

        internal static DataTable ObtenerSucursalesDisponibles()
        {
            DataTable dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Sucursales_Disponibles", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                comm.Dispose();
                dt.Dispose();
                throw new Exception("Error al obtener sucursales: " + ex.Message);
            }
            return dt;
        }

        internal static bool ActualizarSucursal(int id, string sitioEP, string ubicacion, bool habilitado)
        {
            try
            {
                using (comm = new SqlCommand("Upd_Sucursal", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@id", id);
                    comm.Parameters.AddWithValue("@SitioEP", sitioEP);
                    comm.Parameters.AddWithValue("@Ubicacion", string.IsNullOrEmpty(ubicacion) ? DBNull.Value : (object)ubicacion);
                    comm.Parameters.AddWithValue("@Habilitado", habilitado);

                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                comm.Dispose();
                throw new Exception("Error al actualizar sucursal: " + ex.Message);
            }
        }

        internal static string CambiarEstadoSucursal(int id)
        {
            try
            {
                using (comm = new SqlCommand("Toggle_Sucursal", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@id", id);

                    conn.Open();
                    var result = comm.ExecuteScalar();
                    conn.Close();

                    return result?.ToString() ?? "Estado cambiado";
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                comm.Dispose();
                throw new Exception("Error al cambiar estado: " + ex.Message);
            }
        }

        internal static void EliminarSalida(int id)
        {
            try
            {
                using (comm = new SqlCommand("Del_Salida", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception)
            {
                conn.Close();
                comm.Dispose();
            }
            comm.Dispose();
        }
        internal static void EliminarEntrada(int id)
        {
            try
            {
                using (comm = new SqlCommand("Del_Entrada", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception)
            {
                conn.Close();
                comm.Dispose();
            }
            comm.Dispose();
        }

        internal static void EliminarEntradaGeneral(int id)
        {
            try
            {
                using (comm = new SqlCommand("Del_Entrada_General", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception)
            {
                conn.Close();
                comm.Dispose();
            }
            comm.Dispose();
        }

        internal static void guardarEtitar(EditSalida oSalidaEditar)
        {
            try
            {
                using (comm = new SqlCommand("Upd_Salida_Reg", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@idSalida", oSalidaEditar.IdSalida);
                    comm.Parameters.AddWithValue("@SucursalEP", oSalidaEditar.SucursalEP);
                    comm.Parameters.AddWithValue("@Transportista", oSalidaEditar.Transportista);
                    comm.Parameters.AddWithValue("@Operador", oSalidaEditar.Operador);
                    comm.Parameters.AddWithValue("@Economico", oSalidaEditar.Economico);
                    comm.Parameters.AddWithValue("@LocalViaje", oSalidaEditar.LocalViaje);
                    comm.Parameters.AddWithValue("@Cabina", oSalidaEditar.Cabina);
                    comm.Parameters.AddWithValue("@Cargado", oSalidaEditar.Cargado);
                    comm.Parameters.AddWithValue("@Vacio", oSalidaEditar.Vacio);
                    comm.Parameters.AddWithValue("@Exp", oSalidaEditar.Expo);
                    comm.Parameters.AddWithValue("@Caja", oSalidaEditar.Caja);
                    comm.Parameters.AddWithValue("@NumCaja", oSalidaEditar.NumCaja);
                    comm.Parameters.AddWithValue("@NumSello", oSalidaEditar.NumSello);
                    comm.Parameters.AddWithValue("@Auditor", oSalidaEditar.Auditor);
                    comm.Parameters.AddWithValue("@Folio", oSalidaEditar.Folio);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception)
            {
                conn.Close();
            }
        }

        

        internal static string ObtenerFolio()
        {
            dt = new DataTable();
            string folio = string.Empty;
            try
            {
                using (comm = new SqlCommand("Sel_Folio", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.Text;
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                    conn.Close();
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            folio = dr[0].ToString();
                        }
                    }
                    dt.Dispose();
                }
            }
            catch (Exception)
            {
                conn.Close();
                return folio;
            }
            return folio;
        }

        internal static EditSalida ObtenerSalidaEditar(int id)
        {
            EditSalida oSalidaEditar = new EditSalida();
            dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Salida", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                    conn.Close();
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            oSalidaEditar.IdSalida = Int64.Parse(dr[0].ToString());
                            oSalidaEditar.SucursalEP = dr[1].ToString();
                            oSalidaEditar.Transportista = dr[2].ToString();
                            oSalidaEditar.Operador = dr[3].ToString();
                            oSalidaEditar.Economico = dr[4].ToString();
                            oSalidaEditar.LocalViaje = bool.Parse(dr[5].ToString());
                            oSalidaEditar.Cabina = bool.Parse(dr[6].ToString());
                            oSalidaEditar.Cargado = bool.Parse(dr[7].ToString());
                            oSalidaEditar.Vacio = bool.Parse(dr[8].ToString());
                            oSalidaEditar.Expo = bool.Parse(dr[9].ToString());
                            oSalidaEditar.Caja = bool.Parse(dr[10].ToString());
                            oSalidaEditar.NumCaja = dr[11].ToString();
                            oSalidaEditar.NumSello = dr[12].ToString();
                            oSalidaEditar.Auditor = dr[13].ToString();
                            oSalidaEditar.Folio = dr[14].ToString();
                        }
                    }
                    dt.Dispose();
                }
            }
            catch (Exception)
            {
                conn.Close();
                comm.Dispose();
                dt.Dispose();
            }
            return oSalidaEditar;
        }

        

        internal static List<DatosSalida> ObtenerSalidasPendientes()
        {
            dt = new DataTable();
            List<DatosSalida> oSalidas = new List<DatosSalida>();
            try
            {
                using (comm = new SqlCommand("Sel_Salidas_Reg ", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.Text;
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                    conn.Close();
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            DatosSalida oSalida = new DatosSalida();
                            oSalida.IdPaseSalida = int.Parse(dr[0].ToString());
                            oSalida.Folio = dr[1].ToString();
                            oSalida.Economico = dr[2].ToString();
                            oSalida.Transportista = dr[3].ToString();
                            oSalidas.Add(oSalida);
                        }
                    }
                }
                dt.Dispose();
            }
            catch (Exception)
            {
                conn.Close();
                dt.Dispose();
                comm.Dispose();
            }
            return oSalidas;
        }
        internal static List<DatosEntrada> ObtenerEntradasPendientes()
        {
            List<DatosEntrada> oEntradas = new List<DatosEntrada>();
            DataTable dt = new DataTable();

            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Sel_Entradas_Reg", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure; // ← Corregido de Text a StoredProcedure

                    conn.Open();
                    using (var da = new SqlDataAdapter(comm))
                    {
                        da.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            oEntradas.Add(new DatosEntrada() // ← ¡Faltaba agregar a la lista!
                            {
                                IdPaseEntrada = Convert.ToInt32(dr["IdEntrada"]),
                                Economico = dr["EconomicoNombre"].ToString(),
                                Transportista = dr["TransportistaNombre"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Considera registrar el error para diagnóstico
                Console.WriteLine($"Error en ObtenerEntradasPendientes: {ex.Message}");
                // Devuelve lista vacía en caso de error
                return new List<DatosEntrada>();
            }

            return oEntradas;
        }

        internal static List<DatosEntrada> ObtenerEntradasGeneralesPendientes()
        {
            List<DatosEntrada> oEntradas = new List<DatosEntrada>();
            DataTable dt = new DataTable();

            try
            {
                using (var conn = new SqlConnection(sConn))
                using (var comm = new SqlCommand("Sel_Entradas_Generales_Reg", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure; // ← Corregido de Text a StoredProcedure

                    conn.Open();
                    using (var da = new SqlDataAdapter(comm))
                    {
                        da.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            oEntradas.Add(new DatosEntrada() // ← ¡Faltaba agregar a la lista!
                            {
                                IdPaseEntrada = Convert.ToInt32(dr["IdEntrada"]),
                                Economico = dr["EconomicoNombre"].ToString(),
                                Transportista = dr["TransportistaNombre"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Considera registrar el error para diagnóstico
                Console.WriteLine($"Error en ObtenerEntradasPendientes: {ex.Message}");
                // Devuelve lista vacía en caso de error
                return new List<DatosEntrada>();
            }

            return oEntradas;
        }
        internal static List<DatosSalidaReporte> ObtenerVistaReporte(string v1, string v2)
        {
            if (string.IsNullOrEmpty(v1) || string.IsNullOrEmpty(v2))
            {
                throw new ArgumentException("Las fechas no pueden ser nulas o vacías");
            }

            if (!DateTime.TryParse(v1, out DateTime fechaIni) || !DateTime.TryParse(v2, out DateTime fechaFin))
            {
                throw new ArgumentException("Formato de fecha inválido. Use yyyy-MM-dd");
            }

            var result = new List<DatosSalidaReporte>();
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(sConn))
                using (SqlCommand comm = new SqlCommand("Sel_Reporte_Vista", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;

                    comm.Parameters.Add("@FechaIni", SqlDbType.Date).Value = fechaIni;
                    comm.Parameters.Add("@FechaFin", SqlDbType.Date).Value = fechaFin;

                    conn.Open();

                    using (SqlDataReader reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var reporte = new DatosSalidaReporte
                            {
                                // 5. Mapeo seguro con nombres de columnas y manejo de NULLs
                                Folio = reader["Folio"]?.ToString() ?? string.Empty,
                                Sucursal = reader["SucursalEP"]?.ToString() ?? string.Empty,
                                Transportista = reader["Transportista"]?.ToString() ?? string.Empty,
                                Operador = reader["Operador"]?.ToString() ?? string.Empty,
                                Economico = reader["Economico"]?.ToString() ?? string.Empty,
                                FechaCaptura = reader["FechaCreacion"]?.ToString() ?? string.Empty,
                                FechaSalida = reader["FechaSalida"]?.ToString() ?? string.Empty
                            };
                            result.Add(reporte);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // 6. Manejo específico de errores SQL
                throw new Exception($"Error SQL (N° {sqlEx.Number}): {sqlEx.Message}\n" +
                                   $"SP: {sqlEx.Procedure}, Línea: {sqlEx.LineNumber}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado: {ex.Message}\n" +
                                   $"Stack Trace: {ex.StackTrace}");
            }

            return result;
        }

        internal static List<DatosEntradaReporte> ObtenerVistaReporteEntrada(string v1, string v2)
        {
            if (string.IsNullOrEmpty(v1) || string.IsNullOrEmpty(v2))
            {
                throw new ArgumentException("Las fechas no pueden ser nulas o vacías");
            }

            if (!DateTime.TryParse(v1, out DateTime fechaIni) || !DateTime.TryParse(v2, out DateTime fechaFin))
            {
                throw new ArgumentException("Formato de fecha inválido. Use yyyy-MM-dd");
            }

            var result = new List<DatosEntradaReporte>();
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(sConn))
                using (SqlCommand comm = new SqlCommand("Sel_Reporte_Vista_Entrada", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;

                    comm.Parameters.Add("@FechaIni", SqlDbType.Date).Value = fechaIni;
                    comm.Parameters.Add("@FechaFin", SqlDbType.Date).Value = fechaFin;

                    conn.Open();

                    using (SqlDataReader reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var reporte = new DatosEntradaReporte
                            {
                                IdPaseEntrada = reader["IdEntrada"]?.ToString() ?? string.Empty,
                                Sucursal = reader["SitioEP"]?.ToString() ?? string.Empty,
                                Transportista = reader["TransportistaNombre"]?.ToString() ?? string.Empty,
                                Operador = reader["OperadorNombre"]?.ToString() ?? string.Empty,
                                Economico = reader["EconomicoNombre"]?.ToString() ?? string.Empty,
                                FechaEntrada = reader["FechaCreacion"]?.ToString() ?? string.Empty,
                            };
                            result.Add(reporte);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // 6. Manejo específico de errores SQL
                throw new Exception($"Error SQL (N° {sqlEx.Number}): {sqlEx.Message}\n" +
                                   $"SP: {sqlEx.Procedure}, Línea: {sqlEx.LineNumber}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado: {ex.Message}\n" +
                                   $"Stack Trace: {ex.StackTrace}");
            }

            return result;
        }

        internal static List<DatosEntradaReporte> ObtenerVistaReporteEntradaGeneral(string v1, string v2)
        {
            if (string.IsNullOrEmpty(v1) || string.IsNullOrEmpty(v2))
            {
                throw new ArgumentException("Las fechas no pueden ser nulas o vacías");
            }

            if (!DateTime.TryParse(v1, out DateTime fechaIni) || !DateTime.TryParse(v2, out DateTime fechaFin))
            {
                throw new ArgumentException("Formato de fecha inválido. Use yyyy-MM-dd");
            }

            var result = new List<DatosEntradaReporte>();
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(sConn))
                using (SqlCommand comm = new SqlCommand("Sel_Reporte_Vista_Entrada_General", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;

                    comm.Parameters.Add("@FechaIni", SqlDbType.Date).Value = fechaIni;
                    comm.Parameters.Add("@FechaFin", SqlDbType.Date).Value = fechaFin;

                    conn.Open();

                    using (SqlDataReader reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var reporte = new DatosEntradaReporte
                            {
                                IdPaseEntrada = reader["IdEntrada"]?.ToString() ?? string.Empty,
                                Sucursal = reader["SitioEP"]?.ToString() ?? string.Empty,
                                Transportista = reader["TransportistaNombre"]?.ToString() ?? string.Empty,
                                Operador = reader["OperadorNombre"]?.ToString() ?? string.Empty,
                                Economico = reader["EconomicoNombre"]?.ToString() ?? string.Empty,
                                FechaEntrada = reader["FechaCreacion"]?.ToString() ?? string.Empty,
                            };
                            result.Add(reporte);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // 6. Manejo específico de errores SQL
                throw new Exception($"Error SQL (N° {sqlEx.Number}): {sqlEx.Message}\n" +
                                   $"SP: {sqlEx.Procedure}, Línea: {sqlEx.LineNumber}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado: {ex.Message}\n" +
                                   $"Stack Trace: {ex.StackTrace}");
            }

            return result;
        }

        internal static DataTable ObtenerReporteExcel(string v1, string v2)
        {
            dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Reporte_Vista_Excel", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@FechaIni", v1);
                    comm.Parameters.AddWithValue("@FechaFin", v2);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                    conn.Close();
                }
            }
            catch (Exception)
            {
                conn.Close();
                comm.Dispose();
                dt.Dispose();
            }
            return dt;
        }

        internal static DataTable ObtenerReporteExcelEntrada(string v1, string v2)
        {
            dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Reporte_Vista_Excel_Entrada", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@FechaIni", v1);
                    comm.Parameters.AddWithValue("@FechaFin", v2);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                    conn.Close();
                }
            }
            catch (Exception)
            {
                conn.Close();
                comm.Dispose();
                dt.Dispose();
            }
            return dt;
        }


        internal static DataTable ObtenerReporteExcelEntradaGeneral(string v1, string v2)
        {
            dt = new DataTable();
            try
            {
                using (comm = new SqlCommand("Sel_Reporte_Vista_Excel_Entrada_General", conn = new SqlConnection(sConn)))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@FechaIni", v1);
                    comm.Parameters.AddWithValue("@FechaFin", v2);
                    conn.Open();
                    da = new SqlDataAdapter(comm);
                    da.Fill(dt);
                    conn.Close();
                }
            }
            catch (Exception)
            {
                conn.Close();
                comm.Dispose();
                dt.Dispose();
            }
            return dt;
        }
        public static object EjecutarEscalar(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(sConn))
            using (SqlCommand comm = new SqlCommand(query, conn))
            {
                comm.Parameters.AddRange(parameters);
                conn.Open();
                return comm.ExecuteScalar();
            }
        }
    }
}