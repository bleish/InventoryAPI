using FluentAssertions;
using InventoryAPI.EndToEndTests.Fixtures;
using InventoryAPI.EndToEndTests.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace InventoryAPI.EndToEndTests
{
    public class ItemsControllerTests : IClassFixture<ItemsControllerFixture>
    {
        private readonly ItemsControllerFixture _fixture;

        public ItemsControllerTests(ItemsControllerFixture fixture)
        {
            _fixture = fixture;
        }

        // GET Tests

        [Fact]
        public async Task GET_Expect_200_OK_When_Default()
        {
            var result = await _fixture.Client.Get(ItemsControllerFixture.Endpoint);

            result.Response.IsSuccessStatusCode.Should().BeTrue();
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);

            var items = result.GetTypedContent<List<ItemReadModel>>();

            items.Any().Should().BeTrue();
            items.FirstOrDefault().Id.Should().NotBeNullOrWhiteSpace();
        }

        // GET_ID Tests

        [Fact]
        public async Task GET_ID_Expect_200_OK_When_Default()
        {
            var result = await _fixture.Client.Get(ItemsControllerFixture.Endpoint, _fixture.FixtureItemId);

            result.Response.IsSuccessStatusCode.Should().BeTrue();
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);

            var item = result.GetTypedContent<ItemReadModel>();

            item.Id.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("InvalidId")]
        [InlineData("507f1f77bcf86cd799439011")]
        public async Task GET_ID_Expect_404_NotFound_When_InvalidId(string id)
        {
            var result = await _fixture.Client.Get(ItemsControllerFixture.Endpoint, id);

            result.Response.IsSuccessStatusCode.Should().BeFalse();
            result.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // POST Tests

        [Fact]
        public async Task POST_Expect_201_Created_When_Default()
        {
            var result = await _fixture.Client.Post(ItemsControllerFixture.Endpoint, new ItemCreateModel
                {
                    Name = $"{ItemsControllerFixture.TestKey}_POST_201_Name",
                    Model = $"{ItemsControllerFixture.TestKey}_POST_201_Model",
                    Category = $"{ItemsControllerFixture.TestKey}_POST_201_Category"
                });

            result.Response.IsSuccessStatusCode.Should().BeTrue();
            result.Response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.ResourceId.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task POST_Expect_400_BadRequest_When_MissingRequiredFields()
        {
            var result = await _fixture.Client.Post(ItemsControllerFixture.Endpoint, new ItemCreateModel
                {
                    Name = $"{ItemsControllerFixture.TestKey}_POST_400_Name"
                });

            result.Response.IsSuccessStatusCode.Should().BeFalse();
            result.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // PUT Tests

        [Fact]
        public async Task PUT_Expect_204_NoContent_When_Default()
        {
            var result = await _fixture.Client.Put(ItemsControllerFixture.Endpoint, new ItemCreateModel
                {
                    Name = $"{ItemsControllerFixture.TestKey}_PUT_204_Name",
                    Model = $"{ItemsControllerFixture.TestKey}_PUT_204_Model",
                    Category = $"{ItemsControllerFixture.TestKey}_PUT_204_Category"
                }, _fixture.FixtureItemId);

            result.Response.IsSuccessStatusCode.Should().BeTrue();
            result.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task PUT_Expect_400_BadRequest_When_MissingRequiredFields()
        {
            var result = await _fixture.Client.Put(ItemsControllerFixture.Endpoint, new ItemCreateModel(), _fixture.FixtureItemId);

            result.Response.IsSuccessStatusCode.Should().BeFalse();
            result.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("InvalidId")]
        [InlineData("507f1f77bcf86cd799439011")]
        public async Task PUT_Expect_404_NotFound_When_InvalidId(string id)
        {
            var result = await _fixture.Client.Put(ItemsControllerFixture.Endpoint, new ItemCreateModel
                {
                    Name = $"{ItemsControllerFixture.TestKey}_PUT_404_Name",
                    Model = $"{ItemsControllerFixture.TestKey}_PUT_404_Model",
                    Category = $"{ItemsControllerFixture.TestKey}_PUT_404_Category"
                }, id);

            result.Response.IsSuccessStatusCode.Should().BeFalse();
            result.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // DELETE Tests

        [Fact]
        public async Task DELETE_Expect_204_NoContent_When_Default()
        {
            var postResult = await _fixture.Client.Post(ItemsControllerFixture.Endpoint, new ItemCreateModel
                {
                    Name = $"{ItemsControllerFixture.TestKey}_DELETE_204_Name",
                    Model = $"{ItemsControllerFixture.TestKey}_DELETE_204_Model",
                    Category = $"{ItemsControllerFixture.TestKey}_DELETE_204_Category"
                });

            postResult.Response.IsSuccessStatusCode.Should().BeTrue();
            postResult.Response.StatusCode.Should().Be(HttpStatusCode.Created);
            postResult.ResourceId.Should().NotBeNullOrWhiteSpace();

            var result = await _fixture.Client.Delete(ItemsControllerFixture.Endpoint, postResult.ResourceId);

            result.Response.IsSuccessStatusCode.Should().BeTrue();
            result.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Theory]
        [InlineData("InvalidId")]
        [InlineData("507f1f77bcf86cd799439011")]
        public async Task DELETE_Expect_404_NotFound_When_InvalidId(string id)
        {
            var result = await _fixture.Client.Delete(ItemsControllerFixture.Endpoint, id);

            result.Response.IsSuccessStatusCode.Should().BeFalse();
            result.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
