<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmResponsables.aspx.cs" Inherits="LaboratorioSistema.FrmResponsables" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Control de Existencias</title>
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
            <a href="FrmResponsables.aspx">Control de existencias</a>
            <a href="FrmReportes.aspx">Reportes</a>
        </nav>

        <div class="app-wrapper">
            <div class="module-container">
                <div class="module-header">
                    <div>
                        <h2>Control de Existencias</h2>
                        <p>Consulta de equipos disponibles, en mantenimiento, dañados y fuera de servicio.</p>
                    </div>
                </div>

                <div class="form-grid">
                    <div class="form-group">
                        <label for="<%= txtBuscarExistencias.ClientID %>">Buscar equipo o artículo</label>
                        <asp:TextBox ID="txtBuscarExistencias" runat="server" CssClass="form-control"
                            placeholder="Código, nombre o tipo"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="<%= ddlEstadoExistencias.ClientID %>">Estado</label>
                        <asp:DropDownList ID="ddlEstadoExistencias" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Todos los estados" Value="TODOS"></asp:ListItem>
                            <asp:ListItem Text="Disponible" Value="Disponible"></asp:ListItem>
                            <asp:ListItem Text="En mantenimiento" Value="EnMantenimiento"></asp:ListItem>
                            <asp:ListItem Text="Dañado" Value="Danado"></asp:ListItem>
                            <asp:ListItem Text="Fuera de servicio" Value="FueraDeServicio"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="button-group">
                    <asp:Button ID="btnFiltrarExistencias" runat="server" Text="Filtrar"
                        CssClass="btn btn-primary" OnClick="btnFiltrarExistencias_Click" />
                    <asp:Button ID="btnMostrarTodasExistencias" runat="server" Text="Mostrar todos"
                        CssClass="btn btn-secondary" OnClick="btnMostrarTodasExistencias_Click" />
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="success-msg"></asp:Label>

                <div class="form-grid">
                    <div class="info-box">
                        <strong>Disponibles:</strong>
                        <asp:Label ID="lblTotalDisponibles" runat="server"></asp:Label>
                    </div>
                    <div class="info-box">
                        <strong>En mantenimiento:</strong>
                        <asp:Label ID="lblTotalMantenimiento" runat="server"></asp:Label>
                    </div>
                    <div class="info-box">
                        <strong>Dañados:</strong>
                        <asp:Label ID="lblTotalDanados" runat="server"></asp:Label>
                    </div>
                    <div class="info-box">
                        <strong>Fuera de servicio:</strong>
                        <asp:Label ID="lblTotalFueraServicio" runat="server"></asp:Label>
                    </div>
                </div>

                <div class="module-header" style="margin-top: 25px;">
                    <div>
                        <h2>Existencias registradas</h2>
                        <p>Estado actual de los artículos y equipos del laboratorio.</p>
                    </div>
                </div>

                <div class="table-responsive">
                    <asp:GridView ID="gvExistencias" runat="server" CssClass="grid-table"
                        AutoGenerateColumns="False" EmptyDataText="No hay existencias que coincidan con el filtro.">
                        <Columns>
                            <asp:BoundField DataField="Codigo" HeaderText="Código" />
                            <asp:BoundField DataField="Nombre" HeaderText="Artículo / Equipo" />
                            <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                            <asp:BoundField DataField="EstadoTexto" HeaderText="Estado" />
                            <asp:BoundField DataField="CantidadDisponible" HeaderText="Disponibles" />
                            <asp:BoundField DataField="CantidadEnMantenimiento" HeaderText="En mantenimiento" />
                            <asp:BoundField DataField="CantidadDanada" HeaderText="Dañados" />
                            <asp:BoundField DataField="CantidadFueraServicio" HeaderText="Fuera de servicio" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
