using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaboratorioSistema
{
    public class Equipo : Articulo
    {
        // Propiedades propias de Equipo
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string NumeroSerie { get; set; }

        // Constructor
        public Equipo(int codigo, string nombre, string descripcion, int cantidad,
                      string estado, string marca, string modelo, string numeroSerie):base(codigo, nombre, descripcion, cantidad, estado)
        {
            Marca = marca;
            Modelo = modelo;
            NumeroSerie = numeroSerie;
        }
    }
}