<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrmPrincipal.aspx.cs" Inherits="LaboratorioSistema.FrmPrincipal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Formulario Principal</title>
    <link href="styles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <header class="topbar">
            <div class="topbar-brand">
                <h1>Sistema de Gestión de Laboratorio</h1>
                <span>Control de Inventario Informático y Electrónico</span>
            </div>
            <div class="topbar-user">
                <asp:Label ID="lblUsuarioActivo" runat="server" Text="Usuario: Administrador" CssClass="user-badge"></asp:Label>
                <asp:Button ID="btnCerrarSesion" runat="server" Text="Cerrar Sesión" CssClass="btn-logout" PostBackUrl="~/login.aspx" />
            </div>
        </header>

        <!-- Contenedor Principal del Menú -->
        <main class="dashboard-container">
            <div class="dashboard-header">
                <h2>Menú Principal</h2>
                <p>Selecciona un módulo para comenzar a gestionar los recursos del laboratorio</p>
            </div>

            <!-- Cuadrícula de Botones / Módulos -->
            <div class="menu-grid">
                
                <!-- Módulo Inventario -->
                <div class="menu-card">
                    <div class="card-icon">💻</div>
                    <h3>Inventario</h3>
                    <p>Computadoras, instrumentos de medición, componentes y herramientas.</p>
                    <asp:Button ID="btnInventario" runat="server" Text="Ingresar a Inventario" CssClass="btn-menu" PostBackUrl="~/FrmInventario.aspx" />
                </div>

                <!-- Módulo Préstamos -->
                <div class="menu-card">
                    <div class="card-icon">📋</div>
                    <h3>Préstamos y Devoluciones</h3>
                    <p>Control de salidas, fechas de entrega y equipos en uso.</p>
                    <asp:Button ID="btnPrestamos" runat="server" Text="Ingresar a Préstamos" CssClass="btn-menu" PostBackUrl="~/FrmPrestamos.aspx" />
                </div>

                <!-- Módulo Mantenimiento -->
                <div class="menu-card">
                    <div class="card-icon">🛠️</div>
                    <h3>Mantenimiento</h3>
                    <p>Registro de mantenimiento preventivo, correctivo y equipos dañados.</p>
                    <asp:Button ID="btnMantenimiento" runat="server" Text="Ingresar a Mantenimiento" CssClass="btn-menu" PostBackUrl="~/FrmMantenimiento.aspx" />
                </div>

                <!-- Módulo Movimientos -->
                <div class="menu-card">
                    <div class="card-icon">🔄</div>
                    <h3>Historial y Alertas</h3>
                    <p>Trazabilidad de movimientos, alertas de vencimiento y alta demanda.</p>
                    <asp:Button ID="btnMovimientos" runat="server" Text="Ingresar a Movimientos" CssClass="btn-menu" PostBackUrl="~/FrmMovimientos.aspx" />
                </div>

                <!-- Módulo Responsables -->
                <div class="menu-card">
                    <div class="card-icon">👥</div>
                    <h3>Responsables</h3>
                    <p>Administración de estudiantes, docentes y técnicos autorizados.</p>
                    <asp:Button ID="btnResponsables" runat="server" Text="Ingresar a Responsables" CssClass="btn-menu" PostBackUrl="~/FrmResponsables.aspx" />
                </div>

                <!-- Módulo Reportes -->
                <div class="menu-card">
                    <div class="card-icon">📊</div>
                    <h3>Reportes y Exportación</h3>
                    <p>Estadísticas de disponibilidad, uso de equipos y descarga CSV/TXT.</p>
                    <asp:Button ID="btnReportes" runat="server" Text="Ingresar a Reportes" CssClass="btn-menu" PostBackUrl="~/FrmReportes.aspx" />
                </div>

            </div>
        </main>

    </form>
</body>
</html>
    </form>
</body>
</html>
