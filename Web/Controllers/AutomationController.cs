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
    public class AutomationController : Controller
    {
        // GET: Automation
        DAmazon dAmazon = new DAmazon();
        AmazonTest amzAutomation = new AmazonTest();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TrackASINs(int groupId)
        {
            List<MainASINs> lstmainASINs = dAmazon.getEnabledASINs(groupId);

            foreach (MainASINs mainASINs in lstmainASINs)
            {
                try
                {
                    CSObject objCountStock = amzAutomation.test(mainASINs.ASIN);

                    if (objCountStock != null)
                    {
                        dAmazon.InsertTrackingRecord(objCountStock);
                    }
                }
                catch (Exception ex)
                {

                }
                System.Threading.Thread.Sleep(2 * 60 *1000); // 1.5 Minute
            }
            return View();
        }

        public ActionResult TrackASINsNoGroup()
        {
            List<MainASINs> lstmainASINs = dAmazon.getEnabledASINs();

            foreach (MainASINs mainASINs in lstmainASINs)
            {
                try
                {
                    CSObject objCountStock = amzAutomation.test(mainASINs.ASIN);

                    if (objCountStock != null)
                    {
                        dAmazon.InsertTrackingRecord(objCountStock);
                    }
                }
                catch (Exception ex)
                {

                }
                System.Threading.Thread.Sleep(2 * 60 * 1000); // 1.5 Minute
            }
            return View();
        }

        public ActionResult testScheduler()
        {
            dAmazon.Insert_tbltest();
            return View();
        }
    }
}