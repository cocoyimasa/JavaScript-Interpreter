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
    /*"for", "while", "function", 
    "if", "else", "var","return","new","in","this",
    "instanceof","typeof","undefined","break",
    "import","class","let","const","switch",
    "default","continue"
     * 
     * 
     */
    public enum TokenType
    {
        ST,
        NUMBER, STRING, BOOLEAN, IDENTIFY,
        OpenBracket/*[*/, CloseBracket, OpenBrace/*{*/, CloseBrace,
        OpenParenthese/*(*/, CloseParenthese,
        ADD, SUB, MUL, DIV, COLON, EQ/*==*/, BIND/*=*/, SemiColon/*;*/, COMMA/*;*/,
        GT, LT, GE, LE, AND, OR, UNEQ, NOT,
        POINT,/*.*/
        FOR, WHILE, FUNCTION,
        IF, ELSE, VAR, RETURN, NEW, IN, THIS,
        INSTANCEOF, TYPEOF, UNDEFINED, BREAK,
        IMPORT, CLASS, LET, CONST, SWITCH,
        DEFAULT, CONTINUE
    }
    public class Token
    {
        public TokenType type;
        public string name;
        public Token()
        { }
        public Token(TokenType t, string n)
        {
            type = t;
            name = n;
        }
    }
}
