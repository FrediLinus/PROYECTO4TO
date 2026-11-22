<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmPrestamos.aspx.cs" Inherits="LaboratorioSistema.FrmPrestamos" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Gestión de Préstamos - Laboratorio</title>
    <link href="Content/estilos.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="page-container">
            <div class="page-header">
                <h1 class="page-title">Gestión de Préstamos de Laboratorio</h1>
                <p class="page-subtitle">Registro, consulta, detalle y cancelación de préstamos de equipos, instrumentos, componentes y herramientas.</p>
                <div class="nav-links">
                    <a href="Principal.aspx" class="btn btn-outline">Inicio</a>
                    <a href="Responsables.aspx" class="btn btn-outline">Responsables</a>
                    <a href="Devoluciones.aspx" class="btn btn-outline">Devoluciones</a>
                    <a href="Mantenimiento.aspx" class="btn btn-outline">Mantenimiento</a>
                    <a href="Movimientos.aspx" class="btn btn-outline">Movimientos</a>
                </div>
            </div>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert" role="alert">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <div class="card">
                <div class="card-header">
                    <h2 class="card-title">Datos del Préstamo</h2>
                </div>
                <div class="card-body">
                    <div class="form-grid">
                        <div class="form-group">
                            <label for="<%= txtPrestamoId.ClientID %>">ID Préstamo (Automático)</label>
                            <asp:TextBox ID="txtPrestamoId" runat="server" CssClass="form-control" ReadOnly="true" placeholder="Generado al guardar"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="<%= ddlResponsableActivo.ClientID %>">Responsable Activo *</label>
                            <asp:DropDownList ID="ddlResponsableActivo" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlResponsableActivo_SelectedIndexChanged"></asp:DropDownList>
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
                            <label for="<%= txtEstadoPrestamo.ClientID %>">Estado del Préstamo</label>
                            <asp:TextBox ID="txtEstadoPrestamo" runat="server" CssClass="form-control" ReadOnly="true" Text="Activo"></asp:TextBox>
                        </div>

                        <div class="form-group form-group-wide">
                            <label for="<%= txtObservacionesPrestamo.ClientID %>">Observaciones</label>
                            <asp:TextBox ID="txtObservacionesPrestamo" runat="server" CssClass="form-control" MaxLength="250" placeholder="Notas adicionales del préstamo..."></asp:TextBox>
                        </div>
                    </div>

                    <asp:Panel ID="pnlResumenResponsable" runat="server" CssClass="info-box" Visible="false">
                        <strong>Responsable seleccionado:</strong>
                        <asp:Label ID="lblRespResumen" runat="server"></asp:Label>
                    </asp:Panel>

                    <hr class="divider" />

                    <h3 class="section-subtitle">Seleccionar Artículos del Inventario</h3>
                    <div class="form-grid">
                        <div class="form-group form-group-wide">
                            <label for="<%= ddlArticuloInventario.ClientID %>">Artículo del Inventario</label>
                            <asp:DropDownList ID="ddlArticuloInventario" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlArticuloInventario_SelectedIndexChanged"></asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label>Código</label>
                            <asp:TextBox ID="txtArtCodigo" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>Nombre</label>
                            <asp:TextBox ID="txtArtNombre" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>Tipo</label>
                            <asp:TextBox ID="txtArtTipo" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>Estado</label>
                            <asp:TextBox ID="txtArtEstado" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>Cantidad Disponible</label>
                            <asp:TextBox ID="txtArtDisponible" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="<%= txtCantidadSolicitada.ClientID %>">Cantidad Solicitada *</label>
                            <asp:TextBox ID="txtCantidadSolicitada" runat="server" CssClass="form-control" TextMode="Number" min="1" placeholder="1"></asp:TextBox>
                        </div>

                        <div class="form-group form-align-bottom">
                            <asp:Button ID="btnAgregarArticulo" runat="server" Text="Agregar artículo" CssClass="btn btn-secondary" OnClick="btnAgregarArticulo_Click" />
                        </div>
                    </div>

                    <h4 class="table-caption">Artículos incluidos en el préstamo actual</h4>
                    <div class="table-responsive">
                        <asp:GridView ID="gvDetallesTemporales" runat="server" CssClass="table" AutoGenerateColumns="False"
                            DataKeyNames="IdArticulo" OnRowCommand="gvDetallesTemporales_RowCommand"
                            EmptyDataText="Aún no se han agregado artículos a este préstamo.">
                            <Columns>
                                <asp:BoundField DataField="Codigo" HeaderText="Código" />
                                <asp:BoundField DataField="NombreArticulo" HeaderText="Artículo" />
                                <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                                <asp:BoundField DataField="CantidadSolicitada" HeaderText="Cantidad Solicitada" />
                                <asp:TemplateField HeaderText="Acción">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnQuitarDetalle" runat="server" CssClass="btn btn-sm btn-danger"
                                            CommandName="Quitar" CommandArgument='<%# Eval("IdArticulo") %>'>Quitar artículo</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <asp:Panel ID="pnlConsultaPrestamo" runat="server" CssClass="info-panel" Visible="false">
                        <h4>Detalle del Préstamo Consultado: <asp:Label ID="lblDetallePrestamoId" runat="server"></asp:Label></h4>
                        <p><asp:Label ID="lblDetallePrestamoInfo" runat="server"></asp:Label></p>
                        <div class="table-responsive">
                            <asp:GridView ID="gvDetalleConsultado" runat="server" CssClass="table" AutoGenerateColumns="False"
                                EmptyDataText="Sin detalles registrados.">
                                <Columns>
                                    <asp:BoundField DataField="IdArticulo" HeaderText="ID Artículo" />
                                    <asp:BoundField DataField="NombreArticulo" HeaderText="Artículo" />
                                    <asp:BoundField DataField="CantidadPrestada" HeaderText="Prestada" />
                                    <asp:BoundField DataField="CantidadDevuelta" HeaderText="Devuelta" />
                                    <asp:BoundField DataField="CantidadPendiente" HeaderText="Pendiente" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlConfirmarCancelar" runat="server" CssClass="warning-box" Visible="false">
                        <p>
                            <strong>Confirmación requerida:</strong> ¿Está seguro de cancelar el préstamo
                            #<asp:Label ID="lblCancelarPrestamoId" runat="server"></asp:Label>? Se restaurarán las existencias pendientes y se registrará el movimiento correspondiente.
                        </p>
                        <div class="form-group">
                            <label for="<%= txtMotivoCancelacion.ClientID %>">Motivo de cancelación *</label>
                            <asp:TextBox ID="txtMotivoCancelacion" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
                        </div>
                        <asp:Button ID="btnConfirmarCancelarPrestamo" runat="server" Text="Confirmar cancelación" CssClass="btn btn-danger" OnClick="btnConfirmarCancelarPrestamo_Click" />
                        <asp:Button ID="btnAbortarCancelarPrestamo" runat="server" Text="No cancelar" CssClass="btn btn-outline" OnClick="btnAbortarCancelarPrestamo_Click" />
                    </asp:Panel>

                    <div class="form-actions">
                        <asp:Button ID="btnRegistrarPrestamo" runat="server" Text="Registrar préstamo" CssClass="btn btn-primary" OnClick="btnRegistrarPrestamo_Click" />
                        <asp:Button ID="btnBuscarPrestamo" runat="server" Text="Buscar préstamo" CssClass="btn btn-secondary" OnClick="btnBuscarPrestamo_Click" />
                        <asp:Button ID="btnConsultarDetallesPrestamo" runat="server" Text="Consultar detalles" CssClass="btn btn-outline" OnClick="btnConsultarDetallesPrestamo_Click" />
                        <asp:Button ID="btnCancelarPrestamo" runat="server" Text="Cancelar préstamo" CssClass="btn btn-warning" OnClick="btnSolicitarCancelarPrestamo_Click" />
                        <asp:Button ID="btnLimpiarPrestamo" runat="server" Text="Limpiar" CssClass="btn btn-outline" OnClick="btnLimpiarPrestamo_Click" />
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Préstamos Registrados</h3>
                    <div class="search-bar">
                        <asp:TextBox ID="txtBuscarPrestamo" runat="server" CssClass="form-control" placeholder="Buscar por ID préstamo o responsable..."></asp:TextBox>
                        <asp:DropDownList ID="ddlFiltroEstadoPrestamo" runat="server" CssClass="form-control">
                            <asp:ListItem Value="TODOS" Text="Todos los estados"></asp:ListItem>
                            <asp:ListItem Value="Activo" Text="Activo"></asp:ListItem>
                            <asp:ListItem Value="ParcialmenteDevuelto" Text="Parcialmente devuelto"></asp:ListItem>
                            <asp:ListItem Value="Devuelto" Text="Devuelto"></asp:ListItem>
                            <asp:ListItem Value="Vencido" Text="Vencido"></asp:ListItem>
                            <asp:ListItem Value="Cancelado" Text="Cancelado"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="btnFiltrarPrestamos" runat="server" Text="Buscar / Filtrar" CssClass="btn btn-secondary" OnClick="btnFiltrarPrestamos_Click" />
                    </div>
                </div>
                <div class="card-body table-responsive">
                    <asp:GridView ID="gvPrestamos" runat="server" CssClass="table" AutoGenerateColumns="False"
                        DataKeyNames="IdPrestamo" OnRowCommand="gvPrestamos_RowCommand" EmptyDataText="No se encontraron préstamos.">
                        <Columns>
                            <asp:BoundField DataField="IdPrestamo" HeaderText="ID Préstamo" />
                            <asp:BoundField DataField="ResponsableNombre" HeaderText="Responsable" />
                            <asp:BoundField DataField="FechaPrestamoTexto" HeaderText="Fecha Préstamo" />
                            <asp:BoundField DataField="FechaPrevistaTexto" HeaderText="Fecha Prevista" />
                            <asp:BoundField DataField="CantidadArticulos" HeaderText="Cant. Artículos" />
                            <asp:BoundField DataField="EstadoTexto" HeaderText="Estado" />
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkVerDetalle" runat="server" CssClass="btn btn-sm btn-outline"
                                        CommandName="VerDetalle" CommandArgument='<%# Eval("IdPrestamo") %>'>Consultar detalles</asp:LinkButton>
                                    <asp:LinkButton ID="lnkIrDevolucion" runat="server" CssClass="btn btn-sm btn-primary"
                                        CommandName="Devolver" CommandArgument='<%# Eval("IdPrestamo") %>'>Ir a devolución</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>
</body>
</html>