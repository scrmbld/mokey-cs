namespace Interpreter
{
    public enum TokenType
    {
        Illegal,
        Eof,

        // identifiers and literals
        Identifier,
        Int,

        // operators
        Assign,
        Equal,
        Not,
        NotEqual,
        Less,
        Greater,
        Plus,
        Minus,
        Times,
        Divide,

        // delimiters
        Comma,
        Semicolon,
        LParen,
        RParen,
        LBrace,
        RBrace,

        // keywords
        Function,
        Let,
        True,
        False,
        If,
        Else,
        Return
    }

    public struct Token
    {
        public TokenType Type;
        public string Literal;

        public Token(TokenType t, string lit)
        {
            Type = t;
            Literal = lit;
        }
    }

    public class Lexer
    {
        private string Source;
        // index we just read
        private int Position;
        // index we will read next
        private int ReadPosition;
        // the previously read character
        char ch;

        public Lexer(string text)
        {
            Source = text;
            ReadPosition = 0;
            Position = 0;
            ReadChar();
        }

        private void ReadChar()
        {
            if (ReadPosition >= Source.Length)
            {
                ch = '\0';
                Position = ReadPosition;
            }
            else
            {
                ch = Source[ReadPosition];
                Position = ReadPosition;
                ReadPosition++;
            }
        }

        private char PeekChar()
        {
            if (ReadPosition >= Source.Length)
            {
                return '\0';
            }
            else
            {
                return Source[ReadPosition];
            }
        }

        private string ReadIdentifier()
        {
            int startPos = Position;

            while (Char.IsLetter(ch))
            {
                ReadChar();
            }

            return Source.Substring(startPos, Position - startPos);
        }

        private string ReadInt()
        {
            int startPos = Position;

            while (Char.IsDigit(ch))
            {
                ReadChar();
            }

            return Source.Substring(startPos, Position - startPos);
        }

        private void ConsumeWhitespace()
        {
            while (String.IsNullOrWhiteSpace(ch.ToString()) && ch != '\0')
            {
                ReadChar();
            }
        }

        public Token NextToken()
        {
            ConsumeWhitespace();

            Token? tok = ch switch
            {
                '=' => PeekChar() switch
                {
                    '=' => new Token(TokenType.Equal, "=="),
                    _ => new Token(TokenType.Assign, ch.ToString())
                },
                '<' => new Token(TokenType.Less, ch.ToString()),
                '>' => new Token(TokenType.Greater, ch.ToString()),
                '!' => PeekChar() switch
                {
                    '=' => new Token(TokenType.NotEqual, "!="),
                    _ => new Token(TokenType.Not, ch.ToString())
                },
                '+' => new Token(TokenType.Plus, ch.ToString()),
                '-' => new Token(TokenType.Minus, ch.ToString()),
                '*' => new Token(TokenType.Times, ch.ToString()),
                '/' => new Token(TokenType.Divide, ch.ToString()),
                ';' => new Token(TokenType.Semicolon, ch.ToString()),
                '(' => new Token(TokenType.LParen, ch.ToString()),
                ')' => new Token(TokenType.RParen, ch.ToString()),
                '{' => new Token(TokenType.LBrace, ch.ToString()),
                '}' => new Token(TokenType.RBrace, ch.ToString()),
                ',' => new Token(TokenType.Comma, ch.ToString()),
                '\0' => new Token(TokenType.Eof, ""),
                _ => null,
            };

            {
                if (tok is { } result)
                {
                    ReadChar();
                    return result;
                }
            }

            if (Char.IsLetter(ch))
            {
                string literal = ReadIdentifier();
                return literal switch
                {
                    "let" => new Token(TokenType.Let, literal),
                    "fn" => new Token(TokenType.Function, literal),
                    "if" => new Token(TokenType.If, literal),
                    "else" => new Token(TokenType.Else, literal),
                    "true" => new Token(TokenType.True, literal),
                    "false" => new Token(TokenType.False, literal),
                    "return" => new Token(TokenType.Return, literal),
                    _ => new Token(TokenType.Identifier, literal),
                };
            }
            else if (Char.IsDigit(ch))
            {
                string literal = ReadInt();
                return new Token(TokenType.Int, literal);
            }
            else
            {
                return new Token(TokenType.Illegal, ch.ToString());
            }
        }
    }
}
