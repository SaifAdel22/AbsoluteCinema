namespace AbsoluteCinema.ViewModels
{
    public class MovieActorInputVM
    {
        public int ActorId { get; set; }
        public string CharacterName { get; set; }

        public List<MovieActorInputVM> MovieActorsInput { get; set; } = new List<MovieActorInputVM>();
    }

}
