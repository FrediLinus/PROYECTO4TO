using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LaboratorioSistema
{
    public partial class FrmInventario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PrepararNuevo();
                CargarInventario();
            }
        }

        private void CargarInventario(string filtro = "")
        {
            InventarioLogica logica = new InventarioLogica();
            string texto = (filtro ?? "").Trim();

            var lista = logica.ObtenerArticulos()
                .Where(a => CoincideConFiltro(a, texto))
                .Select(a => new
                {
                    IdArticulo = a.Codigo,
                    Nombre = a.Nombre,
                    Tipo = ObtenerTipo(a),
                    Marca = ObtenerMarca(a),
                    Cantidad = a.Cantidad,
                    Estado = a.Estado,
                    Ubicacion = a.Ubicacion
                }).ToList();

            gvInventario.DataSource = lista;
            gvInventario.DataBind();
        }

        private string ObtenerTipo(Articulo articulo)
        {
            if (articulo is Equipo) return "Equipo informático";
            if (articulo is Herramienta) return "Herramienta";
            if (articulo is InstrumentoMedicion) return "Instrumento de medición";
            if (articulo is ComponenteElectronico) return "Componente electrónico";
            return "Otro";
        }

        private string ObtenerMarca(Articulo articulo)
        {
            Equipo equipo = articulo as Equipo;
            if (equipo != null) return equipo.Marca;

            Herramienta herramienta = articulo as Herramienta;
            if (herramienta != null) return herramienta.TipoHerramienta;

            InstrumentoMedicion instrumento = articulo as InstrumentoMedicion;
            if (instrumento != null) return instrumento.TipoInstrumento;

            ComponenteElectronico componente = articulo as ComponenteElectronico;
            if (componente != null) return componente.TipoComponente;

            return "";
        }

        private bool CoincideConFiltro(Articulo articulo, string filtro)
        {
            if (string.IsNullOrEmpty(filtro))
            {
                return true;
            }

            string datos = string.Join(" ",
                articulo.Codigo.ToString(),
                articulo.Nombre,
                ObtenerTipo(articulo),
                ObtenerMarca(articulo),
                articulo.Descripcion,
                articulo.Estado,
                articulo.Ubicacion);

            Equipo equipo = articulo as Equipo;
            if (equipo != null)
            {
                datos += " " + equipo.Modelo + " " + equipo.NumeroSerie;
            }

            Herramienta herramienta = articulo as Herramienta;
            if (herramienta != null)
            {
                datos += " " + herramienta.Material;
            }

            InstrumentoMedicion instrumento = articulo as InstrumentoMedicion;
            if (instrumento != null)
            {
                datos += " " + instrumento.RangoMedicion + " " + instrumento.UnidadMedicion;
            }

            ComponenteElectronico componente = articulo as ComponenteElectronico;
            if (componente != null)
            {
                datos += " " + componente.Valor + " " + componente.Unidad;
            }

            return datos.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void PrepararNuevo()
        {
            InventarioLogica logica = new InventarioLogica();
            var articulos = logica.ObtenerArticulos();
            int nuevoCodigo = articulos.Count > 0 ? articulos.Max(a => a.Codigo) + 1 : 1;

            LimpiarFormulario();
            txtIdArticulo.Text = nuevoCodigo.ToString();
            txtCantidad.Text = "1";
            txtFechaAdquisicion.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtStockMinimo.Text = "1";
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            PrepararNuevo();
            lblMensaje.Text = "Nuevo artículo listo para registrar.";
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje;
            Articulo articulo = CrearArticuloDesdeFormulario(out mensaje);
            if (articulo == null)
            {
                lblMensaje.Text = mensaje;
                return;
            }

            InventarioLogica logica = new InventarioLogica();
            bool resultado = logica.RegistrarArticulo(articulo, out mensaje);
            lblMensaje.Text = mensaje;

            if (resultado)
            {
                CargarInventario();
                PrepararNuevo();
            }
        }

        protected void btnModificar_Click(object sender, EventArgs e)
        {
            string mensaje;
            Articulo articulo = CrearArticuloDesdeFormulario(out mensaje);
            if (articulo == null)
            {
                lblMensaje.Text = mensaje;
                return;
            }

            InventarioLogica logica = new InventarioLogica();
            bool resultado = logica.ActualizarArticulo(articulo, out mensaje);
            lblMensaje.Text = mensaje;

            if (resultado)
            {
                CargarInventario();
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            int codigo;
            if (!int.TryParse(txtIdArticulo.Text, out codigo) || codigo <= 0)
            {
                lblMensaje.Text = "Ingrese un ID de artículo válido.";
                return;
            }

            InventarioLogica logica = new InventarioLogica();
            string mensaje;
            bool resultado = logica.EliminarArticulo(codigo, out mensaje);
            lblMensaje.Text = mensaje;

            if (resultado)
            {
                PrepararNuevo();
                CargarInventario();
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarPorCodigo();
        }

        protected void btnBuscarArticulo_Click(object sender, EventArgs e)
        {
            CargarInventario(txtBuscar.Text);
        }

        protected void gvInventario_RowCommand(
            object sender,
            System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName != "EliminarArticulo")
            {
                return;
            }

            int indice = Convert.ToInt32(e.CommandArgument);
            int codigo = Convert.ToInt32(gvInventario.DataKeys[indice].Value);

            InventarioLogica logica = new InventarioLogica();
            string mensaje;
            bool resultado = logica.EliminarArticulo(codigo, out mensaje);

            lblMensaje.Text = mensaje;
            CargarInventario(txtBuscar.Text);

            if (resultado && txtIdArticulo.Text == codigo.ToString())
            {
                PrepararNuevo();
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            PrepararNuevo();
            lblMensaje.Text = "Formulario limpiado.";
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarInventario();
            lblMensaje.Text = "Listado actualizado.";
        }

        private void BuscarPorCodigo()
        {
            int codigo;
            if (!int.TryParse(txtIdArticulo.Text, out codigo) || codigo <= 0)
            {
                lblMensaje.Text = "Ingrese un ID de artículo válido.";
                return;
            }

            InventarioLogica logica = new InventarioLogica();
            Articulo articulo = logica.BuscarArticulo(codigo);
            if (articulo == null)
            {
                lblMensaje.Text = "No se encontró un artículo con ese ID.";
                return;
            }

            CargarArticuloEnFormulario(articulo);
            lblMensaje.Text = "Artículo encontrado.";
        }

        private Articulo CrearArticuloDesdeFormulario(out string mensaje)
        {
            mensaje = "";
            int codigo;
            int cantidad;
            int stockMinimo;
            DateTime fecha;

            if (!int.TryParse(txtIdArticulo.Text, out codigo) || codigo <= 0)
            {
                mensaje = "Ingrese un ID de artículo válido.";
                return null;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                mensaje = "Ingrese el nombre del artículo.";
                return null;
            }

            if (!int.TryParse(txtCantidad.Text, out cantidad) || cantidad < 0)
            {
                mensaje = "Ingrese una cantidad válida.";
                return null;
            }

            if (!int.TryParse(txtStockMinimo.Text, out stockMinimo) || stockMinimo < 0)
            {
                mensaje = "Ingrese un stock mínimo válido.";
                return null;
            }

            if (!DateTime.TryParseExact(txtFechaAdquisicion.Text, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                mensaje = "Ingrese una fecha válida.";
                return null;
            }

            if (fecha.Date > DateTime.Today)
            {
                mensaje = "La fecha no puede ser futura.";
                return null;
            }

            if (ddlTipo.SelectedIndex == 0 || ddlEstado.SelectedIndex == 0
                || ddlUbicacion.SelectedIndex == 0)
            {
                mensaje = "Seleccione tipo, estado y ubicación del artículo.";
                return null;
            }

            Articulo articulo;
            switch (ddlTipo.SelectedValue)
            {
                case "Equipo informático":
                    articulo = new Equipo(codigo, txtNombre.Text.Trim(), txtDescripcion.Text.Trim(),
                        cantidad, ddlEstado.SelectedValue, txtMarca.Text.Trim(),
                        txtModelo.Text.Trim(), txtNumeroSerie.Text.Trim());
                    break;
                case "Herramienta":
                    articulo = new Herramienta(codigo, txtNombre.Text.Trim(), txtDescripcion.Text.Trim(),
                        cantidad, ddlEstado.SelectedValue, txtMarca.Text.Trim(), txtModelo.Text.Trim());
                    break;
                case "Instrumento de medición":
                    articulo = new InstrumentoMedicion(codigo, txtNombre.Text.Trim(),
                        txtDescripcion.Text.Trim(), cantidad, ddlEstado.SelectedValue,
                        txtMarca.Text.Trim(), txtModelo.Text.Trim(), txtNumeroSerie.Text.Trim());
                    break;
                case "Componente electrónico":
                    articulo = new ComponenteElectronico(codigo, txtNombre.Text.Trim(),
                        txtDescripcion.Text.Trim(), cantidad, ddlEstado.SelectedValue,
                        txtMarca.Text.Trim(), txtModelo.Text.Trim(), txtNumeroSerie.Text.Trim());
                    break;
                default:
                    mensaje = "Seleccione un tipo de artículo.";
                    return null;
            }

            articulo.CantidadMinima = stockMinimo;
            articulo.Ubicacion = ddlUbicacion.SelectedValue;
            articulo.FechaAdquisicion = fecha;
            return articulo;
        }

        private void CargarArticuloEnFormulario(Articulo articulo)
        {
            txtIdArticulo.Text = articulo.Codigo.ToString();
            txtNombre.Text = articulo.Nombre;
            ddlTipo.SelectedValue = ObtenerTipo(articulo);
            txtDescripcion.Text = articulo.Descripcion;
            txtCantidad.Text = articulo.Cantidad.ToString();
            ddlEstado.SelectedValue = articulo.Estado;
            ddlUbicacion.SelectedValue = articulo.Ubicacion;
            txtFechaAdquisicion.Text = articulo.FechaAdquisicion.ToString("yyyy-MM-dd");
            txtStockMinimo.Text = articulo.CantidadMinima.ToString();

            Equipo equipo = articulo as Equipo;
            txtMarca.Text = equipo != null ? equipo.Marca : "";
            txtModelo.Text = equipo != null ? equipo.Modelo : "";
            txtNumeroSerie.Text = equipo != null ? equipo.NumeroSerie : "";

            Herramienta herramienta = articulo as Herramienta;
            if (herramienta != null)
            {
                txtMarca.Text = herramienta.TipoHerramienta;
                txtModelo.Text = herramienta.Material;
            }

            InstrumentoMedicion instrumento = articulo as InstrumentoMedicion;
            if (instrumento != null)
            {
                txtMarca.Text = instrumento.TipoInstrumento;
                txtModelo.Text = instrumento.RangoMedicion;
                txtNumeroSerie.Text = instrumento.UnidadMedicion;
            }

            ComponenteElectronico componente = articulo as ComponenteElectronico;
            if (componente != null)
            {
                txtMarca.Text = componente.TipoComponente;
                txtModelo.Text = componente.Valor;
                txtNumeroSerie.Text = componente.Unidad;
            }
        }

        private void LimpiarFormulario()
        {
            txtIdArticulo.Text = "";
            txtNombre.Text = "";
            ddlTipo.SelectedIndex = 0;
            txtMarca.Text = "";
            txtModelo.Text = "";
            txtNumeroSerie.Text = "";
            txtDescripcion.Text = "";
            txtCantidad.Text = "";
            ddlEstado.SelectedIndex = 0;
            ddlUbicacion.SelectedIndex = 0;
            txtFechaAdquisicion.Text = "";
            txtStockMinimo.Text = "";
        }
    }
}