using System;
using System.Linq;

namespace GuiLabs.MathParser
{
    public class Compiler
    {
        internal IExpressionTreeEvaluatorProvider evaluator;

        public Compiler() : this(new ExpressionTreeCompiler())
        {
        }

        public Compiler(IExpressionTreeEvaluatorProvider evaluator)
        {
            this.evaluator = evaluator;
        }

        public CompileResult CompileFunction(string functionText)
        {
            var result = new CompileResult();
            if (string.IsNullOrEmpty(functionText))
            {
                return result;
            }

            Node ast = Parse(functionText, result);
            if (result.Errors.Any())
            {
                return result;
            }

            var builder = new ExpressionTreeBuilder();
            var expressionTree = builder.CreateFunction(ast, result);
            if (expressionTree == null || result.Errors.Any())
            {
                return result;
            }

            Func<double, double> function = evaluator.InterpretFunction(expressionTree);
            result.Function = function;
            return result;
        }

        public CompileResult CompileExpression(string expressionText)
        {
            var result = new CompileResult();
            if (string.IsNullOrEmpty(expressionText))
            {
                return result;
            }

            Node ast = Parse(expressionText, result);
            if (result.Errors.Any())
            {
                return result;
            }

            var builder = new ExpressionTreeBuilder();
            var expressionTree = builder.CreateExpression(ast, result);
            if (expressionTree == null || result.Errors.Any())
            {
                return result;
            }

            Func<double> function = evaluator.InterpretExpression(expressionTree);
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
