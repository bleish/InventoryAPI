using FluentAssertions;
using InventoryAPI.EndToEndTests.Helpers;
using InventoryAPI.EndToEndTests.Models;
using InventoryAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace InventoryAPI.EndToEndTests.Fixtures
{
    public class ItemsControllerFixture : IDisposable
    {
        public const string Endpoint = "api/items";
        public const string TestKey = "E2ETEST";
        public InventoryClient Client { get; private set; }
        public string FixtureItemId { get; private set; }

        public ItemsControllerFixture()
        {
            Client = new InventoryClient();

            var result = Client.Post(Endpoint, new ItemCreateModel
                {
                    Name = $"{TestKey}_FIXTURE_Name",
                    Model = $"{TestKey}_FIXTURE_Model",
                    Category = $"{TestKey}_FIXTURE_Category"
                }).GetAwaiter().GetResult();

            result.Response.IsSuccessStatusCode.Should().BeTrue();
            result.Response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            FixtureItemId = result.ResourceId;
        }

        public void Dispose()
        {
            var result = Client.Get(Endpoint).GetAwaiter().GetResult();
            var items = result.GetTypedContent<List<Item>>();
            var testItems = items.Where(i => i.Name.Split('_').FirstOrDefault() == TestKey);
            
            foreach (var item in testItems)
            {
                var deleteResult = Client.Delete(Endpoint, item.Id).GetAwaiter().GetResult();

                deleteResult.Response.IsSuccessStatusCode.Should().BeTrue();
                deleteResult.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            }
        }
    }
}