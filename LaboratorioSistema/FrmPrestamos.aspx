<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmPrestamos.aspx.cs" Inherits="LaboratorioSistema.FrmPrestamos" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Gestión de Préstamos - Laboratorio</title>
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
                    <h2>Gestión de Préstamos</h2>
                    <p>Control de préstamos de equipos, instrumentos, componentes y herramientas del laboratorio.</p>
                </div>
                </div>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert" role="alert">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <!-- Formulario principal de préstamo -->
            <div class="card">
                <div class="card-header">
                    <h2 class="card-title">Registro y Administración de Préstamos</h2>
                </div>
                <div class="card-body">
                    <div class="form-grid">
                        <div class="form-group">
                            <label for="<%= txtIdPrestamo.ClientID %>">ID Préstamo (Automático)</label>
                            <asp:TextBox ID="txtIdPrestamo" runat="server" CssClass="form-control" ReadOnly="true" placeholder="Generado automáticamente"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="<%= ddlResponsable.ClientID %>">Responsable Activo *</label>
                            <asp:DropDownList ID="ddlResponsable" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlResponsable_SelectedIndexChanged"></asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label for="<%= txtFechaPrestamo.ClientID %>">Fecha de Préstamo *</label>
                            <asp:TextBox ID="txtFechaPrestamo" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="<%= txtFechaPrevista.ClientID %>">Fecha Prevista de Devolución *</label>
                            <asp:TextBox ID="txtFechaPrevista" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="<%= ddlEstadoPrestamo.ClientID %>">Estado del Préstamo</label>
                            <asp:DropDownList ID="ddlEstadoPrestamo" runat="server" CssClass="form-control" Enabled="false">
                                <asp:ListItem Value="Activo" Text="Activo"></asp:ListItem>
                                <asp:ListItem Value="ParcialmenteDevuelto" Text="Parcialmente devuelto"></asp:ListItem>
                                <asp:ListItem Value="Devuelto" Text="Devuelto"></asp:ListItem>
                                <asp:ListItem Value="Vencido" Text="Vencido"></asp:ListItem>
                                <asp:ListItem Value="Cancelado" Text="Cancelado"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="form-group form-group-wide">
                            <label for="<%= txtObservaciones.ClientID %>">Observaciones</label>
                            <asp:TextBox ID="txtObservaciones" runat="server" CssClass="form-control" MaxLength="250" placeholder="Práctica de laboratorio, proyecto o notas..."></asp:TextBox>
                        </div>
                    </div>

                    <asp:Panel ID="pnlInfoResponsable" runat="server" CssClass="info-box" Visible="false">
                        <strong>Datos del responsable:</strong>
                        <asp:Label ID="lblInfoResponsable" runat="server"></asp:Label>
                    </asp:Panel>

                    <hr class="divider" />

                    <!-- Selección de artículos disponibles -->
                    <h3 class="section-subtitle">Artículos a Prestar</h3>
                    <div class="form-grid">
                        <div class="form-group form-group-wide">
                            <label for="<%= ddlArticulo.ClientID %>">Artículo del Inventario *</label>
                            <asp:DropDownList ID="ddlArticulo" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlArticulo_SelectedIndexChanged"></asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label>Código</label>
                            <asp:TextBox ID="txtCodigoArticulo" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>Tipo</label>
                            <asp:TextBox ID="txtTipoArticulo" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>Estado Actual</label>
                            <asp:TextBox ID="txtEstadoArticulo" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>Cantidad Disponible</label>
                            <asp:TextBox ID="txtDisponibleArticulo" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="<%= txtCantidadSolicitada.ClientID %>">Cantidad Solicitada *</label>
                            <asp:TextBox ID="txtCantidadSolicitada" runat="server" CssClass="form-control" TextMode="Number" min="1" placeholder="1"></asp:TextBox>
                        </div>

                        <div class="form-group form-align-bottom">
                            <asp:Button ID="btnAgregarArticulo" runat="server" Text="Agregar artículo" CssClass="btn btn-secondary" OnClick="btnAgregarArticulo_Click" />
                        </div>
                    </div>

                    <div class="table-responsive">
                        <asp:GridView ID="gvDetallesPrestamo" runat="server" CssClass="grid-table" AutoGenerateColumns="False"
                            DataKeyNames="IdArticulo" OnRowCommand="gvDetallesPrestamo_RowCommand"
                            EmptyDataText="No se han agregado artículos al préstamo.">
                            <Columns>
                                <asp:BoundField DataField="Codigo" HeaderText="Código" />
                                <asp:BoundField DataField="NombreArticulo" HeaderText="Artículo" />
                                <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                                <asp:BoundField DataField="CantidadSolicitada" HeaderText="Cantidad" />
                                <asp:TemplateField HeaderText="Acción">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkQuitar" runat="server" CssClass="btn btn-sm btn-danger"
                                            CommandName="Quitar" CommandArgument='<%# Eval("IdArticulo") %>'>Quitar</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <div class="form-actions">
                        <asp:Button ID="btnRegistrarPrestamo" runat="server" Text="Registrar préstamo" CssClass="btn btn-primary" OnClick="btnRegistrarPrestamo_Click" />
                        <asp:Button ID="btnCancelarPrestamo" runat="server" Text="Cancelar préstamo" CssClass="btn btn-warning" OnClick="btnCancelarPrestamo_Click"
                            OnClientClick="return confirm('¿Está seguro de cancelar este préstamo y restaurar sus existencias?');" />
                        <asp:Button ID="btnLimpiarPrestamo" runat="server" Text="Limpiar" CssClass="btn btn-outline" OnClick="btnLimpiarPrestamo_Click" />
                    </div>
                </div>
            </div>

            <!-- Tabla de préstamos registrados con búsqueda y filtro -->
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Listado de Préstamos</h3>
                    <div class="search-bar">
                        <asp:TextBox ID="txtBuscarPrestamo" runat="server" CssClass="form-control" placeholder="Buscar por ID o responsable..."></asp:TextBox>
                        <asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="form-control">
                            <asp:ListItem Value="TODOS" Text="Todos los estados"></asp:ListItem>
                            <asp:ListItem Value="Activo" Text="Activo"></asp:ListItem>
                            <asp:ListItem Value="ParcialmenteDevuelto" Text="Parcialmente devuelto"></asp:ListItem>
                            <asp:ListItem Value="Devuelto" Text="Devuelto"></asp:ListItem>
                            <asp:ListItem Value="Vencido" Text="Vencido"></asp:ListItem>
                            <asp:ListItem Value="Cancelado" Text="Cancelado"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="btnBuscarPrestamo" runat="server" Text="Buscar" CssClass="btn btn-secondary" OnClick="btnBuscarPrestamo_Click" />
                        <asp:Button ID="btnVerTodosPrestamos" runat="server" Text="Todos" CssClass="btn btn-outline" OnClick="btnVerTodosPrestamos_Click" />
                    </div>
                </div>
                <div class="card-body table-responsive">
                    <asp:GridView ID="gvPrestamos" runat="server" CssClass="grid-table" AutoGenerateColumns="False"
                        DataKeyNames="IdPrestamo" OnRowCommand="gvPrestamos_RowCommand"
                        EmptyDataText="No hay préstamos registrados.">
                        <Columns>
                            <asp:BoundField DataField="IdPrestamo" HeaderText="ID" />
                            <asp:BoundField DataField="ResponsableNombre" HeaderText="Responsable" />
                            <asp:BoundField DataField="FechaPrestamoTexto" HeaderText="Fecha Préstamo" />
                            <asp:BoundField DataField="FechaPrevistaTexto" HeaderText="Fecha Prevista" />
                            <asp:BoundField DataField="TotalArticulos" HeaderText="Cant. Artículos" />
                            <asp:BoundField DataField="EstadoTexto" HeaderText="Estado" />
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkConsultar" runat="server" CssClass="btn btn-sm btn-outline"
                                        CommandName="Consultar" CommandArgument='<%# Eval("IdPrestamo") %>'>Consultar</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            </div>
        </div>
    </form>
</body>
</html>