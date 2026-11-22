using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace LaboratorioSistema
{
    public static class ResponsablesArchivoManager
    {
        private static readonly string RutaArchivo = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "App_Data", "Responsables.json");

        public static List<Responsable> Cargar()
        {
            if (!File.Exists(RutaArchivo))
            {
                return new List<Responsable>();
            }

            string json = File.ReadAllText(RutaArchivo);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Responsable>();
            }

            return JsonConvert.DeserializeObject<List<Responsable>>(
                json,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                }) ?? new List<Responsable>();
        }

        public static void Guardar(List<Responsable> responsables)
        {
            string carpeta = Path.GetDirectoryName(RutaArchivo);
            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            string json = JsonConvert.SerializeObject(
                responsables,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

            File.WriteAllText(RutaArchivo, json);
        }
    }
}
