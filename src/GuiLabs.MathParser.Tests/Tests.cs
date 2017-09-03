using System;
using System.Collections.Generic;
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

        [Fact]
        public void ToStringTests()
        {
            var expressions = new Dictionary<string, string>
            {
                { "2+3", "2 + 3" },
                { "(2+3)*2", "(2 + 3) * 2" },
                { "pi+e", "pi + e" },
                { "(pi+e)", "pi + e" },
                { "sin(pi+e)", "sin(pi + e)" },
            };

            var tautologies = new[]
            {
                "2 + 3",
                "1",
                "(3 * 4) + 2"
            };

            foreach (var tautology in tautologies)
            {
                Assert.Equal(tautology, Parser.Parse(tautology).Root.ToString());
            }

            foreach (var expression in expressions)
            {
                Assert.Equal(expression.Value, Parser.Parse(expression.Key).Root.ToString());
            }
        }
    }
}
