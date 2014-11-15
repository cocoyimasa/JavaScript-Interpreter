JSInterpret
===========

My JavaScript Interpreter is write all by hand.
Surely I read some sources that writed by others.
I have learned many ideas and used them in my code.
It is not completed.Many Bugs 
Now it support only the core features in JavaScript.
The features it supports as fellows:

1. Basic Arithmetic
2. Function Declaration,Function Call
3. var declaration and assignment;if else;return;while;
4. Annoymous function.
5. Bool expression
In the future,it will support some library functions and array. 

Usage:
When you input a statement,add two commas(;;) ahter that.

For example: var v=100; ;; <-- The first ; is the mark that indicates you statement finished, 
and the last two ones tell the inetrpret to run. 

Some cases for you to test my interpreter:

function add(a){a=a+1;return a;};;

var testB=function(){return 10;};;;

var b=1;;;

var c=b;;;

var c=add(1);;;

var obj=new Object();;;

b;;;

1;;;

1+2;;;

100+100;;;

