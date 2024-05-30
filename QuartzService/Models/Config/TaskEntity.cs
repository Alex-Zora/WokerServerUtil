using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzService.Models.Config
{
    public class TaskEntity
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 状态 true开启 false关闭
        /// </summary>
        public bool status { get; set; }
        /// <summary>
        /// 执行策略
        /// </summary>
        public string cron { get; set; }
        public string policy { get; set; }
    }

   /* public enum PolicyType
    {
        Cron = 0,
        Now = 1,
    }*/

    public static class PolicyType
    {
        //立即执行
        public static string CURRENT = "Current";
        //根据策略走
        public static string CRON = "Cron";
    }
}
