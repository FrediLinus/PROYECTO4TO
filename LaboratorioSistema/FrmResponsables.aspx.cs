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
                CargarExistencias();
            }
        }

        protected void btnFiltrarExistencias_Click(object sender, EventArgs e)
        {
            CargarExistencias(txtBuscarExistencias.Text, ddlEstadoExistencias.SelectedValue);
        }

        protected void btnMostrarTodasExistencias_Click(object sender, EventArgs e)
        {
            txtBuscarExistencias.Text = string.Empty;
            ddlEstadoExistencias.SelectedValue = "TODOS";
            CargarExistencias();
        }

        private void CargarExistencias(string filtro = "", string estado = "TODOS")
        {
            var store = new LaboratorioStore();
            string texto = (filtro ?? string.Empty).Trim();
            var articulos = store.ObtenerArticulosConExistencia();

            var filas = articulos.Select(a => new
            {
                a.Codigo,
                a.Nombre,
                a.Tipo,
                EstadoTexto = ObtenerEstadoTexto(a),
                a.CantidadDisponible,
                a.CantidadEnMantenimiento,
                a.CantidadDanada,
                a.CantidadFueraServicio,
                EstadoFiltro = ObtenerEstadoFiltro(a)
            });

            if (!string.IsNullOrWhiteSpace(texto))
            {
                filas = filas.Where(a =>
                    a.Codigo.ToString().Contains(texto) ||
                    (a.Nombre ?? string.Empty).IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (a.Tipo ?? string.Empty).IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.Equals(estado, "TODOS", StringComparison.OrdinalIgnoreCase))
            {
                filas = filas.Where(a => string.Equals(a.EstadoFiltro, estado, StringComparison.OrdinalIgnoreCase));
            }

            var resultado = filas.ToList();
            gvExistencias.DataSource = resultado;
            gvExistencias.DataBind();

            ActualizarResumen(articulos);
            lblMensaje.Text = string.Format("{0} artículo(s) encontrado(s).", resultado.Count);
        }

        private void ActualizarResumen(System.Collections.Generic.List<ArticuloConExistencia> articulos)
        {
            lblTotalDisponibles.Text = articulos.Sum(a => a.CantidadDisponible).ToString();
            lblTotalMantenimiento.Text = articulos.Sum(a => a.CantidadEnMantenimiento).ToString();
            lblTotalDanados.Text = articulos.Sum(a => a.CantidadDanada).ToString();
            lblTotalFueraServicio.Text = articulos.Sum(a => a.CantidadFueraServicio).ToString();
        }

        private static string ObtenerEstadoTexto(ArticuloConExistencia articulo)
        {
            if (EsFueraDeServicio(articulo)) return "Fuera de servicio";
            if (articulo.CantidadDanada > 0) return "Dañado";
            if (articulo.CantidadEnMantenimiento > 0) return "En mantenimiento";
            return "Disponible";
        }

        private static string ObtenerEstadoFiltro(ArticuloConExistencia articulo)
        {
            if (EsFueraDeServicio(articulo)) return "FueraDeServicio";
            if (articulo.CantidadDanada > 0) return "Danado";
            if (articulo.CantidadEnMantenimiento > 0) return "EnMantenimiento";
            return "Disponible";
        }

        private static bool EsFueraDeServicio(ArticuloConExistencia articulo)
        {
            return articulo.EstadoArticulo == EstadoArticulo.FueraDeServicio;
        }

        private static int CantidadFueraServicio(ArticuloConExistencia articulo)
        {
            return articulo.CantidadFueraServicio;
        }
    }
}