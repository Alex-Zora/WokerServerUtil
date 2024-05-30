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
        /// �Զ��嶨ʱ���� ʵ������
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

                 //���������ô�����
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
                 _logger.LogError($"{DateTimeOffset.Now}����ʧ�ܣ�������ϢΪ:{ex.Message}");
             }*/
            while (!stoppingToken.IsCancellationRequested)
            {
                // ��ȡ��ǰʱ��
                var currentTime = DateTime.Now;
                // ������������賿1���ʱ����
                var next1AM = currentTime.Date.AddDays(1).AddHours(1);
                var delay1AM = next1AM - currentTime;

                // ������������賿6���ʱ����
                var next6AM = currentTime.Date.AddDays(1).AddHours(6);
                var delay6AM = next6AM - currentTime;

                // �ȴ�ֱ�������賿1���6���ʱ��
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

                // �������賿1���6�㣬ִ������
                if (currentTime.Hour == 1)
                {
                    // ���賿1��ִ�е�����
                    Console.WriteLine("Task at 1 AM - Executed at: " + DateTime.Now);
                    // ִ�������������
                }
                else if (currentTime.Hour == 6)
                {
                    // ���賿6��ִ�е�����
                    Console.WriteLine("Task at 6 AM - Executed at: " + DateTime.Now);
                    // ִ�������������
                }
            }
        }
    }
}