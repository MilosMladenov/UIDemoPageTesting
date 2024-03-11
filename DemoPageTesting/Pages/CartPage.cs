using OpenQA.Selenium;

namespace DemoPageTesting.Pages
{
    public class CartPage
    {
        private IWebDriver driver;

        public CartPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        IWebElement CartList => driver.FindElement(By.ClassName("cart_list"));
        IWebElement ContinueShoppingButton => driver.FindElement(By.Id("continue-shopping"));
        IWebElement CheckoutButton => driver.FindElement(By.Id("checkout"));


        public bool ItemAdded(string itemNameExpected)
        {
            try
            {
                IWebElement ItemInCart = CartList.FindElement(By.ClassName("cart_item"));
                IWebElement InventoryItemName = ItemInCart.FindElement(By.XPath($"//div[@class='inventory_item_name' and contains(text(), '{itemNameExpected}')]"));
                var nameOfTheItem = InventoryItemName.Text;
                return nameOfTheItem == itemNameExpected && InventoryItemName.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveFromCartAndVerify(string itemName)
        {
            try
            {
                IWebElement ItemInCart = CartList.FindElement(By.ClassName("cart_item"));
                IWebElement RemoveButton = ItemInCart.FindElement(By.XPath($"//div[@class='inventory_item_name' and contains(text(), '{itemName}')]/ancestor::div[2]//button[contains(text(), 'Remove')]"));
                RemoveButton.Click();
                return RemoveButton.Displayed;
            }
            catch (StaleElementReferenceException)
            {
                return true;
            }
        }

        public bool VerifyCartPage()
        {
            try
            {
                IWebElement CartPage = driver.FindElement(By.XPath("//span[text()='Your Cart']"));
                return CartPage.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void ContinueShopping()
        {
            ContinueShoppingButton.Click();
        }

        public void GoToCheckout()
        {
            CheckoutButton.Click();
        }
    }
}
