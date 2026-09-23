using AbsoluteCinema.Models;
using System.Collections.Generic;

namespace AbsoluteCinema.Services
{
    public interface IPromotionService
    {
         IEnumerable<Promotion> GetAllPromotions();
        Promotion? GetPromotionById(int id);
        void CreatePromotion(Promotion promotion);
        void UpdatePromotion(Promotion promotion);
        void DeletePromotion(int id);
    }
}