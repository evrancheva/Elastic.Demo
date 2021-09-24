using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Elastic.Demo.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Elastic.Demo.IntegrationTests
{
    public class ListingTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _testClient;
        private long existingGlobalId = 4648216;
        private readonly StringContent _body;

        public ListingTests(WebApplicationFactory<Startup> factory)
        {
            _testClient = factory.CreateClient();
            var listing = new Listing
            {
                Adres = "TestAddress",
                GlobalId = existingGlobalId,
                KantoorNaam = "TestKantoor",
                Omschrijving = "TestDescription",
                OppervlakteVan = new Random().Next(100, 200),
                OppervlakteTot = new Random().Next(200, 300)
            };
            _body = new StringContent(JsonConvert.SerializeObject(listing), Encoding.UTF8, "application/json");
        }

        [Fact]
        public async void GetListingWithExistingGlobalId_ReturnsValidListing()
        {
            // Arrange
            await CreateTestListing();

            // Act
            var response = await _testClient.GetAsync($"api/listing/globalId/{existingGlobalId}");

            // Assert
            var result = JsonConvert.DeserializeObject<Listing>(await response.Content.ReadAsStringAsync());
            Assert.Equal("TestAddress", result?.Adres);
        }

        [Fact]
        public async void GetListingWithExistingId_ReturnsValidListing()
        {
            // Arrange
            var id = await CreateTestListing();

            // Act
            var response = await _testClient.GetAsync($"api/listing/id/{id}");

            // Assert
            var result = JsonConvert.DeserializeObject<Listing>(await response.Content.ReadAsStringAsync());
            Assert.Equal("TestAddress", result?.Adres);
        }

        [Fact]
        public async void PostNewListing_ReturnsSuccess()
        {
            // Act
            var response = await _testClient.PostAsync("api/listing/", _body);
            var contentString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains($"Document was created", contentString);
        }

        [Fact]
        public async void UpdateExistingListing_ReturnsSuccess()
        {
            // Arrange
            var id = await CreateTestListing();

            // Act
            var response = await _testClient.PutAsync($"api/listing/{id}", _body);
            var contentString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal($"Listing with id - {id} was successfully updated", contentString);
        }

        [Fact]
        public async void DeleteExistingListing_ReturnsSuccess()
        {
            // Arrange
            var id = await CreateTestListing();

            // Act
            var response = await _testClient.DeleteAsync($"api/listing/{id}");
            var contentString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal($"Listing with id - {id} was succesfully deleted", contentString);
        }

        private async Task<string> CreateTestListing()
        {
            var createResponse = await _testClient.PostAsync("api/listing/", _body);
            var createContentString = await createResponse.Content.ReadAsStringAsync();
            var id = createContentString.Split(':')[1].Trim();
            return id;
        }
    }
}
