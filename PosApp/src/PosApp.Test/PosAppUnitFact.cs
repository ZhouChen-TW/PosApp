using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Autofac;
using Xunit;

namespace PosApp.Test
{
    public class PosAppUnitFact:ApiBaseTest
    {

        [Fact]
        public async Task should_return_forbbiden_when_inputs_not_exist()
        {
            var client = CreateHttpClient();
            HttpResponseMessage response = await client.PostAsync("receipt/",
                new {},
                new JsonMediaTypeFormatter());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task should_return_forbbiden_when_inputs_illegal()
        {
            var client = CreateHttpClient();
            HttpResponseMessage response = await client.PostAsync("receipt/",
                new []{""},
                new JsonMediaTypeFormatter());

            Assert.Equal(HttpStatusCode.Forbidden,response.StatusCode);

        }

        [Fact]
        public async Task should_return_ok_when_inputs_legal()
        {
            CreateProductFixture(
                new Product { Barcode = "ITEM001", Price = 10M, Name = "coca" },
                new Product { Barcode = "ITEM002", Price = 20M, Name = "juice" });

            var client = CreateHttpClient();
            HttpResponseMessage response = await client.PostAsync("receipt/",
              new []
              {
                  "ITEM001-2",
                  "ITEM002",
                  "ITEM001"
              },
              new JsonMediaTypeFormatter());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var message = await response.Content.ReadAsAsync<Receipt>();

            Assert.Equal(50M,message.Total);
        }

        void CreateProductFixture(params Product[] products)
        {
            var repository = GetLifetimeScope().Resolve<IProductRepository>();
            Array.ForEach(products, p => repository.Save(p));
        }
    }
}