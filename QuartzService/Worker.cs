using Quartz;
using Quartz.Impl;
using QuartzService.Models;
using QuartzService;
using SqlSugar;

namespace QuartzService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISqlSugarClient _db;
        private IScheduler _scheduler;

        public Worker(ILogger<Worker> logger, ISqlSugarClient db)
        {
            _logger = logger;
            _db = db;
        }


        /// <summary>
        /// 自定义定时任务 实现例子
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /* try
             {
                 var factory = new StdSchedulerFactory();
                 _scheduler = await factory.GetScheduler(stoppingToken);
                 await _scheduler.Start(stoppingToken);

                 //创建和配置触发器
                 var jobDetail = JobBuilder.Create<SynchronousDataTak>()
                     .WithIdentity("synchronousDataTak")
                     .Build();

                 var tigger = TriggerBuilder.Create()
                     .WithIdentity("synchronousDataTrigger")
                     .StartNow()
                     .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(5)).RepeatForever())
                     .Build();

                 await _scheduler.ScheduleJob(jobDetail, tigger, stoppingToken);
             }
             catch (Exception ex)
             {
                 _logger.LogError($"{DateTimeOffset.Now}启动失败，错误信息为:{ex.Message}");
             }*/
            while (!stoppingToken.IsCancellationRequested)
            {
                // 获取当前时间
                var currentTime = DateTime.Now;
                // 计算距离明天凌晨1点的时间间隔
                var next1AM = currentTime.Date.AddDays(1).AddHours(1);
                var delay1AM = next1AM - currentTime;

                // 计算距离明天凌晨6点的时间间隔
                var next6AM = currentTime.Date.AddDays(1).AddHours(6);
                var delay6AM = next6AM - currentTime;

                // 等待直到距离凌晨1点或6点的时间
                if (delay1AM.TotalMilliseconds > 0)
                {
                    await Task.Delay(delay1AM, stoppingToken);
                    if (stoppingToken.IsCancellationRequested)
                        break;
                }
                else if (delay6AM.TotalMilliseconds > 0)
                {
                    await Task.Delay(delay6AM, stoppingToken);
                    if (stoppingToken.IsCancellationRequested)
                        break;
                }

                // 现在是凌晨1点或6点，执行任务
                if (currentTime.Hour == 1)
                {
                    // 在凌晨1点执行的任务
                    Console.WriteLine("Task at 1 AM - Executed at: " + DateTime.Now);
                    // 执行您的任务代码
                }
                else if (currentTime.Hour == 6)
                {
                    // 在凌晨6点执行的任务
                    Console.WriteLine("Task at 6 AM - Executed at: " + DateTime.Now);
                    // 执行您的任务代码
                }
            }
        }
    }
}