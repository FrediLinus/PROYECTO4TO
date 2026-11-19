using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LaboratorioSistema
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string usuario = "admin";
            string clave = "1234";

            if (txtUsuario.Text == usuario && txtClave.Text == clave)
            {
                lblMensaje.Text = "Inicio de sesion exitoso";
                Response.Redirect("FrmPrincipal.aspx");
            }
            else
            {
                lblMensaje.Text = "Usuario o clave incorrectos";
            }
        }
    }
}