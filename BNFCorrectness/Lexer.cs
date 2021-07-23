using SymbolStreams;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System;

namespace BNFCorrectness
{
    /// <summary>
    /// Represents grammar context
    /// </summary>
    public enum Context
    {
        RuleName,
        RuleExpression
    }

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
        public Hashtable Table { get; private set; } = new();

        /// <summary>
        /// Symbol stream to read symbols
        /// </summary>
        private readonly ISymbolStream<char> _symbolStream;

        /// <summary>
        /// Grammar context
        /// </summary>
        private Context _context = Context.RuleName;

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
        /// Reserves token at rules table
        /// </summary>
        private void Reserve(WordToken wordToken) => Table.Add(wordToken.Lexeme, wordToken);

        /// <summary>
        /// Checks if symbol is operator symbol
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

            _context = Context.RuleExpression;
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
            if (_context == Context.RuleName) Reserve(token);
            return token;
        }

        /// <summary>
        /// Reads next literal at the stream
        /// </summary>
        /// <returns>Token representation of next literal</returns>
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
        /// Reads next regular expression as a term at the stream
        /// </summary>
        /// <returns>Token representing regular expression</returns>
        private Token ReadRegex()
        {
            StringBuilder regexBuilder = new();
            char currentSymbol = _symbolStream.Peek();
            regexBuilder.Append(currentSymbol);
            currentSymbol = _symbolStream.Next();
            int nesting = 1;

            while (nesting != 0 && currentSymbol != default)
            {
                if (currentSymbol == '(') nesting++;
                else if (currentSymbol == ')') nesting--;

                regexBuilder.Append(currentSymbol);
                currentSymbol =_symbolStream.Next();
            }

            string regex = regexBuilder.ToString();

            try
            {
                Regex regexCheck = new(regex);
            }
            catch (ArgumentException)
            {
                throw new SyntaxError($"Invalid regular expression at line {Line}");
            }

            return new WordToken(regex, (int)TokenTag.Regex);

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
        /// Skips comment
        /// </summary>
        private void SkipComment()
        {
            char currentSymbol = _symbolStream.Next();
            while (currentSymbol != '%' && currentSymbol != default) currentSymbol = _symbolStream.Next();
            _symbolStream.Next();
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
            if (currentSymbol == '(') return ReadRegex();
            if (currentSymbol == '\n') _context = Context.RuleName;
            if (currentSymbol == '%')
            {
                SkipComment();
                return GetNextToken();
            }
            _symbolStream.Next();
            return new Token(currentSymbol);
        }
    }
}
