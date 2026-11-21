
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmResponsables.aspx.cs" Inherits="LaboratorioSistema.FrmResponsables" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Control de Existencias</title>

    <!-- CSS DEL SISTEMA -->
    <link href="styles.css" rel="stylesheet" type="text/css" />

</head>

<body class="app-body">

    <form id="form1" runat="server">

        <div class="app-wrapper">

            <div class="module-container">

                <!-- ENCABEZADO -->
                <div class="module-header">
                    <div>
                        <h2>Control de Existencias</h2>
                        <p>Administración y control de materiales y equipos disponibles en el laboratorio</p>
                    </div>
                </div>

                <!-- INFORMACIÓN DE EXISTENCIAS -->
                <h3 style="color: #173b67; margin-bottom: 20px;">
                    Información de existencias
                </h3>

                <div class="form-grid">

                    <!-- ID EXISTENCIA -->
                    <div class="form-group">
                        <label>ID de existencia</label>
                        <asp:TextBox ID="txtIdExistencia" runat="server"
                            CssClass="form-control"
                            ReadOnly="true"
                            placeholder="Generado automáticamente">
                        </asp:TextBox>
                    </div>

                    <!-- CODIGO ARTICULO -->
                    <div class="form-group">
                        <label>Código del artículo</label>
                        <asp:TextBox ID="txtCodigoArticulo" runat="server"
                            CssClass="form-control"
                            placeholder="Ej. EQ001">
                        </asp:TextBox>
                    </div>

                    <!-- NOMBRE ARTICULO -->
                    <div class="form-group">
                        <label>Nombre del artículo</label>
                        <asp:TextBox ID="txtNombreArticulo" runat="server"
                            CssClass="form-control"
                            placeholder="Nombre del artículo">
                        </asp:TextBox>
                    </div>

                    <!-- TIPO ARTICULO -->
                    <div class="form-group">
                        <label>Tipo de artículo</label>
                        <asp:DropDownList ID="ddlTipoArticulo" runat="server"
                            CssClass="form-control">

                            <asp:ListItem Text="Seleccionar tipo" Value=""></asp:ListItem>
                            <asp:ListItem>Equipo informático</asp:ListItem>
                            <asp:ListItem>Herramienta</asp:ListItem>
                            <asp:ListItem>Instrumento de medición</asp:ListItem>
                            <asp:ListItem>Componente electrónico</asp:ListItem>

                        </asp:DropDownList>
                    </div>

                    <!-- CANTIDAD EXISTENTE -->
                    <div class="form-group">
                        <label>Cantidad existente</label>
                        <asp:TextBox ID="txtCantidadExistente" runat="server"
                            CssClass="form-control"
                            TextMode="Number"
                            Text="0">
                        </asp:TextBox>
                    </div>

                    <!-- CANTIDAD MINIMA -->
                    <div class="form-group">
                        <label>Cantidad mínima permitida</label>
                        <asp:TextBox ID="txtCantidadMinima" runat="server"
                            CssClass="form-control"
                            TextMode="Number"
                            Text="1">
                        </asp:TextBox>
                    </div>

                    <!-- CANTIDAD MAXIMA -->
                    <div class="form-group">
                        <label>Cantidad máxima</label>
                        <asp:TextBox ID="txtCantidadMaxima" runat="server"
                            CssClass="form-control"
                            TextMode="Number"
                            Text="100">
                        </asp:TextBox>
                    </div>

                    <!-- CANTIDAD ENTRADA -->
                    <div class="form-group">
                        <label>Cantidad de entrada</label>
                        <asp:TextBox ID="txtCantidadEntrada" runat="server"
                            CssClass="form-control"
                            TextMode="Number"
                            Text="0">
                        </asp:TextBox>
                    </div>

                    <!-- CANTIDAD SALIDA -->
                    <div class="form-group">
                        <label>Cantidad de salida</label>
                        <asp:TextBox ID="txtCantidadSalida" runat="server"
                            CssClass="form-control"
                            TextMode="Number"
                            Text="0">
                        </asp:TextBox>
                    </div>

                    <!-- CANTIDAD DISPONIBLE -->
                    <div class="form-group">
                        <label>Cantidad disponible</label>
                        <asp:TextBox ID="txtCantidadDisponible" runat="server"
                            CssClass="form-control"
                            ReadOnly="true"
                            placeholder="Calculado automáticamente">
                        </asp:TextBox>
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

                    <!-- FECHA ACTUALIZACION -->
                    <div class="form-group">
                        <label>Fecha de actualización</label>
                        <asp:TextBox ID="txtFechaActualizacion" runat="server"
                            CssClass="form-control"
                            TextMode="Date">
                        </asp:TextBox>
                    </div>

                    <!-- ESTADO EXISTENCIA -->
                    <div class="form-group">
                        <label>Estado de existencia</label>
                        <asp:DropDownList ID="ddlEstadoExistencia" runat="server"
                            CssClass="form-control">

                            <asp:ListItem Text="Seleccionar estado" Value=""></asp:ListItem>
                            <asp:ListItem>Disponible</asp:ListItem>
                            <asp:ListItem>Existencias bajas</asp:ListItem>
                            <asp:ListItem>Agotado</asp:ListItem>

                        </asp:DropDownList>
                    </div>

                </div>

                <!-- BOTONES -->
                <div class="button-group">

                    <asp:Button ID="btnRegistrarEntrada" runat="server"
                        Text="Registrar entrada"
                        CssClass="btn btn-success" />

                    <asp:Button ID="btnRegistrarSalida" runat="server"
                        Text="Registrar salida"
                        CssClass="btn btn-warning" />

                    <asp:Button ID="btnActualizarExistencias" runat="server"
                        Text="Actualizar existencias"
                        CssClass="btn btn-primary" />

                    <asp:Button ID="btnBuscarArticulo" runat="server"
                        Text="Buscar artículo"
                        CssClass="btn btn-secondary" />

                    <asp:Button ID="btnExistenciasBajas" runat="server"
                        Text="Mostrar existencias bajas"
                        CssClass="btn btn-danger" />

                    <asp:Button ID="btnLimpiar" runat="server"
                        Text="Limpiar"
                        CssClass="btn btn-secondary" />

                    <asp:Button ID="btnActualizarTabla" runat="server"
                        Text="Actualizar tabla"
                        CssClass="btn btn-primary" />

                </div>

                <!-- MENSAJES -->
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>

                <!-- LISTADO DE EXISTENCIAS -->
                <div class="module-header" style="margin-top: 35px;">
                    <div>
                        <h2>Listado de existencias registradas</h2>
                    </div>
                </div>

                <!-- BUSCADOR -->
                <div class="search-bar">

                    <asp:TextBox ID="txtBuscar" runat="server"
                        CssClass="form-control"
                        placeholder="Buscar por código o nombre del artículo..."
                        style="max-width: 400px;">
                    </asp:TextBox>

                    <asp:Button ID="btnBuscar" runat="server"
                        Text="Buscar"
                        CssClass="btn btn-primary" />

                </div>

                <!-- TABLA DE EXISTENCIAS -->
                <div class="table-responsive">

                    <asp:GridView ID="gvExistencias" runat="server"
                        CssClass="grid-table"
                        AutoGenerateColumns="False"
                        GridLines="None"
                        EmptyDataText="No hay existencias registradas."
                        Width="100%">

                        <Columns>

                            <asp:BoundField
                                DataField="CodigoArticulo"
                                HeaderText="Código" />

                            <asp:BoundField
                                DataField="NombreArticulo"
                                HeaderText="Artículo" />

                            <asp:BoundField
                                DataField="CantidadExistente"
                                HeaderText="Existencia" />

                            <asp:BoundField
                                DataField="CantidadMinima"
                                HeaderText="Mínimo" />

                            <asp:BoundField
                                DataField="EstadoExistencia"
                                HeaderText="Disponible" />

                        </Columns>

                    </asp:GridView>

                </div>

            </div>

        </div>

    </form>

</body>
</html>
