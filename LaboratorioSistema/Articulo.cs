using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaboratorioSistema
{
    public abstract class Articulo
    {
        // Propiedades generales
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public int CantidadMinima { get; set; }
        public string Estado { get; set; }
        public string Ubicacion { get; set; }
        public DateTime FechaAdquisicion { get; set; }

        // Constructor
        public Articulo(int codigo, string nombre, string descripcion,
                       int cantidad, string estado)
        {
            Codigo = codigo;
            Nombre = nombre;
            Descripcion = descripcion;
            Cantidad = cantidad;
            Estado = estado;

            CantidadMinima = 0;
            Ubicacion = "";
            FechaAdquisicion = DateTime.Now;
        }

        public bool EsStockBajo()
        {
            return Cantidad <= CantidadMinima;
        }

        public bool EstaDisponible()
        {
            return string.IsNullOrWhiteSpace(Estado)
                || Estado.Equals("Disponible", StringComparison.OrdinalIgnoreCase);
        }

        public void EstablecerExistencias(int cantidad)
        {
            if (cantidad < 0)
            {
                throw new ArgumentOutOfRangeException("cantidad", "La cantidad no puede ser negativa.");
            }

            Cantidad = cantidad;
        }

        public void AplicarVariacionExistencias(int variacion)
        {
            int nuevaCantidad = Cantidad + variacion;
            if (nuevaCantidad < 0)
            {
                throw new InvalidOperationException("La variación dejaría las existencias en negativo.");
            }

            Cantidad = nuevaCantidad;
        }
    }
}
