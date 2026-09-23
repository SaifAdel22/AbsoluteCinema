using AbsoluteCinema.Models;
using AbsoluteCinema.Data;
using AbsoluteCinema.Repositories.IRepositories;

namespace AbsoluteCinema.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(IRepository<Movie> movieRepository,
         IRepository<Cinema> cinemaRepository,
         IBulkRepository<MovieSubImg> subimgRepository,
         IRepository<Category> categoryRepository,
         IRepository<Actor> actorRepository,
         IBulkRepository<MovieActor> movieactorRepository,
         IRepository<Promotion> promotionRepository,
         IRepository<Cart> cartRepository,
         IRepository<Favorite> favoriteRepository,
          IBulkRepository<Seat> seatRepository,
           IBulkRepository<Order> orderRepository,
         ApplicationDbContext context)
        {
            this.cinemaRepository = cinemaRepository;
            this.actorRepository = actorRepository;
            this.categoryRepository = categoryRepository;
            this.movieactorRepository = movieactorRepository;
            _context = context;
            this.movieRepository = movieRepository;
            this.subimgRepository = subimgRepository;
            this.promotionRepository = promotionRepository;
            this.cartRepository = cartRepository;
            this.favoriteRepository = favoriteRepository;
            this.seatRepository = seatRepository;
            this.orderRepository = orderRepository;
        }

        public IRepository<Cinema> cinemaRepository { get; }

        public IRepository<Category> categoryRepository { get; }


        public IRepository<Movie> movieRepository { get; }

        public IRepository<Favorite> favoriteRepository { get; }
        public IBulkRepository<Seat> seatRepository { get; }


        public IBulkRepository<Order> orderRepository { get; }
        public IBulkRepository<MovieActor> movieactorRepository { get; }

        public IBulkRepository<MovieSubImg> subimgRepository { get; }
        public IRepository<Actor> actorRepository { get; }
        public IRepository<Promotion> promotionRepository { get; }
        public IRepository<Cart> cartRepository { get; }

        public void Dispose()
        {
             _context.Dispose();
        }
    }
}
