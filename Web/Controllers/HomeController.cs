using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DL;
using Entity;
using Selenium.Tests;


namespace Web.Controllers
{
    public class HomeController : Controller
    {
        DAmazon dAmazon = new DAmazon();
        AmazonTest amzAutomation = new AmazonTest();
        string currentASIN = "";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RunTracker(string asin)
        {
            asin = "B07Z93JQS6";
            AmazonTest tst = new AmazonTest();
            //CSObject obj = tst.test("B07MWLD3VX");
            //List <MainASINs> lstmainASINs = dAmazon.getEnabledASINs();

            //foreach (MainASINs mainASINs in lstmainASINs)
            //{
            try
            {

                //currentASIN = mainASINs.ASIN;
                CSObject objCountStock = amzAutomation.test(asin);

                if (objCountStock != null)
                {
                    dAmazon.InsertTrackingRecord(objCountStock);
                }
                //System.Threading.Thread.Sleep(10 * 1000);
            }
            catch (Exception ex)
            {

            }


            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}