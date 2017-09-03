using System.Collections.Generic;
using System.Linq;

namespace GuiLabs.MathParser
{
    public class Node
    {
        public readonly IList<Node> Children;
        public readonly NodeType Kind;
        public readonly Token Token;

        public Node(NodeType nodeType, params Node[] children)
        {
            this.Kind = nodeType;
            this.Children = new List<Node>(children);
        }

        public Node(NodeType nodeType, Token token)
        {
            this.Kind = nodeType;
            this.Token = token;
            this.Children = new List<Node>();
        }

        private string ToString(int childIndex)
        {
            return "(" + Children[childIndex].ToString() + ")";
        }

        public override string ToString()
        {
            switch (Kind)
            {
                case NodeType.Negation:
                    return $"-{ToString(0)}";
                case NodeType.Addition:
                    return $"{ToString(0)} + {ToString(1)}";
                case NodeType.Subtraction:
                    return $"{ToString(0)} - {ToString(1)}";
                case NodeType.Multiplication:
                    return $"{ToString(0)} * {ToString(1)}";
                case NodeType.Division:
                    return $"{ToString(0)} / {ToString(1)}";
                case NodeType.Power:
                    return $"{ToString(0)} ^ {ToString(1)}";
                case NodeType.PropertyAccess:
                    return $"{ToString(0)}.{Token.Text}";
                case NodeType.FunctionCall:
                    return Token.Text + "(" + string.Join(", ", Children.Select(c => c.ToString())) + ")";
                case NodeType.Constant:
                    return Token.Text;
                case NodeType.Variable:
                    return Token.Text;
                default:
                    return string.Join(" ", Children.Select(c => c.ToString()));
            }
        }
    }
}
