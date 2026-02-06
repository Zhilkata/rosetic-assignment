using Microsoft.Playwright;

namespace RoseticTask.Models;

public class WaitlistPage
{
    private readonly IPage _page;
    
    // Public property for dialog message verification
    public string? DialogMessage { get; private set; }

    #region Locators - Step 1: Personal Information
    private ILocator FirstNameInput => _page.Locator("#First-Name");
    private ILocator LastNameInput => _page.Locator("#Last-Name");
    private ILocator EmailInput => _page.Locator("#Email-Address");
    private ILocator NextButton => _page.Locator("#Next-form");
    #endregion

    #region Locators - Step 2: Professional Information
    private ILocator ProfessionalButton => _page.GetByText("I Am A Professional");
    private ILocator IndustryDropdown => _page.GetByLabel("Industry");
    private ILocator CompanyDropdown => _page.GetByLabel("Company Size");
    private ILocator JobDropdown => _page.GetByLabel("Job Function");
    private ILocator ProjectDropdown => _page.GetByLabel("Current Or Future Project");
    private ILocator ProjectTextbox => _page.Locator("#Describe-your-project");
    #endregion

    #region Locators - Step 3: Consent & Submission
    private ILocator UpdatesCheckbox => _page.Locator("#communications-consent");
    private ILocator AgreementCheckbox => _page.Locator("#Email-Consent");
    private ILocator JoinWaitlistButton => _page.Locator("input[value='JOIN THE WAITLIST']");
    #endregion

    public WaitlistPage(IPage page)
    {
        _page = page;
        SetupDialogHandler();
    }

    #region Navigation
    public async Task NavigateAsync(string url)
    {
        await _page.GotoAsync(url);
    }
    #endregion

    #region Step 1: Personal Information Actions
    public async Task FillPersonalInformationAsync(string firstName, string lastName, string email)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await EmailInput.FillAsync(email);
    }

    public async Task ProceedToNextStepAsync()
    {
        await NextButton.ClickAsync();
    }
    #endregion

    #region Step 2: Professional Information Actions
    public async Task SelectProfessionalTypeAsync()
    {
        await ProfessionalButton.ClickAsync();
    }

    public async Task FillProfessionalDetailsAsync(
        string industry, 
        string companySize, 
        string jobFunction)
    {
        await IndustryDropdown.SelectOptionAsync(industry);
        await CompanyDropdown.SelectOptionAsync(companySize);
        await JobDropdown.SelectOptionAsync(jobFunction);
    }

    public async Task FillProjectInformationAsync(string projectType, string projectDescription)
    {
        await ProjectDropdown.SelectOptionAsync(projectType);
        await ProjectTextbox.FillAsync(projectDescription);
    }
    #endregion

    #region Step 3: Consent & Submission Actions
    public async Task AcceptConsentsAsync(bool receiveUpdates = true, bool agreeToTerms = true)
    {
        if (receiveUpdates)
            await UpdatesCheckbox.CheckAsync();
        
        if (agreeToTerms)
            await AgreementCheckbox.CheckAsync();
    }

    public async Task SubmitWaitlistFormAsync()
    {
        await JoinWaitlistButton.ClickAsync();
    }
    #endregion

    #region High-Level Workflow Methods
    public async Task CompleteWaitlistRegistrationAsync(
        string firstName,
        string lastName,
        string email,
        string industry,
        string companySize,
        string jobFunction,
        string projectType,
        string projectDescription,
        bool receiveUpdates = true,
        bool agreeToTerms = true)
    {
        await FillPersonalInformationAsync(firstName, lastName, email);
        await ProceedToNextStepAsync();
        await SelectProfessionalTypeAsync();
        await FillProfessionalDetailsAsync(industry, companySize, jobFunction);
        await FillProjectInformationAsync(projectType, projectDescription);
        await AcceptConsentsAsync(receiveUpdates, agreeToTerms);
        await SubmitWaitlistFormAsync();
    }
    #endregion

    #region Verification Methods
    public async Task<bool> IsNextButtonVisibleAsync() => 
        await NextButton.IsVisibleAsync();

    public async Task<bool> IsSubmitButtonEnabledAsync() => 
        await JoinWaitlistButton.IsEnabledAsync();
    
    public async Task<string> GetEmailValidationErrorAsync() =>
        await _page.Locator("#Email-Address-error").TextContentAsync() ?? string.Empty;
    #endregion

    #region Private Helper Methods
    private void SetupDialogHandler()
    {
        _page.Dialog += async (_, dialog) =>
        {
            DialogMessage = dialog.Message;
            await dialog.DismissAsync();
        };
    }
    #endregion
}