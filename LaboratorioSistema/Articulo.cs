using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaboratorioSistema
{
    public class Articulo
    {
            // Atributos
            public int Codigo { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public int Cantidad { get; set; }
            public string Estado { get; set; }

            // Constructor
            public Articulo(int codigo, string nombre, string descripcion, int cantidad, string estado)
            {
                Codigo = codigo;
                Nombre = nombre;
                Descripcion = descripcion;
                Cantidad = cantidad;
                Estado = estado;
            }
    }
}