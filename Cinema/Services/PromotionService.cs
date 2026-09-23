using Mapster;

namespace AbsoluteCinema.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PromotionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async void CreatePromotion(Promotion promotion)
        {
            await _unitOfWork.promotionRepository.CreateAsync(promotion);
            await _unitOfWork.promotionRepository.CommitAsync();
        }

        public void DeletePromotion(int id)
        {
            var promotion = GetPromotionById(id);

            if (promotion != null)
            {
                _unitOfWork.promotionRepository.Delete(promotion);
                _unitOfWork.promotionRepository.Commit();
            }
        }

        public IEnumerable<Promotion> GetAllPromotions()
        {
            return _unitOfWork.promotionRepository.Get().Include(m => m.Movie).ToList();
        }

        public Promotion? GetPromotionById(int id)
        {
            return _unitOfWork.promotionRepository.Get().Include(m => m.Movie).FirstOrDefault(i => i.Id == id);
        }

        public void UpdatePromotion(Promotion promotion)
        {
            var existingPromotion = GetPromotionById(promotion.Id);

            if (existingPromotion != null)
            {
                existingPromotion.Code = promotion.Code;
                existingPromotion.Discount = promotion.Discount;
                existingPromotion.MaxUsage = promotion.MaxUsage;
                existingPromotion.TimesUsed = promotion.TimesUsed;
                existingPromotion.ValidTo = promotion.ValidTo;
                existingPromotion.MovieId = promotion.MovieId;
                existingPromotion.Status = promotion.Status;

                _unitOfWork.promotionRepository.Update(existingPromotion);
                _unitOfWork.promotionRepository.Commit();


            }
        }
    }
}