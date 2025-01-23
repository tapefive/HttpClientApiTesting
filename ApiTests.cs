using System.Net;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

namespace HttpClientApiTesting;

// Define a class for API tests
public class ApiTests(ITestOutputHelper testOutputHelper)
{
    // Create an instance of HttpClient with a base URL for API requests
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://reqres.in") };

    // Test to verify the GET LIST USERS endpoint
    [Fact] public async Task TestGetAllUsers()
    {
        // Send a GET request to the specified endpoint
        var response = await _httpClient.GetAsync("/api/users?page=2");

        // Assert that the response status code indicates success (200)
        Assert.True(response.IsSuccessStatusCode);

        // Read the API response content as a string
        var responseContent = await response.Content.ReadAsStringAsync();

        // Log the response content for debugging and validation
        testOutputHelper.WriteLine($"API Response: {responseContent}");

        // Assert that the response contains specific expected email addresses
        Assert.Contains("michael.lawson@reqres.in", responseContent);
        Assert.Contains("tobias.funke@reqres.in", responseContent);
        Assert.Contains("lindsay.ferguson@reqres.in", responseContent);
    }
    
    // Test to verify the GET SINGLE USER endpoint
    [Fact] public async Task TestGetSingleUser()
    {
        // Send a GET request to the specified endpoint
        var response = await _httpClient.GetAsync("/api/users/2");

        // Assert that the response status code indicates success (200)
        Assert.True(response.IsSuccessStatusCode);

        // Read the API response content as a string
        var responseContent = await response.Content.ReadAsStringAsync();

        // Log the response content for debugging and validation
        testOutputHelper.WriteLine($"API Response: {responseContent}");

        // Assert that the response contains specific expected email addresses
        Assert.Contains("janet.weaver@reqres.in", responseContent);
        Assert.Contains("Janet", responseContent);
        Assert.Contains("Weaver", responseContent);
    }
    
    // Test to verify the GET SINGLE USER NOT FOUND endpoint
    [Fact] public async Task TestGetSingleUserNotFound()
    {
        // Send a GET request to the specified endpoint
        var response = await _httpClient.GetAsync("/api/users/23");

        // Assert that the response status code indicates not found (404)
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        // Read the API response content as a string
        var responseContent = await response.Content.ReadAsStringAsync();

        // Log the response content for debugging and validation
        testOutputHelper.WriteLine($"API Response: {responseContent}");
    }
    
    // Test to verify the CREATE USER endpoint
    [Fact] public async Task TestPostCreateUser()
    {
        // Define the request body
        var requestBody = new
        {
            name = "Test User",
            job = "2024: QA Analyst"
        };
        
        // Serialize the request body to JSON
        string jsonBody = JsonSerializer.Serialize(requestBody);
        
        // Set up the request
        var requestContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        
        // Send a GET request to the specified endpoint
        var response = await _httpClient.PostAsync("api/users", requestContent);

        // Assert that the response status code indicates success (201)
        Assert.True(response.IsSuccessStatusCode);

        // Read the API response content as a string
        var responseContent = await response.Content.ReadAsStringAsync();

        // Log the response content for debugging and validation
        testOutputHelper.WriteLine($"API Response: {responseContent}");
        
        // Assert that the response contains the data
        Assert.Contains("Test User", responseContent);
        Assert.Contains("2024: QA Analyst", responseContent);
    }
}