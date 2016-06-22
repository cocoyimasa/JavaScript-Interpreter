using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 *@author wanghesai
 *@time 2014/11/13-2014/11/15 
 *@time 2016/6/21 fix bugs and add libraries
 *@time 2016/6/22 fix bugs and add bool expressions supporting
 *@name JSInterpret
 ***/
namespace JSInterpret
{
    public static partial class JavaScript
    {
        public static JsExpression Parse(this string code)
        {
            List<Token> tokenList = Tokenizer(code);
            Console.WriteLine(
                tokenList.Select(item => item.name)
                .Aggregate("", (res, str) => res + " " + str).Substring(1)
                );
            JsExpression program = ParseProgram(tokenList);
            return program;
        }

        public static void GetJavaScriptConsole(this Env env, Func<string, Env, JsObject> eval)
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
                        int len = sb.Length;
                        if(sb.ToString().Substring(len-2,2).Equals(";;") && info.KeyChar=='\r')
                        {
                            sb.Remove(sb.Length - 1, 1);
                            break;
                        }
                        input = info.KeyChar + Console.ReadLine();
                    }
                    input = sb.ToString();
                    if (input == "!q;")
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

        public static JsBool NumEval(this JsExpression[] args, Env env, Func<JsNumber, JsNumber, bool> rel)
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
        public static JsBool BoolEval(this JsExpression[] args, Env env, Func<JsBool, JsBool, bool> rel)
        {
            (args.Length > 1).OrThrows("arguments are too less in relation expressions");
            JsBool curr = (JsBool)args[0].evaluate(env);
            foreach (var item in args.Skip(1))
            {
                JsBool next = (JsBool)item.evaluate(env);
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
        public static string Run(this Env env, string input)
        {
            Func<string, Env, JsObject> eval = (code, _env) => code.Parse().evaluate(_env);
            Console.ForegroundColor = ConsoleColor.Cyan;
            return eval(input, env).ToString();
        }
        public static Env InitEnv()
        {
            for (int i = 0; i < keywords.Length; i++)
            {
                keyword.Add(keywords[i], tokenType[i]);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("----JavaScript Interpreter----");
            return new Env()
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
                    return args.NumEval(scope, (s1, s2) => s1 == s2);
                })
                .Builtin(">", (args, scope) =>
                {
                    return args.NumEval(scope, (s1, s2) => s1 > s2);
                })
                .Builtin("<", (args, scope) =>
                {
                    return args.NumEval(scope, (s1, s2) => s1 < s2);
                })
                .Builtin(">=", (args, scope) =>
                {
                    return args.NumEval(scope, (s1, s2) => s1 >= s2);
                })
                .Builtin("<=", (args, scope) =>
                {
                    return args.NumEval(scope, (s1, s2) => s1 <= s2);
                })
                .Builtin("!=", (args, scope) =>
                {
                    return args.NumEval(scope, (s1, s2) => s1 != s2);
                })
                .Builtin("&&", (args, scope) =>
                {
                    return args.BoolEval(scope, (s1, s2) => s1 && s2);
                })
                .Builtin("||", (args, scope) => 
                {
                    return args.BoolEval(scope, (s1, s2) => s1 || s2);
                })
                .Builtin("Object", (args, scope) =>
                {
                    var parameters = args.Skip(1).Select(arg => arg.evaluate(scope)).Cast<JsObject>().ToArray();
                    return new JsObject(parameters, scope);
                })
                .Builtin("Number", (args, scope) =>
                {
                    var parameters = args.Skip(1).Select(arg => arg.evaluate(scope)).Cast<JsNumber>().ToArray();
                    return new JsNumber(parameters[0]);
                })
                .Builtin("Boolean", (args, scope) =>
                {
                    var parameters = args.Skip(1).Select(arg => arg.evaluate(scope)).Cast<JsBool>().ToArray();
                    return new JsBool(parameters[0].value);
                })
                .Builtin("Array", (args, scope) =>
                {
                    var parameters = args.Skip(1).Select(arg => arg.evaluate(scope)).ToArray();
                    return new JsArray(parameters, scope);
                })
                .Builtin("String", (args, scope) =>
                {
                    var parameters = args.Skip(1).Select(arg => arg.evaluate(scope)).Cast<JsString>().ToArray();
                    return new JsString(parameters[0].ToString(), scope);
                })
                ;
        }
        static void Main(string[] cmdArgs)
        {
            //string code = "function(a){\n\ra=10;\nvar b=3*(2+5)+6;\nif(a==10)\n{\t  alert('a');var c=function(){return null;};}return b;}";
            //Start Repl:
            //InitEnv().GetJavaScriptConsole((code, env) => code.Parse().evaluate(env));
            //Run code fragment:
            string[] code = {
                "true && false;"
                };
            Console.ForegroundColor = ConsoleColor.White;
            Env env = JSInterpret.JavaScript.InitEnv();
            List<string> results = new List<string>();
            foreach (var codeFragment in code)
            {
                string result = env.Run(codeFragment);
                results.Add(result);
            }
            Console.ForegroundColor = ConsoleColor.Red;
            if (exceptions.Count > 0)
            {
                foreach (var excep in exceptions)
                {
                    Console.WriteLine(excep.Message);
                }
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
            Console.ReadKey();
        }
    }
    //.Builtin("car", (args, scope) =>
    //{
    //    LList list = null;
    //    (args.Length == 1 && (list = args[0].evaluate(scope) as LList) != null).OrThrows("只能对列表操作");
    //    return list.First();
    //})
    //.Builtin("cdr", (args, scope) =>
    //{
    //    LList list = null;
    //    (args.Length == 1 && (list = args[0].evaluate(scope) as LList) != null).OrThrows("只能对列表操作");
    //    return new LList(list.Skip(1));
    //})    
    
}
