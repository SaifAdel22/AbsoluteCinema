using AbsoluteCinema.ViewModels;

namespace AbsoluteCinema.Services
{
    public interface IActorService
    {
        List<ActorDetailsVM> GetPagedActors(string? query, int pageNumber, int pageSize, out int totalItems);
        ActorDetailsVM? GetActorDetails(int id);
        Task CreateActorAsync(ActorVM model);
        Task UpdateActorAsync(int id, ActorVM model);
        Task DeleteActorAsync(int id);
    }
}