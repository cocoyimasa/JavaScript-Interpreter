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
    public class JsObject
    {
        private JsObject[] objs;
        private Env scope;//inner scope

        public Env Scope
        {
            get { return scope; }
            set { scope = value; }
        }
        public JsObject()
        { }
        public JsObject(JsObject[] _objs, Env _scope)
        {
            objs = (JsObject[])_objs.Clone();
            scope = _scope;
        }
        public JsObject AddDef(string name, JsObject obj)
        {
            scope.AddDef(name, obj);
            return obj;
        }
        public JsObject FindAttribute(string key)
        {
            return scope.Find(key);
        }
        public static implicit operator JsObject(Int64 num)
        {
            return new JsNumber(num);
        }
        public override string ToString()
        {
            return "Object";
        }
    }
    public class JsUndefined : JsObject
    {
        public JsUndefined()
        {
        }
        public override string ToString()
        {
            return "undefined";
        }
    }
    public class JsNumber : JsObject
    {
        public Int64 value { get; set; }
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

    public class JsBool : JsObject
    {
        public bool value { get; set; }
        public readonly static JsBool True = new JsBool(true);
        public readonly static JsBool False = new JsBool(false);
        public JsBool()
        {
            this.value = false;
        }
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
    public class JsString : JsObject
    {
        public string val { get; set; }
        private Env scope;
        public JsString(string _v)
        {
            val = _v;
        }
        public JsString(string _v, Env _scope)
        {
            val = _v;
            scope = _scope;
        }
        public char this[int index]
        {
            get
            {
                bool condition = index >= 0 && index < val.Length;
                condition.OrThrows(this.ToString() + "index out of range");
                if (condition)
                {
                    return val[index];
                }
                else
                {
                    return '\0';
                }
            }
            set
            {
                bool condition = index >= 0 && index < val.Length;
                condition.OrThrows(this.ToString() + "index out of range");
                if (condition)
                {
                    val = val.Replace(val[index], value);//modify string
                }
            }
        }
        public string substring(int startIndex, int endIndex)
        {
            return val.Substring(startIndex, endIndex - startIndex);
        }
        public string substring(int startIndex)
        {
            return val.Substring(startIndex);
        }
        public string[] split(params char[] seperators)
        {
            return val.Split(seperators);
        }
        public override string ToString()
        {
            return val;
        }
        public static implicit operator string(JsString str)
        {
            return str.val;
        }
        public static implicit operator JsString(string str)
        {
            return new JsString(str);
        }
    }
    public class JsArray : JsObject
    {
        private JsObject[] items;
        private Env scope;
        public JsArray()
        { }
        public JsArray(JsObject[] _items, Env _scope)
        {
            items = (JsObject[])_items.Clone();
            scope = _scope;
        }
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns>JsObject</returns>
        public JsObject this[int index]
        {
            get
            {
                bool condition = index >= 0 && index < items.Length;
                condition.OrThrows(this.ToString() + "index out of range");
                if (condition)
                {
                    return items[index];
                }
                else
                {
                    return new JsUndefined();
                }
            }
            set
            {
                bool condition = index >= 0 && index < items.Length;
                condition.OrThrows(this.ToString() + "index out of range");
                if (condition)
                {
                    items[index] = value;
                }
            }
        }
        public string DetailString()
        {
            return "Array[" +
                items.Select(item => item.ToString())
                .Aggregate("", (res, item) => res + item) + "]";
        }
        public override string ToString()
        {
            return "Array";
        }
    }
    public class JsFunction : JsObject
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
        public string DetailString()
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
        public override string ToString()
        {
            return "Function";
        }
    }
}
