using System.Linq;
using PosApp.Domain;
using PosApp.Repositories;
using PosApp.Services.Impl;

namespace PosApp.Services
{
    public class BuyHundredCutHalf:ICalculateReceipt
    {
        readonly IPromotionRepository m_promotionRepository;

        public BuyHundredCutHalf(IPromotionRepository promotionRepository)
        {
            m_promotionRepository = promotionRepository;
        }

        public Receipt GetPromotedReceipt(Receipt receipt)
        {
            string[] boughtBarcodes = receipt.ReceiptItems.Select(r => r.Product.Barcode).ToArray();
            string[] promotionsBarcodes =
                m_promotionRepository.GetByBarcode(boughtBarcodes)
                    .Where(p => p.Type.Equals("BUY_HUNDRED_GET_HALF"))
                    .Select(b => b.Barcode)
                    .ToArray();
            var total =
                receipt.ReceiptItems
                .Where(item => promotionsBarcodes.Contains(item.Product.Barcode))
                    .Sum(r => r.Total);
            var discount = receipt.ReceiptItems
                .Where(item => promotionsBarcodes.Contains(item.Product.Barcode))
                .Sum(r => r.Promoted);
            var discountTotal = total - discount;

            if (discountTotal >= 100)
            {
                receipt.Total = receipt.Total - (int)(discountTotal / 100) * 50;
                receipt.Promoted = receipt.Promoted + (int)(discountTotal / 100) * 50;
            }
            return receipt;
        }
    }
}