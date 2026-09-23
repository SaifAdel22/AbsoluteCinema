namespace AbsoluteCinema.ViewModels
{
    public class AdminOrderVM
    {
        public int Id { get; set; }
        public string MovieTitle { get; set; }
        public string MoviePosterUrl { get; set; }         
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
        public List<string> SeatNumbers { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime MovieDateTime { get; set; }        
    }
}
