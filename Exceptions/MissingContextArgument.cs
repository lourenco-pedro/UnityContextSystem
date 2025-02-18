namespace ContextSystem.Exceptions
{
    public class MissingContextArgument : System.Exception
    {
        public MissingContextArgument(string message) : base(message)
        {}
    }
}