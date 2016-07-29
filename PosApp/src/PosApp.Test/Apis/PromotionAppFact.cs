using System;
using System.Collections.Generic;
using Autofac;
using PosApp.Domain;
using PosApp.Services;
using PosApp.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace PosApp.Test.Apis
{
    public class PromotionAppFact:FactBase
    {
        public PromotionAppFact(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void should_fail_if_the_barcode_product_not_exist()
        {
            Fixtures.Products.Create(new Product
            {
                Barcode = "barcode",
                Id = Guid.NewGuid(),
                Name = "I do not care",
                Price = 3M
            });

            PromotionService promotionService = CreatePromotionSerivice();

            string type = "BUY_TWO_GET_ONE";
            string[] barcodes = {"barcode-not-exist"};

            Assert.Throws<ArgumentException>(() => promotionService.CreatePromotionsForType(type, barcodes));
        }

        [Fact]
        public void should_fail_if_one_of_barcode_product_not_exist_when_give_two_barcodes()
        {
            Fixtures.Products.Create(new Product
            {
                Barcode = "barcode",
                Id = Guid.NewGuid(),
                Name = "I do not care",
                Price = 3M
            });

            PromotionService promotionService = CreatePromotionSerivice();

            string type = "BUY_TWO_GET_ONE";
            string[] barcodes = { "barcode-not-exist","barcode"};

            Assert.Throws<ArgumentException>(() => promotionService.CreatePromotionsForType(type, barcodes));
        }

        [Fact]
        public void should_success_if_the_barcode_product_exist_and_the_barcode_not_for_this_type()
        {
            Fixtures.Products.Create(new Product
            {
                Barcode = "barcode",
                Id = Guid.NewGuid(),
                Name = "I do not care",
                Price = 3M
            });

            Fixtures.Promotions.Create(new Promotion
            {
                Barcode = "barcode",
                Id = Guid.NewGuid(),
                Type = ""
            });

            PromotionService promotionService = CreatePromotionSerivice();

            string type = "BUY_TWO_GET_ONE";
            string[] barcodes = { "barcode"};

            promotionService.CreatePromotionsForType(type,barcodes);
            IList<Promotion> promotions = promotionService.GetAllPromotionsForType(type);

            Assert.Equal(1,promotions.Count);
            Assert.Equal("barcode",promotions[0].Barcode);
            Assert.Equal("BUY_TWO_GET_ONE",promotions[0].Type);
        }

        [Fact]
        public void should_success_if_the_barcode_product_exist_and_the_barcode_is_for_this_type()
        {
            Fixtures.Products.Create(new Product
            {
                Barcode = "barcode",
                Id = Guid.NewGuid(),
                Name = "I do not care",
                Price = 3M
            });

            Fixtures.Promotions.Create(new Promotion
            {
                Barcode = "barcode",
                Id = Guid.NewGuid(),
                Type = "BUY_TWO_GET_ONE"
            });

            PromotionService promotionService = CreatePromotionSerivice();

            string type = "BUY_TWO_GET_ONE";
            string[] barcodes = { "barcode" };

            promotionService.CreatePromotionsForType(type, barcodes);
            IList<Promotion> promotions = promotionService.GetAllPromotionsForType(type);

            Assert.Equal(1, promotions.Count);
        }

        [Fact]
        public void should_list_all_promotions_for_this_type()
        {
            Fixtures.Promotions.Create(new Promotion
            {
                Barcode = "barcode",
                Id = Guid.NewGuid(),
                Type = "BUY_TWO_GET_ONE"
            });

            PromotionService promotionService = CreatePromotionSerivice();
            string type = "BUY_TWO_GET_ONE";

            IList<Promotion> promotions = promotionService.GetAllPromotionsForType(type);

            Assert.Equal(1,promotions.Count);
            Assert.Equal("barcode",promotions[0].Barcode);

        }

        [Fact]
        public void should_delete_all_promotions_for_this_type_when_these_barcodes_exist()
        {
            Fixtures.Products.Create(new Product
            {
                Barcode = "barcode",
                Id = Guid.NewGuid(),
                Name = "I do not care",
                Price = 3M
            });

            Fixtures.Promotions.Create(new Promotion
            {
                Barcode = "barcode",
                Id = Guid.NewGuid(),
                Type = "BUY_TWO_GET_ONE"
            });

            PromotionService promotionService = CreatePromotionSerivice();

            string type = "BUY_TWO_GET_ONE";
            string[] barcodes = { "barcode" };

            promotionService.DeletePromotionsForType(type, barcodes);
            IList<Promotion> allPromotionsForType = promotionService.GetAllPromotionsForType("BUY_TWO_GET_ONE");

            Assert.Equal(0, allPromotionsForType.Count);
        }

        [Fact]
        public void should_delete_some_promotions_for_this_type_when_some_of_barcodes_exist()
        {
            Fixtures.Products.Create(new Product
            {
                Barcode = "barcode",
                Id = Guid.NewGuid(),
                Name = "I do not care",
                Price = 3M
            });

            Fixtures.Promotions.Create(new Promotion
            {
                Barcode = "barcode",
                Id = Guid.NewGuid(),
                Type = "BUY_TWO_GET_ONE"
            });

            PromotionService promotionService = CreatePromotionSerivice();

            string type = "BUY_TWO_GET_ONE";
            string[] barcodes = { "barcode","barcode-not-exist"};

            promotionService.DeletePromotionsForType(type, barcodes);
            IList<Promotion> allPromotionsForType = promotionService.GetAllPromotionsForType("BUY_TWO_GET_ONE");

            Assert.Equal(0, allPromotionsForType.Count);
        }

        PromotionService CreatePromotionSerivice()
        {
            var promotionService = GetScope().Resolve<PromotionService>();
            return promotionService;
        }
    }
}