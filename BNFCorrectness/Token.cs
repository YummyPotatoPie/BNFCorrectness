namespace BNFCorrectness
{
    public enum TokenTag
    {
        Literal = 256,
        RuleID,
        ProductionOperator,
        Regex
    }

    /// <summary>
    /// Base token representation class
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Tag of token
        /// </summary>
        public int Tag { get; private set; }

        /// <summary>
        /// Base token constructor
        /// </summary>
        /// <param name="tag"></param>
        public Token(int tag) => Tag = tag;

        public override string ToString() => $"[{Tag}]";
    }
}
