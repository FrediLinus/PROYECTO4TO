using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaboratorioSistema
{
    public class InstrumentoMedicion : Articulo
    {
        // Propiedades propias del instrumento
        public string TipoInstrumento { get; set; }
        public string RangoMedicion { get; set; }
        public string UnidadMedicion { get; set; }

        // Constructor
        public InstrumentoMedicion(int codigo, string nombre, string descripcion, int cantidad,
                                  string estado, string tipoInstrumento, string rangoMedicion,
                                  string unidadMedicion)
            : base(codigo, nombre, descripcion, cantidad, estado)
        {
            TipoInstrumento = tipoInstrumento;
            RangoMedicion = rangoMedicion;
            UnidadMedicion = unidadMedicion;
        }
    }
}