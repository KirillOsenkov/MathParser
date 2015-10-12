using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GuiLabs.MathParser
{
    public class Binder
    {
        private static List<MethodInfo> methods = new List<MethodInfo>();

        private Dictionary<string, ParameterExpression> parameters = new Dictionary<string, ParameterExpression>();

        static Binder()
        {
            AddMethods(typeof(Math));
        }

        public static void AddMethods(Type type)
        {
            foreach (var methodInfo in type.GetRuntimeMethods())
            {
                methods.Add(methodInfo);
            }
        }

        public void RegisterParameter(ParameterExpression parameter)
        {
            parameters.Add(parameter.Name, parameter);
        }

        ParameterExpression ResolveParameter(string parameterName)
        {
            ParameterExpression parameter;
            if (parameters.TryGetValue(parameterName, out parameter))
            {
                return parameter;
            }

            return null;
        }

        Expression ResolveConstant(string identifier)
        {
            if (identifier.Equals("pi", StringComparison.OrdinalIgnoreCase))
            {
                return Expression.Constant(Math.PI);
            }
            else if (identifier.Equals("e", StringComparison.OrdinalIgnoreCase))
            {
                return Expression.Constant(Math.E);
            }

            return null;
        }

        public Expression Resolve(string identifier)
        {
            return ResolveConstant(identifier) ?? ResolveParameter(identifier);
        }

        public MethodInfo ResolveMethod(string functionName)
        {
            foreach (var methodInfo in typeof(Math).GetRuntimeMethods())
            {
                var parameters = methodInfo.GetParameters();
                if (methodInfo.Name.Equals(functionName, StringComparison.OrdinalIgnoreCase)
                    && parameters.Length == 1
                    && parameters[0].ParameterType == typeof(double))
                {
                    return methodInfo;
                }
            }

            foreach (var methodInfo in methods)
            {
                if (methodInfo.Name.Equals(functionName, StringComparison.OrdinalIgnoreCase))
                {
                    return methodInfo;
                }
            }

            return null;
        }
    }
}
