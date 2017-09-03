using System;
using Xunit;

namespace GuiLabs.MathParser.Tests
{
    public class Tests
    {
        [Fact]
        public void TestParser()
        {
            Binder.Default.RegisterVariable("Random", () => new Random(1).NextDouble());
            var compiler = new Compiler();
            var compilation = compiler.CompileExpression("Random");
            if (compilation.IsSuccess)
            {
                var result = compilation.Expression();
            }
        }
    }
}
