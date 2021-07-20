using SymbolStreams;
using System.Collections;
using System.Text;

namespace BNFCorrectness
{
    /// <summary>
    /// Reads tokens from the stream
    /// </summary>
    public class Lexer
    {
        /// <summary>
        /// Current line 
        /// </summary>
        public int Line { get; private set; } = 1;

        /// <summary>
        /// Table contains literals and keywords
        /// </summary>
        public Hashtable Table = new();

        /// <summary>
        /// Symbol stream to read symbols
        /// </summary>
        private readonly ISymbolStream<char> _symbolStream;

        /// <summary>
        /// Reserved keywords and sets symbol stream
        /// </summary>
        public Lexer(ISymbolStream<char> symbolStream)
        {
            _symbolStream = symbolStream;

            // Reserve keywords
            Reserve(WordToken.ProductionOperator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wordToken"></param>
        private void Reserve(WordToken wordToken) => Table.Add(wordToken.Lexeme, wordToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if operator symbol otherwise false</returns>
        private static bool IsOperatorSymbol(char symbol) => ":=".IndexOf(symbol) > -1; 

        /// <summary>
        /// Reads production operator
        /// </summary>
        /// <returns>Token representation of operator</returns>
        private Token ReadProductionOperator()
        {
            StringBuilder productionOperator = new();
            char buffer = _symbolStream.Peek();
            while (IsOperatorSymbol(buffer))
            {
                productionOperator.Append(buffer);
                buffer = _symbolStream.Next();
            }

            if (productionOperator.ToString() != "::=") 
                throw new SyntaxError($"Invalid operator at line {Line}. Expected \"::=\", actual \"{productionOperator}\"");

            return WordToken.ProductionOperator;
        }

        /// <summary>
        /// Reads next word at the stream
        /// </summary>
        /// <returns>Next token representing next word at the stream</returns>
        private Token ReadRuleID()
        {
            StringBuilder word = new();
            char buffer = _symbolStream.Peek();
            while (char.IsLetterOrDigit(buffer) || buffer == '-')
            {
                word.Append(buffer);
                buffer = _symbolStream.Next();
            }

            string stringWord = word.ToString();
            WordToken token = (WordToken)Table[stringWord];
            if (token != null) return token;

            token = new WordToken(stringWord, (int)TokenTag.RuleID);
            Reserve(token);
            return token;
        }

        private Token ReadLiteral()
        {
            StringBuilder literal = new();
            char quote = _symbolStream.Peek();
            char currentSymbol = _symbolStream.Next();

            while (currentSymbol != quote && currentSymbol != default)
            {
                literal.Append(currentSymbol);
                currentSymbol = _symbolStream.Next();
            }

            if (currentSymbol != quote) throw new SyntaxError($"Expected literal at line {Line}");
            _symbolStream.Next();
            return new WordToken(literal.ToString(), (int)TokenTag.Literal);
        }

        /// <summary>
        /// Skips whitespaces 
        /// </summary>
        private void SkipWhiteSpaces()
        {
            while (char.IsWhiteSpace(_symbolStream.Peek()))
            {
                if (_symbolStream.Peek() == '\n')
                {
                    Line++;
                    return;
                }
                _symbolStream.Next();
            }
        }

        /// <summary>
        /// Reads next token representing current lexeme at the stream
        /// </summary>
        /// <returns>Next token at the stream</returns>
        public Token GetNextToken()
        {
            SkipWhiteSpaces();
            char currentSymbol = _symbolStream.Peek();
            if (currentSymbol == default) return null;
            if (char.IsLetterOrDigit(currentSymbol) || currentSymbol == '-') return ReadRuleID();
            if (currentSymbol == ':') return ReadProductionOperator();
            if (currentSymbol == '"' || currentSymbol == '\'') return ReadLiteral();
            _symbolStream.Next();
            return new Token(currentSymbol);
        }
    }
}
