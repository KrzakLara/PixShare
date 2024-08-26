using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PixShare.Tests.UITests
{
    public class LoginPlaywrightTests : IAsyncLifetime
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;

        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            _page = await _browser.NewPageAsync();
        }

        [Fact]
        public async Task Login_ShouldSucceed_WithValidCredentials()
        {
            try
            {
                // Navigate to the login page
                await _page.GotoAsync("http://localhost:58700/Login/Index");

                // Wait for the email input to be visible
                await _page.WaitForSelectorAsync("input[name='Email']", new PageWaitForSelectorOptions { Timeout = 60000 });

                // Fill in the login form
                await _page.FillAsync("input[name='Email']", "validuser@example.com");
                await _page.FillAsync("input[name='Password']", "validpassword");

                // Click the login button and wait for navigation
                await _page.RunAndWaitForNavigationAsync(async () =>
                {
                    await _page.ClickAsync("input[type='submit']");
                }, new PageRunAndWaitForNavigationOptions
                {
                    WaitUntil = WaitUntilState.Load
                });

                // Assert that the user is redirected to the correct home page URL
                Assert.Equal("http://localhost:58700/", _page.Url);
            }
            catch (Exception ex)
            {
                // Capture the page content and URL for debugging
                var content = await _page.ContentAsync();
                var currentUrl = _page.Url;
                System.IO.File.WriteAllText("page-content.html", content);
                await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot.png" });
                System.Console.WriteLine($"Error: {ex.Message}");
                System.Console.WriteLine($"Current URL: {currentUrl}");
                throw;
            }
        }

        [Fact]
        public async Task Login_ShouldFail_WithInvalidCredentials()
        {
            try
            {
                // Navigate to the login page
                await _page.GotoAsync("http://localhost:58700/Login/Index");

                // Wait for the email input to be visible
                await _page.WaitForSelectorAsync("input[name='Email']", new PageWaitForSelectorOptions { Timeout = 60000 });

                // Fill in the login form with invalid credentials
                await _page.FillAsync("input[name='Email']", "invaliduser@example.com");
                await _page.FillAsync("input[name='Password']", "invalidpassword");

                // Click the login button
                await _page.ClickAsync("input[type='submit']");

                // Wait for the error message to be visible
                await _page.WaitForSelectorAsync(".text-danger", new PageWaitForSelectorOptions { Timeout = 60000 });

                // Assert that the error message is displayed
                var errorMessage = await _page.InnerTextAsync(".text-danger");
                Assert.Contains("Invalid login attempt.", errorMessage);
            }
            catch (Exception ex)
            {
                // Capture the page content and URL for debugging
                var content = await _page.ContentAsync();
                var currentUrl = _page.Url;
                System.IO.File.WriteAllText("page-content.html", content);
                await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot.png" });
                System.Console.WriteLine($"Error: {ex.Message}");
                System.Console.WriteLine($"Current URL: {currentUrl}");
                throw;
            }
        }

        public async Task DisposeAsync()
        {
            await _browser.CloseAsync();
        }
    }
}
