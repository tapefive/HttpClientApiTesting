using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit.Abstractions;

namespace HttpClientApiTesting;

// This class combines front-end (UI) and API testing
public class CombinedTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper; // For logging test output
    private IWebDriver _driver; // WebDriver instance for front-end testing
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://reqres.in") }; // HttpClient instance for API requests

    // Constructor initializes WebDriver and logging
    public CombinedTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _driver = new ChromeDriver(); // Initialize Chrome browser for Selenium
    }

    // Test method to validate both front-end UI and API response
    [Fact]
    public async Task TestFrontEndAndApi()
    {
        // Navigate to the front-end page
        _driver.Navigate().GoToUrl("https://reqres.in");

        // Locate and validate the header element on the page
        var header = _driver.FindElement(By.CssSelector("h2.tagline"));
        var headerText = header.Text;
        _testOutputHelper.WriteLine($"Header Text: {headerText}");
        Assert.Equal("Test your front-end against a real API", header.Text);

        // Send a GET request to the API and validate the response
        var response = await _httpClient.GetAsync("/api/users?page=2");
        Assert.True(response.IsSuccessStatusCode); // Check if the response was successful
        
        // Read and log the response content
        var responseContent = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine($"API Response: {responseContent}");

        // Validate that the response contains specific expected email addresses
        Assert.Contains("michael.lawson@reqres.in", responseContent);
        Assert.Contains("tobias.funke@reqres.in", responseContent);
        Assert.Contains("lindsay.ferguson@reqres.in", responseContent);
    }

    // Dispose method to clean up resources
    public void Dispose()
    {
        _driver.Quit(); // Close the browser after the test is completed
    }
}
