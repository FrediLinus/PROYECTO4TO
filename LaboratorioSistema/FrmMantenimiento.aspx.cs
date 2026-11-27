using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;



namespace LaboratorioSistema
{
    public partial class FrmMantenimiento : System.Web.UI.Page
    {
        private readonly LaboratorioStore _store = new LaboratorioStore();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFechaProgramada.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtFechaInicio.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtFechaFinalizacion.Text = DateTime.Today.ToString("yyyy-MM-dd");

                CargarArticulos();
                CargarMantenimientos();
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

        private void MostrarModalArticulo(string mensaje)
        {
            string script = "document.getElementById('mensajeModalArticulo').innerText = '" +
                mensaje.Replace("'", "\\'") +
                "'; mostrarModalArticulo();";
            ClientScript.RegisterStartupScript(GetType(), "modalArticulo", script, true);
        }

        private void CargarArticulos()
        {
            string previo = ddlArticuloMant.SelectedValue;
            var articulos = _store.ObtenerArticulosConExistencia()
                .Select(a => new
                {
                    Id = a.IdArticulo.ToString(),
                    Texto = string.Format("[{0}] {1} ({2} - Disp: {3})", a.Codigo, a.Nombre, a.EstadoArticulo, a.CantidadDisponible)
                }).ToList();

            ddlArticuloMant.DataSource = articulos;
            ddlArticuloMant.DataTextField = "Texto";
            ddlArticuloMant.DataValueField = "Id";
            ddlArticuloMant.DataBind();
            ddlArticuloMant.Items.Insert(0, new ListItem("-- Seleccione artículo / equipo --", ""));

            if (!string.IsNullOrEmpty(previo) && ddlArticuloMant.Items.FindByValue(previo) != null)
            {
                ddlArticuloMant.SelectedValue = previo;
            }
            ActualizarInfoArticulo();
        }

        protected void ddlArticuloMant_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarInfoArticulo();
        }

        private void ActualizarInfoArticulo()
        {
            if (!int.TryParse(ddlArticuloMant.SelectedValue, out int idArt))
            {
                pnlInfoArticuloMant.Visible = false;
                lblInfoArticuloMant.Text = string.Empty;
                return;
            }

            var item = _store.BuscarArticuloConExistencia(idArt);
            if (item == null)
            {
                MostrarModalArticulo("El artículo seleccionado no aparece en el sistema.");
                return;
            }

            if (item != null)
            {
                pnlInfoArticuloMant.Visible = true;
                lblInfoArticuloMant.Text = string.Format("{0} ({1}) | Estado: {2} | Disponible: {3} | En mantenimiento: {4} | Dañado: {5}",
                    item.Nombre, item.Codigo, item.EstadoArticulo, item.CantidadDisponible, item.CantidadEnMantenimiento, item.CantidadDanada);
            }
        }

        protected void btnRegistrarMantenimiento_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtIdMantenimiento.Text.Trim(), out int idMantenimiento) || idMantenimiento <= 0)
                {
                    MostrarMensaje("Ingrese un ID de mantenimiento válido.", "advertencia");
                    return;
                }

                if (!int.TryParse(ddlArticuloMant.SelectedValue, out int idArticulo))
                {
                    MostrarMensaje("Seleccione un artículo o equipo para el mantenimiento.", "advertencia");
                    MostrarModalArticulo("El artículo seleccionado no aparece en el sistema.");
                    return;
                }

                if (_store.BuscarArticuloConExistencia(idArticulo) == null)
                {
                    MostrarModalArticulo("El artículo seleccionado no aparece en el sistema.");
                    return;
                }

                if (!int.TryParse(txtCantidadMant.Text.Trim(), out int cantidad) || cantidad <= 0)
                {
                    MostrarMensaje("Ingrese una cantidad válida mayor que cero.", "advertencia");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTecnicoResponsable.Text) || string.IsNullOrWhiteSpace(txtDescripcionProblema.Text))
                {
                    MostrarMensaje("Complete el técnico responsable y la descripción del problema o trabajo.", "advertencia");
                    return;
                }

                if (!DateTime.TryParseExact(txtFechaProgramada.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaProg))
                {
                    MostrarMensaje("Ingrese una fecha programada válida.", "advertencia");
                    return;
                }

                var tipo = (TipoMantenimiento)Enum.Parse(typeof(TipoMantenimiento), ddlTipoMantenimiento.SelectedValue);
                var mant = _store.RegistrarMantenimiento(
                    idMantenimiento,
                    idArticulo,
                    tipo,
                    cantidad,
                    fechaProg,
                    txtTecnicoResponsable.Text.Trim(),
                    txtDescripcionProblema.Text.Trim(),
                    txtObservacionesMant.Text.Trim()
                );

                txtIdMantenimiento.Text = mant.IdMantenimiento.ToString();
                ddlEstadoMantenimiento.SelectedValue = mant.EstadoMantenimiento.ToString();
                CargarArticulos();
                CargarMantenimientos();
                MostrarMensaje(string.Format("Orden de mantenimiento #{0} registrada en JSON.", mant.IdMantenimiento), "exito");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al registrar mantenimiento: " + ex.Message, "error");
            }
        }

        protected void btnIniciarMantenimiento_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtIdMantenimiento.Text.Trim(), out int idMant) || idMant <= 0)
                {
                    MostrarMensaje("Seleccione primero una orden de mantenimiento en la tabla.", "advertencia");
                    return;
                }

                DateTime fechaIni = DateTime.Today;
                if (!string.IsNullOrWhiteSpace(txtFechaInicio.Text) &&
                    DateTime.TryParseExact(txtFechaInicio.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedIni))
                {
                    fechaIni = parsedIni;
                }

                _store.IniciarMantenimiento(idMant, fechaIni, txtTecnicoResponsable.Text.Trim());
                ddlEstadoMantenimiento.SelectedValue = EstadoMantenimiento.EnProceso.ToString();
                CargarArticulos();
                CargarMantenimientos();
                MostrarMensaje(string.Format("Mantenimiento #{0} marcado como En Proceso.", idMant), "exito");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al iniciar mantenimiento: " + ex.Message, "error");
            }
        }

        protected void btnFinalizarMantenimiento_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtIdMantenimiento.Text.Trim(), out int idMant) || idMant <= 0)
                {
                    MostrarMensaje("Seleccione primero una orden de mantenimiento para finalizarla.", "advertencia");
                    return;
                }

                DateTime fechaFin = DateTime.Today;
                if (!string.IsNullOrWhiteSpace(txtFechaFinalizacion.Text) &&
                    DateTime.TryParseExact(txtFechaFinalizacion.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedFin))
                {
                    fechaFin = parsedFin;
                }

                var estadoFinal = (EstadoArticulo)Enum.Parse(typeof(EstadoArticulo), ddlResultadoFinal.SelectedValue);
                _store.FinalizarMantenimiento(idMant, fechaFin, txtDiagnosticoSolucion.Text.Trim(), txtObservacionesMant.Text.Trim(), estadoFinal);

                ddlEstadoMantenimiento.SelectedValue = EstadoMantenimiento.Finalizado.ToString();
                CargarArticulos();
                CargarMantenimientos();
                MostrarMensaje(string.Format("Mantenimiento #{0} finalizado y movimiento registrado.", idMant), "exito");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al finalizar mantenimiento: " + ex.Message, "error");
            }
        }

        protected void btnCancelarMantenimiento_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtIdMantenimiento.Text.Trim(), out int idMant) || idMant <= 0)
                {
                    MostrarMensaje("Seleccione una orden de mantenimiento para cancelarla.", "advertencia");
                    return;
                }

                _store.CancelarMantenimiento(idMant, txtObservacionesMant.Text.Trim());
                ddlEstadoMantenimiento.SelectedValue = EstadoMantenimiento.Cancelado.ToString();
                CargarArticulos();
                CargarMantenimientos();
                MostrarMensaje(string.Format("Mantenimiento #{0} cancelado.", idMant), "exito");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cancelar mantenimiento: " + ex.Message, "error");
            }
        }

        protected void btnLimpiarMant_Click(object sender, EventArgs e)
        {
            txtIdMantenimiento.Text = string.Empty;
            ddlArticuloMant.SelectedIndex = 0;
            ddlTipoMantenimiento.SelectedIndex = 0;
            txtCantidadMant.Text = "1";
            txtFechaProgramada.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtFechaInicio.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtFechaFinalizacion.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtTecnicoResponsable.Text = string.Empty;
            ddlEstadoMantenimiento.SelectedValue = EstadoMantenimiento.Pendiente.ToString();
            ddlResultadoFinal.SelectedIndex = 0;
            txtDescripcionProblema.Text = string.Empty;
            txtDiagnosticoSolucion.Text = string.Empty;
            txtObservacionesMant.Text = string.Empty;
            ActualizarInfoArticulo();
            OcultarMensaje();
        }

        protected void btnBuscarMant_Click(object sender, EventArgs e)
        {
            CargarMantenimientos(txtBuscarMant.Text.Trim(), ddlFiltroTipoMant.SelectedValue, ddlFiltroEstadoMant.SelectedValue);
        }

        protected void btnTodosMant_Click(object sender, EventArgs e)
        {
            txtBuscarMant.Text = string.Empty;
            ddlFiltroTipoMant.SelectedValue = "TODOS";
            ddlFiltroEstadoMant.SelectedValue = "TODOS";
            CargarMantenimientos();
        }

        protected void gvMantenimientos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Seleccionar" && int.TryParse(e.CommandArgument.ToString(), out int idMant))
            {
                var m = _store.BuscarMantenimientoPorId(idMant);
                if (m == null)
                {
                    MostrarMensaje("No se encontró el mantenimiento seleccionado.", "advertencia");
                    return;
                }

                txtIdMantenimiento.Text = m.IdMantenimiento.ToString();
                if (ddlArticuloMant.Items.FindByValue(m.IdArticulo.ToString()) != null)
                {
                    ddlArticuloMant.SelectedValue = m.IdArticulo.ToString();
                    ActualizarInfoArticulo();
                }
                ddlTipoMantenimiento.SelectedValue = m.TipoMantenimiento.ToString();
                txtCantidadMant.Text = m.Cantidad.ToString();
                txtFechaProgramada.Text = m.FechaProgramada.ToString("yyyy-MM-dd");
                if (m.FechaInicio.HasValue) txtFechaInicio.Text = m.FechaInicio.Value.ToString("yyyy-MM-dd");
                if (m.FechaFinalizacion.HasValue) txtFechaFinalizacion.Text = m.FechaFinalizacion.Value.ToString("yyyy-MM-dd");
                txtTecnicoResponsable.Text = m.TecnicoResponsable;
                ddlEstadoMantenimiento.SelectedValue = m.EstadoMantenimiento.ToString();
                txtDescripcionProblema.Text = m.DescripcionProblema;
                txtDiagnosticoSolucion.Text = m.DiagnosticoSolucion;
                txtObservacionesMant.Text = m.Observaciones;
                MostrarMensaje(string.Format("Orden #{0} cargada en el formulario.", m.IdMantenimiento), "exito");
            }
        }

        private void CargarMantenimientos(string filtro = "", string tipo = "TODOS", string estado = "TODOS")
        {
            var lista = _store.ObtenerMantenimientos();
            var articulos = _store.ObtenerArticulosConExistencia().ToDictionary(a => a.IdArticulo, a => a.Nombre);

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                string f = filtro.ToLowerInvariant();
                lista = lista.Where(m =>
                    m.IdMantenimiento.ToString().Contains(f) ||
                    (m.TecnicoResponsable ?? "").ToLowerInvariant().Contains(f) ||
                    (articulos.ContainsKey(m.IdArticulo) && articulos[m.IdArticulo].ToLowerInvariant().Contains(f))
                ).ToList();
            }

            if (tipo != "TODOS")
            {
                lista = lista.Where(m => m.TipoMantenimiento.ToString().Equals(tipo, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (estado != "TODOS")
            {
                lista = lista.Where(m => m.EstadoMantenimiento.ToString().Equals(estado, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var filas = lista.OrderByDescending(m => m.FechaProgramada).Select(m => new
            {
                m.IdMantenimiento,
                NombreArticulo = articulos.ContainsKey(m.IdArticulo) ? articulos[m.IdArticulo] : "ID " + m.IdArticulo,
                TipoTexto = m.TipoMantenimiento.ToString(),
                m.Cantidad,
                FechaProgramadaTexto = m.FechaProgramada.ToString("yyyy-MM-dd"),
                m.TecnicoResponsable,
                EstadoTexto = m.EstadoMantenimiento.ToString()
            }).ToList();

            gvMantenimientos.DataSource = filas;
            gvMantenimientos.DataBind();
        }
    }
}