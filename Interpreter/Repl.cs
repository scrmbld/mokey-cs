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

        public static void Start()
        {
            Console.Write(PROMPT);
            while (true)
            {
                string? s = Console.ReadLine();
                if (s is { } line)
                {
                    LexLine(line);
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
