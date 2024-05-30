using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuartzService.Reflex
{
    /// <summary>
    /// 反射工具类
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// 获取程序集中所有的静态方法
        /// </summary>
        /// <param name="extendedType"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetExtentionMethods(this Type extendedType, Assembly assembly)
        {
            return assembly
            .GetTypes()
            // 筛选出静态类
            .Where(t => t.IsSealed && t.IsAbstract && !t.IsGenericType)
            .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public))
            // 过滤出扩展方法（第一个参数带有 this 修饰符的类型为我们寻找的类型）
            .Where(m => m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false) &&
                       m.GetParameters()[0].ParameterType == extendedType);
        }
    }
}
