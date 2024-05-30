using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuartzService.Reflex
{
    public static class MapperUtils
    {
        //反射映射方法
        public static void CopyPorperties<TSource, TDestination>(TSource source, TDestination destination)
        {
            Type typeSource = typeof(TSource);
            Type typeDest = typeof(TDestination);

            PropertyInfo[] properties = typeSource.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                PropertyInfo destProperty = typeDest.GetProperty(property.Name);

                if (destProperty != null && destProperty.PropertyType == property.PropertyType && destProperty.CanWrite)
                {
                    object value = property.GetValue(source);
                    destProperty.SetValue(destination, value);
                }
            }
        }
    }
}
