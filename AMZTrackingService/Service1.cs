using DL;
using Entity;
using Selenium.Tests;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceProcess;
using System.Threading;

namespace AMZTrackingService
{
    public partial class Service1 : ServiceBase
    {
        Thread m_thread = null;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_thread = new Thread(new ThreadStart(ThreadProc));
            // start the thread
            m_thread.Start();
        }

        protected override void OnStop()
        {
            m_thread = null;
        }

        public void ThreadProc()
        {
            int totalGroups = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("totalGroups"));
            int currentGroup = 1;
            while (true)
            {
                TrackASINs(currentGroup);
                int interval = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("intervalMinutes"));
                
                if (currentGroup == totalGroups)
                    currentGroup = 1;                
                else
                    currentGroup++;

                Thread.Sleep(interval * 60 * 1000); // Interval In Minutes
                //try
                //{
                //    WebClient client = new WebClient();
                //    try
                //    {
                //        client.DownloadData(System.Configuration.ConfigurationManager.AppSettings.Get("url"));
                //    }
                //    catch (Exception)
                //    {
                //    }
                //    client = null;
                //    int interval = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("intervalMinutes"));
                //    Thread.Sleep(interval * 60 * 1000); // Interval In Minutes
                //}
                //catch (Exception)
                //{
                //}
            }
        }

        public void TrackASINs(int groupId)
        {

            DAmazon dAmazon = new DAmazon();
            AmazonTest amzAutomation = new AmazonTest();

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
                System.Threading.Thread.Sleep(2 * 60 * 1000); // 1.5 Minute
            }
            GC.Collect();
        }

    }
}
