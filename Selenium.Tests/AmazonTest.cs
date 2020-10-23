using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Entity;
using NLog;

namespace Selenium.Tests
{
    [TestClass]
    public class AmazonTest
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        IWebDriver driver;
        string applicationPath = System.Configuration.ConfigurationManager.AppSettings["ApplicationPath"];
        public AmazonTest()
        {

        }

        [SetUp]
        public void startBrowser()
        {
            try
            {
                var options = new ChromeOptions();
                options.AddExtensions($@"{applicationPath}\drivers\cmjihoeplpkmlmbbiiognkceoechmand.crx");
                options.AddArgument("no-sandbox");
                driver = new ChromeDriver($@"{applicationPath}\drivers", options, TimeSpan.FromSeconds(130));
                _logger.Info("WebDriver Setup conpleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        [Test]
        public CSObject test(string ASIN)
        {
            _logger.Info($"Running test for ASIN {ASIN}");
            try
            {
                var rtn = runAutomation(ASIN);
                _logger.Info("Test completed successfully.");
                return rtn;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }

        }

        private CSObject runAutomation(string ASIN)
        {
            startBrowser();
            CSObject _objCSObject = new CSObject();
            driver.Url = "https://www.amazon.ae/dp/" + ASIN;
            if (IsElementPresent(By.Id("scxt-stock-btn")))
            {
                //IWebElement csButton = driver.FindElement(By.Id("scxt-stock-btn"));
                WebDriverWait csButtonWait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                IWebElement csButton = csButtonWait.Until(ExpectedConditions.ElementIsVisible(By.Id("scxt-stock-btn")));
                csButton.Click();

                WebDriverWait csWindowWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IWebElement csWindow = csWindowWait.Until(ExpectedConditions.ElementIsVisible(By.Id("scxt-widget")));
                if (csWindow.Displayed)
                {

                    IWebElement iFrame = csWindow.FindElement(By.TagName("iframe"));
                    driver.SwitchTo().Frame(iFrame);

                    SleepRecusrive();

                    LoadMoreOffersRecusrive();

                    if (Check503ServerErrorFromAddon())
                    {
                        closeBrowser();
                        runAutomation(ASIN);
                    }

                    IWebElement TotalCountelement = driver.FindElement(By.ClassName("counts-total"));
                    string TotalCount = TotalCountelement.Text;

                    _objCSObject.ASIN = ASIN;
                    _objCSObject.TotalStockCount = TotalCountelement.Text.Split(':')[1].Replace(",", "").Replace("+", "").Trim();


                    IList<IWebElement> stockCountsSegregated = driver.FindElements(By.ClassName("counts-item"));
                    foreach (var stockType in stockCountsSegregated)
                    {
                        IList<IWebElement> stockCountByType = stockType.FindElements(By.TagName("div"));
                        if (stockCountByType[0].Text.ToLower().Contains("fba"))
                        {
                            _objCSObject.totalFBAStock = (stockCountByType[1]?.Text != "") ? stockCountByType[1].Text.Replace(",", "").Replace("+", "") : "";
                        }
                        if (stockCountByType[0].Text.ToLower().Contains("fbm"))
                        {
                            _objCSObject.totalFBMStock = (stockCountByType[1]?.Text != "") ? stockCountByType[1].Text.Replace(",", "").Replace("+", "") : "";
                        }
                        if (stockCountByType[0].Text.ToLower().Contains("amz"))
                        {
                            _objCSObject.totalAMZStock = (stockCountByType[1]?.Text != "") ? stockCountByType[1].Text.Replace(",", "").Replace("+", "") : "";
                        }
                    }

                    IWebElement resultsElement = driver.FindElement(By.Id("results"));

                    IList<IWebElement> tableRow = resultsElement.FindElements(By.TagName("tr"));
                    IList<IWebElement> columns;

                    foreach (IWebElement row in tableRow.Skip(1))
                    {
                        CSSellersObject _sellerObject = new CSSellersObject();
                        columns = row.FindElements(By.TagName("td"));
                        _sellerObject.SellerName = columns[0].Text;
                        _sellerObject.Quantity = (columns[1]?.Text != "") ? columns[1].Text : "";
                        _sellerObject.Price = columns[2].Text.Replace(",", "").Replace("+", "");
                        _sellerObject.Type = columns[3].Text;

                        if (columns[4].Text != "")
                        {
                            string colVal = "";
                            colVal = columns[4].Text.Replace("\r\n", "_");
                            string[] retVal = colVal.Split('_');
                            if (retVal.Length > 0)
                            {
                                _sellerObject.Rating = retVal[0];
                                if (retVal.Length > 1)
                                {
                                    _sellerObject.Reviews = retVal[1];
                                }
                            }
                        }

                        _sellerObject.Condition = columns[5].Text;
                        _objCSObject.SellerDetails.Add(_sellerObject);
                    }
                    _objCSObject.NoOfSellers = _objCSObject.SellerDetails.Count.ToString();
                }
            }
            closeBrowser();
            return _objCSObject;
        }



        [TearDown]
        public void closeBrowser()
        {
            driver.Close();
        }

        private void SleepRecusrive()
        {

            IJavaScriptExecutor _jsExecutor;
            _jsExecutor = driver as IJavaScriptExecutor;

            string _jsReturnedVal;
            _jsReturnedVal = _jsExecutor
            .ExecuteScript(
                "return $('.spinner').is(':visible')")
            .ToString();

            if (_jsReturnedVal.ToLower() == "true")
            {
                Thread.Sleep(2 * 1000);
                SleepRecusrive();
            }
        }

        private bool Check503ServerErrorFromAddon()
        {

            IJavaScriptExecutor _jsExecutor;
            _jsExecutor = driver as IJavaScriptExecutor;

            string _jsReturnedVal;
            _jsReturnedVal = _jsExecutor
            .ExecuteScript(
                "return $('.status').is(':visible')")
            .ToString();

            return _jsReturnedVal.ToLower() == "true";
        }

        private void LoadMoreOffersRecusrive()
        {

            IJavaScriptExecutor _jsExecutor;
            _jsExecutor = driver as IJavaScriptExecutor;

            string _jsReturnedVal;
            _jsReturnedVal = _jsExecutor
            .ExecuteScript(
                "return $('.btn-load-more').is(':visible')")
            .ToString();

            if (_jsReturnedVal.ToLower() == "true")
            {
                IWebElement btnLoadMore = driver.FindElement(By.ClassName("btn-load-more"));
                btnLoadMore.Click();
                SleepRecusrive();
                LoadMoreOffersRecusrive();
            }
        }

        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
