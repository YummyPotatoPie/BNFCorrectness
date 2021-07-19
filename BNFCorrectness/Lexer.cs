using SymbolStreams;

namespace BNFCorrectness
{
    /// <summary>
    /// 
    /// </summary>
    public class Lexer
    {
        /// <summary>
        /// 
        /// </summary>
        public int Line { get; private set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        public int Position { get; private set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        private ISymbolStream<char> _symbolStream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbolStream"></param>
        public Lexer(ISymbolStream<char> symbolStream) => _symbolStream = symbolStream;

        private void SkipWhiteSpaces()
        {
            char buffer = _symbolStream.Peek();
            do 
            {
                if (buffer == '\n') Line++;
                _symbolStream.Next(out buffer);
            } while (char.IsWhiteSpace(buffer) && !_symbolStream.EndOfStream());
        }

        public Token GetNextToken()
        {
            SkipWhiteSpaces();
            if (_symbolStream.Peek() == '<') return 
        }
    }
}
