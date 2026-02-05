using Microsoft.Playwright.MSTest;
using Microsoft.Playwright;

namespace RoseticTask;

[TestClass]
public class WaitlistHappyPathTest : PageTest
{
    [TestMethod]
    public async Task WaitlistE2E()
    {
        await Page.GotoAsync("https://www.rosetic.ai/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Join waitlist ", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "First Name" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "First Name" }).FillAsync("TestFN");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Last Name" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Last Name" }).FillAsync("TestLN");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("test@test.com");
        await Page.GetByText("Next").ClickAsync();
        await Page.Locator(".w-layout-hflex.waitlist-form_switch-btn").First.ClickAsync();
        await Page.GetByLabel("Industry").SelectOptionAsync(new[] { "SaaS / Software" });
        await Page.GetByLabel("Company Size").SelectOptionAsync(new[] { "1 - 10" });
        await Page.GetByLabel("Job Function").SelectOptionAsync(new[] { "Engineering / Dev" });
        await Page.GetByLabel("Current or Future Project").SelectOptionAsync(new[] { "Enterprise Applications (ERP systems, CRM platforms,etc.) " });
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Describe Your Project In a" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Describe Your Project In a" }).FillAsync("test123");
        //await Page.GetByRole(AriaRole.Button, new() { Name = "JOIN THE WISHLIST" }).ClickAsync();
    }
}
