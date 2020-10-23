using DL;
using Entity;
using NLog;
using NLog.Targets;
using Selenium.Tests;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using System.Timers;

namespace AMZTrackingService
{
    public partial class ScheduledService : ServiceBase
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private System.Timers.Timer _timer = new System.Timers.Timer();
        private static string _interval = ConfigurationManager.AppSettings["intervalMinutes"] ?? "5";
        public ScheduledService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var interval = 1000 * 60 * int.Parse(_interval);

            _timer.Interval = interval; // 5 minutes
            _timer.Enabled = true;
            _timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            _logger.Info("service started.");
        }

        protected override void OnStop()
        {
            _logger.Info("service stopped.");
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
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

    }
}
