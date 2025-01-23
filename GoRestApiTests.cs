using System.Text;
using System.Text.Json;
using Bogus;
using Xunit.Abstractions;
using static HttpClientApiTesting.EnvHelper;

namespace HttpClientApiTesting;

// Test class to perform API testing with GoRest API
public class GoRestApiTests
{
    // HttpClient instance for sending requests to the API. Declares the Base URL for all requests
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://gorest.co.in") };
    // Faker instance for generating random test data
    private static readonly Faker Faker = new();
    // For writing test output in the console during execution
    private readonly ITestOutputHelper _testOutputHelper;
    // Stores the ID of the user created during the test
    private int _createdUserId;

    // Constructor that initializes the test output helper
    public GoRestApiTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    // Class to represent the API's user response
    private class UserResponse
    {
        public int id { get; init; }
    }

    // Generates random user data using the Faker library
    private object GenerateRandomUser()
    {
        return new
        {
            name = Faker.Name.FullName(),
            gender = Faker.PickRandom("Male", "Female"),
            email = Faker.Internet.Email(),
            status = Faker.PickRandom("active", "inactive")
        };
    }

    // Helper method to create a user on the POST Create a new user endpoint and return the user ID
    private async Task<int> CreateUser()
    {
        // Retrieve access token from environment variables
        var accessToken = GetEnvVariable("ACCESS_TOKEN");

        // Validate that the access token exists
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new InvalidOperationException("ACCESS_TOKEN environment variable is not set.");
        }

        // Generate random user data for the request
        var postData = GenerateRandomUser();
        string jsonData = JsonSerializer.Serialize(postData); // Serialize user data to JSON
        var requestContent = new StringContent(jsonData, Encoding.UTF8, "application/json"); // Prepare HTTP content

        // Add the Authorization header with the access token
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        // Send a POST request to create a new user
        var response = await _httpClient.PostAsync("/public/v2/users", requestContent);

        // Handle the response
        if (!response.IsSuccessStatusCode)
        {
            // Log error details if the request fails
            var errorContent = await response.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine($"Error: {response.StatusCode} - {errorContent}");
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        // Parse the response body to extract the user ID
        var responseBody = await response.Content.ReadAsStringAsync();
        var userResponse = JsonSerializer.Deserialize<UserResponse>(responseBody);

        if (userResponse == null)
        {
            throw new Exception("Failed to parse user creation response.");
        }

        _testOutputHelper.WriteLine($"Created User ID: {userResponse.id}");
        return userResponse.id; // Return the created user ID
    }
    
    // Test to verify the DELETE delete a user endpoint
    [Fact]
    public async Task TestDeleteUser()
    {
        // Ensure a user is created before attempting to update
        if (_createdUserId == 0)
        {
            _createdUserId = await CreateUser();
        }

        // Retrieve access token from environment variables
        var accessToken = GetEnvVariable("ACCESS_TOKEN");

        // Validate that the access token exists
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new InvalidOperationException("ACCESS_TOKEN environment variable is not set.");
        }

        // Add the Authorization header with the access token
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        // Send a DELETE request to delete the user
        var response = await _httpClient.DeleteAsync($"/public/v2/users/{_createdUserId}");
        
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Handle the response
        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent)
        {
            // Log error details if the request fails
            var errorContent = await response.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine($"Error: {response.StatusCode} - {errorContent}");
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        _testOutputHelper.WriteLine($"User ID: {_createdUserId} deleted successfully.");
    }
    // Test to verify the PUT Update a new user endpoint
    [Fact]
    public async Task TestPutUpdateUser()
    {
        // Ensure a user is created before attempting to update
        if (_createdUserId == 0)
        {
            _createdUserId = await CreateUser();
        }

        // Retrieve access token from environment variables
        var accessToken = GetEnvVariable("ACCESS_TOKEN");

        // Validate that the access token exists
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new InvalidOperationException("ACCESS_TOKEN environment variable is not set.");
        }

        // Generate random user data for the request
        var postData = GenerateRandomUser();
        string jsonData = JsonSerializer.Serialize(postData); // Serialize user data to JSON
        var requestContent = new StringContent(jsonData, Encoding.UTF8, "application/json"); // Prepare HTTP content

        // Add the Authorization header with the access token
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        // Send a PUT request to update the user
        var response = await _httpClient.PutAsync($"/public/v2/users/{_createdUserId}", requestContent);

        // Handle the response
        if (!response.IsSuccessStatusCode)
        {
            // Log error details if the request fails
            var errorContent = await response.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine($"Error: {response.StatusCode} - {errorContent}");
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        // Parse the response body to extract the user ID
        var responseBody = await response.Content.ReadAsStringAsync();
        var userResponse = JsonSerializer.Deserialize<UserResponse>(responseBody);

        if (userResponse == null)
        {
            throw new Exception("Failed to parse user update response.");
        }

        _testOutputHelper.WriteLine($"Updated User ID: {userResponse.id}");
        _testOutputHelper.WriteLine($"Updated User Response: {responseBody}");
    }
}
