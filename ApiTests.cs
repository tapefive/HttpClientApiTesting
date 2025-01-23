using Xunit.Abstractions;

namespace HttpClientApiTesting;

// Define a class for API tests
public class ApiTests(ITestOutputHelper _testOutputHelper)
{
    // Create an instance of HttpClient with a base URL for API requests
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://reqres.in") };

    // Test to verify the GET endpoint
    [Fact]
    public async Task TestGetEndpoint()
    {
        // Send a GET request to the specified endpoint
        var response = await _httpClient.GetAsync("/api/users?page=2");

        // Assert that the response status code indicates success (2xx)
        Assert.True(response.IsSuccessStatusCode);

        // Read the API response content as a string
        var responseContent = await response.Content.ReadAsStringAsync();

        // Log the response content for debugging and validation
        _testOutputHelper.WriteLine($"API Response: {responseContent}");

        // Assert that the response contains specific expected email addresses
        Assert.Contains("michael.lawson@reqres.in", responseContent);
        Assert.Contains("tobias.funke@reqres.in", responseContent);
        Assert.Contains("lindsay.ferguson@reqres.in", responseContent);
    }
}