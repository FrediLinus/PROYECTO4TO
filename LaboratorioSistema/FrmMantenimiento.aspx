<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmMantenimiento.aspx.cs" Inherits="LaboratorioSistema.FrmMantenimiento" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Gestión de Mantenimiento - Laboratorio</title>
    <link href="styles.css?v=20260324" rel="stylesheet" type="text/css" />
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
                    <h2>Gestión de Mantenimiento</h2>
                    <p>Control de mantenimientos preventivos y correctivos de equipos e instrumentos del laboratorio.</p>
                </div>
                </div>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert" role="alert">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>

            <!-- Formulario de registro / seguimiento / finalización de mantenimiento -->
            <div class="card">
                <div class="card-header">
                    <h2 class="card-title">Orden de Mantenimiento</h2>
                </div>
                <div class="card-body">
                    <div class="form-grid">
                        <div class="form-group">
                            <label for="<%= txtIdMantenimiento.ClientID %>">ID del mantenimiento *</label>
                            <asp:TextBox ID="txtIdMantenimiento" runat="server" CssClass="form-control" TextMode="Number" min="1" placeholder="Ingrese el ID del mantenimiento"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="<%= ddlArticuloMant.ClientID %>">Artículo / Equipo *</label>
                            <asp:DropDownList ID="ddlArticuloMant" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlArticuloMant_SelectedIndexChanged"></asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label for="<%= ddlTipoMantenimiento.ClientID %>">Tipo de Mantenimiento *</label>
                            <asp:DropDownList ID="ddlTipoMantenimiento" runat="server" CssClass="form-control">
                                <asp:ListItem Value="Preventivo" Text="Preventivo"></asp:ListItem>
                                <asp:ListItem Value="Correctivo" Text="Correctivo"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label for="<%= txtCantidadMant.ClientID %>">Cantidad de Unidades *</label>
                            <asp:TextBox ID="txtCantidadMant" runat="server" CssClass="form-control" TextMode="Number" min="1" Text="1"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="<%= txtFechaProgramada.ClientID %>">Fecha Programada *</label>
                            <asp:TextBox ID="txtFechaProgramada" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="<%= txtFechaInicio.ClientID %>">Fecha de Inicio</label>
                            <asp:TextBox ID="txtFechaInicio" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="<%= txtFechaFinalizacion.ClientID %>">Fecha de Finalización</label>
                            <asp:TextBox ID="txtFechaFinalizacion" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="<%= txtTecnicoResponsable.ClientID %>">Técnico Responsable *</label>
                            <asp:TextBox ID="txtTecnicoResponsable" runat="server" CssClass="form-control" MaxLength="100" placeholder="Nombre del técnico o encargado"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="<%= ddlEstadoMantenimiento.ClientID %>">Estado del Mantenimiento</label>
                            <asp:DropDownList ID="ddlEstadoMantenimiento" runat="server" CssClass="form-control" Enabled="false">
                                <asp:ListItem Value="Pendiente" Text="Pendiente"></asp:ListItem>
                                <asp:ListItem Value="EnProceso" Text="En proceso"></asp:ListItem>
                                <asp:ListItem Value="Finalizado" Text="Finalizado"></asp:ListItem>
                                <asp:ListItem Value="Cancelado" Text="Cancelado"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label for="<%= ddlResultadoFinal.ClientID %>">Estado Resultante del Artículo (al finalizar)</label>
                            <asp:DropDownList ID="ddlResultadoFinal" runat="server" CssClass="form-control">
                                <asp:ListItem Value="Disponible" Text="Disponible (Operativo)"></asp:ListItem>
                                <asp:ListItem Value="Danado" Text="Dañado (Requiere revisión)"></asp:ListItem>
                                <asp:ListItem Value="FueraDeServicio" Text="Fuera de servicio (Baja)"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="form-group form-group-wide">
                            <label for="<%= txtDescripcionProblema.ClientID %>">Descripción del Problema / Trabajo a Realizar *</label>
                            <asp:TextBox ID="txtDescripcionProblema" runat="server" CssClass="form-control" MaxLength="250" placeholder="Detalle de la falla o rutina preventiva..."></asp:TextBox>
                        </div>

                        <div class="form-group form-group-wide">
                            <label for="<%= txtDiagnosticoSolucion.ClientID %>">Diagnóstico y Solución Aplicada</label>
                            <asp:TextBox ID="txtDiagnosticoSolucion" runat="server" CssClass="form-control" MaxLength="250" placeholder="Se completa durante o al finalizar el mantenimiento..."></asp:TextBox>
                        </div>

                        <div class="form-group form-group-wide">
                            <label for="<%= txtObservacionesMant.ClientID %>">Observaciones</label>
                            <asp:TextBox ID="txtObservacionesMant" runat="server" CssClass="form-control" MaxLength="250"></asp:TextBox>
                        </div>
                    </div>

                    <asp:Panel ID="pnlInfoArticuloMant" runat="server" CssClass="info-box" Visible="false">
                        <strong>Estado actual del artículo:</strong>
                        <asp:Label ID="lblInfoArticuloMant" runat="server"></asp:Label>
                    </asp:Panel>

                    <div class="form-actions">
                        <asp:Button ID="btnRegistrarMantenimiento" runat="server" Text="Registrar mantenimiento" CssClass="btn btn-primary" OnClick="btnRegistrarMantenimiento_Click" />
                        <asp:Button ID="btnIniciarMantenimiento" runat="server" Text="Iniciar mantenimiento" CssClass="btn btn-secondary" OnClick="btnIniciarMantenimiento_Click" />
                        <asp:Button ID="btnFinalizarMantenimiento" runat="server" Text="Finalizar mantenimiento" CssClass="btn btn-success" OnClick="btnFinalizarMantenimiento_Click" />
                        <asp:Button ID="btnCancelarMantenimiento" runat="server" Text="Cancelar" CssClass="btn btn-warning" OnClick="btnCancelarMantenimiento_Click"
                            OnClientClick="return confirm('¿Desea cancelar esta orden de mantenimiento?');" />
                        <asp:Button ID="btnLimpiarMant" runat="server" Text="Limpiar" CssClass="btn btn-outline" OnClick="btnLimpiarMant_Click" />
                    </div>
                </div>
            </div>

            <!-- Tabla de mantenimientos -->
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Mantenimientos Programados y Realizados</h3>
                    <div class="search-bar">
                        <asp:TextBox ID="txtBuscarMant" runat="server" CssClass="form-control" placeholder="Buscar por ID, artículo o técnico..."></asp:TextBox>
                        <asp:DropDownList ID="ddlFiltroTipoMant" runat="server" CssClass="form-control">
                            <asp:ListItem Value="TODOS" Text="Todos los tipos"></asp:ListItem>
                            <asp:ListItem Value="Preventivo" Text="Preventivo"></asp:ListItem>
                            <asp:ListItem Value="Correctivo" Text="Correctivo"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlFiltroEstadoMant" runat="server" CssClass="form-control">
                            <asp:ListItem Value="TODOS" Text="Todos los estados"></asp:ListItem>
                            <asp:ListItem Value="Pendiente" Text="Pendiente"></asp:ListItem>
                            <asp:ListItem Value="EnProceso" Text="En proceso"></asp:ListItem>
                            <asp:ListItem Value="Finalizado" Text="Finalizado"></asp:ListItem>
                            <asp:ListItem Value="Cancelado" Text="Cancelado"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="btnBuscarMant" runat="server" Text="Buscar" CssClass="btn btn-secondary" OnClick="btnBuscarMant_Click" />
                        <asp:Button ID="btnTodosMant" runat="server" Text="Todos" CssClass="btn btn-outline" OnClick="btnTodosMant_Click" />
                    </div>
                </div>
                <div class="card-body table-responsive">
                    <asp:GridView ID="gvMantenimientos" runat="server" CssClass="grid-table" AutoGenerateColumns="False"
                        DataKeyNames="IdMantenimiento" OnRowCommand="gvMantenimientos_RowCommand"
                        EmptyDataText="No hay registros de mantenimiento.">
                        <Columns>
                            <asp:BoundField DataField="IdMantenimiento" HeaderText="ID" />
                            <asp:BoundField DataField="NombreArticulo" HeaderText="Artículo" />
                            <asp:BoundField DataField="TipoTexto" HeaderText="Tipo" />
                            <asp:BoundField DataField="Cantidad" HeaderText="Cant." />
                            <asp:BoundField DataField="FechaProgramadaTexto" HeaderText="F. Programada" />
                            <asp:BoundField DataField="TecnicoResponsable" HeaderText="Técnico" />
                            <asp:BoundField DataField="EstadoTexto" HeaderText="Estado" />
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkSeleccionarMant" runat="server" CssClass="btn btn-sm btn-outline"
                                        CommandName="Seleccionar" CommandArgument='<%# Eval("IdMantenimiento") %>'>Seleccionar</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            </div>
        </div>
        <div id="modalArticuloNoEncontrado" class="modal-articulo" role="dialog" aria-modal="true" aria-labelledby="tituloModalArticulo">
            <div class="modal-articulo-contenido">
                <h2 id="tituloModalArticulo">Artículo no encontrado</h2>
                <p id="mensajeModalArticulo">El artículo no aparece en el sistema.</p>
                <button type="button" class="modal-articulo-cerrar" onclick="cerrarModalArticulo()">Aceptar</button>
            </div>
        </div>
        <script type="text/javascript">
            function mostrarModalArticulo() { document.getElementById('modalArticuloNoEncontrado').classList.add('visible'); }
            function cerrarModalArticulo() { document.getElementById('modalArticuloNoEncontrado').classList.remove('visible'); }
        </script>
    </form>
</body>
</html>