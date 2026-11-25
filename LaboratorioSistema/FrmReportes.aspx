<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmReportes.aspx.cs" Inherits="LaboratorioSistema.FrmReportes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Reportes - Laboratorio</title>
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

            <!-- BARRA SUPERIOR -->
            <div class="topbar">
                <div class="topbar-brand">
                    <h1>Gestión de Laboratorio</h1>
                    <span>Administración y control de equipos informáticos y electrónicos</span>
                </div>

                <div class="topbar-user">
                    <div class="user-badge">
                        Reportes
                    </div>
                </div>
            </div>

            <!-- CONTENEDOR DEL MÓDULO -->
            <div class="module-container reportes-container">

                <!-- ENCABEZADO -->
                <div class="module-header">
                    <h2>Generación de Reportes</h2>
                </div>

                <!-- FILTROS -->
                <h3>Filtros de búsqueda</h3>

                <div class="form-grid reportes-filtros">

                    <div class="form-group">
                        <label>Tipo de reporte</label>
                        <asp:DropDownList ID="ddlTipoReporte" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Seleccione un reporte" Value="" />
                            <asp:ListItem Text="Reporte de inventario" Value="Inventario" />
                            <asp:ListItem Text="Reporte de préstamos" Value="Prestamos" />
                            <asp:ListItem Text="Reporte de devoluciones" Value="Devoluciones" />
                            <asp:ListItem Text="Reporte de mantenimiento" Value="Mantenimiento" />
                            <asp:ListItem Text="Reporte de movimientos" Value="Movimientos" />
                            <asp:ListItem Text="Reporte de existencias" Value="Existencias" />
                        </asp:DropDownList>
                    </div>

                    <div class="form-group">
                        <label>Fecha inicial</label>
                        <asp:TextBox ID="txtFechaInicio" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label>Fecha final</label>
                        <asp:TextBox ID="txtFechaFin" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label>Estado</label>
                        <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Todos" Value="" />
                            <asp:ListItem Text="Disponible" Value="Disponible" />
                            <asp:ListItem Text="Prestado" Value="Prestado" />
                            <asp:ListItem Text="En mantenimiento" Value="En mantenimiento" />
                            <asp:ListItem Text="Activo" Value="Activo" />
                            <asp:ListItem Text="Finalizado" Value="Finalizado" />
                            <asp:ListItem Text="Pendiente" Value="Pendiente" />
                        </asp:DropDownList>
                    </div>

                    <div class="form-group">
                        <label>Responsable</label>
                        <asp:TextBox ID="txtResponsable" runat="server" CssClass="form-control" placeholder="Ingrese el responsable"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label>Buscar por código o nombre</label>
                        <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" placeholder="Ingrese código o nombre"></asp:TextBox>
                    </div>

                </div>

                <!-- BOTONES -->
                <div class="button-group reportes-acciones">

                    <asp:Button ID="btnGenerarReporte" runat="server"
                        Text="Generar Reporte"
                        CssClass="btn btn-primary" OnClick="btnGenerarReporte_Click" />

                    <asp:Button ID="btnLimpiar" runat="server"
                        Text="Limpiar Filtros"
                        CssClass="btn btn-secondary" OnClick="btnLimpiar_Click" />

                    <asp:Button ID="btnExportar" runat="server"
                        Text="Exportar Reporte"
                        CssClass="btn btn-success" OnClick="btnExportar_Click" />

                    <asp:Button ID="btnImprimir" runat="server"
                        Text="Imprimir Reporte"
                        CssClass="btn btn-warning" OnClick="btnImprimir_Click" />

                </div>

                <!-- MENSAJES -->
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>

                <!-- RESULTADOS -->
                <div class="reportes-resultados">

                    <div class="module-header">
                        <h2>Resultados del reporte</h2>
                    </div>

                    <div class="table-responsive">

                        <asp:GridView ID="gvReportes" runat="server"
                            AutoGenerateColumns="true"
                            CssClass="grid-table">
                        </asp:GridView>

                    </div>

                </div>

                <!-- TOTAL -->
                <div class="reportes-total">
                    <asp:Label ID="lblTotalRegistros" runat="server"
                        Text="Total de registros: 0">
                    </asp:Label>
                </div>

                <!-- REGRESAR -->
                <div class="button-group">

                    <asp:Button ID="btnRegresar" runat="server"
                        Text="Regresar al menú principal"
                        CssClass="btn btn-secondary" OnClick="btnRegresar_Click" />

                </div>

            </div>

        </div>
    </form>
</body>
</html>