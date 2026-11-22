<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmResponsables.aspx.cs" Inherits="LaboratorioSistema.FrmResponsables" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Gestión de Responsables</title>
    <link href="styles.css" rel="stylesheet" type="text/css" />
</head>
<body class="app-body">
    <form id="form1" runat="server">
        <nav class="module-nav">
            <a href="FrmPrincipal.aspx">Principal</a>
            <a href="FrmInventario.aspx">Inventario</a>
            <a href="FrmPrestamos.aspx">Préstamos</a>
            <a href="FrmMantenimiento.aspx">Mantenimiento</a>
            <a href="FrmMovimientos.aspx">Movimientos</a>
            <a href="FrmResponsables.aspx">Responsables</a>
            <a href="FrmReportes.aspx">Reportes</a>
        </nav>
        <div class="app-wrapper">
            <div class="module-container">
                <div class="module-header">
                    <div>
                        <h2>Gestión de Responsables</h2>
                        <p>Administración de personas autorizadas para gestionar los recursos del laboratorio</p>
                    </div>
                </div>

                <h3 style="color: #173b67; margin-bottom: 20px;">Información del responsable</h3>

                <div class="form-grid">
                    <div class="form-group">
                        <label>Código</label>
                        <asp:TextBox ID="txtCodigo" runat="server" CssClass="form-control"
                            placeholder="Generado automáticamente"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Nombre</label>
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"
                            placeholder="Ingrese el nombre"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Apellido</label>
                        <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control"
                            placeholder="Ingrese el apellido"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Correo</label>
                        <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control"
                            TextMode="Email" placeholder="correo@ejemplo.com"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Teléfono</label>
                        <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control"
                            placeholder="Ingrese el teléfono"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Rol</label>
                        <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Seleccionar rol" Value=""></asp:ListItem>
                            <asp:ListItem>Estudiante</asp:ListItem>
                            <asp:ListItem>Docente</asp:ListItem>
                            <asp:ListItem>Técnico</asp:ListItem>
                            <asp:ListItem>Administrador</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label>Cargo</label>
                        <asp:TextBox ID="txtCargo" runat="server" CssClass="form-control"
                            placeholder="Ingrese el cargo"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Área asignada</label>
                        <asp:DropDownList ID="ddlAreaAsignada" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Seleccionar área" Value=""></asp:ListItem>
                            <asp:ListItem>Laboratorio de informática</asp:ListItem>
                            <asp:ListItem>Laboratorio de electrónica</asp:ListItem>
                            <asp:ListItem>Bodega</asp:ListItem>
                            <asp:ListItem>Taller</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="button-group">
                    <asp:Button ID="btnNuevo" runat="server" Text="+ Nuevo"
                        CssClass="btn btn-secondary" OnClick="btnNuevo_Click" />
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar"
                        CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                    <asp:Button ID="btnModificar" runat="server" Text="Modificar"
                        CssClass="btn btn-warning" OnClick="btnModificar_Click" />
                    <asp:Button ID="btnEliminar" runat="server" Text="Eliminar"
                        CssClass="btn btn-danger" OnClick="btnEliminar_Click" />
                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar"
                        CssClass="btn btn-primary" OnClick="btnBuscar_Click" />
                    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar"
                        CssClass="btn btn-secondary" OnClick="btnLimpiar_Click" />
                    <asp:Button ID="btnActualizar" runat="server" Text="Actualizar"
                        CssClass="btn btn-primary" OnClick="btnActualizar_Click" />
                </div>

                <asp:Label ID="lblMensaje" runat="server"></asp:Label>

                <div class="module-header" style="margin-top: 35px;">
                    <div>
                        <h2>Responsables registrados</h2>
                    </div>
                </div>

                <div class="search-bar">
                    <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control"
                        placeholder="Buscar por código, nombre, apellido o correo..."
                        style="max-width: 430px;"></asp:TextBox>
                    <asp:Button ID="btnBuscarListado" runat="server" Text="Buscar"
                        CssClass="btn btn-primary" OnClick="btnBuscarListado_Click" />
                </div>

                <div class="table-responsive">
                    <asp:GridView ID="gvResponsables" runat="server" CssClass="grid-table"
                        AutoGenerateColumns="False" GridLines="None"
                        EmptyDataText="No hay responsables registrados." Width="100%">
                        <Columns>
                            <asp:BoundField DataField="Codigo" HeaderText="Código" />
                            <asp:BoundField DataField="NombreCompleto" HeaderText="Nombre" />
                            <asp:BoundField DataField="Correo" HeaderText="Correo" />
                            <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                            <asp:BoundField DataField="Rol" HeaderText="Rol" />
                            <asp:BoundField DataField="Cargo" HeaderText="Cargo" />
                            <asp:BoundField DataField="AreaAsignada" HeaderText="Área asignada" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
