namespace BNFCorrectness
{
    /// <summary>
    /// Token representing words at the stream
    /// </summary>
    public class WordToken : Token
    {
        public string Lexeme { get; private set; }

        /// <summary>
        /// Reserved operator 
        /// </summary>
        public static readonly WordToken ProductionOperator = new("::=", (int)TokenTag.ProductionOperator);

        public WordToken(string lexeme, int tag) : base(tag) => Lexeme = lexeme;

        public override string ToString() => $"[{Tag} : {Lexeme}]";
    }
}
