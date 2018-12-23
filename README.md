JavaScript Interpreter
===========
![type](https://img.shields.io/badge/type-library-pink.svg)
![platform](https://img.shields.io/badge/platform-windows-brightgreen.svg)
![build](https://img.shields.io/wercker/ci/wercker/docs.svg)
![license](https://img.shields.io/aur/license/yaourt.svg)

### <a href= "#china" style="border: 1px solid red ">中文版</a> (中文版链接)

### <p id='english'>English Version</p>

The JavaScript Interpreter is written all by myself.
Surely I read some source code that written by others, in books and websites.
I have learned many ideas and knowledge.
It is not good enough and has many shortages. Many Bugs still can be found. It is not tested strictly.

Now only the core features in JavaScript are supported.

## The features it supports as fellows:

* Basic Arithmetic ,+-*/.Bool expression, true false && ||  > < == != >= <=.
* Control flow -- if else while.
* Function Declaration,Function Call.
* use var to create declaration and assignment.
* Annoymous function.
* Built-in:
  * Object
  * Array
  * String
  * Number
  * Boolean
* . grammar for properties add and get,method call add and call.

In the future,it will support some library functions.

## Usage:
- How to use repl:

  * Use 
  ```
	InitEnv().GetJavaScriptConsole((code, env) => code.Parse().evaluate(env)); 
  ```
  in Main().
- How to run some code fragment without repl:
  
  * Use
  ```
     InitEnv().Run([string]codeFragment);
  ```
  in Main().
  
- While running in the repl mode, add two semi-colons before running.(";;")
Two semi-colons and an Enter key are signals for the interpreter to run the code that you have written.
Just don't forget this.
  - For example,
calculate a expression -- 1+1,you should write 1+1;; ,then click enter key，the interpreter shows the result --2.

 
  - ->>> 1+1;;

     - -- 2

  - ->>> function(){};;

     - --Function

  - ->>> var a =1;;

     - --1

- If not use the repl ,just test some code,no need to add two ';' after your code.Just add one ';' is all right.



## Some cases for you to test my interpreter:

* function add(a){a=a+1;return a;}

* var testB=function(){return 10;};

* var b=1;

* var c=b;

* var c=add(1);

* var obj=new Object();

* obj.v=10;

* obj.func = function(){return true;};

* var obj = new String('sssssssssssss');

* var str = 'ssssss';  

* b;

* 1;

* 1+2;

* 100+100;

@copyright  all rights preserved. 

# <p id='china'>JSInterpreter中文版README</p>


### <a href= "#english" style="border: 1px solid red ">English Version</a> (English Version Link)

这是一个纯手写代码的JavaScript解释器。参考过一些别人的代码，从中学到了一些想法并添加到了自己的代码中。这个解释器并不完整，有很多纰漏和不足，不支持的东西很多，而且难免有不少bug，我并没有进行严格的测试。

现在仅支持JavaScript的核心部分的某些东西，只是一个架子。但是写这个解释器的初衷是验证自己在编译原理的所学，验证完成即停止，对我来说已经是圆满。我并不在乎实用性。毕竟限于时间和精力，不可能在一个只剩下添砖加瓦的项目上继续做重复的事情，还有很多其他事情等着我去做。作为一个人，也是要吃饭的。

## 现在支持的主要特性：
* 基本数学运算，加减乘数+-*/，逻辑表达式 true false && ||  > < == != >= <=
* 控制流 -- if else; while;
* 函数定义，函数调用
* var关键字 声明变量，赋值语句，函数
* 匿名函数的声明和调用
* Built-in对象：
  * Object
  * Array
  * String
  * Number
  * Boolean
* 点语法为对象动态添加属性和方法，访问属性和调用方法

之后将添加一些基本的库函数。

##使用方法：

* 解释-打印循环的使用：
  * 
  ```
	InitEnv().GetJavaScriptConsole((code, env) => code.Parse().evaluate(env)); 
  ```
  在主函数中调用这一句。
* 不使用Repl，执行一些代码片段：
  * 
  ```
     InitEnv().Run([string]codeFragment);
  ```
* 使用repl的时候，写完一句代码，记得在后边加两个分号，分号+回车键是代表输入结束的标志。之后解释器会开始运行。千万不要忘记两个分号。
  * 举例说明一下吧，可能大部分人并没有用过微软的F#，不清楚这种Repl默认操作。
  * 当计算1+1时，在后边加两个分号，按下回车键，输出结果为2.
  - ->>> 1+1;;

     - -- 2

  - ->>> function(){};;

     - --Function

  - ->>> var a =1;;

     - --1  


* 如果不是再repl模式下，没必要添加两个分号，直接在该加分号的地方加一个分号即可。

## 一些用过的测试用例：

* function add(a){a=a+1;return a;}

* var testB=function(){return 10;};

* var b=1;

* var c=b;

* var c=add(1);

* var obj=new Object();

* obj.v=10;

* obj.func = function(){return true;};

* var obj = new String('sssssssssssss');

* var str = 'ssssss';  

* b;

* 1;

* 1+2;

* 100+100;
-----------------------------------------------------

### JavaScript解释器开源项目重启

----2017.5.1

去年的校招彻底打乱了我的开源计划，本来是想在去年做好JavaScript的解释器，今年继续完善类库的。结果就是整整停止了八个月。

我觉得应该继续自己的开源计划。我要做一个功能相对完善的JS解释器。不要再像之前只是一个玩具。

我觉得我要支持更酷的语法，最好加入对ES6的支持。我要研究和完成一些关键特性的实现，避免在被人问到的时候只能尴尬的说没有支持。

我要为它写文档，记录自己的开发历程，同时给新手一些指导。

我在JS解释器上能够投入的时间应该不会有很多了，但是我依然决定以今年年底为限，以上的承诺要兑现。珍惜自己的碎片化时间，为开源贡献自己的代码。就这样。

@copyright  保留一切权利。
