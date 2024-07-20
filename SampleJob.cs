using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Quartz.Logging.OperationName;

namespace TestWorker
{
    [DisallowConcurrentExecution]
    public class SampleJob:IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var logger= context.MergedJobDataMap.Get("logger") as ILogger;
            await File.AppendAllTextAsync(@"D:\temp\sample.log", "Executed job sample job at "+DateTime.Now.ToLongTimeString()+"\n");
            logger.LogInformation("Executed job sample job at " + DateTime.Now.ToLongTimeString());
        }
        
    }
}
