using System;
using System.Collections.Generic;
using System.Text;
using WebApiTalleres.Models;

namespace UTNApiTalleres.Data.Repositorio.Interfaz
{
    public interface ITurnoDao
    {
        IEnumerable<TimeSpan> GetDisponibleSlots(string date);

        System.Threading.Tasks.Task<IEnumerable<WebApiTalleres.Models.Turno>> GetTurnos();

        void AgregarTurno(WebApiTalleres.Models.Turno turno);

        void DeleteTurno(int id);

        int CancelarTurno(int id);

        int ModificarTurno(Turno turno);
    }
}
