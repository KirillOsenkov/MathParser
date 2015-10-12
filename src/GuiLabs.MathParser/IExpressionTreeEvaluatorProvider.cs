using System.Linq.Expressions;

namespace GuiLabs.MathParser
{
    public interface IExpressionTreeEvaluatorProvider
    {
        T InterpretFunction<T>(Expression<T> node);
        T InterpretExpression<T>(Expression<T> node);
    }
}
