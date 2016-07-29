using System;
using System.Linq;
using Autofac;
using PosApp.Domain;
using PosApp.Services;
using PosApp.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace PosApp.Test.Apis
{
    public class PosAppFacts : FactBase
    {
        public PosAppFacts(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void should_fail_if_bought_products_are_not_provided()
        {
            PosService posService = CreatePosService();

            Assert.Throws<ArgumentNullException>(() => posService.GetReceipt(null));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void should_fail_if_one_of_bought_product_amount_is_less_than_or_equal_to_zero(int invalidAmount)
        {
            PosService posService = CreatePosService();
            var invalidProduct = new BoughtProduct("barcode001", invalidAmount);
            var validProduct = new BoughtProduct("barcode002", 1);

            BoughtProduct[] boughtProducts = {invalidProduct, validProduct};

            Assert.Throws<ArgumentException>(() => posService.GetReceipt(boughtProducts));
        }

        [Fact]
        public void should_fail_if_bought_product_does_not_exist()
        {
            PosService posService = CreatePosService();
            var notExistedProduct = new BoughtProduct("barcode", 1);

            Assert.Throws<ArgumentException>(() => posService.GetReceipt(new[] {notExistedProduct}));
        }

        [Fact]
        public void should_merge_receipt_items()
        {
            Fixtures.Products.Create(
                new Product {Barcode = "barcodesame", Name = "I do not care" },
                new Product {Barcode = "barcodediff", Name = "I do not care" });
            PosService posService = CreatePosService();
            var boughtProduct = new BoughtProduct("barcodesame", 1);
            var sameBoughtProduct = new BoughtProduct("barcodesame", 2);
            var differentBoughtProduct = new BoughtProduct("barcodediff", 1);

            Receipt receipt = posService.GetReceipt(
                new[] {boughtProduct, differentBoughtProduct, sameBoughtProduct});

            Assert.Equal(receipt.ReceiptItems.Single(i => i.Product.Barcode == "barcodesame").Amount, 3);
            Assert.Equal(receipt.ReceiptItems.Single(i => i.Product.Barcode == "barcodediff").Amount, 1);
        }

        [Fact]
        public void should_contains_discout_when_product_in_promotions()
        {
            Fixtures.Products.Create(
                new Product {Barcode = "discountbarcode", Name = "I do not care", Price = 3M});
            Fixtures.Promotions.Create(
                new Promotion {Barcode = "discountbarcode", Type = "BUY_TWO_GET_ONE"});
            PosService posService = CreatePosService();
            var boughtProduct = new BoughtProduct("discountbarcode", 3);

            Receipt receipt = posService.GetReceipt(
                new[] {boughtProduct});

            Assert.Equal(receipt.ReceiptItems.Single(i => i.Product.Barcode.Equals("discountbarcode")).Promoted, 3M);
            Assert.Equal(receipt.Total, 6M);
            Assert.Equal(receipt.Promoted, 3M);
        }

        [Fact]
        public void should_contains_discout_when_some_product_in_promotions()
        {
            Fixtures.Products.Create(
                new Product { Barcode = "discountbarcode", Name = "I do not care", Price = 3M },
                new Product { Barcode = "notdiscountbarcode", Name = "I do not care", Price = 1M });
            Fixtures.Promotions.Create(
                new Promotion { Barcode = "discountbarcode", Type = "BUY_TWO_GET_ONE" });
            PosService posService = CreatePosService();
            var boughtDiscountProduct = new BoughtProduct("discountbarcode", 3);
            var boughtProduct = new BoughtProduct("notdiscountbarcode", 3);

            Receipt receipt = posService.GetReceipt(
                new[] { boughtDiscountProduct,boughtProduct });

            Assert.Equal(receipt.ReceiptItems.Single(i => i.Product.Barcode.Equals("discountbarcode")).Promoted, 3M);
            Assert.Equal(receipt.Total, 9M);
            Assert.Equal(receipt.Promoted, 3M);
        }

        [Fact]
        public void should_calculate_subtotal()
        {
            Fixtures.Products.Create(
                new Product { Barcode = "barcode", Price = 10M, Name = "I do not care"});
            PosService posService = CreatePosService();

            Receipt receipt = posService.GetReceipt(
                new[] { new BoughtProduct("barcode", 2) });

            Assert.Equal(20M, receipt.ReceiptItems.Single().Total);
        }

        [Fact]
        public void should_calculate_total()
        {
            // given
            Fixtures.Products.Create(
                new Product { Barcode = "barcode001", Price = 10M, Name = "I do not care" },
                new Product { Barcode = "barcode002", Price = 20M, Name = "I do not care" });

            PosService posService = CreatePosService();

            // when
            Receipt receipt = posService.GetReceipt(
                new[] { new BoughtProduct("barcode001", 2), new BoughtProduct("barcode002", 3) });

            Assert.Equal(80M, receipt.Total);
        }

        [Fact]
        public void should_not_cut_half_when_total_less_than_one_hundred_and_only_one_type()
        {
            Fixtures.Products.Create(new Product
            {
                Barcode = "I do not care",
                Id = Guid.NewGuid(),
                Name = "I do not care",
                Price = 80M
            });

            Fixtures.Promotions.Create(new Promotion
            {
                Barcode = "I do not care",
                Id = Guid.NewGuid(),
                Type = "BUY_HUNDRED_GET_HALF"
            });

            PosService posService = CreatePosService();

            Receipt receipt = posService.GetReceipt(
                new[] { new BoughtProduct("I do not care", 1) });

            Assert.Equal(80M, receipt.Total);
            Assert.Equal(0M, receipt.Promoted);
        }

        [Fact]
        public void should_cut_half_per_hundred_when_total_more_than_one_hundred_and_only_one_type()
        {
            Fixtures.Products.Create(new Product
            {
                Barcode = "I do not care",
                Id = Guid.NewGuid(),
                Name = "I do not care",
                Price = 100M
            });

            Fixtures.Promotions.Create(new Promotion
            {
                Barcode = "I do not care",
                Id = Guid.NewGuid(),
                Type = "BUY_HUNDRED_GET_HALF"
            });

            PosService posService = CreatePosService();

            Receipt receipt = posService.GetReceipt(
                new[] { new BoughtProduct("I do not care", 1) });

            Assert.Equal(50M,receipt.Total);
            Assert.Equal(50M,receipt.Promoted);
        }

        [Fact]
        public void should_not_cut_half_per_hundred_when_discount_total_less_than_one_hundred_and_two_type()
        {
            Fixtures.Products.Create(new Product
            {
                Barcode = "I do not care type one",
                Id = Guid.NewGuid(),
                Name = "I do not care name one",
                Price = 40M
            },
            new Product
            {
                Barcode = "I do not care type two",
                Id = Guid.NewGuid(),
                Name = "I do not care name two",
                Price = 20M
            });

            Fixtures.Promotions.Create(new Promotion
            {
                Barcode = "I do not care type two",
                Id = Guid.NewGuid(),
                Type = "BUY_HUNDRED_GET_HALF"
            },
            new Promotion
            {
                Barcode = "I do not care type two",
                Id = Guid.NewGuid(),
                Type = "BUY_TWO_GET_ONE"
            },new Promotion
            {
                Barcode = "I do not care type one",
                Id = Guid.NewGuid(),
                Type = "BUY_HUNDRED_GET_HALF"
            });

            PosService posService = CreatePosService();

            Receipt receipt = posService.GetReceipt(
                new[] { new BoughtProduct("I do not care type one", 1),new BoughtProduct("I do not care type two",3)});

            Assert.Equal(30M, receipt.Total);
            Assert.Equal(70M, receipt.Promoted);
        }

        [Fact]
        public void should_not_cut_half_per_hundred_when_discount_total_more_than_one_hundred_and_two_type()
        {
            Fixtures.Products.Create(new Product
            {
                Barcode = "I do not care type one",
                Id = Guid.NewGuid(),
                Name = "I do not care name one",
                Price = 40M
            },
            new Product
            {
                Barcode = "I do not care type two",
                Id = Guid.NewGuid(),
                Name = "I do not care name two",
                Price = 40M
            });

            Fixtures.Promotions.Create(new Promotion
            {
                Barcode = "I do not care type two",
                Id = Guid.NewGuid(),
                Type = "BUY_HUNDRED_GET_HALF"
            },
            new Promotion
            {
                Barcode = "I do not care type two",
                Id = Guid.NewGuid(),
                Type = "BUY_TWO_GET_ONE"
            }, new Promotion
            {
                Barcode = "I do not care type one",
                Id = Guid.NewGuid(),
                Type = "BUY_HUNDRED_GET_HALF"
            });

            PosService posService = CreatePosService();

            Receipt receipt = posService.GetReceipt(
                new[] { new BoughtProduct("I do not care type one", 1), new BoughtProduct("I do not care type two", 3) });

            Assert.Equal(70M, receipt.Total);
            Assert.Equal(90M, receipt.Promoted);
        }


        PosService CreatePosService()
        {
            var posService = GetScope().Resolve<PosService>();
            return posService;
        }
    }
}