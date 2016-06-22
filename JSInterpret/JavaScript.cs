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
    
    public static partial class JavaScript
    {
        //built-in Objects                          √
        //basic: Number Boolean                     √
        //Object Function Array(BasicArray) String  √
        //bool expression                           √
        //. 属性和方法调用                             √
        //重构JsExpression.eval                      √
        //for / for..in
        //do..while
        //Array(DictArray) [] {'1':2,'3':3} 直接创建Array
        //object {} 对象字面量{a:10,b:function(){}}
        //支持解释多行语句
        //Math
        //proto
        //es6新特性支持
        //String Array Function Library
        
        public static void OrThrows(this Boolean condition, String message = null)
        {
            if (!condition) { exceptions.Add(new Exception(message == null ? "nothing" : message)); }
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
            retExp.child.Add(GetSingleOrMathExpressionItem(list));
            index++; // match ;
            (list[index].type == TokenType.SemiColon).OrThrows("expected a semicolon");
            current = retExp.parent;
            return retExp;
        }
        // bool expression parse
        public static JsExpression ParseBool(List<Token> list)
        {
            return ParseOrExpression(list);
        }

        // Exp = T || Exp | null --> T [ || T]*
        public static JsExpression ParseOrExpression(List<Token> list)
        {
            JsExpression result = ParseAndExpression(list);
            index++;
            JsExpression tmp = null;
            while (list[index].type == TokenType.OR)
            {
                tmp = new JsExpression(list[index].name, current);
                current = tmp;
                index++;
                JsExpression e2 = ParseAndExpression(list);
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
        // T = F && T | null --> F [&& F]*
        public static JsExpression ParseAndExpression(List<Token> list)
        {
            JsExpression result = ParseCompareExpression(list);
            index++;
            JsExpression tmp = null;
            while (list[index].type == TokenType.AND)
            {
                tmp = new JsExpression(list[index].name, current);
                current = tmp;
                index++;
                JsExpression e2 = ParseCompareExpression(list);
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
        // F = （num | id） [> < ...] | ( Exp )
        public static JsExpression ParseCompareExpression(List<Token> list)
        {
            if(list[index].type == TokenType.NUMBER || 
                list[index].type == TokenType.IDENTIFY   ||
                list[index].name == "true"               ||
                list[index].name == "false"
                )
            {
                return ParseBinaryBoolExpression(list);
            }
            else if (list[index].type == TokenType.NOT)
            {
                return ParseUnaryBoolExpression(list);
            }
            else if (list[index].type == TokenType.OpenParenthese)
            {
                index++;
                JsExpression exp = ParseOrExpression(list);
                index++;//match )
                return exp;
            }
            return null;
        }
        public static JsExpression GetSingleOrBoolExpressionItem(List<Token> list)
        {
            JsExpression result = null;
            if (isMathOperation(list[index + 1]))
            {
                return ParseBool(list);
            }
            else if (list[index].name == "true" ||
                list[index].name == "false" ||
                list[index].type == TokenType.IDENTIFY ||
                list[index].type == TokenType.NUMBER)// Number or Math Exp
            {
                return new JsExpression(list[index].name, current);
            }
            return result;
        }
        public static JsExpression GetSingleOrMathExpressionItem(List<Token> list)
        {
            JsExpression result = null;
            if (isMathOperation(list[index + 1]))
            {
                return ParseRelation(list);
            }
            else if (list[index].name == "true" ||
                list[index].name == "false"||
                list[index].type == TokenType.IDENTIFY ||
                list[index].type == TokenType.NUMBER)// Number or Math Exp
            {
                return new JsExpression(list[index].name, current);
            }
            return result;
        }

        // > < >= <= == !=
        // 1+1 < 2+2
        public static JsExpression ParseBinaryBoolExpression(List<Token> list)
        {
            JsExpression result = GetSingleOrMathExpressionItem(list);
            index++;// match op
            TokenType[] relArray ={
                         TokenType.EQ,TokenType.GT,TokenType.LT,TokenType.GE,
                         TokenType.LE,TokenType.UNEQ
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
                JsExpression e1 = GetSingleOrMathExpressionItem(list);
                result.parent = tmp;
                tmp.child.Add(result);
                tmp.child.Add(e1);
                result = tmp;
            }
            else {
                index--;
            }
            current = result.parent;
            return result;
        }
        //!identifier 
        //!(Exp)
        public static JsExpression ParseUnaryBoolExpression(List<Token> list)
        {
            JsExpression result = new JsExpression(list[index].name, current);
            current = result;
            index++;// match next
            if (list[index].type == TokenType.NUMBER ||
                list[index].type == TokenType.IDENTIFY ||
                list[index].name == "true" ||
                list[index].name == "false"
                )// match !
            {

                JsExpression exp = new JsExpression(list[index].name, current);
                result.child.Add(exp);
                return result;
            }
            else if (list[index].type == TokenType.OpenParenthese)// match !(Exp)
            {
                index++;
                JsExpression exp = ParseOrExpression(list);
                result.child.Add(exp);
                index++;//match )
                return exp;
            }
            current = result.parent;
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
                    assignExp.child.Add(GetSingleOrMathExpressionItem(list));
                    break;
                case TokenType.IDENTIFY://var|function call|a.b|bool
                    switch (list[index + 1].type)
                    {
                        case TokenType.OpenParenthese://function call
                            assignExp.child.Add(ParseFunctionCall(list));
                            break;
                        case TokenType.SemiColon://num or var
                            assignExp.child.Add(new JsExpression(list[index].name,current));
                            break;
                        case TokenType.POINT: //PointExp
                            assignExp.child.Add(ParsePointExpression(list));
                            break;
                        case TokenType.ADD:// Math Exp
                        case TokenType.SUB:
                        case TokenType.MUL:
                        case TokenType.DIV:
                            assignExp.child.Add(ParseRelation(list));
                            break;
                        case TokenType.AND:
                        case TokenType.OR:
                        case TokenType.EQ:
                        case TokenType.UNEQ:
                        case TokenType.LT:
                        case TokenType.GE:
                        case TokenType.LE:
                        case TokenType.GT://Bool Exp
                            assignExp.child.Add(ParseBool(list));
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
        public static bool isBoolOperation(Token token)
        {
            switch (token.type)
            {
                case TokenType.EQ:
                case TokenType.UNEQ:
                case TokenType.LT:
                case TokenType.GE:
                case TokenType.LE:
                case TokenType.GT:
                case TokenType.AND:
                case TokenType.OR:
                    return true;
                default:
                    return false;
            }
        }
        public static bool isMathOperation(Token token)
        {
            switch (token.type)
            {
                case TokenType.ADD:
                case TokenType.SUB:
                case TokenType.MUL:
                case TokenType.DIV:
                    return true;
                default:
                    return false;
            }
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
                            case TokenType.EQ:
                            case TokenType.UNEQ:
                            case TokenType.LT:
                            case TokenType.GE:
                            case TokenType.LE:
                            case TokenType.GT:
                            case TokenType.AND:
                            case TokenType.OR:
                                current = ParseBool(list);
                                index++;//match ;
                                break;
                            default:
                                //func call
                                current = ParseFunctionCall(list);
                                index++;//match ;
                                break;
                        }
                        program.child.Add(current);
                        break;
                    case TokenType.NUMBER: // num | Math Exp | Bool Exp 
                        if (isMathOperation(list[index + 1]))
                        {
                            current = ParseRelation(list);
                        }
                        else if (isBoolOperation(list[index + 1]))
                        {
                            current = ParseBool(list);
                        }
                        else
                        {
                            current = new JsExpression(list[index].name, program);
                        }
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
