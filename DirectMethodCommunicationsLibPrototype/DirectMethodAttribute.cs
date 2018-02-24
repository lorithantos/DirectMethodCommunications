using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DirectMethodCommunicationsLibPrototype
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = true)]
    public class DirectMethodAttribute : Attribute
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public static IEnumerable<MethodAndName> GetMethods<TSource>()
            where TSource : IDirectMethodBase
        {
            var type = typeof(TSource);

            var attributed = type.GetInterfaces().Where(i => ParentHasDirectMethodAttribute(i))
                .Select(t => new { type = t, path = GetInterfacePath(t) });
            var methods = attributed.Aggregate(new List<MethodAndName>(), (list, data) =>
            {
                list.AddRange(data.type.GetMethods().Select(m => new MethodAndName
                {
                    Path = data.path + "." + m.Name,
                    MethodInfo = m,
                }));
                return list;
            });

            return methods;
        }

        public static string GetInterfacePath(Type t)
        {
            if (t == null)
            {
                return string.Empty;
            }

            var name = GetNameFromType(t);

            var attribute = t.GetCustomAttribute<DirectMethodAttribute>();
            if (attribute != null)
            {
                if (!string.IsNullOrWhiteSpace(attribute.Path))
                {
                    return attribute.Path;
                }

                if (!string.IsNullOrWhiteSpace(attribute.Name))
                {
                    name = attribute.Name;
                }
            }

            var parent = t.GetInterfaces()
                .Where(i => ParentHasDirectMethodAttribute(i))
                .FirstOrDefault();

            if (parent != null)
            {
                name = $"{GetInterfacePath(parent)}.{name}";
            }

            return name;
        }

        private static string GetNameFromType(Type t)
        {
            string name = t.Name;
            if (t.IsInterface && name.StartsWith("I"))
            {
                name = name.Substring(1);
            }

            return name;
        }

        private static bool ParentHasDirectMethodAttribute(Type type)
        {
            if (type.GetCustomAttribute<DirectMethodAttribute>() != null)
            {
                return true;
            }

            foreach (var i in type.GetInterfaces())
            {
                bool has = ParentHasDirectMethodAttribute(i);
                if (has)
                {
                    return true;
                }
            }

            return false;
        }

        public class MethodAndName
        {
            public string Path { get; set; }
            public MethodInfo MethodInfo { get; set; }
        }
    }
}
