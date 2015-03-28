using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 *@author wanghesai
 *@time 2014/11/13-2014/11/15 
 *@name JSInterpret
 ***/
namespace JSInterpret
{
    enum TokenType
    {
        ST,
        NUMBER,STRING,BOOLEAN,IDENTIFY,
        OpenBracket/*[*/, CloseBracket, OpenBrace/*{*/, CloseBrace,
        OpenParenthese/*(*/, CloseParenthese,
        FOR,WHILE,FUNCTION,IF,ELSE,VAR,RETURN,NEW,
        ADD, SUB, MUL, DIV, COLON,EQ/*==*/,BIND/*=*/,SemiColon/*;*/,COMMA/*;*/,
        GT,LT,GE,LE,AND,OR,UNEQ,NOT

    }
    class Token
    {
        public TokenType type;
        public string name;
        public Token()
        { }
        public Token(TokenType t,string n)
        {
            type = t;
            name = n;
        }
    }
    enum State
    {
        START, END, NUM, STRING, BOOLEAN,IDENTIFY
    }

    class JsObject
    {
        public static implicit operator JsObject(Int64 num)
        {
            return new JsNumber(num);
        }
    }

    class JsNumber : JsObject
    {
        public Int64 value{get;set;}
        public JsNumber(Int64 v)
        {
            value = v;
        }
        public override string ToString()
        {
            return value.ToString();
        }
        public static implicit operator Int64(JsNumber num)
        {
            return num.value;
        }
        public static implicit operator JsNumber(Int64 num)
        {
            return new JsNumber(num);
        }
    }

    class JsBool : JsObject
    {
        public bool value { get; set; }
        public readonly static JsBool True = new JsBool(true);
        public readonly static JsBool False = new JsBool(false);
        public JsBool(bool v)
        {
            this.value = v;
        }
        public override string ToString()
        {
            return value.ToString();
        }
        public static implicit operator bool(JsBool num)
        {
            return num.value;
        }
        public static implicit operator JsBool(bool num)
        {
            return new JsBool(num);
        }
    }

    class JsFunction : JsObject
    {
        public string[] args { get; set; }
        public JsExpression body { get; set; }
        public Env env { get; set; }
        public JsFunction(string[] args, JsExpression body, Env env)
        {
            this.args = args;
            this.body = body;
            this.env = env;
        }
        public bool isPartial
        {
            get { return this.Curry().Length < args.Length; }
        }
        public string[] Curry()
        {
            return args.Where(item => env.findInScope(item) != null).ToArray();
        }
        public Env updateFuncEnv(JsObject[] parameters)
        {
            (args.Length >= parameters.Length).OrThrows("参数太多");
            Env env1 = new Env(this.env);
            for (int i = 0; i < parameters.Length; i++)
            {
                env1.AddDef(this.args[i], parameters[i]);
            }
            return env1;
        }
        public JsFunction UpdateArgs(JsObject[] parameters)
        {
            var existingArguments =
                this.args.Select(p => this.env.findInScope(p)).Where(obj => obj != null);
            var newArguments = existingArguments.Concat(parameters).ToArray();
            Env newEnv = updateFuncEnv(newArguments);
            return new JsFunction(this.args, this.body, newEnv);
        }
        public JsObject evaluate()
        {
            string[] givenParams = Curry();
            if (givenParams.Length < args.Length)
            {
                return this;
            }
            else
            {
                //Finally all the function call changes to be the body's evaluation
                //that's where's the function call's core 
                return this.body.evaluate(env);
            }
        }
        public override string ToString()
        {
            return String.Format("(function ({0}) {1} ))",
                " ".Join(this.args.Select(
                p =>
                {
                    JsObject val;
                    if ((val = this.env.findInScope(p)) != null)
                    {
                        return p + ":" + val;
                    }
                    return p;
                })),
                this.body);
        }
    }
    class JsExpression
    {
        public string value { get; set; }
        public JsExpression parent { get; set; }
        public List<JsExpression> child { get; set; }
        public JsExpression(string token, JsExpression p)
        {
            this.value = token;
            this.parent = p;
            this.child = new List<JsExpression>();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(value);
            foreach (var item in child)
            {
                sb.Append(" ");
                sb.Append(item);
            }
            return sb.ToString();
        }
        public JsObject evaluate(Env env)
        {
            JsExpression x = this.value == "" ? this.child[0] : this;
            if (x.child.Count == 0)
            {
                Int64 num;
                if (Int64.TryParse(x.value, out num))
                {
                    return num;
                }
                else
                {
                    return env.Find(x.value);
                }
            }
            else
            {
                if (x.value == "if")
                {
                    JsBool cond = (JsBool)(x.child[0].evaluate(env));
                    if(x.child.Count==3)
                    {
                        return cond ? x.child[1].evaluate(env) : x.child[2].evaluate(env);
                    }
                    else
                    {
                        return cond ? x.child[1].evaluate(env) : new JsBool(false);
                    }
                }
                else if (x.value == "while")
                {
                    JsBool cond = (JsBool)(x.child[0].evaluate(env));
                    JsObject res = null; 
                    if(cond)
                    {
                        while ((JsBool)x.child[0].evaluate(env))//build-in
                        {
                            res=x.child[1].evaluate(env);
                        }
                    }
                    return res;
                }
                else if(x.value=="return")
                {
                    return x.child[0].evaluate(env);
                }
                else if(x.value=="new")//build-in
                {
                    return x.child[0].evaluate(env);
                }
                else if(x.value=="var")//=
                {
                    return x.child[0].evaluate(env);
                }
                else if (x.value == "=")
                {
                    return env.AddDef(x.child[0].value, x.child[1].evaluate(env));
                }
                else if (x.value == "function")
                {
                    if (x.child.Count == 3)
                    {
                        string[] args = x.child[1].child.Select(i => i.value).ToArray();
                        JsFunction func = new JsFunction(args, x.child[2], new Env(env));
                        return env.AddDef(x.child[0].value,func);
                    }
                    else
                    {
                        string[] args = x.child[0].child.Select(i => i.value).ToArray();
                        JsFunction func = new JsFunction(args, x.child[1], new Env(env));
                        return func;
                    }
                }
                //else if (x.value == "list")
                //{
                //    return new LList(this.child.Skip(1).Select(item => item.evaluate(env)));
                //}
                else if (x.value == "{")
                {
                    JsObject val = null;
                    foreach (var exp in x.child)
                    {
                        val = exp.evaluate(env);
                    }
                    return val;
                }
                else if (Env.builtins.ContainsKey(x.value))
                {
                    var args = x.child.ToArray();
                    return Env.builtins[x.value](args, env);
                }
                else
                {
                    //匿名函数和自定义函数调用 
                    JsFunction func = x.value == "(" ? 
                        (JsFunction)x.child[0].evaluate(env) : 
                        (JsFunction)env.Find(x.value);
                    //add variable support,because var has child,so it can't be evaluated
                    var arguments = x.child.Skip(1).Select(item => item.evaluate(env)).ToArray();
                    return func.UpdateArgs(arguments).evaluate();
                }
            }
        }
    }
    class Env
    {
        public Env outer { get; set; }
        public Dictionary<string, JsObject> dict;
        public static Dictionary<string, Func<JsExpression[], Env, JsObject>> builtins
            = new Dictionary<string, Func<JsExpression[], Env, JsObject>>();

        public Env(Env outer = null)
        {
            this.outer = outer;
            this.dict = new Dictionary<string, JsObject>();
        }
        public JsObject AddDef(string name, JsObject obj)
        {
            if(dict.ContainsKey(name))
            {
                dict[name] = obj;
            }
            else
            {
                this.dict.Add(name, obj);
            }
            return obj;
        }
        public JsObject findInScope(string key)
        {
            if (this.dict.ContainsKey(key))
            {
                return dict[key];
            }
            return null;
        }
        public JsObject Find(string key)
        {
            Env env = this;
            while (env != null)
            {
                if (env.dict.ContainsKey(key))
                {
                    return env.dict[key];
                }
                env = env.outer;
            }
            throw new Exception(key + " is not defined");
        }
        public Env Builtin(string name, Func<JsExpression[], Env, JsObject> lambda)
        {
            builtins.Add(name, lambda);
            return this;
        }

    }
    static class JavaScript
    {
        public static Dictionary<string, TokenType> keyword=new Dictionary<string,TokenType>();
        public static string[] keywords = { "for", "while", "function", 
                                              "if", "else", "var","return","new" };
        public static TokenType[] tokenType = { TokenType.FOR, TokenType.WHILE, 
                                        TokenType.FUNCTION, TokenType.IF, 
                                        TokenType.ELSE, TokenType.VAR ,
                                        TokenType.RETURN,TokenType.NEW};
        public static int index = 0;
        public static JsExpression current = null;
        public static TokenType getTokenType(string identify)
        {
            return keyword[identify];
        }
        public static void OrThrows(this Boolean condition, String message = null)
        {
            if (!condition) { throw new Exception(message == null ? "nothing" : message); }
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
                if(identify.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool isDelim(char ch)
        {
            char[] cDelims = { ',', '(', ')', '[', ']', ';', ':', '=', '<', '>', '+', '-', '*', '/', '&' ,'{','}'};
            foreach(var item in cDelims)
            {
                if(ch == item)
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
            while(count<code.Length)
            {
                bool isLexeme=false;
                TokenType currentType=0;
                switch(state)
                {
                    case State.START:
                        if (Char.IsWhiteSpace(code[count]))
                        {
                            count++;
                            continue;
                        }
                        else if (Char.IsLetter(code[count])||code[count]=='_')
                        {
                            state = State.IDENTIFY;
                        }
                        else if(Char.IsDigit(code[count]))
                        {
                            state = State.NUM;
                        }
                        else if(code[count]=='\'')
                        {
                            state = State.STRING;
                            count++;
                            continue;
                        }
                        else 
                        {
                            switch((code[count]))
                            {
                                case '+':
                                    currentType=TokenType.ADD;
                                    break;
                                case '-':
                                    currentType=TokenType.SUB;
                                    break;
                                case '*':
                                    currentType=TokenType.MUL;
                                    break;
                                case '/':
                                    currentType=TokenType.DIV;
                                    break;
                                case '{':
                                    currentType=TokenType.OpenBrace;
                                    break;
                                case '}':
                                    currentType=TokenType.CloseBrace;
                                    break;
                                case '[':
                                    currentType=TokenType.OpenBracket;
                                    break;
                                case ']':
                                    currentType=TokenType.CloseBracket;
                                    break;
                                case '(':
                                    currentType=TokenType.OpenParenthese;
                                    break;
                                case ')':
                                    currentType=TokenType.CloseParenthese;
                                    break;
                                case ':':
                                    currentType=TokenType.COLON;
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
                        if(Char.IsDigit(code[count]))
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
                        else if (isDelim(code[count]) || Char.IsWhiteSpace(code[count]))
                        {
                            isLexeme = true;
                        }
                        break;
                    case State.STRING:
                        if (code[count] != '\'')
				        {
					        //stay this state
                            sb.Append(code[count]);
                            state = State.STRING;
                            count++;
				        }
                        else if (code[count] == '\'')
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
                    tokenList.Add(new Token(currentType,sb.ToString()));
                    sb.Clear();
                    state = State.START;
                }
            }
            return tokenList;
        }
        /// <summary>
        /// var i=(num|identify|function|new identify);
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static JsExpression ParseVar(List<Token> list)
        {
            JsExpression varExp=new JsExpression("var",current);
            current = varExp;
            index++;
            varExp.child.Add(ParseAssign(list));
            current = varExp.parent;
            return varExp;
        }
        /// <summary>
        /// number
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static JsExpression ParseNumber(List<Token> list)
        {
            return new JsExpression(list[index].name,current);
        }
        //identify
        public static JsExpression ParseIdentify(List<Token> list )
        {
            return new JsExpression(list[index].name, current); 
        }
        //function [name]([^var[,var]*]){[expression;]*}
        public static JsExpression ParseFunction(List<Token> list )
        {
            JsExpression funcExp = new JsExpression("function", current);
            current=funcExp;
            index++;//match name

            if(list[index].type==TokenType.IDENTIFY)//mathc function name
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
                throw new Exception("syntax error,expected an identifier or an open parenthese");
            }
            
            JsExpression argsExp = new JsExpression("args", current);
            while(list[index].type!=TokenType.CloseParenthese)//add args
            {
                argsExp.child.Add(ParseIdentify(list));
                index++;
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

        public static JsExpression ParseNewExpression(List<Token> list )
        {
            JsExpression newExp = new JsExpression("new", current);
            current = newExp;
            index++;
            newExp.child.Add(ParseFunctionCall(list));
            current = newExp.parent;
            return newExp;
        }
        //annoymous function just like lambda
        public static JsExpression ParseLambdaCall(List<Token>  list)
        {
            current=new JsExpression("(", current);
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
            index++;//match ;
            return current;
        }
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
                    case TokenType.VAR:
                        expList.child.Add(ParseVar(list));
                        break;
                    case TokenType.FUNCTION:
                        expList.child.Add(ParseFunction(list));
                        break;
                    case TokenType.OpenParenthese:
                        current = ParseLambdaCall(list);
                        expList.child.Add(current);
                        current = expList;
                        break;
                    case TokenType.IDENTIFY:
                        
                        switch(list[index+1].type)
                        {
                            case TokenType.BIND:
                                expList.child.Add(ParseAssign(list));
                                break;
                            default:
                                //func call
                                expList.child.Add(ParseFunctionCall(list));
                                break;
                        }
                        break;
                    case TokenType.IF:
                        expList.child.Add(ParseIf(list));
                        break;
                    case TokenType.WHILE:
                        expList.child.Add(ParseWhile(list));
                        break;
                    case TokenType.RETURN:
                        expList.child.Add(ParseReturn(list));
                        break;
                    default:
                        throw new Exception("undefine indentifier");
                }
                index++;
            }
            current = expList.parent;
            return expList;
        }
        public static JsExpression ParseReturn(List<Token> list)
        {
            JsExpression retExp = new JsExpression("return", current);
            current = retExp;
            index++;
            retExp.child.Add(ParseRelation(list));
            index++;
            (list[index].type == TokenType.SemiColon).OrThrows("expected a semicolon");
            current = retExp.parent;
            return retExp;
        }
        //bug
        public static JsExpression ParseBool(List<Token> list)
        {
            JsExpression result = ParseRelation(list);
            index++;
            TokenType[] relArray={
                         TokenType.EQ,TokenType.GT,TokenType.LT,TokenType.GE,
                         TokenType.LE,TokenType.AND,TokenType.OR,TokenType.UNEQ,
                         TokenType.NOT
                         };
            Func<TokenType,bool> inRelArray=(op)=>{
                foreach(var item in relArray)
                {
                    if(op==item)
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
            JsExpression tmp=null;
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
            if(list[index].type==TokenType.NUMBER||list[index].type==TokenType.IDENTIFY)
            {
                return new JsExpression(list[index].name, current);
            }
            else if(list[index].type==TokenType.OpenParenthese)
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
            assignExp.child.Add(ParseIdentify(list));
            index++;
            (list[index].type == TokenType.BIND).OrThrows("expected '='");
            index++;
            switch (list[index].type)
            {
                
                case TokenType.NUMBER:
                    assignExp.child.Add(ParseRelation(list));
                    break;
                case TokenType.IDENTIFY://var|function call|bool
                    if(list[index+1].type==TokenType.OpenParenthese)//function call
                    {
                        assignExp.child.Add(ParseFunctionCall(list));
                    }
                    else if(list[index+1].type!=TokenType.SemiColon)//math
                    {
                        assignExp.child.Add(ParseRelation(list));
                    }
                    else//var
                    {
                        assignExp.child.Add(ParseRelation(list));
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
            if(list[index+1].type==TokenType.ELSE)
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
        public static JsExpression ParseFunctionCall(List<Token> list)
        {
            JsExpression funcCall=new JsExpression(list[index].name,current);
            current = funcCall;
            index++;//match (
            funcCall.child.Add(new JsExpression("(", current));
            index++;
            while (list[index].type != TokenType.CloseParenthese) 
            {
                funcCall.child.Add(new JsExpression(list[index].name,current));
                index++;
                if (list[index].type == TokenType.CloseParenthese)
                {
                    break;
                }
                (list[index].type == TokenType.COMMA).OrThrows("expected an CloseParenthese");
                index++;
            }
            //match )
            current = funcCall.parent;
            return funcCall;
        }

        //var function functionCall ({function})() a=b
        public static JsExpression ParseProgram(List<Token> list )
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
                        current = ParseVar(list );
                        program.child.Add(current);
                        break;
                    case TokenType.FUNCTION:
                        current = ParseFunction(list );
                        program.child.Add(current);
                        break;
                    case TokenType.OpenParenthese:
                        current = ParseLambdaCall(list);
                        program.child.Add(current);
                        break;
                    case TokenType.IDENTIFY:
                        switch(list[index+1].type)
                        {
                            case TokenType.BIND://=
                                current=ParseAssign(list);
                                break;
                            case TokenType.SemiColon://identifier
                                current = ParseIdentify(list);
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
                    case TokenType.NUMBER:
                        current = ParseRelation(list);
                        index++;//match ;
                        program.child.Add(current);
                        break;
                    default:
                        break;
                }
                current = program;
            }
            return program;
        }

        public static JsExpression Parse(this string code)
        {
            List<Token> tokenList = Tokenizer(code);
            JsExpression program = ParseProgram(tokenList);
            return program;
        }

        public static void GetSchemeConsole(this Env env, Func<string, Env, JsObject> eval)
        {
            while (true)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("javascript>>>");
                    StringBuilder sb = new StringBuilder();
                    string input = Console.ReadLine();
                    //if string are not ';;',then continue reading
                    while (true)
                    {
                        sb.Append(input);
                        ConsoleKeyInfo info = Console.ReadKey();
                        if(info.KeyChar=='\r')
                        {
                            break;
                        }
                        input = info.KeyChar + Console.ReadLine();
                    }
                    input = sb.ToString();
                    if (input == "!q")
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Command exit is excuting!...");
                        break;
                    }
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(eval(input, env).ToString());
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                }

            }
        }

        public static JsBool BoolEval(this JsExpression[] args, Env env, Func<JsNumber, JsNumber, bool> rel)
        {
            (args.Length > 1).OrThrows("arguments are too less in relation expressions");
            JsNumber curr = (JsNumber)args[0].evaluate(env);
            foreach (var item in args.Skip(1))
            {
                JsNumber next = (JsNumber)item.evaluate(env);
                if (rel(curr, next))
                {
                    curr = next;
                }
                else
                {
                    return JsBool.False;
                }
            }
            return JsBool.True;
        }
        static void Main(string[] cmdArgs)
        {
            //string code = "function(a){\n\ra=10;\nvar b=3*(2+5)+6;\nif(a==10)\n{\t  alert('a');var c=function(){return null;};}return b;}";
            for (int i = 0; i < keywords.Length; i++)
            {
                keyword.Add(keywords[i], tokenType[i]);
            }
            Console.WriteLine("----Scheme Interpreter----");
            new Env()
                .Builtin("+", (args, scope) =>
                {
                    var numbers = args.Select(obj => obj.evaluate(scope)).Cast<JsNumber>();
                    return numbers.Sum(n => n);
                })
                .Builtin("-", (args, scope) =>
                {
                    var numbers = args.Select(obj => obj.evaluate(scope)).Cast<JsNumber>();
                    if (numbers.ToArray().Length == 1)
                    {
                        return -numbers.ToArray()[0];
                    }
                    else
                    {
                        return numbers.ToArray()[0] - numbers.Skip(1).Sum(n => n);
                    }
                })
                .Builtin("*", (args, scope) =>
                {
                    var numbers = args.Select(obj => obj.evaluate(scope)).Cast<JsNumber>();
                    JsNumber[] nums = numbers.ToArray();
                    Int64 result = 1;
                    foreach (var num in nums)
                    {
                        result = result * num;
                    }
                    return result;
                })
                .Builtin("/", (args, scope) =>
                {
                    var numbers = args.Select(obj => obj.evaluate(scope)).Cast<JsNumber>();
                    JsNumber[] nums = numbers.ToArray();
                    Int64 result = nums[0];
                    for (int i = 1; i < nums.Length; i++)
                    {
                        result = result / nums[i];
                    }
                    return result;
                })
                .Builtin("==", (args, scope) =>
                {
                    return args.BoolEval(scope, (s1, s2) => s1 == s2);
                })
                .Builtin(">", (args, scope) =>
                {
                    return args.BoolEval(scope, (s1, s2) => s1 > s2);
                })
                .Builtin("<", (args, scope) =>
                {
                    return args.BoolEval(scope, (s1, s2) => s1 < s2);
                })
                .Builtin(">=", (args, scope) =>
                {
                    return args.BoolEval(scope, (s1, s2) => s1 >= s2);
                })
                .Builtin("<=", (args, scope) =>
                {
                    return args.BoolEval(scope, (s1, s2) => s1 <= s2);
                })
                .Builtin("!=", (args, scope) =>
                {
                    return args.BoolEval(scope, (s1, s2) => s1 != s2);
                })
                .Builtin("Object", (args, scope) =>
                {
                    return new JsObject();
                })
                .GetSchemeConsole((code, env) => code.Parse().evaluate(env));
        }
    }
    
    
}
