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
        //built-in Objects
        //basic: Number Boolean 
        //Object Function Array(BasicArray,DictArray) String 
        //Math 
        //object {} 
        //. 属性和方法调用
        //proto
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
        public static void OrThrows(this Boolean condition, String message = null)
        {
            if (!condition) { exceptions.Add(new Exception(message == null ? "nothing" : message)); }
        }
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
            char[] cDelims = { ',', '(', ')', '[', ']', ';', ':', '=', '<', '>', '+', '-', '*', '/', '&', '{', '}' };
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
                                    currentType = TokenType.ADD;
                                    break;
                                case '-':
                                    currentType = TokenType.SUB;
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
                                        count++;
                                        sb.Append(code[count]);
                                        currentType = TokenType.EQ;
                                    }
                                    break;
                                case '>':
                                    if (code[count + 1] != '=')
                                        currentType = TokenType.GE;
                                    else
                                    {
                                        count++;
                                        sb.Append(code[count]);
                                        currentType = TokenType.GT;
                                    }
                                    break;
                                case '<':
                                    if (code[count + 1] != '=')
                                        currentType = TokenType.LE;
                                    else
                                    {
                                        count++;
                                        sb.Append(code[count]);
                                        currentType = TokenType.LT;
                                    }
                                    break;
                                case '&':
                                    if (code[count + 1] != '&')
                                        currentType = TokenType.AND;
                                    else
                                    {
                                        count++;
                                        sb.Append(code[count]);
                                        currentType = TokenType.AND;
                                    }
                                    break;
                                case '!':
                                    if (code[count + 1] != '=')
                                        currentType = TokenType.UNEQ;
                                    else
                                    {
                                        count++;
                                        sb.Append(code[count]);
                                        currentType = TokenType.NOT;
                                    }
                                    break;
                                case '|':
                                    if (code[count + 1] != '|')
                                        currentType = TokenType.OR;
                                    else
                                    {
                                        count++;
                                        sb.Append(code[count]);
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
        public static int index = 0;
        public static JsExpression current = null;
        public static List<Exception> exceptions = new List<Exception>();
        public static TokenType getTokenType(string identify)
        {
            return keyword[identify];
        }

        public static JsExpression ParseVar(List<Token> list)
        {
            JsExpression varExp = new JsExpression("var", current);
            current = varExp;
            index++;
            varExp.child.Add(ParseAssign(list));
            current = varExp.parent;
            return varExp;
        }

        public static JsExpression ParseNumber(List<Token> list)
        {
            return new JsExpression(list[index].name, current);
        }
        //identify
        public static JsExpression ParseIdentify(List<Token> list)
        {
            return new JsExpression(list[index].name, current);
        }
        //function [name]([^var[,var]*]){[expression;]*}
        public static JsExpression ParseFunction(List<Token> list)
        {
            JsExpression funcExp = new JsExpression("function", current);
            current = funcExp;
            index++;//match name

            if (list[index].type == TokenType.IDENTIFY)//mathc function name
            {
                funcExp.child.Add(ParseIdentify(list));
                index++;//match (
                (list[index].type == TokenType.OpenParenthese).OrThrows("expected an open parenthese");
                index++;//match args
            }
            else if (list[index].type == TokenType.OpenParenthese)//annoymous function
            {
                index++;//match args
            }
            else //error
            {
                exceptions.Add(new Exception("syntax error,expected an identifier or an open parenthese"));
            }

            JsExpression argsExp = new JsExpression("args", current);
            while (list[index].type != TokenType.CloseParenthese)//add args
            {
                argsExp.child.Add(ParseIdentify(list));
                index++;//match , or )
                if (list[index].type == TokenType.CloseParenthese)
                {
                    break;
                }
                (list[index].type == TokenType.COMMA).OrThrows("expected a comma");
                index++;
            }
            funcExp.child.Add(argsExp);
            index++;
            (list[index].type == TokenType.OpenBrace).OrThrows("expected a OpenBrace");
            funcExp.child.Add(ParseExpList(list));
            (list[index].type == TokenType.CloseBrace).OrThrows("expected a CloseBrace");
            current = funcExp.parent;
            return funcExp;
        }

        public static JsExpression ParseNewExpression(List<Token> list)
        {
            JsExpression newExp = new JsExpression("new", current);
            current = newExp;
            index++;
            newExp.child.Add(ParseFunctionCall(list));
            current = newExp.parent;
            return newExp;
        }
        //annoymous function just like lambda
        public static JsExpression ParseLambdaCall(List<Token> list)
        {
            current = new JsExpression("(", current);
            index++;//match (;annoymous function
            current.child.Add(ParseFunction(list));
            index = index + 3;//match )(,then new word 
            while (list[index].type != TokenType.CloseParenthese)
            {
                current.child.Add(new JsExpression(list[index].name, current));
                index++;
                if (list[index].type == TokenType.CloseParenthese)
                {
                    break;
                }
                //match ,
                index++;//next new word
            }
            //index++;//不match ;
            return current;
        }
        //expList 内所有表达式都需要；
        //有的内部已经match了；
        //funcCall，PointExp没有match
        public static JsExpression ParseExpList(List<Token> list)
        {
            JsExpression expList = new JsExpression("{", current);
            current = expList;
            index++;
            while (list[index].type != TokenType.CloseBrace)
            {
                var item = list[index];
                switch (item.type)
                {
                    case TokenType.VAR:// already matched ;
                        expList.child.Add(ParseVar(list));
                        break;
                    case TokenType.FUNCTION:// already matched ;
                        expList.child.Add(ParseFunction(list));
                        break;
                    case TokenType.OpenParenthese:
                        current = ParseLambdaCall(list);
                        expList.child.Add(current);
                        current = expList;
                        index++;// match ;
                        break;
                    case TokenType.IDENTIFY:

                        switch (list[index + 1].type)
                        {
                            case TokenType.BIND://=
                                // already matched ;
                                expList.child.Add(ParseAssign(list));
                                break;
                            case TokenType.POINT://a.b()| a.b=1 |a.b
                                {
                                    bool isMethodCall = false;
                                    if (LookAhead(list, index, 3, TokenType.OpenParenthese))//if method call
                                    {
                                        isMethodCall = true;
                                    }
                                    expList.child.Add(ParsePointExpression(list));
                                    // method call and get field need match ;
                                    // now only check method call
                                    // field get should not appear
                                    if (isMethodCall)
                                    {
                                        index++;//match ;
                                    }
                                }
                                break;
                            default:
                                //func call
                                expList.child.Add(ParseFunctionCall(list));
                                index++;//match ;
                                break;
                        }
                        break;
                    case TokenType.IF:// no need to macth ;
                        expList.child.Add(ParseIf(list));
                        break;
                    case TokenType.WHILE:// no need to macth ;
                        expList.child.Add(ParseWhile(list));
                        break;
                    case TokenType.RETURN:// already matched ;
                        expList.child.Add(ParseReturn(list));
                        break;
                    default:
                        exceptions.Add(new Exception("undefine indentifier"));
                        break;
                }
                index++;//match }
            }
            current = expList.parent;
            return expList;
        }
        public static JsExpression ParseReturn(List<Token> list)
        {
            JsExpression retExp = new JsExpression("return", current);
            current = retExp;
            index++; // match exp
            retExp.child.Add(ParseRelation(list));
            index++; // match ;
            (list[index].type == TokenType.SemiColon).OrThrows("expected a semicolon");
            current = retExp.parent;
            return retExp;
        }
        //bug
        public static JsExpression ParseBool(List<Token> list)
        {
            JsExpression result = ParseRelation(list);
            index++;
            TokenType[] relArray ={
                         TokenType.EQ,TokenType.GT,TokenType.LT,TokenType.GE,
                         TokenType.LE,TokenType.AND,TokenType.OR,TokenType.UNEQ,
                         TokenType.NOT
                         };
            Func<TokenType, bool> inRelArray = (op) =>
            {
                foreach (var item in relArray)
                {
                    if (op == item)
                    {
                        return true;
                    }
                }
                return false;
            };
            if (inRelArray(list[index].type))
            {
                JsExpression tmp = new JsExpression(list[index].name, current);
                current = tmp;
                index++;
                JsExpression e1 = ParseRelation(list);
                result.parent = tmp;
                tmp.child.Add(result);
                tmp.child.Add(e1);
                result = tmp;
            }
            return result;
        }
        //Exp=Term[(+|-)Term]*
        public static JsExpression ParseRelation(List<Token> list)
        {
            JsExpression result = ParseTerm(list);
            index++;
            JsExpression tmp = null;
            while (list[index].type == TokenType.ADD ||
                list[index].type == TokenType.SUB)
            {
                tmp = new JsExpression(list[index].name, current);
                current = tmp;
                index++;
                JsExpression e2 = ParseTerm(list);
                result.parent = tmp;
                tmp.child.Add(result);
                tmp.child.Add(e2);
                result = tmp;
                current = result;
                index++;
            }
            index--;
            current = result.parent;
            return result;
        }
        //Term=Factor[(*|/)Factor]*
        public static JsExpression ParseTerm(List<Token> list)
        {
            JsExpression result = ParseFactor(list);
            index++;
            JsExpression tmp = null;
            while (list[index].type == TokenType.MUL || list[index].type == TokenType.DIV)
            {
                tmp = new JsExpression(list[index].name, current);
                current = tmp;
                index++;
                JsExpression e2 = ParseFactor(list);
                result.parent = tmp;
                tmp.child.Add(result);
                tmp.child.Add(e2);
                result = tmp;
                current = result;
                index++;
            }
            index--;
            current = result.parent;
            return result;
        }
        //Factor=(Num|Identify)|(Exp)
        public static JsExpression ParseFactor(List<Token> list)
        {
            if (list[index].type == TokenType.NUMBER || list[index].type == TokenType.IDENTIFY)
            {
                return new JsExpression(list[index].name, current);
            }
            else if (list[index].type == TokenType.OpenParenthese)
            {
                index++;
                JsExpression exp = ParseRelation(list);
                index++;//match )
                return exp;
            }
            return null;
        }

        //a=num|function|(|indentify|new identify
        public static JsExpression ParseAssign(List<Token> list)
        {
            JsExpression assignExp = new JsExpression("=", current);
            current = assignExp;
            assignExp.child.Add(ParseIdentify(list));
            index++;
            (list[index].type == TokenType.BIND).OrThrows("expected '='");
            index++;
            switch (list[index].type)
            {

                case TokenType.NUMBER:
                    assignExp.child.Add(ParseRelation(list));
                    break;
                case TokenType.IDENTIFY://var|function call|a.b|bool
                    switch (list[index + 1].type)
                    {
                        case TokenType.OpenParenthese://function call
                            assignExp.child.Add(ParseFunctionCall(list));
                            break;
                        case TokenType.SemiColon://math
                            assignExp.child.Add(ParseRelation(list));
                            break;
                        case TokenType.POINT:
                            assignExp.child.Add(ParsePointExpression(list));
                            break;
                        default://var
                            assignExp.child.Add(ParseRelation(list));
                            break;
                    }
                    break;
                case TokenType.FUNCTION:
                    assignExp.child.Add(ParseFunction(list));
                    break;
                case TokenType.OpenParenthese:
                    index++;//pass ( ;annoymous function
                    assignExp.child.Add(ParseFunction(list));
                    break;
                case TokenType.NEW:
                    assignExp.child.Add(ParseNewExpression(list));
                    break;
                default:
                    assignExp.child.Add(new JsExpression(list[index].name, assignExp));
                    break;
            }
            index++;//match ;
            (list[index].type == TokenType.SemiColon).OrThrows("expected a semicolon");
            current = assignExp.parent;
            return assignExp;
        }

        public static JsExpression ParseIf(List<Token> list)
        {
            JsExpression ifExp = new JsExpression("if", current);
            current = ifExp;
            index++;
            (list[index].type == TokenType.OpenParenthese).OrThrows("expected a OpenParenthese");
            index++;
            ifExp.child.Add(ParseBool(list));//parse bool expression
            index++;
            (list[index].type == TokenType.CloseParenthese).OrThrows("expected a CloseParenthese");
            index++;
            (list[index].type == TokenType.OpenBrace).OrThrows("expected a OpenBrace");
            ifExp.child.Add(ParseExpList(list));
            (list[index].type == TokenType.CloseBrace).OrThrows("expected a CloseBrace");
            if (list[index + 1].type == TokenType.ELSE)
            {
                index++;
                ifExp.child.Add(ParseElse(list));
            }
            current = ifExp.parent;
            return ifExp;
        }

        public static JsExpression ParseElse(List<Token> list)
        {
            JsExpression elseExp = new JsExpression("else", current);
            index++;//match{
            elseExp.child.Add(ParseExpList(list));
            index++;//match}

            return elseExp;
        }

        public static JsExpression ParseWhile(List<Token> list)
        {
            JsExpression whExp = new JsExpression("while", current);
            current = whExp;
            index++;
            (list[index].type == TokenType.OpenParenthese).OrThrows("expected a OpenParenthese");
            index++;
            whExp.child.Add(ParseBool(list));//parse bool expression
            index++;
            (list[index].type == TokenType.CloseParenthese).OrThrows("expected a CloseParenthese");
            index++;
            (list[index].type == TokenType.OpenBrace).OrThrows("expected a OpenBrace");
            whExp.child.Add(ParseExpList(list));
            (list[index].type == TokenType.CloseBrace).OrThrows("expected a CloseBrace");
            current = whExp.parent;
            return whExp;
        }
        // func([arg[,arg]*]);
        public static JsExpression ParseFunctionCall(List<Token> list)
        {
            JsExpression funcCall = new JsExpression(list[index].name, current);
            current = funcCall;
            index++;//match (
            funcCall.child.Add(new JsExpression("(", current));
            index++;
            while (list[index].type != TokenType.CloseParenthese)
            {
                funcCall.child.Add(new JsExpression(list[index].name, current));
                index++;
                if (list[index].type == TokenType.CloseParenthese)
                {
                    break;//match )
                }
                (list[index].type == TokenType.COMMA).OrThrows("expected a CloseParenthese");
                index++;
            }
            current = funcCall.parent;
            return funcCall;
        }
        //identifier.functionCall
        //identifier.identifier = exp;
        //identifier.identifier[ ; | ) ]
        public static JsExpression ParsePointExpression(List<Token> list)
        {
            JsExpression compoundCall = new JsExpression(".", current);
            current = compoundCall;
            string objName = list[index].name;
            index++;//match .
            (list[index].type == TokenType.POINT).OrThrows("expected a Point.");
            current.child.Add(new JsExpression(objName, current));
            index++;
            (list[index].type == TokenType.IDENTIFY).OrThrows("behind point expression needs a identifier");
            if (LookAhead(list, index, 1, TokenType.OpenParenthese))//method call
            {
                current.child.Add(ParseFunctionCall(list));//( methodName args
            }
            else if (LookAhead(list, index, 1, TokenType.BIND))//set or add field
            {
                current.child.Add(ParseAssign(list));//= field value
            }
            else // get field
            {
                current.child.Add(new JsExpression(list[index].name, current));//field
            }
            current = compoundCall.parent;
            return compoundCall;
        }
        public static bool LookAhead(List<Token> list, int currentIndex, int step, TokenType type)
        {
            return list[currentIndex + step].type == type;
        }
        //var function functionCall ({function})() a=b
        public static JsExpression ParseProgram(List<Token> list)
        {
            JsExpression program = new JsExpression("", null);
            current = program;
            index = 0;
            for (; index < list.Count; index++)
            {
                var item = list[index];
                switch (item.type)
                {
                    case TokenType.VAR:
                        current = ParseVar(list);
                        program.child.Add(current);
                        break;
                    case TokenType.FUNCTION:
                        current = ParseFunction(list);
                        program.child.Add(current);
                        break;
                    case TokenType.OpenParenthese:
                        current = ParseLambdaCall(list);
                        program.child.Add(current);
                        break;
                    case TokenType.IDENTIFY:
                        switch (list[index + 1].type)
                        {
                            case TokenType.BIND://=
                                current = ParseAssign(list);
                                break;
                            case TokenType.SemiColon://identifier
                                current = ParseIdentify(list);
                                index++;//match ;
                                break;
                            case TokenType.POINT://call method or get field or set field or add new field
                                {
                                    bool needMatchSemiColon = false;
                                    if (LookAhead(list, index, 3, TokenType.OpenParenthese)
                                        || LookAhead(list, index, 3, TokenType.SemiColon))//if method call
                                    {
                                        needMatchSemiColon = true;
                                    }
                                    current = ParsePointExpression(list);
                                    // method call and get field need match ;
                                    // now only check method call
                                    // field get should not appear
                                    if (needMatchSemiColon)
                                    {
                                        index++;//match ;
                                    }
                                }
                                break;
                            default:
                                //func call
                                current = ParseFunctionCall(list);
                                index++;//match ;
                                break;
                        }
                        program.child.Add(current);
                        break;
                    case TokenType.NUMBER:
                        current = ParseRelation(list);
                        index++;//match ;
                        program.child.Add(current);
                        break;
                    case TokenType.STRING:
                        current = new JsExpression(item.name, current);
                        index++;//match ;
                        program.child.Add(current);
                        break;
                    default:
                        exceptions.Add(new Exception(current.value + " is an unknow expression"));
                        break;
                }
                current = program;
            }
            return program;
        }
    }
}
