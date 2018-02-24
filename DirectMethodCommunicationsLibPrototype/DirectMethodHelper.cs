using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace DirectMethodCommunicationsLibPrototype
{
    public static class DirectMethodHelper
    {
        private static readonly MethodInfo DeserializeMethod = typeof(JsonConvert).GetMethods()
                .Where(m => m.Name == "DeserializeObject")
                .Where(m => m.GetParameters().Count() == 1)
                .Where(m => m.IsGenericMethod)
                .First();

        private static readonly MethodInfo SerializeMethod = typeof(JsonConvert).GetMethods()
                .Where(m => m.Name == "SerializeObject")
                .Where(m => m.GetParameters().Count() == 1)
                .First();

        private static List<IDirectMethodBase> DirectMethodObjects = new List<IDirectMethodBase>();

        public static TResult CallDirectMethod<TCaller, TResult, TInput>(TCaller caller, Func<TInput, TResult> func, TInput input, [CallerMemberName]string callerName = null)
            where TResult : new()
            where TCaller : IDirectMethodBase
        {
            return CallDirectMethod<TCaller, TInput, TResult>(callerName, input);
        }

        public static TResult CallDirectMethod<TCaller, TResult>(TCaller caller, Func<TResult> func, [CallerMemberName]string callerName = null)
            where TResult : new()
        {
            string retv = CallDirectMethod<TCaller>(callerName, null);
            return JsonConvert.DeserializeObject<TResult>(retv);
        }

        public static void CallDirectMethod<TCaller, TSource>(TCaller caller, Action<TSource> func, TSource source, [CallerMemberName]string callerName = null)
        {
            CallDirectMethod<TCaller>(callerName, JsonConvert.SerializeObject(source));
        }

        public static void CallDirectMethod<TCaller>(TCaller caller, Action func, [CallerMemberName]string callerName = null)
        {
            CallDirectMethod<TCaller>(callerName, null);
        }

        public static TResult AddDirectMethodImplementation<TResult>()
            where TResult : class, IDirectMethodBase, new()
        {
            return AddDirectMethodImplementation(new TResult { });
        }

        public static TSource AddDirectMethodImplementation<TSource>(TSource source)
            where TSource : class, IDirectMethodBase, new()
        {
            // Register all of the methods found in the source
            foreach (var namedMethod in DirectMethodAttribute.GetMethods<TSource>())
            {
                var dmCall = GetDirectMethodCall(source, namedMethod.Path, namedMethod.MethodInfo);

                // Now just wrap the call and we are done
                // TODO: what about awaits and async?
            }

            DirectMethodObjects.Add(source);

            return source;
        }

        private static Func<string, string> GetDirectMethodCall<ThisType>(ThisType source, string path, MethodInfo methodInfo)
            where ThisType : class, IDirectMethodBase, new()
        {
            Debug.WriteLine($"RegisterDirectMethod: {path}");
            Debug.Assert(methodInfo.GetParameters().Count() <= 1);

            var methodName = methodInfo.Name;

            var callerThis = Expression.Constant(source);
            var parameterTypeList = methodInfo
                .GetParameters()
                .Select(p => p.ParameterType);

            var parameterType = parameterTypeList.FirstOrDefault();

            IEnumerable<ParameterExpression> methodParam = parameterTypeList
                .Select(p => Expression.Parameter(p))
                .ToList();

            Expression callExpression = Expression.Call(callerThis, methodInfo, methodParam);

            var stringInput = Expression.Parameter(typeof(string));

            if (parameterType != null)
            {
                var converter = DeserializeMethod.MakeGenericMethod(parameterType);
                var conversionCall = Expression.Call(converter, stringInput);

                callExpression = Expression.Call(callerThis, methodInfo, conversionCall);
            }

            var returnType = methodInfo.ReturnType;

            if (returnType != typeof(void))
            {
                callExpression = SerializeOutput(callExpression, returnType);
            }
            else
            {
                var stringOutput = Expression.Constant(string.Empty);
                callExpression = Expression.Block(callExpression, stringOutput);
            }

            var parameterList = new List<ParameterExpression> { stringInput };
            var lambda = Expression.Lambda<Func<string, string>>(callExpression, parameterList);

            return lambda.Compile();
        }

        private static Expression SerializeOutput(Expression callExpression, Type returnType)
        {
            var inner = callExpression;
            if (!returnType.IsClass)
            {
                inner = Expression.Convert(inner, typeof(object));
            }

            callExpression = Expression.Call(SerializeMethod, inner);
            return callExpression;
        }

        private static TOutput CallDirectMethod<TCaller, TInput, TOutput>(string methodName, TInput input)
        {
            var json = JsonConvert.SerializeObject(input);

            var results = CallDirectMethod<TCaller>(methodName, json);

            return JsonConvert.DeserializeObject<TOutput>(results);
        }

        // This is where the magic happens
        // Be nice if it actually did something
        // TODO: Implement DirectMethod call
        private static string CallDirectMethod<TCaller>(string methodName, string input)
        {
            string basePath = DirectMethodAttribute.GetInterfacePath(typeof(TCaller));
            string directMethodName = $"{basePath}.{methodName}";

            Debug.WriteLine($"CallDirectMethod : {directMethodName} : {input}");

            return string.Empty;
        }
    }
}
