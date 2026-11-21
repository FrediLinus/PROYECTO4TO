using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaboratorioSistema
{
    public class Herramienta : Articulo
    {
        // Propiedades propias de Herramienta
        public string TipoHerramienta { get; set; }
        public string Material { get; set; }

        // Constructor
        public Herramienta(int codigo, string nombre, string descripcion, int cantidad,
                           string estado, string tipoHerramienta, string material)
            : base(codigo, nombre, descripcion, cantidad, estado)
        {
            TipoHerramienta = tipoHerramienta;
            Material = material;
        }
    }
}