using System.Collections.Generic;
using PosApp.Domain;

namespace PosApp.Repositories
{
    public interface IPromotionRepository
    {
        IList<Promotion> GetByType(string type);
        IList<Promotion> GetByBarcode(string[] barcodes); 
        int CountByType(string type);
        void Save(IList<Promotion> promotions);
        void Delete(IList<Promotion> promotions);
    }
}