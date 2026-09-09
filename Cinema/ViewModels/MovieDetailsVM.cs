namespace AbsoluteCinema.ViewModels
{
    public class MovieDetailsVM
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DateTime DateTime { get; set; }
        public string? MainImg { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public int CinemaId { get; set; }
        public string CinemaName { get; set; } = string.Empty;

        public List<ActorSimpleVM> Actors { get; set; } = new List<ActorSimpleVM>();

        public List<string> SubImages { get; set; } = new List<string>();
    }
}