using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 *@author wanghesai
 *@time 2014/11/13-2014/11/15 
 *@time 2016/6/21 fix bugs and add library
 *@name JSInterpret
 ***/
namespace JSInterpret
{
    enum State
    {
        START, END, NUM, STRING, BOOLEAN, IDENTIFY
    }
    public static partial class JavaScript
    {
        public static Dictionary<string, TokenType> keyword = new Dictionary<string, TokenType>();
        public static string[] keywords = { 
                                              "for", "while", "function", 
                                              "if", "else", "var","return","new","in","this",
                                              "instanceof","typeof","undefined","break",
                                              "import","class","let","const","switch",
                                              "default","continue"
                                              /* "try","catch","throw","public","private","with" */
                                              /* "static","native","extends","enum","abstract" */
                                          };
        public static TokenType[] tokenType = { 
                                        TokenType.FOR, TokenType.WHILE, 
                                        TokenType.FUNCTION, TokenType.IF, 
                                        TokenType.ELSE, TokenType.VAR ,
                                        TokenType.RETURN,TokenType.NEW,
                                        TokenType.IN,TokenType.THIS,
                                        TokenType.INSTANCEOF,TokenType.TYPEOF,
                                        TokenType.UNDEFINED,TokenType.BREAK,
                                        TokenType.IMPORT,TokenType.CLASS,
                                        TokenType.LET,TokenType.CONST,
                                        TokenType.SWITCH,TokenType.DEFAULT,
                                        TokenType.CONTINUE
                                              };
        public static string Join(this string sep, IEnumerable<Object> tokens)
        {
            return string.Join(sep, tokens);
        }
        public static string PrettyPrint(string[] tokens)
        {
            return "[" + ", ".Join(tokens.Select(s => "'" + s + "'")) + "]";
        }
        public static bool isKeyword(string identify)
        {
            foreach (var item in keywords)
            {
                if (identify.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool isDelim(char ch)
        {
            char[] cDelims = { ',', '(', ')', '[', ']', ';', ':', '=','!','|','<', '>', '+', '-', '*', '/', '&', '{', '}' };
            foreach (var item in cDelims)
            {
                if (ch == item)
                {
                    return true;
                }
            }
            return false;
        }
        public static List<Token> Tokenizer(string code)
        {
            List<Token> tokenList = new List<Token>();
            int count = 0;
            StringBuilder sb = new StringBuilder();
            State state = State.START;
            while (count < code.Length)
            {
                bool isLexeme = false;
                TokenType currentType = 0;
                switch (state)
                {
                    case State.START:
                        if (Char.IsWhiteSpace(code[count]))
                        {
                            count++;
                            continue;
                        }
                        else if (Char.IsLetter(code[count]) || code[count] == '_')
                        {
                            state = State.IDENTIFY;
                            continue;
                        }
                        else if (Char.IsDigit(code[count]))
                        {
                            state = State.NUM;
                            continue;
                        }
                        else if (code[count] == '\'' || code[count] == '"')
                        {
                            state = State.STRING;
                            //count++;
                            continue;
                        }
                        else
                        {
                            switch ((code[count]))
                            {
                                case '.':
                                    currentType = TokenType.POINT;
                                    break;
                                case '+':
                                    if (code[count + 1] != '+')
                                        currentType = TokenType.ADD;
                                    else
                                    {
                                        sb.Append(code[count]);
                                        count++;
                                        currentType = TokenType.PLUS_PLUS;
                                    }
                                    break;
                                case '-':
                                    if (code[count + 1] != '-')
                                        currentType = TokenType.SUB;
                                    else
                                    {
                                        sb.Append(code[count]);
                                        count++;
                                        currentType = TokenType.SUB_SUB;
                                    }
                                    break;
                                case '*':
                                    currentType = TokenType.MUL;
                                    break;
                                case '/':
                                    currentType = TokenType.DIV;
                                    break;
                                case '{':
                                    currentType = TokenType.OpenBrace;
                                    break;
                                case '}':
                                    currentType = TokenType.CloseBrace;
                                    break;
                                case '[':
                                    currentType = TokenType.OpenBracket;
                                    break;
                                case ']':
                                    currentType = TokenType.CloseBracket;
                                    break;
                                case '(':
                                    currentType = TokenType.OpenParenthese;
                                    break;
                                case ')':
                                    currentType = TokenType.CloseParenthese;
                                    break;
                                case ':':
                                    currentType = TokenType.COLON;
                                    break;
                                case ';':
                                    currentType = TokenType.SemiColon;
                                    break;
                                case '=':
                                    if (code[count + 1] != '=')
                                        currentType = TokenType.BIND;
                                    else
                                    {
                                        sb.Append(code[count]);
                                        count++;
                                        currentType = TokenType.EQ;
                                    }
                                    break;
                                case '>':
                                    if (code[count + 1] != '=')
                                        currentType = TokenType.GT;
                                    else
                                    {
                                        sb.Append(code[count]);
                                        count++;
                                        currentType = TokenType.GE;
                                    }
                                    break;
                                case '<':
                                    if (code[count + 1] != '=')
                                        currentType = TokenType.LT;
                                    else
                                    {
                                        sb.Append(code[count]);
                                        count++;
                                        currentType = TokenType.LE;
                                    }
                                    break;
                                case '&':
                                    if (code[count + 1] != '&')
                                        currentType = TokenType.BIT_AND;
                                    else
                                    {
                                        sb.Append(code[count]);
                                        count++;
                                        currentType = TokenType.AND;
                                    }
                                    break;
                                case '!':
                                    if (code[count + 1] != '=')
                                        currentType = TokenType.NOT;
                                    else
                                    {
                                        sb.Append(code[count]);
                                        count++;
                                        currentType = TokenType.UNEQ;
                                    }
                                    break;
                                case '|':
                                    if (code[count + 1] != '|')
                                        currentType = TokenType.BIT_OR;
                                    else
                                    {
                                        sb.Append(code[count]);
                                        count++;
                                        currentType = TokenType.OR;
                                    }
                                    break;
                                case ',':
                                    currentType = TokenType.COMMA;
                                    break;
                            }
                            isLexeme = true;
                        }
                        sb.Append(code[count]);
                        count++;
                        break;
                    case State.NUM:
                        if (Char.IsDigit(code[count]))
                        {
                            sb.Append(code[count]);
                            state = State.NUM;
                            count++;
                        }
                        else if (isDelim(code[count]) || Char.IsWhiteSpace(code[count]))
                        {
                            isLexeme = true;
                        }
                        break;
                    case State.IDENTIFY:
                        if (Char.IsLetterOrDigit(code[count]) || code[count] == '_')
                        {
                            sb.Append(code[count]);
                            state = State.IDENTIFY;
                            count++;
                        }
                        else if (isDelim(code[count]) || code[count] == '.' || Char.IsWhiteSpace(code[count]))
                        {
                            isLexeme = true;
                        }
                        break;
                    case State.STRING:
                        if (code[count] == '\'')
                        {
                            //stay this state
                            sb.Append(code[count++]);
                            state = State.STRING;
                        }
                        while (code[count] != '\'')
                        {
                            sb.Append(code[count++]);
                        }
                        if (code[count] == '\'')
                        {
                            isLexeme = true;
                            count++;
                        }
                        break;
                }
                if (isLexeme)
                {
                    switch (state)
                    {
                        case State.START:
                            break;
                        case State.END:
                            break;
                        case State.NUM:
                            currentType = TokenType.NUMBER;
                            break;
                        case State.STRING:
                            currentType = TokenType.STRING;
                            break;
                        case State.IDENTIFY:
                            if (isKeyword(sb.ToString()))
                            {
                                currentType = getTokenType(sb.ToString());
                            }
                            else
                                currentType = TokenType.IDENTIFY;
                            break;
                        default:
                            break;
                    }
                    tokenList.Add(new Token(currentType, sb.ToString()));
                    sb.Clear();
                    state = State.START;
                }
            }
            return tokenList;
        }
    }
}
