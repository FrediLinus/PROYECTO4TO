using System;
using System.Collections.Generic;
using System.Linq;

namespace LaboratorioSistema
{
    public class ResponsablesLogica
    {
        private readonly List<Responsable> responsables;

        public ResponsablesLogica()
        {
            responsables = ResponsablesArchivoManager.Cargar();
        }

        public List<Responsable> ObtenerResponsables()
        {
            return responsables.ToList();
        }

        public Responsable Buscar(int codigo)
        {
            return responsables.FirstOrDefault(r => r.Codigo == codigo);
        }

        public bool Registrar(Responsable responsable, out string mensaje)
        {
            if (!Validar(responsable, out mensaje))
            {
                return false;
            }

            if (Buscar(responsable.Codigo) != null)
            {
                mensaje = "Ya existe un responsable con ese código.";
                return false;
            }

            responsables.Add(responsable);
            ResponsablesArchivoManager.Guardar(responsables);
            mensaje = "Responsable registrado correctamente.";
            return true;
        }

        public bool Actualizar(Responsable responsable, out string mensaje)
        {
            if (!Validar(responsable, out mensaje))
            {
                return false;
            }

            int indice = responsables.FindIndex(r => r.Codigo == responsable.Codigo);
            if (indice < 0)
            {
                mensaje = "El responsable no existe.";
                return false;
            }

            responsables[indice] = responsable;
            ResponsablesArchivoManager.Guardar(responsables);
            mensaje = "Responsable actualizado correctamente.";
            return true;
        }

        public bool Eliminar(int codigo, out string mensaje)
        {
            Responsable responsable = Buscar(codigo);
            if (responsable == null)
            {
                mensaje = "El responsable no existe.";
                return false;
            }

            responsables.Remove(responsable);
            ResponsablesArchivoManager.Guardar(responsables);
            mensaje = "Responsable eliminado correctamente.";
            return true;
        }

        private bool Validar(Responsable responsable, out string mensaje)
        {
            if (responsable == null || responsable.Codigo <= 0)
            {
                mensaje = "Ingrese un código válido.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(responsable.Nombre)
                || string.IsNullOrWhiteSpace(responsable.Apellido))
            {
                mensaje = "El nombre y el apellido son obligatorios.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(responsable.Correo))
            {
                mensaje = "El correo es obligatorio.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(responsable.Rol)
                || string.IsNullOrWhiteSpace(responsable.Cargo)
                || string.IsNullOrWhiteSpace(responsable.AreaAsignada))
            {
                mensaje = "El rol, el cargo y el área asignada son obligatorios.";
                return false;
            }

            mensaje = "";
            return true;
        }
    }
}
