namespace DemoPageTesting.Tests;

using DemoPageTesting.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

[TestFixture]
public class LoginTests
{
    private IWebDriver driver;

    private LoginPage loginPage;

    private ItemsPage itemsPage;

    private const string SITE_URL = "https://www.saucedemo.com";
    private const string CORRECT_PASSWORD = "secret_sauce";


    [SetUp]
    public void Setup()
    {
        driver = new ChromeDriver();
        driver.Navigate().GoToUrl(SITE_URL);
        driver.Manage().Window.Maximize();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        loginPage = new LoginPage(driver);
        itemsPage = new ItemsPage(driver);
    }

    [TestCase("standard_user")]
    [TestCase("problem_user")]
    [TestCase("error_user")]
    [TestCase("visual_user")]
    [TestCase("performance_glitch_user")]
    public void LoginSuccessfully(string userName)
    {
        Assert.That(loginPage.LoginFieldsDisplayed(), "Login fields are not displayed.");
        loginPage.LogIn(userName, CORRECT_PASSWORD);
        Assert.That(!loginPage.LoginFieldsDisplayed(), "Login fields are displayed.");
        Assert.That(itemsPage.VerifyMainPage(), "Main page not loaded as expected.");
    }

    [TestCase("unexisting_user", CORRECT_PASSWORD, "Epic sadface: Username and password do not match any user in this service")]
    [TestCase("", CORRECT_PASSWORD, "Epic sadface: Username is required")]
    [TestCase("standard_user", "", "Epic sadface: Password is required")]
    [TestCase("locked_out_user", CORRECT_PASSWORD, "Epic sadface: Sorry, this user has been locked out.")]
    public void LoginUnsuccessfully(string userName, string password, string error)
    {
        loginPage.LogIn(userName, password);
        Assert.That(loginPage.LoginErrorDisplayed(error), "Login Error not displayed.");
    }

    [Test]
    public void CloseLoginErrorMessage()
    {
        loginPage.LogIn("asd", "123");
        Assert.That(loginPage.LoginErrorDisplayed("Epic sadface: Username and password do not match any user in this service"), "Expected login error not displayed.");
        Assert.That(loginPage.ErrorMessageClosed(), "Error message is not closed.");
    }

    [Test]
    public void Logout()
    {
        Assert.That(loginPage.LoginFieldsDisplayed(), "Login fields are not displayed.");
        loginPage.LogIn("standard_user", CORRECT_PASSWORD);
        Assert.That(!loginPage.LoginFieldsDisplayed(), "Login fields are displayed.");
        Assert.That(itemsPage.VerifyMainPage(), "Main page not loaded as expected.");
        itemsPage.OpenLeftPannelMenu();
        itemsPage.NavigateFromLeftPannel("Logout");
        Assert.That(loginPage.LoginFieldsDisplayed(), "Login fields are not displayed.");
        Assert.That(!itemsPage.VerifyMainPage(), "Main page is still present.");
    }

    [TearDown]
    public void Teardown()
    {
        driver.Quit();
    }
}

