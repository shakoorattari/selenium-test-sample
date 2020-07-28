using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Selenium.Tests
{
    [TestClass]
    public class AmazonTest
    {
        IWebDriver driver;

        [SetUp]
        public void startBrowser()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddExtensions(@"D:\github\selenium-test-sample\drivers\cmjihoeplpkmlmbbiiognkceoechmand.crx");
            driver = new ChromeDriver(@"D:\github\selenium-test-sample\drivers", options);
        }

        [Test]
        public void test()
        {
            driver.Url = "https://www.amazon.ae/";
        }

        [TearDown]
        public void closeBrowser()
        {
            driver.Close();
        }

    }
}
