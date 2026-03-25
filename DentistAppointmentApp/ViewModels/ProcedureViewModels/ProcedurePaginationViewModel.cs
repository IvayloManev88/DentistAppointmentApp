namespace DentistApp.ViewModels.ProcedureViewModels
{
    using static DentistApp.GCommon.GlobalCommon;
    public class ProcedurePaginationViewModel
    {
        public IEnumerable<ProcedureViewViewModel> Procedures { get; set; } 
            = new List<ProcedureViewViewModel>();

        public string? SearchQuery { get; set; }

        public int CurrentPage { get; set; }
        public int ProceduresPerPage { get; set; } 
            = ItemsPerPage;

        public int TotalItemsCount {  get; set; }

        public int TotalPagesCount 
            => (int)Math.Ceiling((double)TotalItemsCount / ProceduresPerPage);

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPagesCount;

    }
}
