namespace AbsoluteCinema.ViewModels
{
    public class CinemaDetailsVM
    {
        public int CinemaId { get; set; }
        public string CinemaName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? Logo { get; set; }

        // قائمة الأفلام المعروضة في السينما دي
        public List<MovieVM> Movies { get; set; } = new List<MovieVM>();
    }
}