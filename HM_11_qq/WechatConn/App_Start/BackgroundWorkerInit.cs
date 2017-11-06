using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebBackgrounder;
using HM.Eleven.QQPlugins;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(WechatConn.App_Start.BackgroundWorkerInit), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(WechatConn.App_Start.BackgroundWorkerInit), "Shutdown")]
namespace WechatConn.App_Start
{
    /// <summary>
    /// Job To Refresh Project Tree into Memory
    /// </summary>
    public class GetCucumberProjectTreeJob : Job
    {
        public GetCucumberProjectTreeJob(TimeSpan interval, TimeSpan timeout)
            : base("GetCucumberProjectTree Job", interval, timeout)
        {
        }

        public override Task Execute()
        {
            return new Task(() =>
            {
                string type = "client_credential";
                string appid = "wxbe2e5c2dd5927594";
                string appsec = "fef2f0f1eb6f89bbea10a947d2f5adfc";
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type={0}&appid={1}&secret={2}", type, appid, appsec);
                //string res= HM.Eleven.QQPlugins.Helper.WebConnectHelper.getData(url);

                //Logger.Debug("Refreshing Project Tree...");
                //Utils.Projects = Utils.GetTreeFromDisk();
            });
        }
    }

    public static class BackgroundWorkerInit
    {
        static readonly JobManager _jobManager = CreateJobWorkersManager();

        public static void Start()
        {
            _jobManager.Start();
        }

        public static void Shutdown()
        {
            _jobManager.Dispose();
        }

        private static JobManager CreateJobWorkersManager()
        {
            var jobs = new IJob[]
            {
                 //new GetCucumberProjectTreeJob(TimeSpan.FromSeconds(60 * 60), TimeSpan.FromSeconds(20)),
            };

            var coordinator = new SingleServerJobCoordinator();
            var manager = new JobManager(jobs, coordinator);
            manager.Fail(null);
            //manager.Fail(ex => Logger.Error("Web Job Blow Up.", ex));
            return manager;
        }
    }
}