using Xunit;
using System.Collections.Generic;
using SymbolStreams;

namespace BNFCorrectness.Tests
{
    public class SyntaxAnalyserTests
    {
        [Fact]
        public void CorrectSyntaxTest()
        {
            string testCase = "<rule> ::= <hello-world> | 'aboba'\n <hello-world> ::= 'Hello' | 'World'";
            List<Token> parsedTokens = new();
            Lexer lexer = new(new LoadedStream<char>(testCase.ToCharArray()));

            Token currentToken = lexer.GetNextToken();

            while (currentToken != null)
            {
                parsedTokens.Add(currentToken);
                currentToken = lexer.GetNextToken();
            }

            SyntaxAnalyser syntaxAnalyser = new(lexer.Table, parsedTokens.ToArray());
            syntaxAnalyser.Parse();
        }

        [Fact]
        public void RegexSyntaxTest()
        {
            string testCase = "<rule> ::= ([a-z]|[A-Z]) | (abs[a-z]?)";
            List<Token> parsedTokens = new();
            Lexer lexer = new(new LoadedStream<char>(testCase.ToCharArray()));

            Token currentToken = lexer.GetNextToken();

            while (currentToken != null)
            {
                parsedTokens.Add(currentToken);
                currentToken = lexer.GetNextToken();
            }

            SyntaxAnalyser syntaxAnalyser = new(lexer.Table, parsedTokens.ToArray());
            syntaxAnalyser.Parse();
        }
    }
}
