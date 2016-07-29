using System;
using System.Collections.Generic;
using System.Linq;
using PosApp.Domain;
using PosApp.Repositories;
using PosApp.Services.Impl;

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

        public Receipt BuildAllPromotions(Receipt receipt)
        {
            return m_promotionRepository.GetAllTypes()
                .Select(CreatePromotion)
                .Aggregate(receipt, (r, promotion) => promotion.GetPromotedReceipt(r));
        }

        ICalculateReceipt CreatePromotion(string type)
        {
            if (type.Equals("BUY_TWO_GET_ONE"))
            {
                return new BuyTwoGetOne(m_promotionRepository);
            }
            if (type.Equals("BUY_HUNDRED_GET_HALF"))
            {
                return new BuyHundredCutHalf(m_promotionRepository);
            }

            throw new NotSupportedException($"Not supported type: {type}");
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