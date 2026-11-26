
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmInventario.aspx.cs" Inherits="LaboratorioSistema.FrmInventario" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Gestión de Inventario</title>
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

            <!-- ENCABEZADO -->
            <div class="module-container">

                <div class="module-header">
                    <div>
                        <h2>Gestión de Inventario</h2>
                        <p>Administración y control de equipos informáticos y electrónicos</p>
                    </div>
                </div>

                <!-- INFORMACIÓN DEL ARTÍCULO -->
                <h3 style="color: #173b67; margin-bottom: 20px;">
                    Información del artículo
                </h3>

                <div class="form-grid">

                    <!-- ID -->
                    <div class="form-group">
                        <label>ID del artículo</label>
                        <asp:TextBox ID="txtIdArticulo" runat="server"
                            CssClass="form-control"
                            placeholder="Generado automáticamente">
                        </asp:TextBox>
                    </div>

                    <!-- NOMBRE -->
                    <div class="form-group">
                        <label>Nombre del artículo</label>
                        <asp:TextBox ID="txtNombre" runat="server"
                            CssClass="form-control"
                            placeholder="Ingrese el nombre">
                        </asp:TextBox>
                    </div>

                    <!-- TIPO -->
                    <div class="form-group">
                        <label>Tipo de artículo</label>
                        <asp:DropDownList ID="ddlTipo" runat="server"
                            CssClass="form-control">

                            <asp:ListItem Text="Seleccionar tipo" Value=""></asp:ListItem>
                            <asp:ListItem>Equipo informático</asp:ListItem>
                            <asp:ListItem>Herramienta</asp:ListItem>
                            <asp:ListItem>Instrumento de medición</asp:ListItem>
                            <asp:ListItem>Componente electrónico</asp:ListItem>

                        </asp:DropDownList>
                    </div>

                    <!-- MARCA -->
                    <div class="form-group">
                        <label>Marca</label>
                        <asp:TextBox ID="txtMarca" runat="server"
                            CssClass="form-control"
                            placeholder="Ingrese la marca">
                        </asp:TextBox>
                    </div>

                    <!-- MODELO -->
                    <div class="form-group">
                        <label>Modelo</label>
                        <asp:TextBox ID="txtModelo" runat="server"
                            CssClass="form-control"
                            placeholder="Ingrese el modelo">
                        </asp:TextBox>
                    </div>

                    <!-- NUMERO DE SERIE -->
                    <div class="form-group">
                        <label>Número de serie</label>
                        <asp:TextBox ID="txtNumeroSerie" runat="server"
                            CssClass="form-control"
                            placeholder="Ingrese el número de serie">
                        </asp:TextBox>
                    </div>

                    <!-- DESCRIPCION -->
                    <div class="form-group">
                        <label>Descripción</label>
                        <asp:TextBox ID="txtDescripcion" runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="3"
                            placeholder="Descripción del artículo">
                        </asp:TextBox>
                    </div>

                    <!-- CANTIDAD -->
                    <div class="form-group">
                        <label>Cantidad</label>
                        <asp:TextBox ID="txtCantidad" runat="server"
                            CssClass="form-control"
                            TextMode="Number"
                            Text="1">
                        </asp:TextBox>
                    </div>

                    <!-- ESTADO -->
                    <div class="form-group">
                        <label>Estado</label>
                        <asp:DropDownList ID="ddlEstado" runat="server"
                            CssClass="form-control">

                            <asp:ListItem Text="Seleccionar estado" Value=""></asp:ListItem>
                            <asp:ListItem>Disponible</asp:ListItem>
                            <asp:ListItem>Prestado</asp:ListItem>
                            <asp:ListItem>En mantenimiento</asp:ListItem>
                            <asp:ListItem>Dañado</asp:ListItem>
                            <asp:ListItem>Fuera de servicio</asp:ListItem>

                        </asp:DropDownList>
                    </div>

                    <!-- UBICACION -->
                    <div class="form-group">
                        <label>Ubicación</label>
                        <asp:DropDownList ID="ddlUbicacion" runat="server"
                            CssClass="form-control">

                            <asp:ListItem Text="Seleccionar ubicación" Value=""></asp:ListItem>
                            <asp:ListItem>Laboratorio de informática</asp:ListItem>
                            <asp:ListItem>Laboratorio de electrónica</asp:ListItem>
                            <asp:ListItem>Bodega</asp:ListItem>
                            <asp:ListItem>Taller</asp:ListItem>

                        </asp:DropDownList>
                    </div>

                    <!-- FECHA -->
                    <div class="form-group">
                        <label>Fecha de adquisición</label>
                        <asp:TextBox ID="txtFechaAdquisicion" runat="server"
                            CssClass="form-control"
                            TextMode="Date">
                        </asp:TextBox>
                    </div>

                    <!-- STOCK MINIMO -->
                    <div class="form-group">
                        <label>Stock mínimo</label>
                        <asp:TextBox ID="txtStockMinimo" runat="server"
                            CssClass="form-control"
                            TextMode="Number"
                            Text="1">
                        </asp:TextBox>
                    </div>

                </div>

                <!-- BOTONES -->
                <div class="button-group">

                    <asp:Button ID="btnNuevo" runat="server"
                        Text="+ Nuevo"
                        CssClass="btn btn-secondary" OnClick="btnNuevo_Click" />

                    <asp:Button ID="btnGuardar" runat="server"
                          Text="Guardar"
                          CssClass="btn btn-primary"
                          OnClick="btnGuardar_Click"/>

                    <asp:Button ID="btnModificar" runat="server"
                        Text="Modificar"
                        CssClass="btn btn-warning"
                        OnClick="btnModificar_Click" />

                    <asp:Button ID="btnEliminar" runat="server"
                        Text="Eliminar"
                        CssClass="btn btn-danger"
                        OnClick="btnEliminar_Click" />

                    <asp:Button ID="btnBuscar" runat="server"
                        Text="Buscar"
                        CssClass="btn btn-primary"
                        OnClick="btnBuscar_Click" />

                    <asp:Button ID="btnLimpiar" runat="server"
                        Text="Limpiar"
                        CssClass="btn btn-secondary"
                        OnClick="btnLimpiar_Click" />

                    <asp:Button ID="btnActualizar" runat="server"
                        Text="Actualizar"
                        CssClass="btn btn-primary"
                        OnClick="btnActualizar_Click" />

                </div>

                <!-- MENSAJES -->
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>

                <!-- LISTADO DE INVENTARIO -->
                <div class="module-header" style="margin-top: 35px;">
                    <div>
                        <h2>Listado de artículos registrados</h2>
                    </div>
                </div>

                <!-- BUSCADOR -->
                <div class="search-bar">

                    <asp:TextBox ID="txtBuscar" runat="server"
                        CssClass="form-control"
                        placeholder="Buscar por ID, nombre o número de serie..."
                        style="max-width: 400px;">
                    </asp:TextBox>

                    <asp:Button ID="btnBuscarArticulo" runat="server"
                        Text="Buscar"
                        CssClass="btn btn-primary"
                        OnClick="btnBuscarArticulo_Click" />

                </div>

                <!-- TABLA -->
                <div class="table-responsive">

                    <asp:GridView ID="gvInventario" runat="server"
                        CssClass="grid-table"
                        AutoGenerateColumns="False"
                        GridLines="None"
                        EmptyDataText="No hay artículos registrados."
                        Width="100%"
                        DataKeyNames="IdArticulo"
                        OnRowCommand="gvInventario_RowCommand">

                        <Columns>

                            <asp:BoundField
                                DataField="IdArticulo"
                                HeaderText="ID" />

                            <asp:BoundField
                                DataField="Nombre"
                                HeaderText="Nombre" />

                            <asp:BoundField
                                DataField="Tipo"
                                HeaderText="Tipo" />

                            <asp:BoundField
                                DataField="Marca"
                                HeaderText="Marca" />

                            <asp:BoundField
                                DataField="Cantidad"
                                HeaderText="Cantidad" />

                            <asp:BoundField
                                DataField="Estado"
                                HeaderText="Estado" />

                            <asp:BoundField
                                DataField="Ubicacion"
                                HeaderText="Ubicación" />

                            <asp:ButtonField
                                ButtonType="Button"
                                CommandName="EliminarArticulo"
                                HeaderText="Acciones"
                                Text="Eliminar" />

                        </Columns>

                    </asp:GridView>

                </div>

            </div>

        </div>

    </form>

</body>
</html>
