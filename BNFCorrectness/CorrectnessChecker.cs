using System;
using System.IO;
using System.Collections.Generic;
using SymbolStreams;

namespace BNFCorrectness
{
    class CorrectnessChecker
    {
        public static void Main(string[] args)
        {
            string grammarFile = args[0];

            if (Path.GetExtension(grammarFile) != ".bnf")
            {
                Console.WriteLine("Invalid file extension or file does not exist");
                return;
            }

            try
            {
                StreamReader reader = new(grammarFile);
                Lexer lexer = new(new LoadedStream<char>(reader.ReadToEnd().ToCharArray()));

                List<Token> parsedTokens = new();
                Token currentToken = lexer.GetNextToken();

                while (currentToken != null)
                {
                    parsedTokens.Add(currentToken);
                    currentToken = lexer.GetNextToken();
                }

                SyntaxAnalyser syntaxAnalyser = new(lexer.Table, parsedTokens.ToArray());
                syntaxAnalyser.Parse();
            }
            catch (SyntaxError ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
