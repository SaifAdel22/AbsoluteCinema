using AbsoluteCinema.Repositories.IRepositories;
using AbsoluteCinema.Models;



namespace AbsoluteCinema.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {

        IRepository<Actor> actorRepository { get; }
        IRepository<Cinema> cinemaRepository { get; }
        IRepository<Category> categoryRepository { get; }
        IRepository<Promotion> promotionRepository { get; }
        IRepository<Cart> cartRepository { get; }
        IBulkRepository<Seat> seatRepository { get; }
        IRepository<Movie> movieRepository { get; }
        IBulkRepository<MovieSubImg> subimgRepository { get; }
        IBulkRepository<MovieActor> movieactorRepository { get; }
        IBulkRepository<Order> orderRepository { get; }
        IRepository<Favorite> favoriteRepository { get; }
    }
}
