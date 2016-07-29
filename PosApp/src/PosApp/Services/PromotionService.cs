using System;
using System.Collections.Generic;
using System.Linq;
using PosApp.Domain;
using PosApp.Repositories;

namespace PosApp.Services
{
    public class PromotionService
    {
        readonly IPromotionRepository m_promotionRepository;
        readonly IProductRepository m_productRepository;

        public PromotionService(IPromotionRepository promotionRepository, IProductRepository productRepository)
        {
            m_promotionRepository = promotionRepository;
            m_productRepository = productRepository;
        }

        public IList<PromotionItem> BuildPromotionItems(IList<ReceiptItem> receiptItems)
        {
            IList<Promotion> promotions = m_promotionRepository.GetByType("BUY_TWO_GET_ONE");
            return receiptItems.Where(
                    item => promotions.Select(p => p.Barcode).ToArray().Contains(item.Product.Barcode))
                    .Select(item => new PromotionItem(item.Product, item.Amount /3))
                    .ToArray();
        }
    
        public void CreatePromotionsForType(string type, string[] barcodes)
        {
            Validate(barcodes);

            IList<Promotion> addPromotions = m_promotionRepository
                .GetByBarcode(barcodes)
                .Where(p => !p.Type.Equals(type))
                .Select(b => new Promotion {Barcode = b.Barcode, Type = type})
                .ToList();
            m_promotionRepository.Save(addPromotions);
        }

        public IList<Promotion> GetAllPromotionsForType(string type)
        {
            return m_promotionRepository.GetByType(type);
        }

        public void DeletePromotionsForType(string type, string[] barcodes)
        {
            IList<Promotion> deletePromotions = m_promotionRepository
                .GetByBarcode(barcodes)
                .Where(p => p.Type.Equals(type)).ToList();
            m_promotionRepository.Delete(deletePromotions);
        } 

        void Validate(string[] barcodes)
        {
            if (m_productRepository.CountByBarcodes(barcodes) != barcodes.Length)
            {
                throw new ArgumentException("Product is not existed");
            }
        }
    }
}