namespace AbsoluteCinema.ViewModels
{
    public class CategoryDetailsVM
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<MovieVM> Movies { get; set; } = new List<MovieVM>();
    }
}
