using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DL;
using Entity;
using NLog;
using NLog.Targets;
using Selenium.Tests;


namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        DAmazon dAmazon = new DAmazon();
        AmazonTest amzAutomation = new AmazonTest();
        string currentASIN = "";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RunTracker(string asin)
        {
            try
            {
                int totalGroups = Convert.ToInt32(ConfigurationManager.AppSettings.Get("totalGroups"));
                int currentGroup = 1;
                _logger.Info($"Total Groups {totalGroups}, Current Group {currentGroup}");
                while (currentGroup <= totalGroups)
                {
                    _logger.Info($"service starting for group {currentGroup} totalGroups {totalGroups}.");
                    TrackASINs(currentGroup);
                    currentGroup++;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw ex;
            }


            return RedirectToAction("Index");
        }

        private void TrackASINs(int groupId)
        {

            DAmazon dAmazon = new DAmazon();
            AmazonTest amzAutomation = new AmazonTest();

            List<MainASINs> lstmainASINs = dAmazon.getEnabledASINs(groupId);
            _logger.Info($"Tracking ASINs for group {groupId}");
            _logger.Info(lstmainASINs);
            foreach (MainASINs mainASINs in lstmainASINs)
            {
                try
                {
                    _logger.Info($"Running Test for ASIN {mainASINs.ASIN}");
                    CSObject objCountStock = amzAutomation.test(mainASINs.ASIN);

                    if (objCountStock != null)
                    {
                        _logger.Info($"objCountStock {DefaultJsonSerializer.Instance.SerializeObject(objCountStock)}");
                        dAmazon.InsertTrackingRecord(objCountStock);
                    }
                    else
                    {
                        _logger.Info($"objCountStock is null");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }

            }
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