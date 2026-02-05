using Microsoft.Playwright.MSTest;
using Microsoft.Playwright;

namespace RoseticTask;

[TestClass]
public class ContactFormTest : PageTest
{
    [TestMethod]
    public async Task ContactFormTestMethod()
    {
        Page.SetDefaultTimeout(5000); // skip waiting 30sec for fail 
        await Page.GotoAsync("https://www.rosetic.ai/");
        await Page.GetByRole(AriaRole.Button, new() { Name = "connect" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "contact us", Exact = true }).ClickAsync();
        await Page.GetByLabel("Contact form textbox", new() { Exact = true }).ClickAsync(); //simulates the failure observed during manual execution
    }
}