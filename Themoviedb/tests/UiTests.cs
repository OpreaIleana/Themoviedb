using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;


namespace Themoviedb.tests
{
    public class UiTests

    {
        // TODO: Create a Base Class for UI tests and a Util class with all the methods used frequently
        // TODO: Create a Constants file with all the urls, XPaths, ids 
        // TODO: Replace XPaths with ids, replace hardcoded numbers

        private ChromeDriver driver;

        const string url = "https://www.themoviedb.org/movie";

        private void NavigateTo(string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        private void ClickOnElement(IWebElement element)
        {
            element.Click();
        }

        private void ClickOnElementById(string id)
        {
            driver.FindElement(By.Id(id)).Click();
        }

        private IWebElement FindElementByClassName(string className)
        {
            return driver.FindElement(By.ClassName(className));
        }

        private IWebElement FindElementByClassNameAndTagName(string className, string tagName)
        {
            return driver.FindElement(By.ClassName(className)).FindElement(By.TagName(tagName));
        }

        private IWebElement FindElementById(string id)
        {
            return driver.FindElement(By.Id(id));
        }

        private IWebElement FindElementByXPath(string xPath)
        {
            return driver.FindElement(By.XPath(xPath));
        }

        private void SelectSortOptionNr(IWebElement element, int number)
        {
            for (int i = 1; i < number; i++)
            {
                element.SendKeys(Keys.ArrowDown);
            }
            element.SendKeys(Keys.Enter);
        }

        private void DeleteDateInATextBox(IWebElement element, int nrOfChar)
        {
            for (int i = 0; i < nrOfChar; i++)
            {
                element.SendKeys(Keys.Backspace);
            }
        }

        // Assert methods

        private void AssertElementByXPathIsDisplayed(string xPath)
        {
            Assert.IsTrue(driver.FindElement(By.XPath(xPath)).Displayed);
        }

        private void AssertElementContainsTextIsTrue(IWebElement element, string text)
        {
            Assert.That(element.Text, Does.Contain(text));
        }

        private void ImplicitWaitForElement(int seconds)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(seconds);
        }

        [SetUp]

        public void Setup()

        {
            // Get the drivers folder path dynamically
            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

            // Creates the ChomeDriver object, Executes tests on Google Chrome
            driver = new ChromeDriver(path + @"\drivers\");

            //driver.Manage().Window.Maximize();
        }

        [Test]
        [Description("Test filtering the movies by ascending release date and check the results")]

        public void FilterByAscendingReleaseDateAndCheckResults()

        {
            NavigateTo(url);
            AssertElementByXPathIsDisplayed("//*[@id=\"media_v4\"]/div/div/div[2]/div[1]/div[1]/div[1]/h2");

            // Close Cookies
            ClickOnElementById("onetrust-close-btn-container");

            // Expand Sort section
            IWebElement sortButton = FindElementByClassNameAndTagName("name", "h2");
            AssertElementContainsTextIsTrue(sortButton, "Sort");
            ClickOnElement(sortButton);

            // Expand Sort options dropdown list and select ascending release date sorting option
            IWebElement dropdown = FindElementByXPath("//*[@id=\"media_v4\"]/div/div/div[2]/div[1]/div[1]/div[2]/span");
            ClickOnElement(dropdown);
            SelectSortOptionNr(dropdown, 6);

            IWebElement search = FindElementByClassName("load_more");
            ClickOnElement(search);

            // Waiting for the page to load with new results
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.TagName("p")).Displayed);
            AssertElementByXPathIsDisplayed("//*[@id=\"media_v4\"]/div/div/div[2]/div[1]/div[1]/div[1]/h2");

            // Check results
            IWebElement parentResults = FindElementById("media_results");
            ReadOnlyCollection<IWebElement> results = parentResults.FindElements(By.ClassName("content"));
            Assert.That(actual: results.Count, Is.EqualTo(20));

            // Create an array with half of results from the first page and extract the release date 
            DateTime[] dates = new DateTime[results.Count / 2];

            for (int i = 0; i < dates.Length; i++)
            {
                var date = DateTime.Parse(results[i].Text.Substring(results[i].Text.Length - 12, 12));
                dates[i] = date;
            }

            // Assert that the sorting is in ascending order
            for (int i = 1; i < dates.Length; i++)
            {
                Assert.That(dates[i - 1] <= dates[i]);

            }

        }

        [Test]
        [Description("Test selecting generes and click on search")]

        public void SelectGenres()

        {
            NavigateTo(url);
            ImplicitWaitForElement(5);

            // Close cookies
            ClickOnElementById("onetrust-close-btn-container");

            // Find one of the generes element, move the click because the element is not clickable
            // TODO: Raise a bug
            IWebElement generes = FindElementByXPath("//*[@id=\"with_genres\"]/li[2]");
            Actions actions = new(driver);
            actions.MoveToElement(generes, 3, 0).Build().Perform();
            generes.Click();
            ImplicitWaitForElement(5);

            // Click on search
            ClickOnElement(FindElementByClassName("load_more"));
        }

        [Test]
        [Description("Test searching by release date and check the results")]

        public void SearchByReleaseDateAndCheckResults()

        {
            var dateFrom = DateTime.Parse("01/01/1995 12:00:00 AM");
            var dateTo = DateTime.Parse("01/01/2005 12:00:00 AM");

            NavigateTo(url);
            ImplicitWaitForElement(5);

            // Close cookies
            ClickOnElementById("onetrust-close-btn-container");

            // Find search from and search to elements, move the click because the element is not clickable
            IWebElement searchFrom = FindElementById("release_date_gte");
            IWebElement searchTo = FindElementById("release_date_lte");
            Actions actions = new(driver);
            actions.MoveToElement(searchFrom, 2, 0).Build().Perform();
            actions.MoveToElement(searchTo, 2, 0).Build().Perform();
            ClickOnElement(searchFrom);
            searchFrom.SendKeys("1/1/1995");
            ClickOnElement(searchTo);
            DeleteDateInATextBox(searchTo, 8);
            searchTo.SendKeys("1/1/2005");

            IWebElement search = FindElementByClassName("load_more");
            search.Click();

            // Waiting for the page to load with new results
            WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.TagName("p")).Displayed);
            AssertElementByXPathIsDisplayed("//*[@id=\"media_v4\"]/div/div/div[2]/div[1]/div[1]/div[1]/h2");

            // Check the results
            IWebElement parentResults = FindElementById("media_results");
            ReadOnlyCollection<IWebElement> results = parentResults.FindElements(By.ClassName("content"));
            Assert.That(actual: results.Count, Is.EqualTo(20));

            // Create an array with half of results from the first page and extract the release date 
            DateTime[] dates = new DateTime[results.Count / 2];

            for (int i = 0; i < dates.Length; i++)
            {
                var date = DateTime.Parse(results[i].Text.Substring(results[i].Text.Length - 12, 12));
                dates[i] = date;
            }

            // Check if the release date is between search from and search to date
            for (int i = 0; i < dates.Length; i++)
            {
                Assert.That(dates[i] >= dateFrom && dates[i] <= dateTo);
            }

        }

        [TearDown]

        public void TearDown()

        {
            driver.Quit();
            driver.Dispose();
        }

    }

}