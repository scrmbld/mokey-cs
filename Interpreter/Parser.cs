using System.Text;

namespace Interpreter
{
    public enum OperatorType
    {
        Lowest,
        Equals,
        LessGreater,
        Sum,
        Product,
        Prefix,
        Call
    }

    public interface Node
    {
        public string TokenLiteral();
    }

    public interface Statement : Node { }

    public interface Expression : Node { }

    public class Program : Node
    {
        public List<Statement> Statements = new List<Statement>();
        public string TokenLiteral()
        {
            if (Statements.Count > 0)
            {
                return Statements[0].TokenLiteral();
            }
            else
            {
                return "";
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("(block");
            foreach (Statement stmt in Statements)
            {
                sb.Append("\n  ");
                StringBuilder field = new StringBuilder(stmt.ToString());
                field.Replace("\n", "\n  ");
                sb.Append(field);
            }

            sb.Append(")");
            return sb.ToString();
        }
    }

    public class Identifier : Expression
    {
        public Token Tok;
        public string Value;

        public Identifier(Token token, string value)
        {
            Tok = token;
            Value = value;
        }

        public string TokenLiteral()
        {
            return Tok.Literal;
        }

        // public override string ToString()
        // {
        //     return $"(identifier ({Value}))";
        // }
    }

    public class IntLiteral : Expression
    {
        public Token Tok;
        public int Value;
        public IntLiteral(Token token, int value)
        {
            Tok = token;
            Value = value;
        }

        public string TokenLiteral()
        {
            return Tok.Literal;
        }

        public override string ToString()
        {
            return $"(int ({Value}))";
        }
    }

    public class PrefixOperator : Expression
    {
        public Token Tok;
        public string Operator;
        public Expression Value;

        public PrefixOperator(Token token, string op, Expression value)
        {
            Tok = token;
            Operator = op;
            Value = value;
        }

        public string TokenLiteral()
        {
            return Tok.Literal;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder($"(unary_{Operator}\n");
            StringBuilder valString = new StringBuilder(Value.ToString());
            valString.Replace("\n", "\n  ");
            sb.Append(valString);
            sb.Append(")");

            return sb.ToString();
        }
    }

    public class LetStatement : Statement
    {
        public Token Tok;
        public Identifier Name;
        public Expression Value;

        public LetStatement(Token token, Identifier name, Expression value)
        {
            Tok = token;
            Name = name;
            Value = value;
        }

        public string TokenLiteral()
        {
            return Tok.Literal;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("(let\n");
            sb.Append("  Right: ");
            sb.AppendLine(Name.ToString());
            sb.Append("  Left: ");
            sb.AppendLine(Value.ToString());
            sb.Append(")");
            return sb.ToString();
        }
    }

    public class ReturnStatement : Statement
    {
        public Token Tok;
        public Expression Value;

        public ReturnStatement(Token t, Expression value)
        {
            Tok = t;
            Value = value;
        }

        public string TokenLiteral()
        {
            return Tok.Literal;
        }

        public override string ToString()
        {
            return $"(return_statement\n  {Value})";
        }
    }

    public class ExpressionStatement : Statement
    {
        public Token Tok;
        public Expression Value;

        public ExpressionStatement(Token t, Expression value)
        {
            Tok = t;
            Value = value;
        }

        public string TokenLiteral()
        {
            return Tok.Literal;
        }

        // public override string ToString()
        // {
        //     return $"(expression_statement\n  {Value.ToString()})";
        // }
    }

    public class Parser
    {
        private Dictionary<TokenType, Func<Expression?>> PrefixTable;
        private Dictionary<TokenType, Func<Expression, Expression?>> InfixTable;
        private List<OperatorType> OperatorPrecedence = new List<OperatorType> {
            OperatorType.Lowest,
            OperatorType.Equals,
            OperatorType.LessGreater,
            OperatorType.Sum,
            OperatorType.Product,
            OperatorType.Prefix,
            OperatorType.Call
        };

        private Lexer Lex;
        private Token CurrentToken;
        private Token PeekToken;
        private List<String> ErrorsList;

        public Parser(Lexer lexer)
        {
            Lex = lexer;

            ErrorsList = new List<string>();

            NextToken();
            NextToken();

            PrefixTable = new Dictionary<TokenType, Func<Expression?>>();
            PrefixTable.Add(TokenType.Identifier, ParseIdent);
            PrefixTable.Add(TokenType.Int, ParseInt);
            InfixTable = new Dictionary<TokenType, Func<Expression, Expression?>>();
        }

        private void NextToken()
        {
            CurrentToken = PeekToken;
            PeekToken = Lex.NextToken();
        }
        public Program ParseProgram()
        {
            Program program = new Program();
            ErrorsList = new List<string>();

            while (CurrentToken.Type != TokenType.Eof)
            {
                Statement? stmt = ParseStatement();
                if (stmt is { } s)
                {
                    program.Statements.Add(s);
                }
                NextToken();
            }
            return program;
        }

        private Statement? ParseStatement()
        {
            return CurrentToken.Type switch
            {
                TokenType.Let => ParseLetStatement(),
                TokenType.Return => ParseReturnStatement(),
                _ => ParseExpressionStatement()
            };
        }

        private Statement? ParseLetStatement()
        {
            Token letToken = CurrentToken;

            if (!ExpectPeek(TokenType.Identifier))
            {
                return null;
            }

            Identifier varName = new Identifier(CurrentToken, CurrentToken.Literal);

            if (!ExpectPeek(TokenType.Assign))
            {
                return null;
            }

            NextToken();

            Expression? exp = ParseExpression();

            if (exp is { } e)
            {
                if (CurrentToken.Type == TokenType.Semicolon)
                {
                    return new LetStatement(letToken, varName, exp);
                }
                else
                {
                    PeekError(TokenType.Semicolon);
                }
            }
            return null;
        }

        private Statement? ParseReturnStatement()
        {
            Token returnToken = CurrentToken;
            NextToken();
            Expression? exp = ParseExpression();
            if (exp is { } e)
            {
                if (CurrentToken.Type == TokenType.Semicolon)
                {
                    return new ReturnStatement(returnToken, e);
                }
                else
                {
                    PeekError(TokenType.Semicolon);
                }
            }

            return null;
        }

        private Statement? ParseExpressionStatement()
        {
            Token expressionToken = CurrentToken;
            Expression? exp = ParseExpression();
            if (exp is { } e)
            {
                // semicolons are optional for expression statements
                if (CurrentToken.Type == TokenType.Semicolon)
                {
                    NextToken();
                }

                return new ExpressionStatement(expressionToken, e);
            }

            return null;
        }

        // TODO: this
        private Expression? ParseExpression()
        {
            try
            {
                Func<Expression?> prefix = PrefixTable[CurrentToken.Type];
                Expression? left_exp = prefix();
                return left_exp;
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        private IntLiteral ParseInt()
        {
            IntLiteral result = new IntLiteral(CurrentToken, int.Parse(CurrentToken.Literal));
            NextToken();
            return result;
        }

        private Identifier ParseIdent()
        {
            Identifier result = new Identifier(CurrentToken, CurrentToken.Literal);
            NextToken();
            return result;
        }

        private PrefixOperator? ParsePrefixOp()
        {
            Token opToken = CurrentToken;
            string? opType = CurrentToken.Type switch
            {
                TokenType.Minus => "-",
                TokenType.Not => "!",
                _ => null
            };
            if (opType is { } op)
            {
                NextToken();
                Expression? rhs = ParseExpression();
                if (rhs is { } r)
                {
                    return new PrefixOperator(opToken, opType, r);
                }
                else
                {
                    ErrorsList.Add($"Invalid expression at {CurrentToken.Literal}");
                }
            }
            else
            {
                ErrorsList.Add($"Expected unary operator, found {CurrentToken.Type}");
            }

            return null;
        }

        /// <summary>
        /// Advances the parser if and only if the peek token has the given type.
        /// </summary>
        private bool ExpectPeek(TokenType t)
        {
            if (PeekToken.Type == t)
            {
                NextToken();
                return true;
            }
            else
            {
                PeekError(t);
                return false;
            }
        }

        private void PeekError(TokenType t)
        {
            if (PeekToken.Type == TokenType.Eof)
            {
                ErrorsList.Add($"expected {t}, got Eof");
            }
            else
            {
                ErrorsList.Add($"expected {t}, got {PeekToken.Type}");
            }
        }

        public List<string> Errors()
        {
            return ErrorsList;
        }
    }
}
