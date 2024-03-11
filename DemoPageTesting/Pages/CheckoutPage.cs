using NUnit.Framework;
using OpenQA.Selenium;

namespace DemoPageTesting.Pages
{
    public class CheckoutPage
    {
        private IWebDriver driver;

        public CheckoutPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        IWebElement FirstNameCheckout => driver.FindElement(By.Id("first-name"));
        IWebElement LastNameCheckout => driver.FindElement(By.Id("last-name"));
        IWebElement PostalCodeCheckout => driver.FindElement(By.Id("postal-code"));
        IWebElement ErrorField => driver.FindElement(By.XPath("//div[@class='error-message-container error']/h3"));
        IWebElement ErrorButton => driver.FindElement(By.ClassName("error-button"));
        IWebElement CancelButton => driver.FindElement(By.Id("cancel"));
        IWebElement ContinueButton => driver.FindElement(By.Id("continue"));
        IWebElement FinishButton => driver.FindElement(By.Id("finish"));
        IWebElement BackToProductsButton => driver.FindElement(By.Id("back-to-products"));
        IWebElement PaymentInfo => driver.FindElement(By.XPath("//div[text()='Payment Information']"));
        IWebElement ShippingInfo => driver.FindElement(By.XPath("//div[text()='Shipping Information']"));
        IWebElement PriceTotal => driver.FindElement(By.XPath("//div[text()='Price Total']"));
        IWebElement Tax => driver.FindElement(By.ClassName("summary_tax_label"));
        IWebElement CompleteHeader => driver.FindElement(By.ClassName("complete-header"));
        IWebElement CompleteDetails => driver.FindElement(By.ClassName("complete-text"));

        public void EnterCheckoutData(string name, string lastName, string postalCode)
        {
            FirstNameCheckout.SendKeys(name);
            LastNameCheckout.SendKeys(lastName);
            PostalCodeCheckout.SendKeys(postalCode);
        }

        public bool VerifyErrorCheckout(string expectedError)
        {
            return ErrorField.Text == expectedError && ErrorField.Displayed && ErrorButton.Displayed;

        }

        public bool ErrorMessageClosed()
        {
            Assert.That(ErrorButton.Displayed);
            ErrorButton.Click();
            try
            {
                return !ErrorField.Displayed && !ErrorButton.Displayed;
            }
            catch (NoSuchElementException)
            {
                return true;
            }
        }

        public void ClickContinue()
        {
            ContinueButton.Click();
        }

        public void ClickCancel()
        {
            CancelButton.Click();
        }

        public void ClickFinish()
        {
            FinishButton.Click();
        }

        public void ClickBackHome()
        {
            BackToProductsButton.Click();
        }


        public bool CheckoutPageDisplayed(string pageName)
        {
            try
            {
                IWebElement CheckoutPageTitle = driver.FindElement(By.XPath($"//span[text()='Checkout: {pageName}']"));
                return CheckoutPageTitle.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool VerifyOverviewPageDetails()
        {
            return PaymentInfo.Displayed && ShippingInfo.Displayed && PriceTotal.Displayed && Tax.Displayed;
        }

        public bool VerifyCompletedActionPage()
        {
            var completeHeaderText = CompleteHeader.Text;
            var expectedHeaderText = "Thank you for your order!";
            var completeDetailsText = CompleteDetails.Text;
            var expectedDetailsText = "Your order has been dispatched, and will arrive just as fast as the pony can get there!";
            return CompleteHeader.Displayed && CompleteDetails.Displayed && completeDetailsText == expectedDetailsText && completeHeaderText == expectedHeaderText;
        }

        public double GetItemPrice(string itemName)
        {
            IWebElement PriceItemElement = driver.FindElement(By.XPath($"//div[text()='{itemName}' and @class='inventory_item_name']/ancestor::div[1]//div[@class='inventory_item_price']"));
            var price = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].childNodes[1].textContent;", PriceItemElement);
            return double.Parse(price);
        }

        public double GetTaxPrice()
        {
            IWebElement TaxPriceElement = driver.FindElement(By.ClassName("summary_tax_label"));
            var taxPprice = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].childNodes[1].textContent;", TaxPriceElement);
            return double.Parse(taxPprice);
        }

        public bool VerifyCorrectTotalPrice(double itemOnePrice, double itemTwoPrice, double taxPrice, double expectedPrice)
        {
            return itemOnePrice + itemTwoPrice + taxPrice == expectedPrice;
        }
    }
}
