using System.Text;

namespace Interpreter
{
    public interface Node
    {
        public string TokenLiteral();
    }

    static public class Precedences
    {
        public static int LOWEST = 0;
        public static int EQUALS = 1;
        public static int LESSGREATER = 2;
        public static int SUM = 3;
        public static int PRODUCT = 4;
        public static int PREFIX = 5;
        public static int CALL = 6;
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

        public override string ToString()
        {
            return $"(identifier ({Value}))";
        }
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
            StringBuilder sb = new StringBuilder($"(unary_operator\n  ({Operator})\n  ");
            StringBuilder valString = new StringBuilder(Value.ToString());
            valString.Replace("\n", "\n  ");
            sb.Append(valString);
            sb.Append(")");

            return sb.ToString();
        }
    }

    public class InfixOperator : Expression
    {
        public Token Tok;
        public string Operator;
        public Expression Lhs;
        public Expression Rhs;

        public InfixOperator(Token token, string op, Expression lhs, Expression rhs)
        {
            Tok = token;
            Operator = op;
            Lhs = lhs;
            Rhs = rhs;
        }

        public string TokenLiteral()
        {
            return Tok.Literal;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder($"(binary_operator\n  ({Operator})\n  ");
            StringBuilder lhsString = new StringBuilder(Lhs.ToString());
            lhsString.Replace("\n", "\n  ");
            StringBuilder rhsString = new StringBuilder(Rhs.ToString());
            rhsString.Replace("\n", "\n  ");
            sb.Append(lhsString.ToString());
            sb.Append("\n  ");
            sb.Append(rhsString.ToString());
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder($"(expression_statement\n");
            StringBuilder valSb = new StringBuilder(Value.ToString());
            valSb.Replace("\n", "\n  ");
            sb.Append($"{valSb})");
            return sb.ToString();
        }
    }

    public class Parser
    {
        private Dictionary<TokenType, Func<Expression?>> PrefixTable;
        private Dictionary<TokenType, Func<Expression, Expression?>> InfixTable;
        private Dictionary<TokenType, int> OperatorPrecedence = new Dictionary<TokenType, int> {
            {TokenType.Equal, 1},
            {TokenType.NotEqual, 1},
            {TokenType.Less, 2},
            {TokenType.Greater, 2},
            {TokenType.Plus, 3},
            {TokenType.Minus, 3},
            {TokenType.Asterisk, 4},
            {TokenType.Slash, 4}
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
            PrefixTable.Add(TokenType.Exclam, ParsePrefixOp);
            PrefixTable.Add(TokenType.Minus, ParsePrefixOp);
            InfixTable = new Dictionary<TokenType, Func<Expression, Expression?>>();
            InfixTable.Add(TokenType.Minus, ParseInfixOp);
            InfixTable.Add(TokenType.Plus, ParseInfixOp);
            InfixTable.Add(TokenType.Asterisk, ParseInfixOp);
            InfixTable.Add(TokenType.Slash, ParseInfixOp);
            InfixTable.Add(TokenType.Equal, ParseInfixOp);
            InfixTable.Add(TokenType.NotEqual, ParseInfixOp);
            InfixTable.Add(TokenType.Less, ParseInfixOp);
            InfixTable.Add(TokenType.Greater, ParseInfixOp);
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

            Expression? exp = ParseExpression(Precedences.LOWEST);

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
            Expression? exp = ParseExpression(Precedences.LOWEST);
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
            Expression? exp = ParseExpression(Precedences.LOWEST);
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

        private Expression? ParseExpression(int precedence)
        {
            try
            {
                Func<Expression?> prefix = PrefixTable[CurrentToken.Type];
                Expression? leftExp = prefix();
                if (leftExp == null)
                {
                    ErrorsList.Add($"Invalid expression {CurrentToken.Literal}...");
                    return null;
                }

                while (PeekToken.Type != TokenType.Semicolon && precedence < CurrentPrecedence())
                {
                    try
                    {
                        Func<Expression, Expression?> infix = InfixTable[CurrentToken.Type];

                        if (leftExp is { } lhs)
                        {
                            leftExp = infix(lhs);
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        return leftExp;
                    }
                }

                return leftExp;
            }
            catch (KeyNotFoundException)
            {
                ErrorsList.Add($"{CurrentToken.Literal} is not prefix/left side value for expression");
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
            string opType = CurrentToken.Literal;

            NextToken();
            Expression? rhs = ParseExpression(Precedences.PREFIX);

            if (rhs is { } r)
            {
                return new PrefixOperator(opToken, opType, r);
            }

            return null;
        }

        private InfixOperator? ParseInfixOp(Expression lhs)
        {
            Token opToken = CurrentToken;
            string opType = CurrentToken.Literal;

            int precedence = CurrentPrecedence();
            NextToken();
            Expression? rhs = ParseExpression(precedence);

            if (rhs is { } r)
            {
                return new InfixOperator(opToken, opType, lhs, rhs);
            }

            return null;
        }

        private int PeekPrecedence()
        {
            try
            {
                return OperatorPrecedence[PeekToken.Type];
            }
            catch (KeyNotFoundException)
            {
                return 0;
            }
        }

        private int CurrentPrecedence()
        {
            try
            {
                return OperatorPrecedence[CurrentToken.Type];
            }
            catch (KeyNotFoundException)
            {
                return 0;
            }
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
