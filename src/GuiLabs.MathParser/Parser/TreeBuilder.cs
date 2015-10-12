using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GuiLabs.MathParser
{
    public class ExpressionTreeBuilder
    {
        public ExpressionTreeBuilder()
        {
            Binder = new Binder();
        }

        public Binder Binder { get; set; }
        CompileResult Status { get; set; }

        public Expression<Func<double, double>> CreateFunction(Node root, CompileResult status)
        {
            Status = status;
            ParameterExpression parameter = Expression.Parameter(typeof(double), "x");
            Binder.RegisterParameter(parameter);
            Expression body = CreateExpressionCore(root);
            if (body == null)
            {
                return null;
            }

            var expressionTree = Expression.Lambda<Func<double, double>>(body, parameter);
            return expressionTree;
        }

        public Expression<Func<double>> CreateExpression(Node root, CompileResult status)
        {
            Status = status;
            Expression body = CreateExpressionCore(root);
            if (body == null)
            {
                return null;
            }

            // If expression does not return double, we'll get an error.
            if (body.Type != typeof(double))
            {
                return null;
            }

            var expressionTree = Expression.Lambda<Func<double>>(body);
            return expressionTree;
        }

        Expression CreateExpressionCore(Node root)
        {
            switch (root.Kind)
            {
                case NodeType.Negation:
                    return CreateUnaryExpression(root);
                case NodeType.Addition:
                case NodeType.Subtraction:
                case NodeType.Multiplication:
                case NodeType.Division:
                case NodeType.Power:
                    return CreateBinaryExpression(root);
                case NodeType.Variable:
                    return CreateIdentifierExpression(root);
                case NodeType.Constant:
                    return CreateLiteralExpression(Convert.ToDouble(root.Token.Text));
                case NodeType.FunctionCall:
                    return CreateCallExpression(root);
                case NodeType.PropertyAccess:
                    return CreatePropertyAccessExpression(root);
                default:
                    return null;
            }
        }

        Expression CreateUnaryExpression(Node root)
        {
            Expression operand = CreateExpressionCore(root.Children[0]);
            if (operand == null)
            {
                return null;
            }

            return Expression.Negate(operand);
        }

        Expression CreateIdentifierExpression(Node root)
        {
            var text = root.Token.Text;

            var parameter = Binder.Resolve(text);
            if (parameter == null)
            {
                Status.AddUnknownIdentifierError(text);
            }

            return parameter;
        }

        Expression CreatePropertyAccessExpression(Node root)
        {
            string propertyName = root.Children[1].Token.Text;
            var argument = CreateExpressionCore(root.Children[0]);
            if (argument == null)
            {
                return null;
            }

            Type type = argument.Type;
            var property = type.GetRuntimeProperty(propertyName);
            if (property == null)
            {
                Status.AddBindError(propertyName);
                return null;
            }

            var propertyExpression = Expression.Property(argument, property);
            return propertyExpression;
        }

        Expression CreateCallExpression(Node root)
        {
            string functionName = root.Token.Text;
            MethodInfo method = Binder.ResolveMethod(functionName);
            if (method == null)
            {
                Status.AddMethodNotFoundError(functionName);
                return null;
            }

            var arguments = root.Children;
            if (arguments.Count == 1)
            {
                var argument = CreateExpressionCore(arguments[0]);
                if (argument == null)
                {
                    return null;
                }

                return Expression.Call(method, argument);
            }

            Status.AddMethodNotFoundError(functionName);
            return null;
        }

        Expression CreateLiteralExpression(double arg)
        {
            return Expression.Constant(arg);
        }

        Expression CreateBinaryExpression(Node node)
        {
            Expression left = CreateExpressionCore(node.Children[0]);
            Expression right = CreateExpressionCore(node.Children[1]);

            if (left == null || right == null)
            {
                return null;
            }

            switch (node.Kind)
            {
                case NodeType.Addition:
                    return Expression.Add(left, right);
                case NodeType.Subtraction:
                    return Expression.Subtract(left, right);
                case NodeType.Multiplication:
                    return Expression.Multiply(left, right);
                case NodeType.Division:
                    return Expression.Divide(left, right);
                case NodeType.Power:
                    return Expression.Power(left, right);
            }
            return null;
        }
    }
}
