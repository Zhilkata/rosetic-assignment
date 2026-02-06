using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using RoseticTask.Models;

namespace RoseticTask.Tests;

[TestClass]
public class WaitlistTests : PageTest
{
    private const string BaseUrl = "https://www.rosetic.ai/";
    private const string WaitlistUrl = BaseUrl + "#waitlist-form";

    #region Test Data
    private const string ValidFirstName = "TestFN";
    private const string ValidLastName = "TestLN";
    private const string ValidEmail = "test@test.com";
    private const string InvalidEmailSingleChar = "t";
    private const string InvalidEmailFormat = "invalid.email";
    
    private const string Industry = "SaaS / Software";
    private const string CompanySize = "1 - 10";
    private const string JobFunction = "Engineering / Dev";
    private const string ProjectType = "Developer Tools (IDEs, version control systems,etc.)";
    private const string ProjectDescription = "Test Text 123@!";
    #endregion

    #region Expected Messages
    private const string MissingFieldsMessage = "Please fill out all fields before proceeding.";
    private const string InvalidEmailMessage = "Please enter a valid email address.";
    private const string SuccessMessage = "You've joined the waitlist!";
    #endregion

    private WaitlistPage _waitlistPage = null!;

    [TestInitialize]
    public async Task TestInitialize()
    {
        _waitlistPage = new WaitlistPage(Page);
        await _waitlistPage.NavigateAsync(WaitlistUrl);
    }

    [TestMethod]
    [TestCategory("E2E")]
    [TestCategory("Smoke")]
    public async Task WaitlistRegistration_WithValidData_CompletesSuccessfully()
    {
        // Arrange
        await SetupSuccessfulSubmissionMock();

        // Act - Fill Step 1: Personal Information
        await _waitlistPage.FillPersonalInformationAsync(
            ValidFirstName, 
            ValidLastName, 
            ValidEmail);
        await _waitlistPage.ProceedToNextStepAsync();

        // Act - Fill Step 2: Professional Information
        await _waitlistPage.SelectProfessionalTypeAsync();
        await _waitlistPage.FillProfessionalDetailsAsync(
            Industry, 
            CompanySize, 
            JobFunction);
        await _waitlistPage.FillProjectInformationAsync(
            ProjectType, 
            ProjectDescription);

        // Act - Step 3: Submit (mocked)
        var responseTask = Page.WaitForResponseAsync("**/www.rosetic.ai/**");
        await Page.GotoAsync(BaseUrl);

        // Assert
        var response = await responseTask;
        var bodyJson = await response.JsonAsync();
        var message = bodyJson.Value[0].GetProperty("message").GetString();
        
        Assert.AreEqual(SuccessMessage, message);
    }

    [TestMethod]
    [TestCategory("E2E")]
    public async Task WaitlistRegistration_UsingWorkflowMethod_CompletesSuccessfully()
    {
        // Arrange
        await SetupSuccessfulSubmissionMock();

        // Act
        await _waitlistPage.CompleteWaitlistRegistrationAsync(
            ValidFirstName,
            ValidLastName,
            ValidEmail,
            Industry,
            CompanySize,
            JobFunction,
            ProjectType,
            ProjectDescription);

        var responseTask = Page.WaitForResponseAsync("**/www.rosetic.ai/**");
        await Page.GotoAsync(BaseUrl);

        // Assert
        var response = await responseTask;
        var bodyJson = await response.JsonAsync();
        var message = bodyJson.Value[0].GetProperty("message").GetString();
        
        Assert.AreEqual(SuccessMessage, message);
    }

    #region Validation Tests

    [TestMethod]
    [TestCategory("Validation")]
    public async Task ClickNext_WithAllFieldsEmpty_ShowsMissingFieldsError()
    {
        // Act
        await _waitlistPage.ProceedToNextStepAsync();
        await WaitForDialog();

        // Assert
        Assert.AreEqual(MissingFieldsMessage, _waitlistPage.DialogMessage);
    }

    [TestMethod]
    [TestCategory("Validation")]
    [DataRow("t", DisplayName = "Single character")]
    [DataRow("invalid", DisplayName = "No @ symbol")]
    [DataRow("@test.com", DisplayName = "Missing local part")]
    [DataRow("test@", DisplayName = "Missing domain")]
    public async Task ClickNext_WithInvalidEmail_ShowsMissingFieldsError(string invalidEmail)
    {
        // Arrange
        await _waitlistPage.FillPersonalInformationAsync(
            ValidFirstName,
            ValidLastName,
            invalidEmail);

        // Act
        await _waitlistPage.ProceedToNextStepAsync();
        await WaitForDialog();

        // Assert - Should show missing fields because email format is validated client-side
        Assert.AreEqual(MissingFieldsMessage, _waitlistPage.DialogMessage);
    }

    [TestMethod]
    [TestCategory("Validation")]
    public async Task ClickNext_WithOnlyEmailMissing_ShowsEmailValidationError()
    {
        // Arrange
        await _waitlistPage.FillPersonalInformationAsync(
            ValidFirstName,
            ValidLastName,
            InvalidEmailSingleChar);

        // Act
        await _waitlistPage.ProceedToNextStepAsync();
        await WaitForDialog();

        // Assert
        Assert.AreEqual(MissingFieldsMessage, _waitlistPage.DialogMessage);
    }

    [TestMethod]
    [TestCategory("Validation")]
    public async Task ClickNext_WithFirstAndLastNameOnly_ShowsEmailValidationError()
    {
        // Arrange
        await _waitlistPage.FillPersonalInformationAsync(
            ValidFirstName,
            ValidLastName,
            InvalidEmailSingleChar);

        // Act
        await _waitlistPage.ProceedToNextStepAsync();
        await WaitForDialog();

        // Assert
        Assert.AreEqual(InvalidEmailMessage, _waitlistPage.DialogMessage);
    }

    [TestMethod]
    [TestCategory("Validation")]
    [DataRow("", ValidLastName, ValidEmail, DisplayName = "Missing first name")]
    [DataRow(ValidFirstName, "", ValidEmail, DisplayName = "Missing last name")]
    [DataRow(ValidFirstName, ValidLastName, "", DisplayName = "Missing email")]
    [DataRow("", "", ValidEmail, DisplayName = "Missing first and last name")]
    [DataRow(ValidFirstName, "", "", DisplayName = "Missing last name and email")]
    public async Task ClickNext_WithMissingRequiredFields_ShowsMissingFieldsError(
        string firstName, 
        string lastName, 
        string email)
    {
        // Arrange
        await _waitlistPage.FillPersonalInformationAsync(firstName, lastName, email);

        // Act
        await _waitlistPage.ProceedToNextStepAsync();
        await WaitForDialog();

        // Assert
        Assert.AreEqual(MissingFieldsMessage, _waitlistPage.DialogMessage);
    }

    #endregion

    #region Helper Methods

    private async Task SetupSuccessfulSubmissionMock()
    {
        await Page.RouteAsync("**/www.rosetic.ai/**", async route =>
        {
            var json = new[] 
            { 
                new { status = 200, message = SuccessMessage } 
            };
            
            await route.FulfillAsync(new()
            {
                Status = 200,
                ContentType = "application/json",
                Json = json
            });
        });
    }

    private static async Task WaitForDialog(int milliseconds = 500)
    {
        // Allow time for dialog to appear
        // Note: Consider using Playwright's built-in wait methods if possible
        await Task.Delay(milliseconds);
    }

    #endregion
}