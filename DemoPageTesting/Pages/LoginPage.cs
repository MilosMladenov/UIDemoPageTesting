using NUnit.Framework;
using OpenQA.Selenium;

namespace DemoPageTesting.Pages
{
    public class LoginPage
    {
        private IWebDriver driver;

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
        }
        IWebElement UsernameField => driver.FindElement(By.Id("user-name"));
        IWebElement PasswordField => driver.FindElement(By.Id("password"));
        IWebElement LoginButton => driver.FindElement(By.Id("login-button"));
        IWebElement LoginError => driver.FindElement(By.CssSelector("#login_button_container > div > form > div.error-message-container.error > h3"));
        IWebElement LoginErrorButton => LoginError.FindElement(By.CssSelector("#login_button_container > div > form > div.error-message-container.error > h3 > button"));

        public void LogIn(string username, string passowrd)
        {
            UsernameField.SendKeys(username);
            PasswordField.SendKeys(passowrd);
            LoginButton.Click();
        }

        public bool LoginErrorDisplayed(string error)
        {
            return LoginError.Text == error && LoginError.Displayed && LoginErrorButton.Displayed;
        }

        public bool ErrorMessageClosed()
        {
            Assert.That(LoginErrorButton.Displayed);
            LoginErrorButton.Click();
            try
            {
                return !LoginError.Displayed && !LoginErrorButton.Displayed;
            } catch (NoSuchElementException) 
            {
                return true;
            }
        }

        public bool LoginFieldsDisplayed()
        {
            try
            {
                return UsernameField.Displayed && PasswordField.Displayed && LoginButton.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
