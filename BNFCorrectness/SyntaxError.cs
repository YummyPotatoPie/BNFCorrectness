namespace BNFCorrectness
{
    public class SyntaxError : System.Exception
    {
        public SyntaxError() : base() { }

        public SyntaxError(string message) : base(message) { }
    }
}
