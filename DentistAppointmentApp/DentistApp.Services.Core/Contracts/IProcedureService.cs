using DentistApp.ViewModels.AppointmentViewModels;
using DentistApp.ViewModels.ProcedureViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DentistApp.Services.Core.Contracts
{
    public interface IProcedureService
    {
        Task<IEnumerable<ProcedureViewViewModel>> GetAllProceduresViewModelsAsync(string userId);

        Task<ProcedureCreateViewModel> GetCreateViewModelAsync();

        Task CreateProcedureAsync(ProcedureCreateViewModel procedureToCreate, DateTime procedureDate);
    }
}
