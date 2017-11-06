using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace TokenGet
{
    public class Functions
    {
        // This function will be triggered based on the schedule you have set for this WebJob
        // This function will enqueue a message on an Azure Queue called queue
        [NoAutomaticTrigger]
        public static void ManualTrigger(TextWriter log, int value, [Queue("queue")] out string message)
        {
            log.WriteLine("Function is invoked with value={0}", value);
            message = GetAccessToken(log).Result;
            Console.WriteLine(message);
            log.WriteLine("Following message will be written on the Queue={0}", message);
        }

        public static string Url = "http://cwwebservice.azurewebsites.net/api/wx?method=token";
        static async Task<string> GetAccessToken(TextWriter log)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync(Url);
            return result;
        }
    }
}
