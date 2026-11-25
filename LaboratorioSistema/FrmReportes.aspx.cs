using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LaboratorioSistema
{
    public partial class FrmReportes : System.Web.UI.Page
    {
        private readonly LaboratorioStore _store = new LaboratorioStore();

        [Serializable]
        private class FilaReporte
        {
            public DateTime Fecha { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Tipo { get; set; }
            public string Estado { get; set; }
            public int Cantidad { get; set; }
            public string Responsable { get; set; }
            public string Descripcion { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFechaInicio.Text = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
                txtFechaFin.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
        }

        protected void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            CargarReporte(true);
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            ddlTipoReporte.SelectedIndex = 0;
            ddlEstado.SelectedIndex = 0;
            txtFechaInicio.Text = string.Empty;
            txtFechaFin.Text = string.Empty;
            txtResponsable.Text = string.Empty;
            txtBuscar.Text = string.Empty;
            gvReportes.DataSource = null;
            gvReportes.DataBind();
            lblTotalRegistros.Text = "Total de registros: 0";
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
            contenido.AppendLine("Fecha\tCódigo\tNombre\tTipo\tEstado\tCantidad\tResponsable\tDescripción");
            foreach (FilaReporte fila in filas)
            {
                contenido.AppendLine(string.Join("\t",
                    Formatear(fila.Fecha), fila.Codigo, fila.Nombre, fila.Tipo,
                    fila.Estado, fila.Cantidad.ToString(), fila.Responsable,
                    fila.Descripcion));
            }

            Response.Clear();
            Response.ContentType = "text/plain";
            Response.AddHeader("Content-Disposition", "attachment; filename=ReporteLaboratorio.txt");
            Response.Write(contenido.ToString());
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            if (!CargarReporte(false))
            {
                return;
            }

            ClientScript.RegisterStartupScript(
                GetType(), "imprimirReporte", "window.print();", true);
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
                .Where(CoincideConFiltros)
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
                    return _store.ObtenerArticulos().Select(a => new FilaReporte
                    {
                        Fecha = a.FechaAdquisicion,
                        Codigo = a.Codigo.ToString(),
                        Nombre = a.Nombre,
                        Tipo = a.GetType().Name,
                        Estado = a.Estado,
                        Cantidad = a.Cantidad,
                        Descripcion = a.Descripcion
                    }).ToList();

                case "Existencias":
                    return _store.ObtenerArticulosConExistencia().Select(a => new FilaReporte
                    {
                        Fecha = DateTime.Today,
                        Codigo = a.Codigo.ToString(),
                        Nombre = a.Nombre,
                        Tipo = a.Tipo,
                        Estado = a.Estado,
                        Cantidad = a.CantidadDisponible,
                        Descripcion = string.Format("Disponibles: {0}; Mantenimiento: {1}; Dañados: {2}; Fuera de servicio: {3}",
                            a.CantidadDisponible, a.CantidadEnMantenimiento, a.CantidadDanada, a.CantidadFueraServicio)
                    }).ToList();

                case "Prestamos":
                    return _store.ObtenerPrestamos().Select(p => new FilaReporte
                    {
                        Fecha = p.FechaPrestamo,
                        Codigo = p.IdPrestamo.ToString(),
                        Nombre = "Préstamo",
                        Tipo = "Préstamo",
                        Estado = p.EstadoPrestamo.ToString(),
                        Cantidad = p.ObtenerCantidadTotal(),
                        Responsable = ObtenerResponsable(p.IdResponsable),
                        Descripcion = p.Observaciones
                    }).ToList();

                case "Mantenimiento":
                    return _store.ObtenerMantenimientos().Select(m => new FilaReporte
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

                case "Movimientos":
                    return _store.ObtenerMovimientos().Select(m => new FilaReporte
                    {
                        Fecha = m.FechaHora,
                        Codigo = m.IdArticulo.ToString(),
                        Nombre = "Movimiento #" + m.IdMovimiento,
                        Tipo = m.TipoMovimiento.ToString(),
                        Estado = m.TipoMovimiento.ToString(),
                        Cantidad = m.Cantidad,
                        Descripcion = string.Format("{0} {1}", m.ReferenciaId, m.Descripcion).Trim()
                    }).ToList();

                case "Devoluciones":
                    return new List<FilaReporte>();

                default:
                    return new List<FilaReporte>();
            }
        }

        private bool CoincideConFiltros(FilaReporte fila)
        {
            string buscar = (txtBuscar.Text ?? string.Empty).Trim();
            string responsable = (txtResponsable.Text ?? string.Empty).Trim();
            string estado = ddlEstado.SelectedValue ?? string.Empty;
            string texto = string.Join(" ", fila.Codigo, fila.Nombre, fila.Tipo, fila.Estado, fila.Descripcion);

            return (string.IsNullOrEmpty(buscar) || texto.IndexOf(buscar, StringComparison.OrdinalIgnoreCase) >= 0)
                && (string.IsNullOrEmpty(responsable) || (fila.Responsable ?? string.Empty).IndexOf(responsable, StringComparison.OrdinalIgnoreCase) >= 0)
                && (string.IsNullOrEmpty(estado) || fila.Estado.IndexOf(estado, StringComparison.OrdinalIgnoreCase) >= 0);
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