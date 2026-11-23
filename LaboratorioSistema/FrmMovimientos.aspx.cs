using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace LaboratorioSistema
{
    public partial class FrmMovimientos : System.Web.UI.Page
    {
        private readonly LaboratorioStore _store = new LaboratorioStore();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarArticulos();
                CargarMovimientos();
            }
        }

        private void MostrarMensaje(string texto, string tipo)
        {
            pnlMensaje.Visible = true;
            lblMensaje.Text = texto;

            string baseClass = "alert ";
            if (tipo == "exito") pnlMensaje.CssClass = baseClass + "alert-success";
            else if (tipo == "advertencia") pnlMensaje.CssClass = baseClass + "alert-warning";
            else pnlMensaje.CssClass = baseClass + "alert-error";
        }

        private void OcultarMensaje()
        {
            pnlMensaje.Visible = false;
            lblMensaje.Text = string.Empty;
        }

        private void CargarArticulos()
        {
            string previo = ddlArticuloMov.SelectedValue;
            var articulos = _store.ObtenerArticulosConExistencia()
                .Select(a => new
                {
                    Id = a.IdArticulo.ToString(),
                    Texto = string.Format("[{0}] {1} (Disp: {2})", a.Codigo, a.Nombre, a.CantidadDisponible)
                }).ToList();

            ddlArticuloMov.DataSource = articulos;
            ddlArticuloMov.DataTextField = "Texto";
            ddlArticuloMov.DataValueField = "Id";
            ddlArticuloMov.DataBind();
            ddlArticuloMov.Items.Insert(0, new ListItem("-- Seleccione artículo --", ""));

            if (!string.IsNullOrEmpty(previo) && ddlArticuloMov.Items.FindByValue(previo) != null)
            {
                ddlArticuloMov.SelectedValue = previo;
            }
        }

        protected void btnRegistrarMovimiento_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(ddlArticuloMov.SelectedValue, out int idArticulo))
                {
                    MostrarMensaje("Seleccione un artículo para el movimiento.", "advertencia");
                    return;
                }

                if (!int.TryParse(txtCantidadMov.Text.Trim(), out int cantidad) || cantidad <= 0)
                {
                    MostrarMensaje("Ingrese una cantidad válida mayor que cero.", "advertencia");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDescripcionMov.Text))
                {
                    MostrarMensaje("Ingrese una descripción o motivo del movimiento.", "advertencia");
                    return;
                }

                var tipo = (TipoMovimiento)Enum.Parse(typeof(TipoMovimiento), ddlTipoMovimiento.SelectedValue);
                var mov = _store.RegistrarMovimientoManual(
                    idArticulo,
                    tipo,
                    cantidad,
                    txtReferenciaId.Text.Trim(),
                    txtDescripcionMov.Text.Trim()
                );

                txtIdMovimiento.Text = mov.IdMovimiento.ToString();
                CargarArticulos();
                CargarMovimientos();
                MostrarMensaje(string.Format("Movimiento #{0} registrado en JSON.", mov.IdMovimiento), "exito");
            }
            catch (Exception ex)
            {
                MostrarMensaje("No se pudo registrar el movimiento: " + ex.Message, "error");
            }
        }

        protected void btnLimpiarMov_Click(object sender, EventArgs e)
        {
            txtIdMovimiento.Text = string.Empty;
            ddlArticuloMov.SelectedIndex = 0;
            ddlTipoMovimiento.SelectedIndex = 0;
            txtCantidadMov.Text = string.Empty;
            txtReferenciaId.Text = string.Empty;
            txtDescripcionMov.Text = string.Empty;
            OcultarMensaje();
        }

        protected void btnFiltrarMov_Click(object sender, EventArgs e)
        {
            DateTime? desde = null;
            DateTime? hasta = null;

            if (!string.IsNullOrWhiteSpace(txtFechaDesde.Text) &&
                DateTime.TryParseExact(txtFechaDesde.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fDesde))
            {
                desde = fDesde.Date;
            }

            if (!string.IsNullOrWhiteSpace(txtFechaHasta.Text) &&
                DateTime.TryParseExact(txtFechaHasta.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fHasta))
            {
                hasta = fHasta.Date.AddDays(1).AddTicks(-1);
            }

            if (desde.HasValue && hasta.HasValue && desde.Value > hasta.Value)
            {
                MostrarMensaje("La fecha 'desde' no puede ser mayor que la fecha 'hasta'.", "advertencia");
                return;
            }

            CargarMovimientos(txtBuscarMov.Text.Trim(), ddlFiltroTipoMov.SelectedValue, desde, hasta);
        }

        protected void btnTodosMov_Click(object sender, EventArgs e)
        {
            txtBuscarMov.Text = string.Empty;
            ddlFiltroTipoMov.SelectedValue = "TODOS";
            txtFechaDesde.Text = string.Empty;
            txtFechaHasta.Text = string.Empty;
            CargarMovimientos();
        }

        private void CargarMovimientos(string filtro = "", string tipo = "TODOS", DateTime? desde = null, DateTime? hasta = null)
        {
            var movimientos = _store.ObtenerMovimientos();
            var articulos = _store.ObtenerArticulosConExistencia().ToDictionary(a => a.IdArticulo, a => a.Nombre);

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                string f = filtro.ToLowerInvariant();
                movimientos = movimientos.Where(m =>
                    m.IdMovimiento.ToString().Contains(f) ||
                    (m.ReferenciaId ?? "").ToLowerInvariant().Contains(f) ||
                    (m.Descripcion ?? "").ToLowerInvariant().Contains(f) ||
                    (articulos.ContainsKey(m.IdArticulo) && articulos[m.IdArticulo].ToLowerInvariant().Contains(f))
                ).ToList();
            }

            if (tipo != "TODOS")
            {
                movimientos = movimientos.Where(m => m.TipoMovimiento.ToString().Equals(tipo, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (desde.HasValue)
            {
                movimientos = movimientos.Where(m => m.FechaHora >= desde.Value).ToList();
            }

            if (hasta.HasValue)
            {
                movimientos = movimientos.Where(m => m.FechaHora <= hasta.Value).ToList();
            }

            var filas = movimientos.OrderByDescending(m => m.FechaHora).Select(m => new
            {
                m.IdMovimiento,
                FechaHoraTexto = m.FechaHora.ToString("yyyy-MM-dd HH:mm"),
                TipoTexto = m.TipoMovimiento.ToString(),
                NombreArticulo = articulos.ContainsKey(m.IdArticulo) ? articulos[m.IdArticulo] : "ID " + m.IdArticulo,
                m.Cantidad,
                m.ReferenciaId,
                m.Descripcion
            }).ToList();

            gvMovimientos.DataSource = filas;
            gvMovimientos.DataBind();
        }
    }
}