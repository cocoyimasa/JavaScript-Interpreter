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
-----------------------------------------------------

### JavaScript解释器开源项目重启

我依然会失败，只是从此以后，我不会再怀疑自己。----2017.5.1

去年的校招彻底打乱了我的开源计划，本来是想在去年做好JavaScript的解释器，今年继续完善类库的。结果就是整整停止了八个月。在这八个月里，我受尽别人白眼，在校招的面试中屡战屡败，几乎开始怀疑自己是不是一个合格的程序员，自尊心受到了严重的挑战，整个人是很颓废的，从来不玩游戏的我开始玩阴阳师，这个游戏带给我一个避风港，让我能够苦闷的日子里存活下来。感谢那些茨木童子、青行灯、雪女、姑获鸟、烟烟罗、萤草陪伴的日子，给我单调乏味的生活增添了很多色彩，在我远离技术的八个月里，给我另一片天空。

我的解释器项目不受人待见，面试官连问一句的心情都没有。其实我是很郁闷的。难道非要千篇一律的写个什么管理系统，做点数据挖掘才能让人感兴趣吗？一个解释器项目难道不令人耳目一新？我其实一直在想这个问题。后来想到我败在没有很好的表达自己。一个成功的面试，不只是要简历，还需要自己的引导和表达，我没有在适当的时机表达自己，他们都认为我简历全是编的。

不过现在我还是从失败的打击中缓了过来。我依然认为自己还是懂技术的，不管别人待不待见我。因为我曾经在编程的路上走过了那么多的风风雨雨，经验丰富，不能轻言放弃。

我觉得应该继续自己的开源计划。我要做一个功能相对完善的JS解释器。不要再像之前只是一个玩具。

我觉得我要支持更酷的语法，最好加入对ES6的支持。我要研究和完成一些关键特性的实现，避免在被人问到的时候只能尴尬的说没有支持。

我要为它写文档，记录自己的开发历程，同时给新手一些指导。

我的工作很无聊，很浪费时间，我不喜欢，可是依然要做，因为我要吃饭，因为我没本事换一个工作。我的掏粪机能薄弱，算法水平一般，所以大公司小公司我都进不去了，尤其在已经入职若干时间的今天。我感谢现在的公司收留我，虽然我的编译器开发岗被调了，成了网络协议开发，不过我依然不能一走了之。我在JS解释器上能够投入的时间应该不会有很多了，但是我依然决定以今年年底为限，以上的承诺要兑现。珍惜自己的碎片化时间，为开源贡献自己的代码。就这样。

@copyright  保留一切权利。
