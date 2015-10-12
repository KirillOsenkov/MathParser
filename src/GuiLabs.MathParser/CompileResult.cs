using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuiLabs.MathParser
{
    public class CompileResult
    {
        public Func<double, double> Function;
        public Func<double> Expression { get; set; }
        public readonly List<CompileError> Errors = new List<CompileError>();

        public bool IsSuccess
        {
            get
            {
                return !Errors.Any() && (Expression != null || Function != null);
            }
        }

        public void AddError(string error)
        {
            Errors.Add(new CompileError()
            {
                Text = error
            });
        }

        public void AddBindError(string objectName)
        {
            AddError(string.Format("Could not find object with name '{0}'", objectName));
        }

        public void AddMethodNotFoundError(string functionName)
        {
            AddError(string.Format("Could not find method '{0}'", functionName));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var error in Errors)
            {
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                }

                sb.Append(error.Text);
            }

            return sb.ToString();
        }

        public void AddUnknownIdentifierError(string text)
        {
            AddError(string.Format("Unknown identifier: '{0}'", text));
        }

        public void AddFigureIsNotAPointError(string longestPrefix)
        {
            AddError(string.Format("Figure '{0}' is not a point."));
        }

        public void AddIncorrectNumberOfArgumentsError(System.Reflection.MethodInfo method, int actualNumberOfArguments)
        {
            AddError(string.Format("Function '{0}' expects {1} arguments, and it was passed {2}",
                method.Name, method.GetParameters().Length, actualNumberOfArguments));
        }
    }
}
