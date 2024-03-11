namespace DemoPageTesting.Tests;
using DemoPageTesting.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

[TestFixture, Parallelizable(ParallelScope.Fixtures)]
public class AddingItemsTests
{
    private IWebDriver driver;

    private LoginPage loginPage;

    private CartPage cartPage;

    private ItemsPage itemsPage;

    private const string SITE_URL = "https://www.saucedemo.com";

    private const string SAUCE_BACKPACK = "Sauce Labs Backpack";
    private const string BOLT_TSHIRT = "Sauce Labs Bolt T-Shirt";
    private const string FLEECE_JACKET = "Sauce Labs Fleece Jacket";
    private const string BIKE_LIGHT = "Sauce Labs Bike Light";

    private const string ALL_ITEMS = "All Items";
    private const string RESET_APP_STATE = "Reset App State";

    [OneTimeSetUp]
    public void Setup()
    {
        driver = new ChromeDriver();
        driver.Navigate().GoToUrl(SITE_URL);
        driver.Manage().Window.Maximize();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        loginPage = new LoginPage(driver);
        itemsPage = new ItemsPage(driver);
        cartPage = new CartPage(driver);
        loginPage.LogIn("standard_user", "secret_sauce");
    }

    [SetUp]
    public void AppStateReset()
    {
        itemsPage.OpenLeftPannelMenu();
        itemsPage.NavigateFromLeftPannel(RESET_APP_STATE);
        itemsPage.NavigateFromLeftPannel(ALL_ITEMS);
        driver.Navigate().Refresh();
    }

    [Test]
    public void SimplyAddToCart()
    {
        Assert.That(itemsPage.VerifyMainPage(), "Main page not loaded as expected.");
        itemsPage.AddToCart(SAUCE_BACKPACK);
        itemsPage.GoToCart();
        Assert.That(cartPage.VerifyCartPage(), "Cart page not loaded as expected.");
        Assert.That(cartPage.ItemAdded(SAUCE_BACKPACK), "Item is not present in cart.");
    }

    [Test]
    public void AddMultipleItems()
    {
        itemsPage.AddToCart(SAUCE_BACKPACK);
        itemsPage.AddToCart(BIKE_LIGHT);
        itemsPage.GoToCart();
        Assert.That(cartPage.VerifyCartPage(), "Cart page not loaded as expected.");
        Assert.That(cartPage.ItemAdded(SAUCE_BACKPACK), "Item is not present in cart.");
        Assert.That(cartPage.ItemAdded(BIKE_LIGHT), "Item is not present in cart.");
    }

    [Test]
    public void RemoveMultipeItemsMainPage()
    {
        itemsPage.AddToCart(SAUCE_BACKPACK);
        itemsPage.AddToCart(BIKE_LIGHT);
        Assert.That(itemsPage.RemoveItemFromMainPageAndVerify(SAUCE_BACKPACK), "Item not removed from cart.");
        Assert.That(itemsPage.RemoveItemFromMainPageAndVerify(BIKE_LIGHT), "Item not removed from cart.");
    }

    [Test]
    public void VerifyCartNumber()
    {
        itemsPage.AddToCart(SAUCE_BACKPACK);
        itemsPage.AddToCart(BIKE_LIGHT);
        Assert.That(itemsPage.VerifyCartNumberIsCorrect("2"), "Number of items added is not correct.");
        Assert.That(itemsPage.RemoveItemFromMainPageAndVerify(BIKE_LIGHT), "Item not removed from cart.");
        Assert.That(itemsPage.VerifyCartNumberIsCorrect("1"), "Number of items added is not correct.");
    }

    [Test]
    public void ContinueShopping()
    {
        Assert.That(itemsPage.VerifyMainPage(), "Main page not loaded as expected.");
        itemsPage.GoToCart();
        Assert.That(!itemsPage.VerifyMainPage(), "Main page is still present.");
        Assert.That(cartPage.VerifyCartPage(), "Cart page not loaded as expected.");
        cartPage.ContinueShopping();
        Assert.That(itemsPage.VerifyMainPage(), "Main page not loaded as expected.");
        Assert.That(!cartPage.VerifyCartPage(), "Cart page is still present.");
    }

    [Test]
    public void RemoveItemsFromCart()
    {
        itemsPage.AddToCart(FLEECE_JACKET);
        itemsPage.AddToCart(BOLT_TSHIRT);
        itemsPage.GoToCart();
        Assert.That(cartPage.VerifyCartPage(), "Cart page not loaded as expected.");
        Assert.That(cartPage.ItemAdded(FLEECE_JACKET), "Item is not present in cart.");
        Assert.That(cartPage.ItemAdded(BOLT_TSHIRT), "Item is not present in cart.");
        Assert.That(itemsPage.VerifyCartNumberIsCorrect("2"), "Number of items added is not correct.");
        Assert.That(cartPage.RemoveFromCartAndVerify(FLEECE_JACKET), "Item not removed from cart.");
        Assert.That(itemsPage.VerifyCartNumberIsCorrect("1"), "Number of items added is not correct.");
        Assert.That(!cartPage.ItemAdded(FLEECE_JACKET), "Item is still present in cart.");
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        driver.Quit();
    }
}
