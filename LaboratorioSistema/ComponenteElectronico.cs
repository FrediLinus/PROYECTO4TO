using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaboratorioSistema
{
    public class ComponenteElectronico : Articulo
    {
        // Propiedades propias del componente
        public string TipoComponente { get; set; }
        public string Valor { get; set; }
        public string Unidad { get; set; }

        // Constructor
        public ComponenteElectronico(int codigo, string nombre, string descripcion, int cantidad,
                                    string estado, string tipoComponente, string valor,
                                    string unidad)
            : base(codigo, nombre, descripcion, cantidad, estado)
        {
            TipoComponente = tipoComponente;
            Valor = valor;
            Unidad = unidad;
        }
    }
}