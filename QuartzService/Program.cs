using QuartzService;
using SqlSugar;
using System.Configuration;
using Quartz;
using QuartzService.Extensions;
using QuartzService.Models.Config;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()    //添加Windows Services依赖作为Windows服务运行
    .ConfigureServices((hostContext, services) =>
    {
        //services.AddHostedService<Worker>();
        services.AddSingleton<ISqlSugarClient>(s =>
        {
            IConfiguration _configuration = hostContext.Configuration;
            List<DatabaseConfig> dbs = _configuration.GetSection("DBS").Get<List<DatabaseConfig>>();
            SqlSugarScope Db = new SqlSugarScope(new List<ConnectionConfig>()
            { 
                new ConnectionConfig(){ConfigId=dbs[0].ConnId,DbType=dbs[0].Type,ConnectionString = dbs[0].ConnectionString,IsAutoCloseConnection = true},
                new ConnectionConfig(){ConfigId=dbs[1].ConnId,DbType=dbs[1].Type,ConnectionString = dbs[1].ConnectionString,IsAutoCloseConnection = true}
                //ConfigId用来区别是哪个库
            }, db =>
            {
                //打印sql
                /*db.GetConnection("MSSql").Aop.OnLogExecuting = (sql, p) =>
                {
                    Console.WriteLine( "sqlserver数据库" + sql);
                };
                db.GetConnection("MySql").Aop.OnLogExecuting = (sql, p) =>
                {
                    Console.WriteLine("mysql数据库" + sql);
                };*/
            });
            return Db;
        });
        //注册Quartz.net服务
        services.AddQuartz(q =>
        {
            //q.UseMicrosoftDependencyInjectionJobFactory();
            //注册任务
            q.AddJobAnTrigger<NewJob>(hostContext.Configuration);

            //获取配置文件中开启的定时任务
            List<TaskEntity> tasks = hostContext.Configuration.GetSection("QuartzTask").Get<List<TaskEntity>>().Where(x => x.status).ToList();
            if(tasks.Count > 0)
            {
                foreach(TaskEntity item in tasks)
                {
                    q.AddJobAndTrigger(item);
                }
            }

        });

        //WaitForJobsToComplete:此设置确保当请求关闭时，Quartz.NET在退出之前等待作业优雅地结束。
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    })
    .ConfigureLogging((hostContext, logConfig) =>
    {
        logConfig.AddFilter("System", LogLevel.Warning);
        logConfig.AddFilter("Microsoft", LogLevel.Warning);

        //不携带参数表示配置文件在根目录下
        logConfig.AddLog4Net();
        logConfig.SetMinimumLevel(LogLevel.Information);
    })
    .Build();

await host.RunAsync();

public class DatabaseConfig
{
    public string ConnId { get; set; }
    public int DbType { get; set; }
    public string ConnectionString { get; set; }

    public DbType Type
    {
        get
        {
            if (DbType == 0)
            {
                return SqlSugar.DbType.MySql;
            }
            else if (DbType == 1)
            {
                return SqlSugar.DbType.SqlServer;
            }
            else
            {
                // 如果DbType没有匹配的值，可以返回默认值或抛出异常
                
                throw new Exception("数据库类型设置异常!");
            }
        }
    }
}
