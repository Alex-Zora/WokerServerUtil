using QuartzService;
using SqlSugar;
using System.Configuration;
using Quartz;
using QuartzService.Extensions;
using QuartzService.Models.Config;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()    //���Windows Services������ΪWindows��������
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
                //ConfigId�����������ĸ���
            }, db =>
            {
                //��ӡsql
                /*db.GetConnection("MSSql").Aop.OnLogExecuting = (sql, p) =>
                {
                    Console.WriteLine( "sqlserver���ݿ�" + sql);
                };
                db.GetConnection("MySql").Aop.OnLogExecuting = (sql, p) =>
                {
                    Console.WriteLine("mysql���ݿ�" + sql);
                };*/
            });
            return Db;
        });
        //ע��Quartz.net����
        services.AddQuartz(q =>
        {
            //q.UseMicrosoftDependencyInjectionJobFactory();
            //ע������
            q.AddJobAnTrigger<NewJob>(hostContext.Configuration);

            //��ȡ�����ļ��п����Ķ�ʱ����
            List<TaskEntity> tasks = hostContext.Configuration.GetSection("QuartzTask").Get<List<TaskEntity>>().Where(x => x.status).ToList();
            if(tasks.Count > 0)
            {
                foreach(TaskEntity item in tasks)
                {
                    q.AddJobAndTrigger(item);
                }
            }

        });

        //WaitForJobsToComplete:������ȷ��������ر�ʱ��Quartz.NET���˳�֮ǰ�ȴ���ҵ���ŵؽ�����
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    })
    .ConfigureLogging((hostContext, logConfig) =>
    {
        logConfig.AddFilter("System", LogLevel.Warning);
        logConfig.AddFilter("Microsoft", LogLevel.Warning);

        //��Я��������ʾ�����ļ��ڸ�Ŀ¼��
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
                // ���DbTypeû��ƥ���ֵ�����Է���Ĭ��ֵ���׳��쳣
                
                throw new Exception("���ݿ����������쳣!");
            }
        }
    }
}
