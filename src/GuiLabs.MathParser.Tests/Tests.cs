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
            var binder = new Binder();
            binder.RegisterVariable("Random", () => new Random(1).NextDouble());
            var compiler = new Compiler();
            var compilation = compiler.CompileExpression("Random");
            if (compilation.IsSuccess)
            {
                var result = compilation.Expression();
            }
        }

        public class TestMethodClass
        {
            public static double Method() => 42;
        }

        [Fact]
        public void TestMethods()
        {
            double number = Evaluate<TestMethodClass>("Method");
            Assert.Equal(42, number);
            number = Evaluate<TestMethodClass>("Method()");
            Assert.Equal(42, number);
        }

        private static double Evaluate<T>(string text)
        {
            var binder = new Binder();
            binder.RegisterStaticMethods<T>();
            var compiler = new Compiler(binder: binder);
            var result = compiler.CompileExpression(text);
            var number = result.Expression();
            return number;
        }

        [Fact]
        public void ParseMethodWithNoArguments()
        {
            Assert.Equal("M()", Parser.Parse("M()").Root.ToString());
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
                { "(((((365) * 4 + 1) * 25 - 1) * 4 + 1) * 25 - 366) * (10000 * 1000 * 60 * 60 * 24) - 1", "(((((((((365 * 4) + 1) * 25) - 1) * 4) + 1) * 25) - 366) * ((((10000 * 1000) * 60) * 60) * 24)) - 1" }
            };

            var tautologies = new[]
            {
                "2 + 3",
                "Method()",
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
