using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PosApp.Domain;
using PosApp.Dtos.Responses;
using PosApp.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace PosApp.Test.Apis
{
    public class PromotionControllerFacts:ApiFactBase
    {
        public PromotionControllerFacts(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task should_get_400_bad_request_when_only_one_barcode_and_the_barcode_product_not_exist()
        {
            Fixtures.Products.Create(
                new Product { Barcode = "barcode_coca", Id = Guid.NewGuid(), Name = "Coca Cola", Price = 1M },
                new Product { Barcode = "barcode_poky", Id = Guid.NewGuid(), Name = "Poky", Price = 10M });

            HttpClient httpClient = CreateHttpClient();
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                "promotions/BUY_TWO_GET_ONE",
                new []{ "barcode-does-not-exist" });
       
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var message = await response.Content.ReadAsAsync<MessageDto>();
            Assert.Equal("Add Promotions error", message.Message);
        }

        [Fact]
        public async Task should_get_400_bad_request_when_two_barcode_and_one_barcode_product_not_exist_and_the_other_can_not_be_added()
        {
            Fixtures.Products.Create(
                new Product { Barcode = "barcode_coca", Id = Guid.NewGuid(), Name = "Coca Cola", Price = 1M },
                new Product { Barcode = "barcode_poky", Id = Guid.NewGuid(), Name = "Poky", Price = 10M });

            HttpClient httpClient = CreateHttpClient();
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                "promotions/BUY_TWO_GET_ONE",
                new[] { "barcode-does-not-exist","barcode_coca"});

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var message = await response.Content.ReadAsAsync<MessageDto>();
            Assert.Equal("Add Promotions error", message.Message);
        }

        [Fact]
        public async Task should_get_201_created_if_the_barcode_product_exist_and_the_barcode_is_for_this_type()
        {
            Fixtures.Products.Create(
                new Product
                {
                    Barcode = "barcode-not-for-this-type",
                    Id = Guid.NewGuid(),
                    Name = "I do not care",
                    Price = 1M
                });

            Fixtures.Promotions.Create(new Promotion
            {
                Barcode = "barcode-not-for-this-type",
                Id = Guid.NewGuid(),
                Type = "BUY_TWO_GET_ONE"
            });

            HttpClient httpClient = CreateHttpClient();
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                "promotions/BUY_TWO_GET_ONE",
                new[] { "barcode-not-for-this-type" });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var message = await response.Content.ReadAsAsync<MessageDto>();
            Assert.Equal("Add Promotions success",message.Message);
        }

        [Fact]
        public async Task should_get_201_created_if_the_barcode_product_exist_and_the_barcode_is_not_for_this_type()
        {
            Fixtures.Products.Create(
                new Product
                {
                    Barcode = "barcode-not-for-this-type",
                    Id = Guid.NewGuid(),
                    Name = "I do not care",
                    Price = 1M
                });

            Fixtures.Promotions.Create(new Promotion
            {
                Barcode = "barcode-not-for-this-type",
                Id = Guid.NewGuid(),
                Type = ""
            });

            HttpClient httpClient = CreateHttpClient();
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                "promotions/BUY_TWO_GET_ONE",
                new[] { "barcode-not-for-this-type" });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var message = await response.Content.ReadAsAsync<MessageDto>();
            Assert.Equal("Add Promotions success", message.Message);
        }

        [Fact]
        public async Task should_get_200_when_need_list_all_promotions_for_this_type()
        {
            Fixtures.Promotions.Create(new Promotion
            {
                Barcode = "barcode",
                Id = Guid.NewGuid(),
                Type = "BUY_TWO_GET_ONE"
            });
            HttpClient httpClient = CreateHttpClient();
            HttpResponseMessage response = await httpClient.GetAsync("promotions/BUY_TWO_GET_ONE");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var message = await response.Content.ReadAsAsync<IList<Promotion>>();

            Assert.Equal("BUY_TWO_GET_ONE", message[0].Type);
            Assert.Equal("barcode", message[0].Barcode);
        }

        [Fact]
        public async Task should_get_200_when_delete_barcodes_some_not_exist()
        {
            Fixtures.Products.Create(
                new Product
                {
                    Barcode = "barcode_Coca",
                    Id = Guid.NewGuid(),
                    Name = "I do not care",
                    Price = 1M
                });

            Fixtures.Promotions.Create(new Promotion
            {
                Barcode = "barcode_Coca",
                Id = Guid.NewGuid(),
                Type = "BUY_TWO_GET_ONE"
            });

            HttpClient httpClient = CreateHttpClient();
            HttpResponseMessage response = await httpClient.DeleteAsync(
                "promotions/BUY_TWO_GET_ONE",
                new[] { "barcode","barcode-not-exist" });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var message = await response.Content.ReadAsAsync<MessageDto>();
            Assert.Equal("Delete success", message.Message);
        }

        [Fact]
        public async Task should_get_200_when_delete_barcodes_all_exist()
        {
            Fixtures.Products.Create(
                new Product
                {
                    Barcode = "barcode_Coca",
                    Id = Guid.NewGuid(),
                    Name = "I do not care",
                    Price = 1M
                });

            Fixtures.Promotions.Create(new Promotion
            {
                Barcode = "barcode_Coca",
                Id = Guid.NewGuid(),
                Type = "BUY_TWO_GET_ONE"
            });

            HttpClient httpClient = CreateHttpClient();
            HttpResponseMessage response = await httpClient.DeleteAsync(
                "promotions/BUY_TWO_GET_ONE",
                new[] { "barcode" });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var message = await response.Content.ReadAsAsync<MessageDto>();
            Assert.Equal("Delete success", message.Message);
        }
    }
}