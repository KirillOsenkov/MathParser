namespace GuiLabs.MathParser
{
    public enum NodeType
    {
        Unknown,
        Negation,
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Power,
        PropertyAccess,
        FunctionCall,
        Constant,
        Variable
    }
}
