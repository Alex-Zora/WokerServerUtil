using Quartz;
using QuartzService.Models.Config;
using QuartzService.Reflex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuartzService.Extensions
{
    public static class QuartzConfiguratorExtension
    {
        public static void AddJobAnTrigger<T>(this IServiceCollectionQuartzConfigurator quartz, 
            IConfiguration configuration) where T : IJob
        {
            //把当前工作任务的类作为任务名称
            string jobName = typeof(T).Name;
            var configKey = $"Quartz:{jobName}";

            //读取配置文件中的任务策略
            var cronSchedule = configuration[configKey];

            if(String.IsNullOrEmpty(cronSchedule))
            {
                throw new Exception("定时任务策略配置异常!");
            }

            //添加任务和配置触发器
            var jobKey = new JobKey(jobName);
            quartz.AddJob<T>(options => options.WithIdentity(jobKey));

            quartz.AddTrigger(options => options
                .ForJob(jobKey)
                .WithIdentity(jobName + "-trigger")
                .WithCronSchedule(cronSchedule));
            /*quartz.AddTrigger(options => options
               .ForJob(jobKey)
               .WithIdentity(jobName + "-trigger")
               .StartNow());*/
            //.WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(1))));
        }

        /// <summary>
        /// 使用reflection特性灵活控制注册任务
        /// </summary>
        /// <param name="task"></param>
        /// <param name="taskConfig"></param>
        /// <exception cref="Exception"></exception>
        public static void AddJobAndTrigger(this IServiceCollectionQuartzConfigurator task, TaskEntity taskConfig)
        {
            if(taskConfig.status)
            {
                string jobKey = taskConfig.name;
                string cronSchedule = taskConfig.cron;

                //获取所有类
                List<Type> types = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, "QuartzService.dll")).GetTypes().ToList().Where(x => !x.IsInterface).ToList();
                

                foreach(Type type in types)
                {
                    if(type.Name == taskConfig.name)
                    {
                        //加载AddJob方法所在的包DependencyInjection
                        Assembly quartzAssembly = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, "Quartz.Extensions.DependencyInjection.dll"));
                        Type objectType = typeof(IServiceCollectionQuartzConfigurator);

                        //获取所有的静态方法
                        List<MethodInfo> list = ReflectionUtils.GetExtentionMethods(objectType, quartzAssembly).ToList();

                        //获取方法并指定泛型参数
                        MethodInfo method = list.Where(m => m.Name == "AddJob").FirstOrDefault()!.MakeGenericMethod(type);

                        //注册任务和触发器
                        if (method != null)
                        {
                            //定时任务key
                            var jabKeyInstance = new JobKey(jobKey);
                            //定义委托参数
                            Action<IJobConfigurator> jobConfigurator = options => options.WithIdentity(jabKeyInstance).StoreDurably();

                            if(taskConfig.policy == PolicyType.CURRENT)
                            {
                                
                                //调用方法添加任务传递委托参数
                                method.Invoke(task,new object[] { task, jobConfigurator });

                                task.AddTrigger(option => option.ForJob(jabKeyInstance)
                                    .WithIdentity(taskConfig.name + "-trigger")
                                     .StartNow());
                            } else if(taskConfig.policy == PolicyType.CRON)
                            {
                                if (String.IsNullOrEmpty(cronSchedule))
                                {
                                    throw new Exception($"{jobKey}定时任务策略配置异常!");
                                }
                                //调用方法添加任务传递委托参数
                                method.Invoke(task, new object[] { task, jobConfigurator });
                                task.AddTrigger(option => option.ForJob(jabKeyInstance)
                                    .WithIdentity(taskConfig.name + "-trigger")
                                    .WithCronSchedule(taskConfig.cron));
                            }
                        }
                    }
                }
            }
        }
    }
}
