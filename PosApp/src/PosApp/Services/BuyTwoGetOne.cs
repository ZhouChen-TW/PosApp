using System.Linq;
using NHibernate.Util;
using PosApp.Domain;
using PosApp.Repositories;
using PosApp.Services.Impl;

namespace PosApp.Services
{
    public class BuyTwoGetOne:ICalculateReceipt
    {
        readonly IPromotionRepository m_promotionRepository;

        public BuyTwoGetOne(IPromotionRepository promotionRepository)
        {
            m_promotionRepository = promotionRepository;
        }

        public Receipt GetPromotedReceipt(Receipt receipt)
        {
            string[] boughtBarcodes = receipt.ReceiptItems.Select(r => r.Product.Barcode).ToArray();
            string[] promotionsBarcodes =
                m_promotionRepository.GetByBarcode(boughtBarcodes)
                    .Where(p => p.Type.Equals("BUY_TWO_GET_ONE"))
                    .Select(b => b.Barcode)
                    .ToArray();

            receipt.ReceiptItems.Where(
                    item => promotionsBarcodes.Contains(item.Product.Barcode)).ForEach(r => r.Promoted = r.Amount / 3 * r.Product.Price);

            decimal additionalPromoted = receipt.ReceiptItems.Sum(p => p.Promoted);
            receipt.Promoted += additionalPromoted;
            receipt.Total = receipt.Total - additionalPromoted;
            return receipt;
        }
    }
}