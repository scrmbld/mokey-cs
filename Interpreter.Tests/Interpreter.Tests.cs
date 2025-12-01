namespace Interpreter.UnitTests
{
    /// <summary>
    /// Tests lexing for all of the tokens that should just be a single char
    /// </summary>
    public class OperatorShould
    {
        [Fact]
        public void LexerEq()
        {
            Lexer l = new Lexer("=");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Assign, "="), result);
        }

        [Fact]
        public void LexerEqual()
        {
            Lexer l = new Lexer("==");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Equal, "=="), result);
        }

        [Fact]
        public void LexerNotEqual()
        {
            Lexer l = new Lexer("!=");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.NotEqual, "!="), result);
        }

        [Fact]
        public void LexerLess()
        {
            Lexer l = new Lexer("<");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Less, "<"), result);
        }

        [Fact]
        public void LexerGreater()
        {
            Lexer l = new Lexer(">");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Greater, ">"), result);
        }

        [Fact]
        public void LexerNot()
        {
            Lexer l = new Lexer("!");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Not, "!"), result);
        }

        [Fact]
        public void LexerPlus()
        {
            Lexer l = new Lexer("+");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Plus, "+"), result);
        }

        [Fact]
        public void LexerMinus()
        {
            Lexer l = new Lexer("-");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Minus, "-"), result);
        }

        [Fact]
        public void LexerTimes()
        {
            Lexer l = new Lexer("*");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Times, "*"), result);
        }

        [Fact]
        public void LexerDivide()
        {
            Lexer l = new Lexer("/");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Divide, "/"), result);
        }

    }

    public class DelimiterShould
    {
        [Fact]
        public void LexerSemi()
        {
            Lexer l = new Lexer(";");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Semicolon, ";"), result);
        }

        [Fact]
        public void LexerLParen()
        {
            Lexer l = new Lexer("(");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.LParen, "("), result);
        }

        [Fact]
        public void LexerRParen()
        {
            Lexer l = new Lexer(")");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.RParen, ")"), result);
        }

        [Fact]
        public void LexerLBrace()
        {
            Lexer l = new Lexer("{");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.LBrace, "{"), result);
        }

        [Fact]
        public void LexerRBrace()
        {
            Lexer l = new Lexer("}");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.RBrace, "}"), result);
        }

        [Fact]
        public void LexerComma()
        {
            Lexer l = new Lexer(",");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Comma, ","), result);
        }

        [Fact]
        public void LexerEof()
        {
            Lexer l = new Lexer("");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Eof, ""), result);
        }

    }

    public class LexerStrTokShould
    {
        [Fact]
        public void LexerLet()
        {
            Lexer l = new Lexer("let");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Let, "let"), result);
        }

        [Fact]
        public void LexerIf()
        {
            Lexer l = new Lexer("if");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.If, "if"), result);
        }

        [Fact]
        public void LexerElse()
        {
            Lexer l = new Lexer("else");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Else, "else"), result);
        }

        [Fact]
        public void LexerTrue()
        {
            Lexer l = new Lexer("true");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.True, "true"), result);
        }

        [Fact]
        public void LexerFalse()
        {
            Lexer l = new Lexer("false");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.False, "false"), result);
        }

        [Fact]
        public void LexerReturn()
        {
            Lexer l = new Lexer("return");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Return, "return"), result);
        }

        [Fact]
        public void LexerIdent()
        {
            Lexer l = new Lexer("five");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Identifier, "five"), result);
        }

        [Fact]
        public void LexerInt()
        {
            Lexer l = new Lexer("5");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Int, "5"), result);
        }

        [Fact]
        public void LexerFunction()
        {
            Lexer l = new Lexer("fn");
            Token result = l.NextToken();
            Assert.Equivalent(new Token(TokenType.Function, "fn"), result);
        }
    }

    public class CompoundShould()
    {
        [Fact]
        void LexerLet()
        {
            string program = @"let five = 5;";
            List<Token> expected = new List<Token>
            {
              new Token(TokenType.Let, "let"),
              new Token(TokenType.Identifier, "five"),
              new Token(TokenType.Assign, "="),
              new Token(TokenType.Int, "5"),
              new Token(TokenType.Semicolon, ";"),
              new Token(TokenType.Eof, "")
            };

            Lexer l = new Lexer(program);
            List<Token> result = new List<Token>();

            for (Token t = l.NextToken(); t.Type != TokenType.Eof; t = l.NextToken())
            {
                result.Append(t);
            }

            Assert.Equivalent(result, expected);
        }

        [Fact]
        void LexerFn()
        {
            string program = @"let add = fn(x, y) { return x + y; };";
            List<Token> expected = new List<Token>
            {
              new Token(TokenType.Let, "let"),
              new Token(TokenType.Identifier, "add"),
              new Token(TokenType.Assign, "="),
              new Token(TokenType.Int, "fn"),
              new Token(TokenType.LParen, "("),
              new Token(TokenType.Identifier, "x"),
              new Token(TokenType.Identifier, "y"),
              new Token(TokenType.RParen, ")"),
              new Token(TokenType.LBrace, "{"),
              new Token(TokenType.Return, "return"),
              new Token(TokenType.Identifier, "x"),
              new Token(TokenType.Plus, "+"),
              new Token(TokenType.Identifier, "y"),
              new Token(TokenType.RBrace, "}"),
              new Token(TokenType.Eof, "")
            };

            Lexer l = new Lexer(program);
            List<Token> result = new List<Token>();

            for (Token t = l.NextToken(); t.Type != TokenType.Eof; t = l.NextToken())
            {
                result.Append(t);
            }

            Assert.Equivalent(result, expected);
        }
    }
}
