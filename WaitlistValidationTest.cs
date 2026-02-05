using Microsoft.Playwright.MSTest;
using Microsoft.Playwright;

namespace RoseticTask;

[TestClass]
public class WaitlistValidationTest : PageTest
{
    [TestMethod]
    public async Task WaitlistValidation()
    {
        await Page.GotoAsync("https://www.rosetic.ai/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Join waitlist ", Exact = true }).ClickAsync();
        void Page_Dialog_EventHandler(object sender, IDialog dialog)
        {
            Console.WriteLine($"Dialog message: {dialog.Message}");
            dialog.DismissAsync();
            Page.Dialog -= Page_Dialog_EventHandler;
        }
        Page.Dialog += Page_Dialog_EventHandler;
        await Page.GetByText("Next").ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("t");
        void Page_Dialog1_EventHandler(object sender, IDialog dialog)
        {
            Console.WriteLine($"Dialog message: {dialog.Message}");
            dialog.DismissAsync();
            Page.Dialog -= Page_Dialog1_EventHandler;
        }
        Page.Dialog += Page_Dialog1_EventHandler;
        await Page.GetByText("Next").ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "First Name" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "First Name" }).FillAsync("t");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Last Name" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Last Name" }).FillAsync("t");
        void Page_Dialog2_EventHandler(object sender, IDialog dialog)
        {
            Console.WriteLine($"Dialog message: {dialog.Message}");
            dialog.DismissAsync();
            Page.Dialog -= Page_Dialog2_EventHandler;
        }
        Page.Dialog += Page_Dialog2_EventHandler;
        await Page.GetByText("Next").ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("test@test.com");
        await Page.GetByText("Next").ClickAsync();
        await Page.GetByText("I Am In academia").ClickAsync();
        //Past that point, the recorder doesn't recognise the stopping event and requires more manual intervention
    }
}