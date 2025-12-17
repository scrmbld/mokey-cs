using Interpreter;

namespace Repl
{
    public static class Repl
    {
        public static string PROMPT = ">> ";

        private static void LexLine(string line)
        {
            Lexer l = new Lexer(line);
            for (var token = l.NextToken(); token.Type != TokenType.Eof; token = l.NextToken())
            {
                Console.WriteLine($"{token.Type}, \"{token.Literal}\"");
            }

        }

        private static void ParseLine(string line)
        {
            Lexer l = new Lexer(line);
            Parser p = new Parser(l);
            Interpreter.Program prog = p.ParseProgram();
            foreach (string e in p.Errors())
            {
                Console.WriteLine(e);
            }
            Console.WriteLine(prog);
        }

        public static void Start()
        {
            Console.Write(PROMPT);
            while (true)
            {
                string? s = Console.ReadLine();
                if (s is { } line)
                {
                    ParseLine(line);
                }
                else
                {
                    break;
                }
                Console.Write(PROMPT);
            }
        }
    }
}
