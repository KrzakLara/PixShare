using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;


namespace PixShare.Tests.IntegrationTests
{
    public class PlaywrightTests : IAsyncLifetime
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IPage _page;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        _page = await _browser.NewPageAsync();
    }

    [Fact]
    public async Task PageLoad_ShouldDisplayImages()
    {
        // Navigate to the ManageImages page
        await _page.GotoAsync("http://localhost:58700/Admin/ManageImages");

        // Wait for images to be loaded
        await _page.WaitForSelectorAsync(".photo img");

        // Get all images
        var images = await _page.QuerySelectorAllAsync(".photo img");

        // Debugging output
        var content = await _page.ContentAsync();
        System.IO.File.WriteAllText("page-content.html", content); // Save the page content to a file

        // Log the number of images found
        System.Console.WriteLine($"Number of images found: {images.Count}");

        // Assert that the images collection is not empty
        Assert.NotEmpty(images);
    }

    public async Task DisposeAsync()
    {
        await _browser.CloseAsync();
    }
}
}