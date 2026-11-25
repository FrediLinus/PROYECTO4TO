using System;
using System.Collections.Generic;
using System.Linq;

namespace LaboratorioSistema
{
    public class InventarioLogica
    {
        private List<Articulo> articulos;

        // CONSTRUCTOR
        public InventarioLogica()
        {
            articulos = ArchivoManager.CargarArticulos();
        }

        // REGISTRAR ARTÍCULO
        public bool RegistrarArticulo(Articulo articulo, out string mensaje)
        {
            if (articulo == null)
            {
                mensaje = "El artículo no puede ser nulo.";
                return false;
            }

            if (articulo.Codigo <= 0)
            {
                mensaje = "El código debe ser mayor que cero.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(articulo.Nombre))
            {
                mensaje = "El nombre es obligatorio.";
                return false;
            }

            if (articulo.Cantidad < 0 || articulo.CantidadMinima < 0)
            {
                mensaje = "Las cantidades no pueden ser negativas.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(articulo.Ubicacion))
            {
                mensaje = "La ubicación es obligatoria.";
                return false;
            }

            if (articulo.FechaAdquisicion > DateTime.Now)
            {
                mensaje = "La fecha de adquisición no puede ser futura.";
                return false;
            }

            if (BuscarArticulo(articulo.Codigo) != null)
            {
                mensaje = "Ya existe un artículo con ese código.";
                return false;
            }

            articulos.Add(articulo);
            ArchivoManager.GuardarArticulos(articulos);

            mensaje = "Artículo registrado correctamente.";
            return true;
        }

        // OBTENER TODOS LOS ARTÍCULOS
        public List<Articulo> ObtenerArticulos()
        {
            return articulos.ToList();
        }

        // BUSCAR POR CÓDIGO
        public Articulo BuscarArticulo(int codigo)
        {
            return articulos.FirstOrDefault(a => a.Codigo == codigo);
        }

        // ACTUALIZAR ARTÍCULO
        public bool ActualizarArticulo(Articulo articulo, out string mensaje)
        {
            if (articulo == null)
            {
                mensaje = "El artículo no puede ser nulo.";
                return false;
            }

            if (articulo.Cantidad < 0 || articulo.CantidadMinima < 0)
            {
                mensaje = "Las cantidades no pueden ser negativas.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(articulo.Nombre))
            {
                mensaje = "El nombre es obligatorio.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(articulo.Ubicacion))
            {
                mensaje = "La ubicación es obligatoria.";
                return false;
            }

            if (articulo.FechaAdquisicion > DateTime.Now)
            {
                mensaje = "La fecha de adquisición no puede ser futura.";
                return false;
            }

            int indice = articulos.FindIndex(
                a => a.Codigo == articulo.Codigo);

            if (indice == -1)
            {
                mensaje = "El artículo no existe.";
                return false;
            }

            articulos[indice] = articulo;
            ArchivoManager.GuardarArticulos(articulos);

            mensaje = "Artículo actualizado correctamente.";
            return true;
        }

        // ELIMINAR ARTÍCULO
        public bool EliminarArticulo(int codigo, out string mensaje)
        {
            Articulo articulo = BuscarArticulo(codigo);

            if (articulo == null)
            {
                mensaje = "El artículo no existe.";
                return false;
            }

            articulos.Remove(articulo);
            ArchivoManager.GuardarArticulos(articulos);

            mensaje = "Artículo eliminado correctamente.";
            return true;
        }

        // CONSULTAR STOCK BAJO
        public List<Articulo> ObtenerStockBajo()
        {
            return articulos.Where(a => a.EsStockBajo()).ToList();
        }

        // MODIFICAR EXISTENCIAS
        public bool ModificarExistencias(
            int codigo, int cantidad, out string mensaje)
        {
            Articulo articulo = BuscarArticulo(codigo);

            if (articulo == null)
            {
                mensaje = "El artículo no existe.";
                return false;
            }

            if (cantidad < 0)
            {
                mensaje = "La cantidad no puede ser negativa.";
                return false;
            }

            articulo.EstablecerExistencias(cantidad);

            ArchivoManager.GuardarArticulos(articulos);

            mensaje = "Existencias actualizadas correctamente.";
            return true;
        }
    }
}