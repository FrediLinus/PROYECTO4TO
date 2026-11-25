using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LaboratorioSistema
{
    public partial class FrmPrestamos : System.Web.UI.Page
    {
        private readonly LaboratorioStore _store = new LaboratorioStore();

        [Serializable]
        private class DetalleTemporalItem
        {
            public int IdArticulo { get; set; }
            public string Codigo { get; set; }
            public string NombreArticulo { get; set; }
            public string Tipo { get; set; }
            public int CantidadSolicitada { get; set; }
        }

        private List<DetalleTemporalItem> DetallesTemporales
        {
            get
            {
                if (ViewState["DetallesTemporales"] == null)
                {
                    ViewState["DetallesTemporales"] = new List<DetalleTemporalItem>();
                }
                return (List<DetalleTemporalItem>)ViewState["DetallesTemporales"];
            }
            set { ViewState["DetallesTemporales"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFechaPrestamo.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtFechaPrevista.Text = DateTime.Today.AddDays(3).ToString("yyyy-MM-dd");

                CargarCombos();
                CargarPrestamos();
                RefrescarDetallesTemporales();
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

        private void CargarCombos()
        {
            string artPrevio = ddlArticulo.SelectedValue;
            var articulos = _store.ObtenerArticulosConExistencia()
                .Select(a => new
                {
                    Id = a.IdArticulo.ToString(),
                    Texto = string.Format("[{0}] {1} (Disp: {2})", a.Codigo, a.Nombre, a.CantidadDisponible)
                }).ToList();

            ddlArticulo.DataSource = articulos;
            ddlArticulo.DataTextField = "Texto";
            ddlArticulo.DataValueField = "Id";
            ddlArticulo.DataBind();
            ddlArticulo.Items.Insert(0, new ListItem("-- Seleccione artículo --", ""));

            if (!string.IsNullOrEmpty(artPrevio) && ddlArticulo.Items.FindByValue(artPrevio) != null)
            {
                ddlArticulo.SelectedValue = artPrevio;
            }
            ActualizarInfoArticulo();
        }

        protected void ddlArticulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarInfoArticulo();
        }

        private void ActualizarInfoArticulo()
        {
            if (!int.TryParse(ddlArticulo.SelectedValue, out int idArt))
            {
                txtCodigoArticulo.Text = string.Empty;
                txtTipoArticulo.Text = string.Empty;
                txtEstadoArticulo.Text = string.Empty;
                txtDisponibleArticulo.Text = string.Empty;
                return;
            }

            var item = _store.BuscarArticuloConExistencia(idArt);
            if (item != null)
            {
                txtCodigoArticulo.Text = item.Codigo.ToString();
                txtTipoArticulo.Text = item.Tipo;
                txtEstadoArticulo.Text = item.EstadoArticulo.ToString();
                txtDisponibleArticulo.Text = item.CantidadDisponible.ToString();
            }
        }

        protected void btnAgregarArticulo_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(ddlArticulo.SelectedValue, out int idArt))
                {
                    MostrarMensaje("Seleccione un artículo del inventario.", "advertencia");
                    return;
                }

                if (!int.TryParse(txtCantidadSolicitada.Text.Trim(), out int cantidad) || cantidad <= 0)
                {
                    MostrarMensaje("Ingrese una cantidad solicitada válida mayor que cero.", "advertencia");
                    return;
                }

                var item = _store.BuscarArticuloConExistencia(idArt);
                if (item == null)
                {
                    MostrarMensaje("El artículo seleccionado no existe.", "error");
                    return;
                }

                if (item.EstadoArticulo != EstadoArticulo.Disponible || item.CantidadDisponible <= 0)
                {
                    MostrarMensaje("El artículo no está disponible para préstamo.", "advertencia");
                    return;
                }

                var lista = DetallesTemporales;
                var existente = lista.FirstOrDefault(x => x.IdArticulo == item.IdArticulo);
                int totalSolicitado = (existente != null ? existente.CantidadSolicitada : 0) + cantidad;

                if (totalSolicitado > item.CantidadDisponible)
                {
                    MostrarMensaje(string.Format("La cantidad solicitada ({0}) supera lo disponible ({1}).",
                        totalSolicitado, item.CantidadDisponible), "advertencia");
                    return;
                }

                if (existente != null)
                {
                    existente.CantidadSolicitada = totalSolicitado;
                }
                else
                {
                    lista.Add(new DetalleTemporalItem
                    {
                        IdArticulo = item.IdArticulo,
                        Codigo = item.Codigo.ToString(),
                        NombreArticulo = item.Nombre,
                        Tipo = item.Tipo,
                        CantidadSolicitada = cantidad
                    });
                }

                DetallesTemporales = lista;
                txtCantidadSolicitada.Text = string.Empty;
                RefrescarDetallesTemporales();
                OcultarMensaje();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al agregar artículo: " + ex.Message, "error");
            }
        }

        protected void gvDetallesPrestamo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Quitar" && int.TryParse(e.CommandArgument.ToString(), out int idArt))
            {
                var lista = DetallesTemporales;
                lista.RemoveAll(x => x.IdArticulo == idArt);
                DetallesTemporales = lista;
                RefrescarDetallesTemporales();
                MostrarMensaje("Artículo eliminado de la lista del préstamo.", "advertencia");
            }
        }

        private void RefrescarDetallesTemporales()
        {
            gvDetallesPrestamo.DataSource = DetallesTemporales;
            gvDetallesPrestamo.DataBind();
        }

        protected void btnRegistrarPrestamo_Click(object sender, EventArgs e)
        {
            try
            {
                string mensajeResponsable;
                int idResponsable;
                if (!ObtenerResponsableDesdeFormulario(out idResponsable, out mensajeResponsable))
                {
                    MostrarMensaje(mensajeResponsable, "advertencia");
                    return;
                }

                if (!DateTime.TryParseExact(txtFechaPrestamo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaPrestamo) ||
                    !DateTime.TryParseExact(txtFechaPrevista.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaPrevista))
                {
                    MostrarMensaje("Ingrese fechas válidas de préstamo y devolución prevista.", "advertencia");
                    return;
                }

                if (fechaPrevista.Date < fechaPrestamo.Date)
                {
                    MostrarMensaje("La fecha prevista de devolución no puede ser anterior a la fecha de préstamo.", "advertencia");
                    return;
                }

                if (DetallesTemporales.Count == 0)
                {
                    MostrarMensaje("Agregue al menos un artículo antes de registrar el préstamo.", "advertencia");
                    return;
                }

                var items = DetallesTemporales.Select(d => new Tuple<int, int>(d.IdArticulo, d.CantidadSolicitada)).ToList();
                var prestamo = _store.RegistrarPrestamo(
                    idResponsable,
                    fechaPrestamo,
                    fechaPrevista,
                    txtObservaciones.Text.Trim(),
                    items
                );

                txtIdPrestamo.Text = prestamo.IdPrestamo.ToString();
                ddlEstadoPrestamo.SelectedValue = prestamo.EstadoPrestamo.ToString();
                DetallesTemporales = new List<DetalleTemporalItem>();
                RefrescarDetallesTemporales();
                CargarCombos();
                CargarPrestamos();

                MostrarMensaje(string.Format("Préstamo #{0} registrado y guardado en JSON.", prestamo.IdPrestamo), "exito");
            }
            catch (Exception ex)
            {
                MostrarMensaje("No se pudo registrar el préstamo: " + ex.Message, "error");
            }
        }

        protected void btnCancelarPrestamo_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtIdPrestamo.Text.Trim(), out int idPrestamo) || idPrestamo <= 0)
                {
                    MostrarMensaje("Seleccione o consulte primero un préstamo en la tabla para cancelarlo.", "advertencia");
                    return;
                }

                _store.CancelarPrestamo(idPrestamo);
                ddlEstadoPrestamo.SelectedValue = EstadoPrestamo.Cancelado.ToString();
                CargarCombos();
                CargarPrestamos();
                MostrarMensaje(string.Format("Préstamo #{0} cancelado y existencias actualizadas.", idPrestamo), "exito");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cancelar préstamo: " + ex.Message, "error");
            }
        }

        protected void btnLimpiarPrestamo_Click(object sender, EventArgs e)
        {
            txtIdPrestamo.Text = string.Empty;
            txtResponsableCodigo.Text = string.Empty;
            txtResponsableNombres.Text = string.Empty;
            txtResponsableApellidos.Text = string.Empty;
            txtResponsableCorreo.Text = string.Empty;
            txtResponsableTelefono.Text = string.Empty;
            txtResponsableRol.Text = string.Empty;
            txtResponsableCargo.Text = string.Empty;
            txtResponsableArea.Text = string.Empty;
            pnlInfoResponsable.Visible = false;
            txtFechaPrestamo.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtFechaPrevista.Text = DateTime.Today.AddDays(3).ToString("yyyy-MM-dd");
            ddlEstadoPrestamo.SelectedValue = EstadoPrestamo.Activo.ToString();
            txtObservaciones.Text = string.Empty;
            txtCantidadSolicitada.Text = string.Empty;
            DetallesTemporales = new List<DetalleTemporalItem>();
            RefrescarDetallesTemporales();
            ActualizarInfoArticulo();
            OcultarMensaje();
        }

        protected void btnBuscarPrestamo_Click(object sender, EventArgs e)
        {
            CargarPrestamos(txtBuscarPrestamo.Text.Trim(), ddlFiltroEstado.SelectedValue);
        }

        protected void btnVerTodosPrestamos_Click(object sender, EventArgs e)
        {
            txtBuscarPrestamo.Text = string.Empty;
            ddlFiltroEstado.SelectedValue = "TODOS";
            CargarPrestamos();
        }

        protected void gvPrestamos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Consultar" && int.TryParse(e.CommandArgument.ToString(), out int idPrestamo))
            {
                var p = _store.BuscarPrestamoPorId(idPrestamo);
                if (p == null)
                {
                    MostrarMensaje("No se encontró el préstamo seleccionado.", "advertencia");
                    return;
                }

                txtIdPrestamo.Text = p.IdPrestamo.ToString();
                var responsable = _store.BuscarResponsablePorId(p.IdResponsable);
                if (responsable != null)
                {
                    txtResponsableCodigo.Text = responsable.IdResponsable.ToString();
                    txtResponsableNombres.Text = responsable.Nombres;
                    txtResponsableApellidos.Text = responsable.Apellidos;
                    txtResponsableCorreo.Text = responsable.Correo;
                    txtResponsableTelefono.Text = responsable.Telefono;
                    txtResponsableRol.Text = responsable.TipoResponsable;
                    txtResponsableArea.Text = responsable.CarreraDepartamento;
                    pnlInfoResponsable.Visible = true;
                    lblInfoResponsable.Text = string.Format("{0} {1} | Código: {2} | Área: {3} | Tel: {4} | Correo: {5}",
                        responsable.Nombres, responsable.Apellidos, responsable.IdResponsable,
                        responsable.CarreraDepartamento, responsable.Telefono, responsable.Correo);
                }
                txtFechaPrestamo.Text = p.FechaPrestamo.ToString("yyyy-MM-dd");
                txtFechaPrevista.Text = p.FechaPrevistaDevolucion.ToString("yyyy-MM-dd");
                ddlEstadoPrestamo.SelectedValue = p.EstadoPrestamo.ToString();
                txtObservaciones.Text = p.Observaciones;
                MostrarMensaje(string.Format("Consultando préstamo #{0}.", p.IdPrestamo), "exito");
            }
        }

        private void CargarPrestamos(string filtro = "", string estado = "TODOS")
        {
            var prestamos = _store.ObtenerPrestamos();
            var responsables = _store.ObtenerTodosResponsables()
                .ToDictionary(r => r.IdResponsable, r => r.Nombres + " " + r.Apellidos);

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                string f = filtro.ToLowerInvariant();
                prestamos = prestamos.Where(p =>
                    p.IdPrestamo.ToString().Contains(f) ||
                    (responsables.ContainsKey(p.IdResponsable) && responsables[p.IdResponsable].ToLowerInvariant().Contains(f))
                ).ToList();
            }

            if (estado != "TODOS")
            {
                prestamos = prestamos.Where(p => p.EstadoPrestamo.ToString().Equals(estado, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var filas = prestamos.OrderByDescending(p => p.FechaPrestamo).Select(p => new
            {
                p.IdPrestamo,
                ResponsableNombre = responsables.ContainsKey(p.IdResponsable) ? responsables[p.IdResponsable] : "ID " + p.IdResponsable,
                FechaPrestamoTexto = p.FechaPrestamo.ToString("yyyy-MM-dd"),
                FechaPrevistaTexto = p.FechaPrevistaDevolucion.ToString("yyyy-MM-dd"),
                TotalArticulos = p.Detalles.Sum(d => d.CantidadPrestada),
                EstadoTexto = p.EstadoPrestamo.ToString()
            }).ToList();

            gvPrestamos.DataSource = filas;
            gvPrestamos.DataBind();
        }

        private bool ObtenerResponsableDesdeFormulario(out int idResponsable, out string mensaje)
        {
            idResponsable = 0;
            mensaje = string.Empty;
            int codigo;

            if (!int.TryParse(txtResponsableCodigo.Text.Trim(), out codigo) || codigo <= 0)
            {
                mensaje = "Ingrese un código de responsable válido.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtResponsableNombres.Text) ||
                string.IsNullOrWhiteSpace(txtResponsableApellidos.Text) ||
                string.IsNullOrWhiteSpace(txtResponsableCorreo.Text) ||
                string.IsNullOrWhiteSpace(txtResponsableRol.Text) ||
                string.IsNullOrWhiteSpace(txtResponsableCargo.Text) ||
                string.IsNullOrWhiteSpace(txtResponsableArea.Text))
            {
                mensaje = "Complete todos los campos obligatorios del responsable.";
                return false;
            }

            if (!txtResponsableCorreo.Text.Contains("@"))
            {
                mensaje = "Ingrese un correo válido para el responsable.";
                return false;
            }

            Responsable responsable = new Responsable(
                codigo,
                txtResponsableNombres.Text.Trim(),
                txtResponsableApellidos.Text.Trim(),
                txtResponsableCorreo.Text.Trim(),
                txtResponsableTelefono.Text.Trim(),
                txtResponsableRol.Text.Trim(),
                txtResponsableCargo.Text.Trim(),
                txtResponsableArea.Text.Trim());

            ResponsablesLogica logica = new ResponsablesLogica();
            string resultado;
            bool guardado = logica.Buscar(codigo) == null
                ? logica.Registrar(responsable, out resultado)
                : logica.Actualizar(responsable, out resultado);

            if (!guardado)
            {
                mensaje = resultado;
                return false;
            }

            idResponsable = codigo;
            pnlInfoResponsable.Visible = true;
            lblInfoResponsable.Text = string.Format("{0} {1} | Código: {2} | Área: {3} | Tel: {4} | Correo: {5}",
                responsable.Nombre, responsable.Apellido, responsable.Codigo,
                responsable.AreaAsignada, responsable.Telefono, responsable.Correo);
            return true;
        }
    }
}