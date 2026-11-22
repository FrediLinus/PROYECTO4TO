using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaboratorioSistema
{
    public class Usuario
    {
        // Propiedades
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Rol { get; set; }

        // Constructor
        public Usuario(int codigo, string nombre, string apellido,
                      string correo, string telefono, string rol)
        {
            Codigo = codigo;
            Nombre = nombre;
            Apellido = apellido;
            Correo = correo;
            Telefono = telefono;
            Rol = rol;
        }
    }
}