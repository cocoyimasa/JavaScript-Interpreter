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
    public static class CoreInterpreter
    {
        public static JsObject EvalSingleExpression(JsExpression x, Env env)
        {
            Int64 num;
            if (Int64.TryParse(x.value, out num))
            {
                return num;
            }
            else if (x.value[0] == '\'')
            {
                return new JsString(x.value.Substring(1));
            }
            else if (x.value.Equals("true") || x.value.Equals("false"))
            {
                return new JsBool(Convert.ToBoolean(x.value));
            }
            else if (x.value.Equals("undefined"))
            {
                return new JsUndefined();
            }
            else
            {
                JsObject obj = env.Find(x.value);
                (obj != null).OrThrows(x.value + " not found");
                return obj != null ? obj : new JsUndefined();
            }
        }
        public static JsObject EvalIf(JsExpression x,Env env)
        {
            JsBool cond = (JsBool)(x.child[0].evaluate(env));
            if (x.child.Count == 3)
            {
                return cond ? x.child[1].evaluate(env) : x.child[2].evaluate(env);
            }
            else
            {
                return cond ? x.child[1].evaluate(env) : new JsBool(false);
            }
        }
        public static JsObject EvalPointExpression(JsExpression x, Env env)
        {
            JsObject obj = env.Find(x.child[0].value);//root obj
            JsExpression exp = x.child[1];
            if (exp.value == "=")
            {
                return obj.AddDef(exp.child[0].value, exp.child[1].evaluate(obj.Scope));
            }
            else
            {
                if (exp.value == "(")
                {
                    JsFunction func = (JsFunction)obj.FindAttribute(exp.child[0].value);
                    var arguments = exp.child.Skip(1).Select(item => item.evaluate(env)).ToArray();
                    return func.UpdateArgs(arguments).evaluate();
                }
                else
                {
                    return obj.FindAttribute(exp.value);
                }
            }
        }
        public static JsObject EvalWhile(JsExpression x, Env env)
        {
            JsBool cond = (JsBool)(x.child[0].evaluate(env));
            JsObject res = null;
            if (cond)
            {
                while ((JsBool)x.child[0].evaluate(env))//build-in
                {
                    res = x.child[1].evaluate(env);
                }
            }
            return res;
        }
        public static JsObject EvalFunction(JsExpression x, Env env)
        {
            if (x.child.Count == 3)
            {
                string[] args = x.child[1].child.Select(i => i.value).ToArray();
                JsFunction func = new JsFunction(args, x.child[2], new Env(env));
                return env.AddDef(x.child[0].value, func);
            }
            else
            {
                string[] args = x.child[0].child.Select(i => i.value).ToArray();
                JsFunction func = new JsFunction(args, x.child[1], new Env(env));
                return func;
            }
        }
        public static JsObject EvalBlock(JsExpression x, Env env)
        {
            JsObject val = null;
            foreach (var exp in x.child)
            {
                val = exp.evaluate(env);
            }
            return val;
        }
        public static JsObject EvalBuiltIn(JsExpression x,Env env)
        {
            var args = x.child.ToArray();
            return Env.builtins[x.value](args, env);
        }
        public static JsObject EvalFunctionInvoke(JsExpression x, Env env)
        {
            JsFunction func = x.value == "(" ?
                        (JsFunction)x.child[0].evaluate(env) :
                        (JsFunction)env.Find(x.value);
            //add variable support,because var has child,so it can't be evaluated
            var arguments = x.child.Skip(1).Select(item => item.evaluate(env)).ToArray();
            return func.UpdateArgs(arguments).evaluate();
        }
    }
    public class JsExpression
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
        public string DetailString()
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
        public override string ToString()
        {
            return "Expression";
        }
        public JsObject evaluate(Env env)
        {
            JsExpression x = this.value == "" ? this.child[0] : this;
            if (x.child.Count == 0)
            {
                return CoreInterpreter.EvalSingleExpression(x, env);
            }
            else
            {
                if (x.value == "if")//考虑if内部变量作用域问题！！！Lexical Scope
                {
                    return CoreInterpreter.EvalIf(x, env);
                }
                else if (x.value == ".")
                {
                    return CoreInterpreter.EvalPointExpression(x, env);
                }
                else if (x.value == "while")
                {
                    return CoreInterpreter.EvalWhile(x, env);
                }
                else if (x.value == "return")
                {
                    return x.child[0].evaluate(env);
                }
                else if (x.value == "new")//build-in
                {
                    return x.child[0].evaluate(env);
                }
                else if (x.value == "var")//=
                {
                    return x.child[0].evaluate(env);
                }
                else if (x.value == "=")
                {
                    return env.AddDef(x.child[0].value, x.child[1].evaluate(env));
                }
                else if (x.value == "function")
                {
                    return CoreInterpreter.EvalFunction(x, env);
                }
                //else if (x.value == "list")
                //{
                //    return new LList(this.child.Skip(1).Select(item => item.evaluate(env)));
                //}
                else if (x.value == "{")
                {
                    return CoreInterpreter.EvalBlock(x, env);
                }
                else if (Env.builtins.ContainsKey(x.value))
                {
                    return CoreInterpreter.EvalBuiltIn(x, env);
                }
                else
                {
                    //匿名函数和自定义函数调用 
                    return CoreInterpreter.EvalFunctionInvoke(x, env);
                }
            }
        }
    }
}
