namespace BNFCorrectness
{
    /// <summary>
    /// 
    /// </summary>
    public class Token
    {
        /// <summary>
        /// 
        /// </summary>
        public int Tag { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        public Token(int tag) => Tag = tag;
    }
}
