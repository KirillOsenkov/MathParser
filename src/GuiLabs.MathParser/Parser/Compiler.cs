using System;
using System.Linq;

namespace GuiLabs.MathParser
{
    public class Compiler
    {
        public IExpressionTreeEvaluatorProvider ExpressionTreeEvaluatorProvider { get; set; }

        public CompileResult CompileFunction(object context, string functionText)
        {
            CompileResult result = new CompileResult();
            if (string.IsNullOrEmpty(functionText))
            {
                return result;
            }

            Node ast = Parse(functionText, result);
            if (result.Errors.Any())
            {
                return result;
            }

            ExpressionTreeBuilder builder = new ExpressionTreeBuilder();
            builder.SetContext(context);
            var expressionTree = builder.CreateFunction(ast, result);
            if (expressionTree == null || result.Errors.Any())
            {
                return result;
            }

            Func<double, double> function = ExpressionTreeEvaluatorProvider.InterpretFunction(expressionTree);
            result.Function = function;
            return result;
        }

        public CompileResult CompileExpression(
            object context,
            string expressionText)
        {
            CompileResult result = new CompileResult();
            if (string.IsNullOrEmpty(expressionText))
            {
                return result;
            }

            Node ast = Parse(expressionText, result);
            if (result.Errors.Any())
            {
                return result;
            }

            ExpressionTreeBuilder builder = new ExpressionTreeBuilder();
            builder.SetContext(context);
            var expressionTree = builder.CreateExpression(ast, result);
            if (expressionTree == null || result.Errors.Any())
            {
                return result;
            }

            Func<double> function = ExpressionTreeEvaluatorProvider.InterpretExpression(expressionTree);
            result.Expression = function;
            return result;
        }

        private static Node Parse(string text, CompileResult result)
        {
            ParseResult ast = Parser.Parse(text);
            if (ast.Errors.Any())
            {
                result.Errors.AddRange(ast.Errors);
            }

            return ast.Root;
        }
    }
}
