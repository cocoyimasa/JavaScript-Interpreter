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
    public class Env
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
            if (dict.ContainsKey(name))
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
            return null;
        }
        public Env Builtin(string name, Func<JsExpression[], Env, JsObject> lambda)
        {
            builtins.Add(name, lambda);
            return this;
        }

    }
}
