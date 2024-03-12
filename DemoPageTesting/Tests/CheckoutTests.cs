using DemoPageTesting.Pages;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace DemoPageTesting.Tests
{
    [TestFixture, Parallelizable(ParallelScope.Fixtures)]
    public class CheckoutTests
    {
        private IWebDriver driver;

        private LoginPage loginPage;

        private CartPage cartPage;

        private ItemsPage itemsPage;

        private CheckoutPage checkoutPage;

        private const string SITE_URL = "https://www.saucedemo.com";

        private const string SAUCE_BACKPACK = "Sauce Labs Backpack";
        private const string BIKE_LIGHT = "Sauce Labs Bike Light";

        private const string ALL_ITEMS = "All Items";
        private const string RESET_APP_STATE = "Reset App State";

        private const string YOUR_INFO = "Your Information";
        private const string OVERVIEW = "Overview";
        private const string COMPLETE = "Complete!";

        [OneTimeSetUp]
        public void Setup()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl(SITE_URL);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            loginPage = new LoginPage(driver);
            itemsPage = new ItemsPage(driver);
            cartPage = new CartPage(driver);
            checkoutPage = new CheckoutPage(driver);
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

        [TestCase("Walter", "White", "", "Error: Postal Code is required")]
        [TestCase("", "White", "12345", "Error: First Name is required")]
        [TestCase("Walter", "", "12345", "Error: Last Name is required")]
        public void LeaveEmptyFieldsAndVerifyError(string firstName, string lastName, string postalCode, string error)
        {
            Assert.That(itemsPage.VerifyMainPage(), "Main page not loaded as expected.");
            itemsPage.AddToCart(SAUCE_BACKPACK);
            itemsPage.GoToCart();
            Assert.That(cartPage.VerifyCartPage(), "Cart page not loaded as expected.");
            Assert.That(cartPage.ItemAdded(SAUCE_BACKPACK), "Item not added in cart.");
            cartPage.GoToCheckout();
            Assert.That(checkoutPage.CheckoutPageDisplayed(YOUR_INFO), "Checkout page not loaded as expected.");
            checkoutPage.EnterCheckoutData(firstName, lastName, postalCode);
            checkoutPage.ClickContinue();
            Assert.That(checkoutPage.VerifyErrorCheckout(error), "Error not displayed.");
            Assert.That(checkoutPage.ErrorMessageClosed(), "Error message is still present.");
        }

        [Test]
        public void FinishCheckoutSuccessfully()
        {
            Assert.That(itemsPage.VerifyMainPage(), "Main page not loaded as expected.");
            itemsPage.AddToCart(SAUCE_BACKPACK);
            itemsPage.GoToCart();
            Assert.That(cartPage.VerifyCartPage(), "Cart page not loaded as expected.");
            Assert.That(cartPage.ItemAdded(SAUCE_BACKPACK), "Item not added in cart.");
            cartPage.GoToCheckout();
            Assert.That(checkoutPage.CheckoutPageDisplayed(YOUR_INFO), "Checkout page not loaded as expected.");
            checkoutPage.EnterCheckoutData("Walter", "White", "12345");
            checkoutPage.ClickContinue();
            Assert.That(checkoutPage.CheckoutPageDisplayed(OVERVIEW), "Checkout page not loaded as expected.");
            Assert.That(checkoutPage.VerifyOverviewPageDetails(), "Overview page elements not present.");
            checkoutPage.ClickFinish();
            Assert.That(checkoutPage.CheckoutPageDisplayed(COMPLETE), "Checkout page not loaded as expected.");
            Assert.That(checkoutPage.VerifyCompletedActionPage(), "Complete page elements not present.");
            checkoutPage.ClickBackHome();
            Assert.That(!checkoutPage.CheckoutPageDisplayed(COMPLETE), "Checkout page is still loaded.");
            Assert.That(itemsPage.VerifyMainPage(), "Main page not loaded as expected.");
        }

        [Test]
        public void CheckIfTotalPriceIsCorrect()
        {
            Assert.That(itemsPage.VerifyMainPage(), "Main page not loaded as expected.");
            itemsPage.AddToCart(SAUCE_BACKPACK);
            itemsPage.AddToCart(BIKE_LIGHT);
            itemsPage.GoToCart();
            Assert.That(cartPage.VerifyCartPage(), "Cart page not loaded as expected.");
            Assert.That(cartPage.ItemAdded(SAUCE_BACKPACK), "Item not added in cart.");
            Assert.That(cartPage.ItemAdded(BIKE_LIGHT), "Item not added in cart.");
            cartPage.GoToCheckout();
            Assert.That(checkoutPage.CheckoutPageDisplayed(YOUR_INFO), "Checkout page not loaded as expected.");
            checkoutPage.EnterCheckoutData("Walter", "White", "12345");
            checkoutPage.ClickContinue();
            Assert.That(checkoutPage.CheckoutPageDisplayed(OVERVIEW), "Checkout page not loaded as expected.");
            Assert.That(checkoutPage.VerifyOverviewPageDetails(), "Overview page elements not present.");
            var priceItemOne = checkoutPage.GetItemPrice(BIKE_LIGHT);
            var priceItemTwo = checkoutPage.GetItemPrice(SAUCE_BACKPACK);
            var priceTax = checkoutPage.GetTaxPrice();
            Assert.That(checkoutPage.VerifyCorrectTotalPrice(priceItemOne, priceItemTwo, priceTax, 43.18), "The total price is not correct.");
        }

        [Test]
        public void CancelCheckoutAndVerifyPage()
        {
            Assert.That(itemsPage.VerifyMainPage(), "Main page not loaded as expected.");
            itemsPage.AddToCart(SAUCE_BACKPACK);
            itemsPage.GoToCart();
            Assert.That(cartPage.VerifyCartPage(), "Cart page not loaded as expected.");
            Assert.That(cartPage.ItemAdded(SAUCE_BACKPACK), "Item not added in cart.");
            cartPage.GoToCheckout();
            Assert.That(checkoutPage.CheckoutPageDisplayed(YOUR_INFO), "Checkout page not loaded as expected.");
            checkoutPage.EnterCheckoutData("Walter", "White", "12345");
            checkoutPage.ClickContinue();
            Assert.That(checkoutPage.CheckoutPageDisplayed(OVERVIEW), "Checkout page not loaded as expected.");
            checkoutPage.ClickCancel();
            Assert.That(!checkoutPage.CheckoutPageDisplayed(OVERVIEW), "Checkout page still present.");
            Assert.That(itemsPage.VerifyMainPage(), "Main page not loaded as expected.");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            driver.Quit();
        }
    }
}
