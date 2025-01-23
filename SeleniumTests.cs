using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit.Abstractions;

namespace HttpClientApiTesting;

// This class contains Selenium tests for the web application
public class SeleniumTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper; // For logging test output
    private IWebDriver _driver; // WebDriver instance for interacting with the browser

    // Constructor initializes the WebDriver and sets up the output helper
    public SeleniumTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _driver = new ChromeDriver(); // Initializes Chrome WebDriver to run the test
    }

    // Test method to verify the homepage loads and the header is correct
    [Fact]
    public void TestHomePageLoads()
    {
        // Navigate to the specified URL
        _driver.Navigate().GoToUrl("https://reqres.in");

        // Locate the header element on the page
        var header = _driver.FindElement(By.CssSelector("h2.tagline"));
        
        // Get the text of the header
        var headerText = header.Text;

        // Log the header text to the test output
        _testOutputHelper.WriteLine($"Header Text: {headerText}");

        // Assert that the header text matches the expected value
        Assert.Equal("Test your front-end against a real API", header.Text);
    }

    // Dispose method to clean up resources after the test
    public void Dispose()
    {
        _driver.Quit(); // Close the browser once the test completes
    }
}