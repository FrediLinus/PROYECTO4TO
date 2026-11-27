
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="LaboratorioSistema.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Inicio de sesión</title>
    <link href="styles.css?v=20260324" rel="stylesheet" />
</head>
<body>

    <form id="form1" runat="server">

        <div class="login-container">

            <h2>Iniciar sesión</h2>

            <asp:Label ID="Label1" runat="server" Text="Usuario:"></asp:Label>
            <br />

            <asp:TextBox
                ID="txtUsuario"
                runat="server"
                placeholder="Ingrese su usuario">
            </asp:TextBox>

            <br /><br />

            <asp:Label ID="Label2" runat="server" Text="Contraseña:"></asp:Label>
            <br />

            <asp:TextBox
                ID="txtClave"
                runat="server"
                placeholder="Ingrese su contraseña"
                TextMode="Password">
            </asp:TextBox>

            <br /><br />

            <asp:Button
                ID="btnLogin"
                runat="server"
                Text="Ingresar"
                CssClass="btn-login"
                OnClick="btnLogin_Click"/>

            <br />

            <asp:Label
                ID="lblMensaje"
                runat="server"
                Text=""
                CssClass="error">
            </asp:Label>

        </div>

    </form>

</body>
</html>

