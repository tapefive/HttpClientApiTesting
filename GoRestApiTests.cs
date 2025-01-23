using System.Text;
using System.Text.Json;
using Bogus;
using Xunit;
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
        public int id { get; set; }
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

    [Fact]
    public async Task TestPostCreateUser()
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
        if (response.IsSuccessStatusCode)
        {
            _testOutputHelper.WriteLine("User created successfully.");
        }
        else
        {
            // Log error details if the request fails
            var errorContent = await response.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine($"Error: {response.StatusCode} - {errorContent}");
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        // Parse the response body to extract the user ID
        var responseBody = await response.Content.ReadAsStringAsync();
        var userResponse = JsonSerializer.Deserialize<UserResponse>(responseBody);

        if (userResponse != null)
        {
            _createdUserId = userResponse.id; // Store the created user ID
            _testOutputHelper.WriteLine($"Created User ID: {_createdUserId}");
        }
        else
        {
            throw new Exception("Failed to parse user creation response.");
        }
    }
}
