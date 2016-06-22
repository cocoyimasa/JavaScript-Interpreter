JSInterpreter
===========
![type](https://img.shields.io/badge/type-library-pink.svg)
![platform](https://img.shields.io/badge/platform-windows-brightgreen.svg)
![build](https://img.shields.io/wercker/ci/wercker/docs.svg)
![license](https://img.shields.io/aur/license/yaourt.svg)

### <a href= "#china" style="border: 1px solid red ">中文版</a> (中文版链接)

### <p id='english'>English Version</p>

My JavaScript Interpreter is written all by hand.
Surely I read some sources that written by others.
I have learned many ideas and used them in my code.
It is not completed and has many shortages.Many Bugs still can be found in this program.I have not test it strictly.

Now it supports only the core features in JavaScript.

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
  
- When running in repl mode, all two semi-colon before running.(";;")
Two semi-colon and a Enter key are signals for interpreter to run the code you write.
Don't forget these.
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

=====================

### <a href= "#english" style="border: 1px solid red ">English Version</a> (English Version Link)

这是一个纯手写代码的JavaScript解释器。参考过一些别人的代码，从中学到了一些想法并添加到了自己的代码中。这个解释器并不完整，有很多纰漏和不足，不支持的东西很多，而且难免有不少bug，我并没有进行严格的测试。

现在仅支持JavaScript的核心部分的某些东西，只是一个架子。略显空洞啊。

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


@copyright  保留一切权利。
