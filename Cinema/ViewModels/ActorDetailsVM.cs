namespace AbsoluteCinema.ViewModels
{
    public class ActorDetailsVM
    {
        public int ActorId { get; set; }
        public string ActorName { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        public int bio { get; set; }
        public List<MovieVM> Movies { get; set; } = new List<MovieVM>();
    }
}