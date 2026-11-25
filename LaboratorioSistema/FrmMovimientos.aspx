<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmMovimientos.aspx.cs" Inherits="LaboratorioSistema.FrmMovimientos" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Historial de Movimientos - Laboratorio</title>
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
                    <h2>Historial de Movimientos de Inventario</h2>
                    <p>Trazabilidad completa de entradas, salidas, préstamos, devoluciones, mantenimientos, bajas y ajustes.</p>
                </div>
                </div>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert" role="alert">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <!-- Registro manual de movimiento administrativo (Entrada / Salida / Ajuste / Baja) -->
            <div class="card">
                <div class="card-header">
                    <h2 class="card-title">Registro de Movimiento Administrativo</h2>
                </div>
                <div class="card-body">
                    <div class="form-grid">
                        <div class="form-group">
                            <label for="<%= txtIdMovimiento.ClientID %>">ID Movimiento (Automático)</label>
                            <asp:TextBox ID="txtIdMovimiento" runat="server" CssClass="form-control" ReadOnly="true" placeholder="Generado automáticamente"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="<%= ddlArticuloMov.ClientID %>">Artículo *</label>
                            <asp:DropDownList ID="ddlArticuloMov" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label for="<%= ddlTipoMovimiento.ClientID %>">Tipo de Movimiento *</label>
                            <asp:DropDownList ID="ddlTipoMovimiento" runat="server" CssClass="form-control">
                                <asp:ListItem Value="Entrada" Text="Entrada (Ingreso de existencias)"></asp:ListItem>
                                <asp:ListItem Value="Salida" Text="Salida (Consumo interno)"></asp:ListItem>
                                <asp:ListItem Value="Ajuste" Text="Ajuste de inventario"></asp:ListItem>
                                <asp:ListItem Value="Baja" Text="Baja (Fuera de servicio)"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label for="<%= txtCantidadMov.ClientID %>">Cantidad *</label>
                            <asp:TextBox ID="txtCantidadMov" runat="server" CssClass="form-control" TextMode="Number" min="1" placeholder="1"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="<%= txtReferenciaId.ClientID %>">Referencia / Documento</label>
                            <asp:TextBox ID="txtReferenciaId" runat="server" CssClass="form-control" MaxLength="50" placeholder="Ej. ACTA-012 o INV-2026"></asp:TextBox>
                        </div>

                        <div class="form-group form-group-wide">
                            <label for="<%= txtDescripcionMov.ClientID %>">Descripción / Motivo *</label>
                            <asp:TextBox ID="txtDescripcionMov" runat="server" CssClass="form-control" MaxLength="250" placeholder="Justificación del movimiento registrado..."></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-actions">
                        <asp:Button ID="btnRegistrarMovimiento" runat="server" Text="Registrar movimiento" CssClass="btn btn-primary" OnClick="btnRegistrarMovimiento_Click" />
                        <asp:Button ID="btnLimpiarMov" runat="server" Text="Limpiar" CssClass="btn btn-outline" OnClick="btnLimpiarMov_Click" />
                    </div>
                </div>
            </div>

            <!-- Consulta, filtros por criterio/tipo/rango de fechas y tabla histórica -->
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Bitácora General de Movimientos</h3>
                    <div class="search-bar">
                        <asp:TextBox ID="txtBuscarMov" runat="server" CssClass="form-control" placeholder="Buscar por ID, artículo, referencia o descripción..."></asp:TextBox>
                        <asp:DropDownList ID="ddlFiltroTipoMov" runat="server" CssClass="form-control">
                            <asp:ListItem Value="TODOS" Text="Todos los tipos"></asp:ListItem>
                            <asp:ListItem Value="Entrada" Text="Entrada"></asp:ListItem>
                            <asp:ListItem Value="Salida" Text="Salida"></asp:ListItem>
                            <asp:ListItem Value="Prestamo" Text="Préstamo"></asp:ListItem>
                            <asp:ListItem Value="Devolucion" Text="Devolución"></asp:ListItem>
                            <asp:ListItem Value="InicioMantenimiento" Text="Inicio mantenimiento"></asp:ListItem>
                            <asp:ListItem Value="FinalizacionMantenimiento" Text="Fin mantenimiento"></asp:ListItem>
                            <asp:ListItem Value="Ajuste" Text="Ajuste"></asp:ListItem>
                            <asp:ListItem Value="Baja" Text="Baja"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:TextBox ID="txtFechaDesde" runat="server" CssClass="form-control" TextMode="Date" ToolTip="Fecha desde"></asp:TextBox>
                        <asp:TextBox ID="txtFechaHasta" runat="server" CssClass="form-control" TextMode="Date" ToolTip="Fecha hasta"></asp:TextBox>
                        <asp:Button ID="btnFiltrarMov" runat="server" Text="Filtrar" CssClass="btn btn-secondary" OnClick="btnFiltrarMov_Click" />
                        <asp:Button ID="btnTodosMov" runat="server" Text="Todos" CssClass="btn btn-outline" OnClick="btnTodosMov_Click" />
                    </div>
                </div>
                <div class="card-body table-responsive">
                    <asp:GridView ID="gvMovimientos" runat="server" CssClass="grid-table" AutoGenerateColumns="False"
                        DataKeyNames="IdMovimiento" EmptyDataText="No existen movimientos registrados para los filtros aplicados.">
                        <Columns>
                            <asp:BoundField DataField="IdMovimiento" HeaderText="ID" />
                            <asp:BoundField DataField="FechaHoraTexto" HeaderText="Fecha y Hora" />
                            <asp:BoundField DataField="TipoTexto" HeaderText="Tipo Movimiento" />
                            <asp:BoundField DataField="NombreArticulo" HeaderText="Artículo" />
                            <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                            <asp:BoundField DataField="ReferenciaId" HeaderText="Referencia" />
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            </div>
        </div>
    </form>
</body>
</html>