using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSInterpret;
using System.Linq;
using System.Collections.Generic;
/*
 *@author wanghesai
 *@time 2014/11/13-2014/11/15 
 *@time 2016/6/21 fix bugs and add library
 *@name JSInterpret
 ***/
namespace JsInterpreterUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestPointExpression()
        {
            string[] code = {
                "var a=new Object();\n" ,
                "a.b=1;\n" ,
                "a.b;"
                };
            Env env = JSInterpret.JavaScript.InitEnv();
            List<string> results =new List<string>();
            foreach(var codeFragment in code)
            {
                string result =env.Run(codeFragment);
                results.Add(result);
            }
            Assert.AreEqual(results[0], "Object");
            Assert.AreEqual(results[1], "1");
            Assert.AreEqual(results[2], "1");

        }
        [TestMethod]
        public void TestFunctionStatementAndCall()
        {
            string[] code ={
                               "function testExpList(a,b){\n"+
                               "a=10;\n"+
                               "b=100;\n"+
                               "var c=0;"+
                               "if(a==b){\n"+
                               "c=1;}"+
                               "return c;}",
                               "testExpList(10,20);"
                           };
            Env env = JSInterpret.JavaScript.InitEnv();
            List<string> results = new List<string>();
            foreach (var codeFragment in code)
            {
                string result = env.Run(codeFragment);
                results.Add(result);
            }
            Assert.AreEqual(results[0], "Function");
            Assert.AreEqual(results[1], "0");
        }
    }
}
