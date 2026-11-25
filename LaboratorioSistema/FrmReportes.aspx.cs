using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LaboratorioSistema
{
    public partial class FrmReportes : System.Web.UI.Page
    {
        private readonly LaboratorioStore _store = new LaboratorioStore();

        [Serializable]
        public class FilaReporte
        {
            public DateTime Fecha { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Tipo { get; set; }
            public string Estado { get; set; }
            public string Ubicacion { get; set; }
            public int Cantidad { get; set; }
            public string Responsable { get; set; }
            public string Descripcion { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFechaInicio.Text = string.Empty;
                txtFechaFin.Text = string.Empty;
                ddlTipoReporte.SelectedValue = "Inventario";
                CargarReporte(false);
            }
        }

        protected void ddlTipoReporte_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarReporte(false);
        }

        protected void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            CargarReporte(true);
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            ddlTipoReporte.SelectedValue = "Inventario";
            ddlEstado.SelectedIndex = 0;
            txtFechaInicio.Text = string.Empty;
            txtFechaFin.Text = string.Empty;
            CargarReporte(false);
            lblMensaje.Text = "Filtros limpiados.";
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            List<FilaReporte> filas;
            if (!TryObtenerReporte(out filas))
            {
                return;
            }

            StringBuilder contenido = new StringBuilder();
            contenido.AppendLine(ObtenerTituloReporte());
            contenido.AppendLine(new string('=', 100));
            contenido.AppendLine("Fecha\tCódigo\tArtículo\tTipo\tCantidad\tEstado\tUbicación\tResponsable/Técnico\tDetalle");
            foreach (FilaReporte fila in filas)
            {
                contenido.AppendLine(string.Join("\t",
                    Formatear(fila.Fecha), fila.Codigo, fila.Nombre, fila.Tipo,
                    fila.Cantidad.ToString(), fila.Estado, fila.Ubicacion, fila.Responsable,
                    fila.Descripcion));
            }

            contenido.AppendLine(new string('-', 100));
            contenido.AppendLine(string.Format("Total de registros: {0}", filas.Count));

            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType = "text/plain";
            Response.ContentEncoding = Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment; filename=ReporteLaboratorio.txt");
            Response.Write(contenido.ToString());
            Response.End();
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("FrmPrincipal.aspx");
        }

        private bool CargarReporte(bool mostrarMensaje)
        {
            List<FilaReporte> filas;
            if (!TryObtenerReporte(out filas))
            {
                return false;
            }

            gvReportes.DataSource = filas;
            gvReportes.DataBind();
            lblTotalRegistros.Text = string.Format("Total de registros: {0}", filas.Count);
            if (mostrarMensaje)
            {
                lblMensaje.Text = "Reporte generado correctamente.";
            }
            return true;
        }

        private bool TryObtenerReporte(out List<FilaReporte> filas)
        {
            filas = new List<FilaReporte>();
            if (string.IsNullOrEmpty(ddlTipoReporte.SelectedValue))
            {
                lblMensaje.Text = "Seleccione un tipo de reporte.";
                return false;
            }

            DateTime? desde;
            DateTime? hasta;
            if (!TryLeerFechas(out desde, out hasta))
            {
                return false;
            }

            filas = CrearFilas(ddlTipoReporte.SelectedValue);
            filas = filas.Where(f =>
                    (!desde.HasValue || f.Fecha.Date >= desde.Value.Date) &&
                    (!hasta.HasValue || f.Fecha.Date <= hasta.Value.Date))
                .Where(CoincideConFiltrosRestantes)
                .OrderByDescending(f => f.Fecha)
                .ToList();
            return true;
        }

        private bool TryLeerFechas(out DateTime? desde, out DateTime? hasta)
        {
            desde = null;
            hasta = null;
            DateTime fecha = DateTime.MinValue;

            if (!string.IsNullOrWhiteSpace(txtFechaInicio.Text))
            {
                if (!DateTime.TryParseExact(txtFechaInicio.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                {
                    lblMensaje.Text = "La fecha inicial no es válida.";
                    return false;
                }
                desde = fecha;
            }

            if (!string.IsNullOrWhiteSpace(txtFechaFin.Text))
            {
                if (!DateTime.TryParseExact(txtFechaFin.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                {
                    lblMensaje.Text = "La fecha final no es válida.";
                    return false;
                }
                hasta = fecha;
            }

            if (desde.HasValue && hasta.HasValue && desde.Value.Date > hasta.Value.Date)
            {
                lblMensaje.Text = "La fecha inicial no puede ser posterior a la fecha final.";
                return false;
            }
            return true;
        }

        private List<FilaReporte> CrearFilas(string tipoReporte)
        {
            switch (tipoReporte)
            {
                case "Inventario":
                    return CrearFilasInventario();

                case "Utilizacion":
                    return CrearFilasUtilizacion();

                case "MantenimientoPendiente":
                    return CrearFilasMantenimientoPendiente();

                default:
                    return new List<FilaReporte>();
            }
        }

        private List<FilaReporte> CrearFilasInventario()
        {
            var inventario = _store.ObtenerArticulos().Select(a => new FilaReporte
            {
                Fecha = a.FechaAdquisicion,
                Codigo = a.Codigo.ToString(),
                Nombre = a.Nombre,
                Tipo = a.GetType().Name,
                Estado = a.Estado,
                Cantidad = a.Cantidad,
                Ubicacion = a.Ubicacion,
                Descripcion = a.Descripcion
            });

            return inventario.ToList();
        }

        private List<FilaReporte> CrearFilasUtilizacion()
        {
            var nombresArticulos = _store.ObtenerArticulos()
                .ToDictionary(a => a.Codigo, a => a.Nombre);

            var prestamos = _store.ObtenerPrestamos().Select(p => new FilaReporte
            {
                Fecha = p.FechaPrestamo,
                Codigo = p.IdPrestamo.ToString(),
                Nombre = "Préstamo",
                Tipo = "Préstamo",
                Estado = p.EstadoPrestamo.ToString(),
                Cantidad = p.ObtenerCantidadTotal(),
                Responsable = ObtenerResponsable(p.IdResponsable),
                Descripcion = p.Observaciones ?? string.Empty
            });

            var movimientos = _store.ObtenerMovimientos()
                .Select(m => new FilaReporte
                {
                    Fecha = m.FechaHora,
                    Codigo = m.IdArticulo.ToString(),
                    Nombre = nombresArticulos.ContainsKey(m.IdArticulo)
                        ? nombresArticulos[m.IdArticulo]
                        : "Artículo " + m.IdArticulo,
                    Tipo = m.TipoMovimiento.ToString(),
                    Estado = m.TipoMovimiento.ToString(),
                    Cantidad = m.Cantidad,
                    Descripcion = string.Format("{0} {1}", m.ReferenciaId, m.Descripcion).Trim()
                });

            return prestamos.Concat(movimientos).ToList();
        }

        private List<FilaReporte> CrearFilasMantenimientoPendiente()
        {
            return _store.ObtenerMantenimientos()
                .Where(m => m.EstadoMantenimiento != EstadoMantenimiento.Finalizado
                    && m.EstadoMantenimiento != EstadoMantenimiento.Cancelado)
                .Select(m => new FilaReporte
            {
                Fecha = m.FechaProgramada,
                Codigo = m.IdArticulo.ToString(),
                Nombre = "Mantenimiento #" + m.IdMantenimiento,
                Tipo = m.TipoMantenimiento.ToString(),
                Estado = m.EstadoMantenimiento.ToString(),
                Cantidad = m.Cantidad,
                Responsable = m.TecnicoResponsable,
                Descripcion = m.DescripcionProblema
            }).ToList();
        }

        private bool CoincideConFiltrosRestantes(FilaReporte fila)
        {
            string estado = ddlEstado.SelectedValue ?? string.Empty;

            return string.IsNullOrEmpty(estado)
                || fila.Estado.IndexOf(estado, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private string ObtenerTituloReporte()
        {
            switch (ddlTipoReporte.SelectedValue)
            {
                case "Inventario": return "REPORTE DE INVENTARIO";
                case "Utilizacion": return "REPORTE DE UTILIZACIÓN DE EQUIPOS";
                case "MantenimientoPendiente": return "REPORTE DE MANTENIMIENTO PENDIENTE";
                default: return "REPORTE DEL LABORATORIO";
            }
        }

        private string ObtenerResponsable(int idResponsable)
        {
            ResponsableInfo responsable = _store.BuscarResponsablePorId(idResponsable);
            return responsable == null ? string.Empty : responsable.Nombres + " " + responsable.Apellidos;
        }

        private static string Formatear(DateTime fecha)
        {
            return fecha.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        }
    }
}