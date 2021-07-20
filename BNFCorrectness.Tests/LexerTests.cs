using System;
using SymbolStreams;
using System.Collections.Generic;
using Xunit;

namespace BNFCorrectness.Tests
{
    public class LexerTests
    {
        [Fact]
        public void CorrectTokenBuildTest()
        {
            // Arrange 
            string testCase = "<rule> ::= <another-rule> | \"\"\n";
            Lexer lexer = new(new LoadedStream<char>(testCase.ToCharArray()));
            Token[] expectedTokens =
            {
                new Token('<'),
                new WordToken("rule", (int)TokenTag.RuleID),
                new Token('>'),
                WordToken.ProductionOperator,
                new Token('<'),
                new WordToken("another-rule", (int)TokenTag.RuleID),
                new Token('>'),
                new Token('|'),
                new WordToken("", (int)TokenTag.Literal),
                new Token('\n')
            };

            // Act
            List<Token> actualTokens = new();
            Token currentToken = lexer.GetNextToken();

            while (currentToken != null)
            {
                actualTokens.Add(currentToken);
                currentToken = lexer.GetNextToken();
            }

            // Assert
            Assert.True(IsArraysEqual(expectedTokens, actualTokens.ToArray()));
        }

        private static bool IsArraysEqual<T>(T[] arrayA, T[] arrayB)
        {
            if (arrayA.Length != arrayB.Length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < arrayA.Length; i++)
                    if (arrayA[i].ToString() != arrayB[i].ToString())
                        return false;
                return true;
            }
        }

        [Fact]
        public void SyntaxErrorThrownTest()
        {
            Action actionCode = SyntaxErrorThrownActionCode;
            Assert.Throws<SyntaxError>(actionCode);
        }

        private void SyntaxErrorThrownActionCode()
        {
            string testCase = "< := :>";
            Lexer lexer = new(new LoadedStream<char>(testCase.ToCharArray()));

            while (lexer.GetNextToken() != null) lexer.GetNextToken();
        }

        [Fact]
        public void LiteralReadTest()
        {
            // Arrange
            string testCase = "<rule> ::= \"Hello\" | 'World'";
            Lexer lexer = new(new LoadedStream<char>(testCase.ToCharArray()));
            Token[] expectedTokens =
            {
                new Token('<'),
                new WordToken("rule", (int)TokenTag.RuleID),
                new Token('>'),
                WordToken.ProductionOperator,
                new WordToken("Hello", (int)TokenTag.Literal),
                new Token('|'),
                new WordToken("World", (int)TokenTag.Literal),
            };

            // Act
            List<Token> actualTokens = new();
            Token currentToken = lexer.GetNextToken();

            while (currentToken != null)
            {
                actualTokens.Add(currentToken);
                currentToken = lexer.GetNextToken();
            }

            // Assert
            Assert.True(IsArraysEqual(expectedTokens, actualTokens.ToArray()));
        }
    }
}
