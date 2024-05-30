using Quartz;
using QuartzService.Models;
using QuartzService.Reflex;
using SqlSugar;

namespace QuartzService
{
    /// <summary>
    /// 自定义任务类
    /// </summary>
    public class NewJob : IJob
    {
        public readonly ISqlSugarClient _client;
        public readonly IConfiguration _configuration;
        public SqlSugarProvider? mysqlDB;
        public SqlSugarProvider? mssqlDB;
        private readonly ILogger<NewJob> _logger;

        public NewJob(ISqlSugarClient client, IConfiguration configuration, ILogger<NewJob> logger)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger;
            InitDb();
        }

        public void InitDb()
        {
            if(_client != null)
            {
                mssqlDB = _client.AsTenant().GetConnection("MSSql");
                mysqlDB = _client.AsTenant().GetConnection("MySql");    
            }
        }

        /// <summary>
        /// 在这里写自己的任务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("执行任务！");
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }
    }
}
