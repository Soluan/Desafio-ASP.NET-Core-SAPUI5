using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Xunit;
using System.Text.Json;
using System.Threading.Tasks;

public class UsersIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_WithPaginationAndFilter_ReturnsExpected()
    {
        var resp = await _client.GetAsync("/todos?page=1&pageSize=3&title=t");
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(1, json.GetProperty("page").GetInt32());
    }

    [Fact]
    public async Task Put_MarkIncomplete_WhenUserHasFive_Returns400()
    {
        var resp = await _client.PutAsJsonAsync("/todos/6", new { completed = false });

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }
}