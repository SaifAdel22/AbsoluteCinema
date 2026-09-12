using AbsoluteCinema.Repositories.IRepositories;
using AbsoluteCinema.Models;



namespace AbsoluteCinema.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {

        IRepository<Actor> actorRepository { get; }
        IRepository<Cinema> cinemaRepository { get; }
        IRepository<Category> categoryRepository { get; }
        IRepository<Movie> movieRepository { get; }
        IBulkRepository<MovieSubImg> subimgRepository { get; }
        IBulkRepository<MovieActor> movieactorRepository { get; }
    }
}
