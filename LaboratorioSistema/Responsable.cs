using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaboratorioSistema
{
    public class Responsable : Usuario
    {
        // Propiedades propias
        public string Cargo { get; set; }
        public string AreaAsignada { get; set; }

        // Constructor
        public Responsable(int codigo, string nombre, string apellido,
                          string correo, string telefono, string rol,
                          string cargo, string areaAsignada)
            : base(codigo, nombre, apellido, correo, telefono, rol)
        {
            Cargo = cargo;
            AreaAsignada = areaAsignada;
        }
    }
}