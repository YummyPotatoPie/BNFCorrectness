using System;
using System.Collections;

namespace BNFCorrectness
{
    public class SyntaxAnalyser
    {
        /// <summary>
        /// Table of reserved grammar rules
        /// </summary>
        private readonly Hashtable _table;

        /// <summary>
        /// Tokens that lexer parsed
        /// </summary>
        private readonly Token[] _parsedTokens;

        /// <summary>
        /// Current token at the stream
        /// </summary>
        private Token _currentToken;

        /// <summary>
        /// Position at the stream of token
        /// </summary>
        private int _currentTokenPosition = 0;

        /// <summary>
        /// Current rule line
        /// </summary>
        private int _line = 1;

        /// <summary>
        /// Sets table of rules and parsed tokens
        /// </summary>
        /// <param name="table">Table of rules</param>
        /// <param name="parsedTokens">Tokens array that lexer parsed</param>
        public SyntaxAnalyser(Hashtable table, Token[] parsedTokens)
        {
            _table = table;
            _parsedTokens = parsedTokens;
        }

        /// <summary>
        /// Match the current token tag and the token tag to match
        /// </summary>
        /// <param name="tokenTag">The next token tag to be matched</param>
        private void Match(int tokenTag)
        {
            if (_currentTokenPosition == _parsedTokens.Length) throw new SyntaxError("Unexpected end of grammar");

            if (tokenTag == _currentToken.Tag)
            {
                if (_currentTokenPosition == _parsedTokens.Length - 1)
                {
                    _currentTokenPosition++;
                    return;
                }
                _currentToken = _parsedTokens[++_currentTokenPosition];
            }
            else throw new SyntaxError($"Unexpected lexeme at line {_line}");
        }

        /// <summary>
        /// Parsed grammar
        /// </summary>
        public void Parse()
        {
            
            if (_parsedTokens.Length == 0)
            {
                Console.WriteLine("Empty grammar");
                return;
            }
            _currentToken = _parsedTokens[_currentTokenPosition];
            Rules();
        }

        /// <summary>
        /// Rules production method
        /// </summary>
        private void Rules()
        {
            if (_currentTokenPosition != _parsedTokens.Length)
            {
                Rule(); Rules();
            }
        }

        /// <summary>
        /// Rule production method
        /// </summary>
        public void Rule()
        {
            if (_currentTokenPosition == _parsedTokens.Length) return;
            if (_currentToken.Tag == '\n')
            {
                Match('\n'); _line++; Rule(); return;
            }
            Match('<'); RuleName(); Match('>'); Match((int)TokenTag.ProductionOperator); RuleExpression();
            if (_currentTokenPosition == _parsedTokens.Length) return;
            Match('\n'); _line++;
        }

        /// <summary>
        /// Rule expression production method
        /// </summary>
        private void RuleExpression()
        {
            TermList();
            if (_currentToken.Tag == '|')
            {
                Match('|');
                RuleExpression();
            }
            if (_currentToken.Tag == '\n') return;
        }

        /// <summary>
        /// Name of rule production method
        /// </summary>
        private void RuleName()
        {
            WordToken wordToken;
            if (_currentToken is WordToken token) wordToken = token;
            else throw new SyntaxError($"Unexpected lexem at line {_line} expected [rule-name]");

            if (wordToken.Tag == (int)TokenTag.RuleID && _table[wordToken.Lexeme] != null)
            {
                Match((int)TokenTag.RuleID);
                return;
            }
            throw new SyntaxError($"Unknown rule at line {_line}: {wordToken.Lexeme}");
        }

        /// <summary>
        /// List of terms production method
        /// </summary>
        private void TermList()
        {
            if (_currentToken.Tag == '|' || _currentToken.Tag == '\n' || _currentTokenPosition == _parsedTokens.Length) return;
            Term(); TermList();
        }

        /// <summary>
        /// Term production method
        /// </summary>
        private void Term()
        {
            if (_currentToken.Tag == (int)TokenTag.Literal)
            {
                Match((int)TokenTag.Literal);
                return;
            }
            Match('<'); RuleName(); Match('>');
        }
    }
}
