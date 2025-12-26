namespace Vidly.Interfaces
{
    public interface IExpressionHandler
    {
        string InfixToPostfix(string infix);

        decimal EvaluatePostfix(string postfix);
    }
}
