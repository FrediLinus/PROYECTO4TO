using System;
using System.Linq;

namespace LaboratorioSistema
{
    public partial class FrmResponsables : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PrepararNuevo();
                CargarResponsables();
            }
        }

        private void CargarResponsables(string filtro = "")
        {
            string texto = (filtro ?? "").Trim();
            ResponsablesLogica logica = new ResponsablesLogica();

            var lista = logica.ObtenerResponsables()
                .Where(r => string.IsNullOrEmpty(texto)
                    || r.Codigo.ToString().Contains(texto)
                    || (r.Nombre ?? "").IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0
                    || (r.Apellido ?? "").IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0
                    || (r.Correo ?? "").IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(r => new
                {
                    r.Codigo,
                    NombreCompleto = (r.Nombre + " " + r.Apellido).Trim(),
                    r.Correo,
                    r.Telefono,
                    r.Rol,
                    r.Cargo,
                    r.AreaAsignada
                }).ToList();

            gvResponsables.DataSource = lista;
            gvResponsables.DataBind();
        }

        private void PrepararNuevo()
        {
            ResponsablesLogica logica = new ResponsablesLogica();
            var responsables = logica.ObtenerResponsables();
            int codigo = responsables.Count > 0
                ? responsables.Max(r => r.Codigo) + 1
                : 1;

            LimpiarFormulario();
            txtCodigo.Text = codigo.ToString();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            PrepararNuevo();
            lblMensaje.Text = "Nuevo responsable listo para registrar.";
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje;
            Responsable responsable = CrearDesdeFormulario(out mensaje);
            if (responsable == null)
            {
                lblMensaje.Text = mensaje;
                return;
            }

            ResponsablesLogica logica = new ResponsablesLogica();
            bool resultado = logica.Registrar(responsable, out mensaje);
            lblMensaje.Text = mensaje;

            if (resultado)
            {
                CargarResponsables();
                PrepararNuevo();
            }
        }

        protected void btnModificar_Click(object sender, EventArgs e)
        {
            string mensaje;
            Responsable responsable = CrearDesdeFormulario(out mensaje);
            if (responsable == null)
            {
                lblMensaje.Text = mensaje;
                return;
            }

            ResponsablesLogica logica = new ResponsablesLogica();
            bool resultado = logica.Actualizar(responsable, out mensaje);
            lblMensaje.Text = mensaje;

            if (resultado)
            {
                CargarResponsables();
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            int codigo;
            if (!int.TryParse(txtCodigo.Text, out codigo) || codigo <= 0)
            {
                lblMensaje.Text = "Ingrese un código válido.";
                return;
            }

            string mensaje;
            ResponsablesLogica logica = new ResponsablesLogica();
            bool resultado = logica.Eliminar(codigo, out mensaje);
            lblMensaje.Text = mensaje;

            if (resultado)
            {
                PrepararNuevo();
                CargarResponsables();
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            int codigo;
            if (!int.TryParse(txtCodigo.Text, out codigo) || codigo <= 0)
            {
                lblMensaje.Text = "Ingrese un código válido.";
                return;
            }

            ResponsablesLogica logica = new ResponsablesLogica();
            Responsable responsable = logica.Buscar(codigo);
            if (responsable == null)
            {
                lblMensaje.Text = "No se encontró un responsable con ese código.";
                return;
            }

            CargarEnFormulario(responsable);
            lblMensaje.Text = "Responsable encontrado.";
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            PrepararNuevo();
            lblMensaje.Text = "Formulario limpiado.";
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarResponsables();
            lblMensaje.Text = "Listado actualizado.";
        }

        protected void btnBuscarListado_Click(object sender, EventArgs e)
        {
            CargarResponsables(txtBuscar.Text);
        }

        private Responsable CrearDesdeFormulario(out string mensaje)
        {
            mensaje = "";
            int codigo;

            if (!int.TryParse(txtCodigo.Text, out codigo) || codigo <= 0)
            {
                mensaje = "Ingrese un código válido.";
                return null;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text)
                || string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                mensaje = "Ingrese el nombre y el apellido.";
                return null;
            }

            if (string.IsNullOrWhiteSpace(txtCorreo.Text))
            {
                mensaje = "Ingrese el correo.";
                return null;
            }

            if (ddlRol.SelectedIndex == 0 || ddlAreaAsignada.SelectedIndex == 0)
            {
                mensaje = "Seleccione el rol y el área asignada.";
                return null;
            }

            if (string.IsNullOrWhiteSpace(txtCargo.Text))
            {
                mensaje = "Ingrese el cargo.";
                return null;
            }

            return new Responsable(
                codigo,
                txtNombre.Text.Trim(),
                txtApellido.Text.Trim(),
                txtCorreo.Text.Trim(),
                txtTelefono.Text.Trim(),
                ddlRol.SelectedValue,
                txtCargo.Text.Trim(),
                ddlAreaAsignada.SelectedValue);
        }

        private void CargarEnFormulario(Responsable responsable)
        {
            txtCodigo.Text = responsable.Codigo.ToString();
            txtNombre.Text = responsable.Nombre;
            txtApellido.Text = responsable.Apellido;
            txtCorreo.Text = responsable.Correo;
            txtTelefono.Text = responsable.Telefono;
            ddlRol.SelectedValue = responsable.Rol;
            txtCargo.Text = responsable.Cargo;
            ddlAreaAsignada.SelectedValue = responsable.AreaAsignada;
        }

        private void LimpiarFormulario()
        {
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtCorreo.Text = "";
            txtTelefono.Text = "";
            ddlRol.SelectedIndex = 0;
            txtCargo.Text = "";
            ddlAreaAsignada.SelectedIndex = 0;
        }
    }
}