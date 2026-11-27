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

        public void Validar()
        {
            if (IdArticulo <= 0 || CantidadPrestada <= 0)
            {
                throw new InvalidOperationException("El detalle del préstamo no es válido.");
            }
        }
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

        public int ObtenerCantidadTotal()
        {
            return (Detalles ?? new List<DetallePrestamo>()).Sum(d => d.CantidadPrestada);
        }

        public void ValidarDetalles()
        {
            if (Detalles == null || Detalles.Count == 0)
            {
                throw new InvalidOperationException("El préstamo debe contener al menos un detalle.");
            }

            foreach (DetallePrestamo detalle in Detalles)
            {
                detalle.Validar();
            }
        }
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

        public void Iniciar(DateTime fechaInicio, string tecnico)
        {
            if (EstadoMantenimiento != EstadoMantenimiento.Pendiente)
            {
                throw new InvalidOperationException("El mantenimiento no está pendiente.");
            }

            FechaInicio = fechaInicio;
            if (!string.IsNullOrWhiteSpace(tecnico))
            {
                TecnicoResponsable = tecnico.Trim();
            }
            EstadoMantenimiento = EstadoMantenimiento.EnProceso;
        }

        public void Finalizar(DateTime fechaFinalizacion, string diagnostico,
                              string observaciones)
        {
            if (EstadoMantenimiento != EstadoMantenimiento.EnProceso)
            {
                throw new InvalidOperationException("El mantenimiento no está en proceso.");
            }

            FechaFinalizacion = fechaFinalizacion;
            DiagnosticoSolucion = diagnostico ?? string.Empty;
            Observaciones = observaciones ?? string.Empty;
            EstadoMantenimiento = EstadoMantenimiento.Finalizado;
        }

        public void Cancelar(string observaciones)
        {
            if (EstadoMantenimiento == EstadoMantenimiento.Finalizado)
            {
                throw new InvalidOperationException("Un mantenimiento finalizado no se puede cancelar.");
            }

            EstadoMantenimiento = EstadoMantenimiento.Cancelado;
            Observaciones = observaciones ?? string.Empty;
        }
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

        public bool EsTrazabilidadAutomatica()
        {
            return TipoMovimiento == TipoMovimiento.Prestamo
                || TipoMovimiento == TipoMovimiento.Devolucion
                || TipoMovimiento == TipoMovimiento.InicioMantenimiento
                || TipoMovimiento == TipoMovimiento.FinalizacionMantenimiento;
        }
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

        public List<Articulo> ObtenerArticulos()
        {
            return ArchivoManager.CargarArticulos();
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

        public Prestamo RegistrarPrestamo(int idPrestamo, int idResponsable, DateTime fechaPrestamo, DateTime fechaPrevista, string observaciones, List<Tuple<int, int>> items)
        {
            if (idPrestamo <= 0) throw new InvalidOperationException("Ingrese un ID de préstamo válido.");
            if (BuscarResponsablePorId(idResponsable) == null)
                throw new InvalidOperationException("El responsable no existe.");

            if (fechaPrevista.Date < fechaPrestamo.Date)
                throw new InvalidOperationException("La fecha prevista de devolución no puede ser anterior a la fecha del préstamo.");

            if (items == null || items.Count == 0)
                throw new InvalidOperationException("El préstamo debe contener al menos un artículo.");

            var articulos = ArchivoManager.CargarArticulos();
            var detalles = new List<DetallePrestamo>();
            foreach (var item in items)
            {
                var articulo = articulos.FirstOrDefault(a => a.Codigo == item.Item1);
                if (articulo == null || item.Item2 <= 0 || articulo.Cantidad < item.Item2)
                    throw new InvalidOperationException("No hay existencias suficientes para uno de los artículos.");
                if (!articulo.EstaDisponible())
                    throw new InvalidOperationException("Uno de los artículos no está disponible.");
                articulo.AplicarVariacionExistencias(-item.Item2);
                detalles.Add(new DetallePrestamo { IdArticulo = item.Item1, CantidadPrestada = item.Item2 });
            }

            var prestamos = ObtenerPrestamos();
            if (prestamos.Any(p => p.IdPrestamo == idPrestamo))
                throw new InvalidOperationException("No se permiten dos préstamos con el mismo ID.");

            var prestamo = new Prestamo
            {
                IdPrestamo = idPrestamo,
                IdResponsable = idResponsable,
                FechaPrestamo = fechaPrestamo,
                FechaPrevistaDevolucion = fechaPrevista,
                EstadoPrestamo = EstadoPrestamo.Activo,
                Observaciones = observaciones ?? string.Empty,
                Detalles = detalles
            };
            prestamo.ValidarDetalles();
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
                if (articulo != null) articulo.AplicarVariacionExistencias(detalle.CantidadPrestada);
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

        public Mantenimiento RegistrarMantenimiento(int idMantenimiento, int idArticulo, TipoMantenimiento tipo, int cantidad, DateTime fechaProgramada, string tecnico, string descripcion, string observaciones)
        {
            if (idMantenimiento <= 0) throw new InvalidOperationException("Ingrese un ID de mantenimiento válido.");
            if (cantidad <= 0) throw new InvalidOperationException("La cantidad debe ser mayor que cero.");
            var articulo = ArchivoManager.CargarArticulos().FirstOrDefault(a => a.Codigo == idArticulo);
            if (articulo == null) throw new InvalidOperationException("El artículo no existe.");
            if (articulo.Cantidad < cantidad || !articulo.EstaDisponible()) throw new InvalidOperationException("No hay existencias disponibles suficientes.");

            var lista = ObtenerMantenimientos();
            if (lista.Any(m => m.IdMantenimiento == idMantenimiento))
                throw new InvalidOperationException("No se permiten dos mantenimientos con el mismo ID.");

            var mantenimiento = new Mantenimiento
            {
                IdMantenimiento = idMantenimiento,
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
            mantenimiento.Iniciar(fechaInicio, tecnico);
            Guardar(_rutaMantenimientos, lista);
            RegistrarMovimientoInterno(TipoMovimiento.InicioMantenimiento, mantenimiento.Cantidad, idMantenimiento.ToString(), "Inicio de mantenimiento", new[] { mantenimiento.IdArticulo });
        }

        public void FinalizarMantenimiento(int idMantenimiento, DateTime fechaFin, string diagnostico, string observaciones, EstadoArticulo estadoFinal)
        {
            var lista = ObtenerMantenimientos();
            var mantenimiento = ObtenerMantenimiento(lista, idMantenimiento);
            var articulos = ArchivoManager.CargarArticulos();
            var articulo = articulos.FirstOrDefault(a => a.Codigo == mantenimiento.IdArticulo);
            if (articulo == null) throw new InvalidOperationException("El artículo no existe.");
            articulo.Estado = EstadoTexto(estadoFinal);
            mantenimiento.Finalizar(fechaFin, diagnostico, observaciones);
            Guardar(_rutaMantenimientos, lista);
            ArchivoManager.GuardarArticulos(articulos);
            RegistrarMovimientoInterno(TipoMovimiento.FinalizacionMantenimiento, mantenimiento.Cantidad, idMantenimiento.ToString(), "Finalización de mantenimiento", new[] { mantenimiento.IdArticulo });
        }

        public void CancelarMantenimiento(int idMantenimiento, string observaciones)
        {
            var lista = ObtenerMantenimientos();
            var mantenimiento = ObtenerMantenimiento(lista, idMantenimiento);
            mantenimiento.Cancelar(observaciones);
            Guardar(_rutaMantenimientos, lista);
        }

        public List<Movimiento> ObtenerMovimientos()
        {
            return Leer<Movimiento>(_rutaMovimientos);
        }

        public Movimiento RegistrarMovimientoManual(int idMovimiento, int idArticulo, TipoMovimiento tipo, int cantidad, string referencia, string descripcion)
        {
            if (idMovimiento <= 0) throw new InvalidOperationException("Ingrese un ID de movimiento válido.");
            if (cantidad <= 0) throw new InvalidOperationException("La cantidad debe ser mayor que cero.");
            if (ObtenerMovimientos().Any(m => m.IdMovimiento == idMovimiento))
                throw new InvalidOperationException("No se permiten dos movimientos con el mismo ID.");

            var articulos = ArchivoManager.CargarArticulos();
            var articulo = articulos.FirstOrDefault(a => a.Codigo == idArticulo);
            if (articulo == null) throw new InvalidOperationException("El artículo no existe.");
            if (tipo == TipoMovimiento.Salida || tipo == TipoMovimiento.Baja)
            {
                if (articulo.Cantidad < cantidad) throw new InvalidOperationException("No hay existencias suficientes.");
                articulo.AplicarVariacionExistencias(-cantidad);
            }
            else if (tipo == TipoMovimiento.Entrada)
            {
                articulo.AplicarVariacionExistencias(cantidad);
            }
            ArchivoManager.GuardarArticulos(articulos);
            var movimiento = new Movimiento
            {
                IdMovimiento = idMovimiento,
                FechaHora = DateTime.Now,
                IdArticulo = idArticulo,
                TipoMovimiento = tipo,
                Cantidad = cantidad,
                ReferenciaId = referencia ?? string.Empty,
                Descripcion = descripcion ?? string.Empty
            };
            var movimientos = ObtenerMovimientos();
            movimientos.Add(movimiento);
            Guardar(_rutaMovimientos, movimientos);
            return movimiento;
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
