using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;


namespace LaboratorioSistema
{
    public class ArchivoManager
    {
        private static readonly string rutaArchivo =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                         "App_Data", "Inventario.json");

        // GUARDAR ARTÍCULOS
        public static void GuardarArticulos(List<Articulo> articulos)
        {
            string carpeta = Path.GetDirectoryName(rutaArchivo);

            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            string json = JsonConvert.SerializeObject(
                articulos,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

            File.WriteAllText(rutaArchivo, json);
        }

        // CARGAR ARTÍCULOS
        public static List<Articulo> CargarArticulos()
        {
            if (!File.Exists(rutaArchivo))
            {
                return new List<Articulo>();
            }

            string json = File.ReadAllText(rutaArchivo);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Articulo>();
            }

            return JsonConvert.DeserializeObject<List<Articulo>>(
                json,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                }) ?? new List<Articulo>();
        }
    }
}