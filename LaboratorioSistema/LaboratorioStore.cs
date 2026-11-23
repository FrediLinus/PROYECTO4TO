using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LaboratorioSistema
{
    public enum EstadoArticulo
    {
        Disponible,
        Danado,
        FueraDeServicio
    }

    public enum TipoMovimiento
    {
        Entrada,
        Salida,
        Prestamo,
        Devolucion,
        InicioMantenimiento,
        FinalizacionMantenimiento,
        Ajuste,
        Baja
    }

    public enum TipoMantenimiento
    {
        Preventivo,
        Correctivo
    }

    public enum EstadoMantenimiento
    {
        Pendiente,
        EnProceso,
        Finalizado,
        Cancelado
    }

    public enum EstadoPrestamo
    {
        Activo,
        ParcialmenteDevuelto,
        Devuelto,
        Vencido,
        Cancelado
    }

    public class ResponsableInfo
    {
        public int IdResponsable { get; set; }
        public string Carnet { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string TipoResponsable { get; set; }
        public string CarreraDepartamento { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public bool Activo { get; set; }
    }

    public class ArticuloConExistencia
    {
        public int IdArticulo { get; set; }
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public EstadoArticulo EstadoArticulo { get; set; }
        public int CantidadDisponible { get; set; }
        public int CantidadEnMantenimiento { get; set; }
        public int CantidadDanada { get; set; }
        public int CantidadFueraServicio { get; set; }
    }

    public class DetallePrestamo
    {
        public int IdArticulo { get; set; }
        public int CantidadPrestada { get; set; }
    }

    public class Prestamo
    {
        public int IdPrestamo { get; set; }
        public int IdResponsable { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaPrevistaDevolucion { get; set; }
        public EstadoPrestamo EstadoPrestamo { get; set; }
        public string Observaciones { get; set; }
        public List<DetallePrestamo> Detalles { get; set; }
    }

    public class Mantenimiento
    {
        public int IdMantenimiento { get; set; }
        public int IdArticulo { get; set; }
        public TipoMantenimiento TipoMantenimiento { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaProgramada { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public string TecnicoResponsable { get; set; }
        public EstadoMantenimiento EstadoMantenimiento { get; set; }
        public string DescripcionProblema { get; set; }
        public string DiagnosticoSolucion { get; set; }
        public string Observaciones { get; set; }
    }

    public class Movimiento
    {
        public int IdMovimiento { get; set; }
        public DateTime FechaHora { get; set; }
        public int IdArticulo { get; set; }
        public TipoMovimiento TipoMovimiento { get; set; }
        public int Cantidad { get; set; }
        public string ReferenciaId { get; set; }
        public string Descripcion { get; set; }
    }

    public class LaboratorioStore
    {
        private readonly string _directorio;
        private readonly string _rutaPrestamos;
        private readonly string _rutaMantenimientos;
        private readonly string _rutaMovimientos;

        public LaboratorioStore()
        {
            _directorio = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
            _rutaPrestamos = Path.Combine(_directorio, "Prestamos.json");
            _rutaMantenimientos = Path.Combine(_directorio, "Mantenimientos.json");
            _rutaMovimientos = Path.Combine(_directorio, "Movimientos.json");
        }

        public List<ResponsableInfo> ObtenerTodosResponsables()
        {
            return ResponsablesArchivoManager.Cargar().Select(r => new ResponsableInfo
            {
                IdResponsable = r.Codigo,
                Carnet = r.Codigo.ToString(),
                Nombres = r.Nombre ?? string.Empty,
                Apellidos = r.Apellido ?? string.Empty,
                TipoResponsable = r.Rol ?? string.Empty,
                CarreraDepartamento = r.AreaAsignada ?? string.Empty,
                Telefono = r.Telefono ?? string.Empty,
                Correo = r.Correo ?? string.Empty,
                Activo = true
            }).ToList();
        }

        public List<ResponsableInfo> ObtenerResponsablesActivos()
        {
            return ObtenerTodosResponsables().Where(r => r.Activo).ToList();
        }

        public ResponsableInfo BuscarResponsablePorId(int id)
        {
            return ObtenerTodosResponsables().FirstOrDefault(r => r.IdResponsable == id);
        }

        public List<ArticuloConExistencia> ObtenerArticulosConExistencia()
        {
            var mantenimientos = Leer<Mantenimiento>(_rutaMantenimientos);
            var articulos = ArchivoManager.CargarArticulos();

            return articulos.Select(a => CrearResumenArticulo(a, mantenimientos)).ToList();
        }

        public ArticuloConExistencia BuscarArticuloConExistencia(int idArticulo)
        {
            return ObtenerArticulosConExistencia().FirstOrDefault(a => a.IdArticulo == idArticulo);
        }

        public List<Prestamo> ObtenerPrestamos()
        {
            return Leer<Prestamo>(_rutaPrestamos);
        }

        public Prestamo BuscarPrestamoPorId(int idPrestamo)
        {
            return ObtenerPrestamos().FirstOrDefault(p => p.IdPrestamo == idPrestamo);
        }

        public Prestamo RegistrarPrestamo(int idResponsable, DateTime fechaPrestamo, DateTime fechaPrevista, string observaciones, List<Tuple<int, int>> items)
        {
            if (BuscarResponsablePorId(idResponsable) == null)
                throw new InvalidOperationException("El responsable no existe.");

            var articulos = ArchivoManager.CargarArticulos();
            var detalles = new List<DetallePrestamo>();
            foreach (var item in items)
            {
                var articulo = articulos.FirstOrDefault(a => a.Codigo == item.Item1);
                if (articulo == null || item.Item2 <= 0 || articulo.Cantidad < item.Item2)
                    throw new InvalidOperationException("No hay existencias suficientes para uno de los artículos.");
                if (!EsDisponible(articulo.Estado))
                    throw new InvalidOperationException("Uno de los artículos no está disponible.");
                articulo.Cantidad -= item.Item2;
                detalles.Add(new DetallePrestamo { IdArticulo = item.Item1, CantidadPrestada = item.Item2 });
            }

            var prestamos = ObtenerPrestamos();
            var prestamo = new Prestamo
            {
                IdPrestamo = SiguienteId(prestamos.Select(p => p.IdPrestamo)),
                IdResponsable = idResponsable,
                FechaPrestamo = fechaPrestamo,
                FechaPrevistaDevolucion = fechaPrevista,
                EstadoPrestamo = EstadoPrestamo.Activo,
                Observaciones = observaciones ?? string.Empty,
                Detalles = detalles
            };
            prestamos.Add(prestamo);
            ArchivoManager.GuardarArticulos(articulos);
            Guardar(_rutaPrestamos, prestamos);
            RegistrarMovimientoInterno(TipoMovimiento.Prestamo, detalles.Sum(d => d.CantidadPrestada), prestamo.IdPrestamo.ToString(), "Préstamo registrado", detalles.Select(d => d.IdArticulo));
            return prestamo;
        }

        public void CancelarPrestamo(int idPrestamo)
        {
            var prestamos = ObtenerPrestamos();
            var prestamo = prestamos.FirstOrDefault(p => p.IdPrestamo == idPrestamo);
            if (prestamo == null) throw new InvalidOperationException("El préstamo no existe.");
            if (prestamo.EstadoPrestamo == EstadoPrestamo.Cancelado) return;

            var articulos = ArchivoManager.CargarArticulos();
            foreach (var detalle in prestamo.Detalles ?? new List<DetallePrestamo>())
            {
                var articulo = articulos.FirstOrDefault(a => a.Codigo == detalle.IdArticulo);
                if (articulo != null) articulo.Cantidad += detalle.CantidadPrestada;
            }
            prestamo.EstadoPrestamo = EstadoPrestamo.Cancelado;
            ArchivoManager.GuardarArticulos(articulos);
            Guardar(_rutaPrestamos, prestamos);
        }

        public List<Mantenimiento> ObtenerMantenimientos()
        {
            return Leer<Mantenimiento>(_rutaMantenimientos);
        }

        public Mantenimiento BuscarMantenimientoPorId(int idMantenimiento)
        {
            return ObtenerMantenimientos().FirstOrDefault(m => m.IdMantenimiento == idMantenimiento);
        }

        public Mantenimiento RegistrarMantenimiento(int idArticulo, TipoMantenimiento tipo, int cantidad, DateTime fechaProgramada, string tecnico, string descripcion, string observaciones)
        {
            if (cantidad <= 0) throw new InvalidOperationException("La cantidad debe ser mayor que cero.");
            var articulo = ArchivoManager.CargarArticulos().FirstOrDefault(a => a.Codigo == idArticulo);
            if (articulo == null) throw new InvalidOperationException("El artículo no existe.");
            if (articulo.Cantidad < cantidad || !EsDisponible(articulo.Estado)) throw new InvalidOperationException("No hay existencias disponibles suficientes.");

            var lista = ObtenerMantenimientos();
            var mantenimiento = new Mantenimiento
            {
                IdMantenimiento = SiguienteId(lista.Select(m => m.IdMantenimiento)),
                IdArticulo = idArticulo,
                TipoMantenimiento = tipo,
                Cantidad = cantidad,
                FechaProgramada = fechaProgramada,
                TecnicoResponsable = tecnico ?? string.Empty,
                EstadoMantenimiento = EstadoMantenimiento.Pendiente,
                DescripcionProblema = descripcion ?? string.Empty,
                Observaciones = observaciones ?? string.Empty
            };
            lista.Add(mantenimiento);
            Guardar(_rutaMantenimientos, lista);
            return mantenimiento;
        }

        public void IniciarMantenimiento(int idMantenimiento, DateTime fechaInicio, string tecnico)
        {
            var lista = ObtenerMantenimientos();
            var mantenimiento = ObtenerMantenimiento(lista, idMantenimiento);
            if (mantenimiento.EstadoMantenimiento != EstadoMantenimiento.Pendiente) throw new InvalidOperationException("El mantenimiento no está pendiente.");
            mantenimiento.FechaInicio = fechaInicio;
            mantenimiento.TecnicoResponsable = string.IsNullOrWhiteSpace(tecnico) ? mantenimiento.TecnicoResponsable : tecnico;
            mantenimiento.EstadoMantenimiento = EstadoMantenimiento.EnProceso;
            Guardar(_rutaMantenimientos, lista);
            RegistrarMovimientoInterno(TipoMovimiento.InicioMantenimiento, mantenimiento.Cantidad, idMantenimiento.ToString(), "Inicio de mantenimiento", new[] { mantenimiento.IdArticulo });
        }

        public void FinalizarMantenimiento(int idMantenimiento, DateTime fechaFin, string diagnostico, string observaciones, EstadoArticulo estadoFinal)
        {
            var lista = ObtenerMantenimientos();
            var mantenimiento = ObtenerMantenimiento(lista, idMantenimiento);
            if (mantenimiento.EstadoMantenimiento != EstadoMantenimiento.EnProceso) throw new InvalidOperationException("El mantenimiento no está en proceso.");
            var articulos = ArchivoManager.CargarArticulos();
            var articulo = articulos.FirstOrDefault(a => a.Codigo == mantenimiento.IdArticulo);
            if (articulo == null) throw new InvalidOperationException("El artículo no existe.");
            articulo.Estado = EstadoTexto(estadoFinal);
            mantenimiento.FechaFinalizacion = fechaFin;
            mantenimiento.DiagnosticoSolucion = diagnostico ?? string.Empty;
            mantenimiento.Observaciones = observaciones ?? string.Empty;
            mantenimiento.EstadoMantenimiento = EstadoMantenimiento.Finalizado;
            Guardar(_rutaMantenimientos, lista);
            ArchivoManager.GuardarArticulos(articulos);
            RegistrarMovimientoInterno(TipoMovimiento.FinalizacionMantenimiento, mantenimiento.Cantidad, idMantenimiento.ToString(), "Finalización de mantenimiento", new[] { mantenimiento.IdArticulo });
        }

        public void CancelarMantenimiento(int idMantenimiento, string observaciones)
        {
            var lista = ObtenerMantenimientos();
            var mantenimiento = ObtenerMantenimiento(lista, idMantenimiento);
            mantenimiento.EstadoMantenimiento = EstadoMantenimiento.Cancelado;
            mantenimiento.Observaciones = observaciones ?? string.Empty;
            Guardar(_rutaMantenimientos, lista);
        }

        public List<Movimiento> ObtenerMovimientos()
        {
            return Leer<Movimiento>(_rutaMovimientos);
        }

        public Movimiento RegistrarMovimientoManual(int idArticulo, TipoMovimiento tipo, int cantidad, string referencia, string descripcion)
        {
            if (cantidad <= 0) throw new InvalidOperationException("La cantidad debe ser mayor que cero.");
            var articulos = ArchivoManager.CargarArticulos();
            var articulo = articulos.FirstOrDefault(a => a.Codigo == idArticulo);
            if (articulo == null) throw new InvalidOperationException("El artículo no existe.");
            if (tipo == TipoMovimiento.Salida || tipo == TipoMovimiento.Baja)
            {
                if (articulo.Cantidad < cantidad) throw new InvalidOperationException("No hay existencias suficientes.");
                articulo.Cantidad -= cantidad;
            }
            else if (tipo == TipoMovimiento.Entrada)
            {
                articulo.Cantidad += cantidad;
            }
            ArchivoManager.GuardarArticulos(articulos);
            return RegistrarMovimientoInterno(tipo, cantidad, referencia, descripcion, new[] { idArticulo });
        }

        private Movimiento RegistrarMovimientoInterno(TipoMovimiento tipo, int cantidad, string referencia, string descripcion, IEnumerable<int> ids)
        {
            var movimientos = ObtenerMovimientos();
            var movimiento = new Movimiento
            {
                IdMovimiento = SiguienteId(movimientos.Select(m => m.IdMovimiento)),
                FechaHora = DateTime.Now,
                IdArticulo = ids.FirstOrDefault(),
                TipoMovimiento = tipo,
                Cantidad = cantidad,
                ReferenciaId = referencia ?? string.Empty,
                Descripcion = descripcion ?? string.Empty
            };
            movimientos.Add(movimiento);
            Guardar(_rutaMovimientos, movimientos);
            return movimiento;
        }

        private static Mantenimiento ObtenerMantenimiento(List<Mantenimiento> lista, int id)
        {
            var mantenimiento = lista.FirstOrDefault(m => m.IdMantenimiento == id);
            if (mantenimiento == null) throw new InvalidOperationException("El mantenimiento no existe.");
            return mantenimiento;
        }

        private static ArticuloConExistencia CrearResumenArticulo(Articulo articulo, List<Mantenimiento> mantenimientos)
        {
            EstadoArticulo estado;
            if (!Enum.TryParse(articulo.Estado == "Dañado" ? "Danado" : articulo.Estado, true, out estado)) estado = EstadoArticulo.Disponible;
            var enMantenimiento = mantenimientos.Where(m => m.IdArticulo == articulo.Codigo && m.EstadoMantenimiento == EstadoMantenimiento.EnProceso).Sum(m => m.Cantidad);
            return new ArticuloConExistencia
            {
                IdArticulo = articulo.Codigo,
                Codigo = articulo.Codigo,
                Nombre = articulo.Nombre ?? string.Empty,
                Tipo = articulo.GetType().Name,
                Estado = articulo.Estado ?? string.Empty,
                EstadoArticulo = estado,
                CantidadDisponible = estado == EstadoArticulo.Disponible ? Math.Max(0, articulo.Cantidad - enMantenimiento) : 0,
                CantidadEnMantenimiento = enMantenimiento,
                CantidadDanada = estado == EstadoArticulo.Danado ? articulo.Cantidad : 0,
                CantidadFueraServicio = estado == EstadoArticulo.FueraDeServicio ? articulo.Cantidad : 0
            };
        }

        private static bool EsDisponible(string estado)
        {
            return string.IsNullOrWhiteSpace(estado) || estado.Equals("Disponible", StringComparison.OrdinalIgnoreCase);
        }

        private static string EstadoTexto(EstadoArticulo estado)
        {
            if (estado == EstadoArticulo.Danado) return "Dañado";
            if (estado == EstadoArticulo.FueraDeServicio) return "Fuera de servicio";
            return "Disponible";
        }

        private static int SiguienteId(IEnumerable<int> ids)
        {
            return ids.Any() ? ids.Max() + 1 : 1;
        }

        private static List<T> Leer<T>(string ruta)
        {
            if (!File.Exists(ruta)) return new List<T>();
            var json = File.ReadAllText(ruta);
            if (string.IsNullOrWhiteSpace(json)) return new List<T>();
            return JsonConvert.DeserializeObject<List<T>>(json, new JsonSerializerSettings()) ?? new List<T>();
        }

        private void Guardar<T>(string ruta, List<T> datos)
        {
            if (!Directory.Exists(_directorio)) Directory.CreateDirectory(_directorio);
            File.WriteAllText(ruta, JsonConvert.SerializeObject(datos, Formatting.Indented));
        }
    }
}
