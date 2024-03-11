using NUnit.Framework;
using OpenQA.Selenium;

namespace DemoPageTesting.Pages
{
    public class ItemsPage
    {
        
        private IWebDriver driver;

        public ItemsPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        IWebElement Cart => driver.FindElement(By.Id("shopping_cart_container"));
        IWebElement CartNumber => Cart.FindElement(By.ClassName("shopping_cart_badge"));
        IWebElement LeftPanelMenu => driver.FindElement(By.Id("react-burger-menu-btn"));
        IWebElement Sorter => driver.FindElement(By.ClassName("product_sort_container"));
        IWebElement MainPage => driver.FindElement(By.XPath("//span[text()='Products']"));


        public bool VerifyMainPage()
        {
            try
            {
                return MainPage.Displayed && Sorter.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        public void AddToCart(string itemName)
        {
            IWebElement AddToCartButton = driver.FindElement(By.XPath($"//div[text()='{itemName}' and @class='inventory_item_name ']/ancestor::div[3]//button[contains(text(), 'Add to cart')]"));
            IWebElement ItemPrice = driver.FindElement(By.XPath($"//div[text()='{itemName}' and @class='inventory_item_name ']/ancestor::div[3]//div[@class='inventory_item_price']"));
            Assert.That(AddToCartButton.Displayed && ItemPrice.Displayed);
            AddToCartButton.Click();
        }

        public bool VerifyCorrectNumberAdded(int numberExpected)
        {
            Assert.That(CartNumber.Displayed);
            var numberStringDisplayed = CartNumber.Text;
            var numberDisplayed = int.Parse(numberStringDisplayed);
            return numberDisplayed == numberExpected;
        }

        public bool RemoveItemFromMainPageAndVerify(string itemName)
        {
            IWebElement RemoveButton = driver.FindElement(By.XPath($"//div[text()='{itemName}' and @class='inventory_item_name ']/ancestor::div[3]//button[contains(text(), 'Remove')]"));
            Assert.That(RemoveButton.Displayed);
            RemoveButton.Click();
            IWebElement AddToCartButton = driver.FindElement(By.XPath($"//div[text()='{itemName}' and @class='inventory_item_name ']/ancestor::div[3]//button[contains(text(), 'Add to cart')]"));
            return AddToCartButton.Displayed;
        }

        public void GoToCart()
        {
            Cart.Click();
        }

        public void OpenLeftPannelMenu()
        {
            LeftPanelMenu.Click();
        }

        public void NavigateFromLeftPannel(string option)
        {
            IWebElement ItemOption = driver.FindElement(By.XPath($"//a[text()='{option}']"));
            ItemOption.Click();
        }

        public bool VerifyCartNumberIsCorrect(string expectedNumber)
        {
            IWebElement CartNumber = driver.FindElement(By.ClassName("shopping_cart_badge"));
            var actualNumber = CartNumber.Text;
            return actualNumber == expectedNumber;
        }
    }
}
