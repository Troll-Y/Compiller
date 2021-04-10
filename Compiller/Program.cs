﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Compiller
{
    class Program
    {
        static List<string> K = new List<string> { "program", "and", "bool", "break", "case", "char", "default", "do", "else", "false", "for", "if", "int", "or", "read", "switch", "true", "void", "while", "write", "not", "declare", "main", "end", "array", "of" };
        static List<string> R = new List<string> { "=", "<", ">", "+", "-", "(", ")", "/", "*", ".", ",", ";", "!", "|", "&", ":", "{", "}", "[", "]", "=" };
        static List<string> D = new List<string> { "==", "++", "--", "<=", ">=", "||", "&&", "!=", "<<", ">>", "+=", "-=", "*=", "/=", ":=" };
        static List<string> I = new List<string>();
        static List<string> C = new List<string>();
        static List<string> L = new List<string>();
        static List<string> my_R_Separ = new List<string>();
        static List<string> my_Doub_Separ = new List<string>();
        static List<string> my_K = new List<string>();
        static List<string> Comments = new List<string>();

        static List<string> startCode = new List<string>();
        static List<string> dataCode = new List<string>();
        static List<string> mainCode = new List<string>();

        public struct Word
        {
            public string word;
            public int type;
            public bool mas;
        }

        static List<Word> words = new List<Word>();
        static List<Word> ids = new List<Word>();

        static string generateAsmMark()
        {
            return String.Format("Mark{0}", markCount++);
        }

        static string[] str = File.ReadAllLines("data.txt");
        static int posB = 0;
        static int SavedPosB = 0;
        static int row = 0;
        static int counter = 0;
        static int body = 0;
        static int loop = 0;
        static int masLenght;
        static int markCount = 0;
        static string identificator;
        static int cmpType;
        static bool type;

        static int checkListConst(List<string> list, string keyWord)
        {
            int index = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (keyWord == list[i])
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public struct Token
        {
            public char key;
            public int value;
        }

        static void Translator()
        {
            string path = @"Y:\Лабораторная\DOS\D\prog.asm";
            //string path = @"Y:\Compiller\prog.asm";
            if (File.Exists(path))
                File.Delete(path);
            using (StreamWriter code = new StreamWriter(path, true))
            {
                foreach (string SCode in startCode)
                {
                    code.WriteLine(SCode);
                }

                foreach (string DCode in dataCode)
                {
                    code.WriteLine(DCode);
                }

                foreach (string MCOde in mainCode)
                {
                    code.WriteLine(MCOde);
                }
            }
        }

        static Token Scan(bool jump)
        {
            Token token;
            token.key = ' ';
            token.value = -1;
            int key = 0;
            string temp = "";
            string word = "";
            char[] buff = new char[str[row].Length + 1];
            buff = str[row].ToCharArray(0, str[row].Length);
            if (!jump)
            {
                SavedPosB--;
                if (SavedPosB > -1)
                    posB = SavedPosB;
                else
                {
                    row--;
                    buff = str[row].ToCharArray(0, str[row].Length);
                    posB = str[row].Length - 1;
                }
            }
            while (jump)
            {
                switch (key)
                {
                    case 0:
                        {
                            if (posB >= str[row].Length || buff[posB].Equals(' '))
                            {
                                posB++;
                                if (posB > str[row].Length)
                                {
                                    row++;
                                    posB = 0;
                                    buff = str[row].ToCharArray(0, str[row].Length);
                                }
                            }
                            else if (Char.IsLetter(buff[posB]))
                            {
                                key = 1;
                            }
                            else if (Char.IsDigit(buff[posB]))
                            {
                                key = 2;
                            }
                            else if (buff[posB] == '"')
                            {
                                key = 5;
                            }
                            else if (checkListConst(R, buff[posB].ToString()) != -1)
                            {
                                key = 3;
                            }
                            else if (buff[posB].Equals('#'))
                            {
                                jump = false;
                            }
                            else
                            {
                                Console.WriteLine("Неизвестный символ! - '" + buff[posB] + "'");
                                Thread.Sleep(5000);
                                Environment.Exit(0);
                            }
                        }
                        break;
                    case 1:
                        {
                            if (posB < buff.Length)
                            {
                                if (Char.IsLetter(buff[posB]) || buff[posB] == '_' || Char.IsDigit(buff[posB]))
                                {
                                    word += buff[posB];
                                    key = 1;
                                    posB++;
                                }
                                else
                                {
                                    if (checkListConst(K, word) != -1)
                                    {
                                        key = 6;
                                    }
                                    else if (checkListConst(I, word) == -1)
                                    {
                                        I.Add(word);
                                        token.key = 'I';
                                        token.value = checkListConst(I, word);
                                        jump = false;
                                        word = "";
                                        key = 0;
                                    }
                                    else
                                    {
                                        token.key = 'I';
                                        token.value = checkListConst(I, word);
                                        jump = false;
                                        word = "";
                                        key = 0;
                                    }
                                }
                            }
                            else
                            {
                                if (checkListConst(K, word) != -1)
                                {
                                    key = 6;
                                }
                                else if (checkListConst(I, word) == -1)
                                {
                                    posB--;
                                    I.Add(word);
                                    token.key = 'I';
                                    token.value = checkListConst(I, word);
                                    jump = false;
                                    word = "";
                                    key = 0;
                                }
                            }
                        }
                        break;
                    case 2:
                        {
                            if (posB < buff.Length)
                            {
                                if (Char.IsDigit(buff[posB]))
                                {
                                    word += buff[posB];
                                    key = 2;
                                    posB++;
                                }
                                else
                                {
                                    key = 0;
                                    if (checkListConst(C, word) == -1)
                                        C.Add(word);
                                    token.key = 'C';
                                    token.value = checkListConst(C, word);
                                    jump = false;
                                    word = "";
                                }
                            }
                            else
                            {
                                key = 0;
                                if (checkListConst(C, word) == -1)
                                    C.Add(word);
                                token.key = 'C';
                                token.value = checkListConst(C, word);
                                jump = false;
                                word = "";
                            }
                        }
                        break;
                    case 3:
                        {
                            word += buff[posB];
                            temp = word;
                            posB++;
                            if (posB <= buff.Length)
                            {
                                if ("=+-|&<>:".IndexOf(word) != -1 &&
                                checkListConst(D, temp += buff[posB].ToString()) != -1)
                                {
                                    key = 4;
                                }
                                else if (word == "/" && "/*".IndexOf(buff[posB]) != -1)
                                {
                                    char start = buff[posB];
                                    if (start.Equals('/'))
                                        key = 7;
                                    else if (start.Equals('*'))
                                        key = 8;
                                }
                                else
                                {
                                    temp = "";
                                    my_R_Separ.Add(word);
                                    token.key = 'R';
                                    token.value = checkListConst(R, word);
                                    jump = false;
                                    word = "";
                                    key = 0;
                                }
                            }
                            else
                            {
                                posB--;
                                temp = "";
                                my_R_Separ.Add(word);
                                token.key = 'R';
                                token.value = checkListConst(R, word);
                                jump = false;
                                word = "";
                                key = 0;
                            }
                        }
                        break;
                    case 4:
                        {
                            if (checkListConst(D, temp) != -1)
                            {
                                word += buff[posB];
                                my_Doub_Separ.Add(word);
                                token.key = 'D';
                                token.value = checkListConst(D, word);
                                jump = false;
                                word = "";
                                key = 0;
                                posB++;
                            }
                            if (posB >= str[row].Length)
                            {
                                row++;
                                posB = 0;
                                buff = str[row].ToCharArray(0, str[row].Length);
                            }
                        }
                        break;
                    case 5:
                        {
                            word += buff[posB];
                            posB++;
                            while (buff[posB] != '"')
                            {
                                word += buff[posB];
                                posB++;
                            }
                            word += buff[posB];
                            posB++;
                            L.Add(word);
                            token.key = 'L';
                            token.value = checkListConst(L, word);
                            jump = false;
                            word = "";
                            key = 0;
                        }
                        break;
                    case 6:
                        {
                            my_K.Add(word);
                            token.key = 'K';
                            token.value = checkListConst(K, word);
                            jump = false;
                            word = "";
                            key = 0;
                        }
                        break;
                    case 7:
                        {
                            while (posB < str[row].Length)
                            {
                                word += buff[posB];
                                posB++;
                            }
                            if (posB >= str[row].Length)
                            {
                                row++;
                                posB = 0;
                                buff = str[row].ToCharArray(0, str[row].Length);
                            }
                            word = word.Replace("//", "");
                            Comments.Add(word);
                            mainCode.Add("; " + word);
                            token.key = 'S';
                            token.value = checkListConst(Comments, word);
                            jump = true;
                            word = "";
                            key = 0;
                        }
                        break;
                    case 8:
                        {
                            while (buff[posB] != '/')
                            {
                                word += buff[posB];
                                posB++;
                                if (posB >= str[row].Length)
                                {
                                    row++;
                                    posB = 0;
                                    buff = str[row].ToCharArray(0, str[row].Length);
                                }
                            }
                            word += buff[posB];
                            posB++;
                            Comments.Add(word);
                            token.key = 'S';
                            token.value = checkListConst(Comments, word);
                            jump = true;
                            word = "";
                            key = 0;
                        }
                        break;

                }
            }
            SavedPosB = posB;
            return token;
        }

        static void E()
        {
            T();
            Token token = Scan(true);
            if (token.key == 'R' && (token.value == 3 || token.value == 4))
            {
                int value = token.value;
                E();
                if (value == 3)
                {
                    mainCode.Add("\tpop bx");
                    mainCode.Add("\tpop ax");
                    mainCode.Add("\tadd ax, bx");
                    mainCode.Add("\tpush ax");
                }
                else
                {
                    mainCode.Add("\tpop bx");
                    mainCode.Add("\tpop ax");
                    mainCode.Add("\tsub ax, bx");
                    mainCode.Add("\tpush ax");
                }
            }
            else if (token.key == 'D')
            {
                for (int i = 0; i < GetWord(token, D).Length; i++)
                {
                    Scan(false);
                }
            }
            else
                Scan(false);
        }

        static void T()
        {
            F();
            Token token = Scan(true);
            if (counter == 0)
                if (token.key == 'R' && token.value == 6)
                {
                    Error("T", 8);
                }
                else if (token.key == 'R' && (token.value == 7 || token.value == 8))
                {
                    int value = token.value;
                    T();
                    if (value == 7)
                    {
                        mainCode.Add("\tpop bx");
                        mainCode.Add("\tpop ax");
                        mainCode.Add("\txor dx, dx");
                        mainCode.Add("\tdiv bx");
                        mainCode.Add("\tpush ax");
                    }
                    else
                    {
                        mainCode.Add("\tpop bx");
                        mainCode.Add("\tpop ax");
                        mainCode.Add("\tmul bx");
                        mainCode.Add("\tpush ax");
                    }
                }
                else if (token.key == 'D')
                {
                    for (int i = 0; i < GetWord(token, D).Length; i++)
                    {
                        Scan(false);
                    }
                }
                else
                    Scan(false);
            else if (counter != 0)
                if (token.key == 'R' && (token.value == 7 || token.value == 8))
                {
                    int value = token.value;
                    T();
                    if (value == 7)
                    {
                        mainCode.Add("\tpop bx");
                        mainCode.Add("\tpop ax");
                        mainCode.Add("\tmul bx");
                        mainCode.Add("\tpush ax");
                    }
                    else
                    {
                        mainCode.Add("\tpop bx");
                        mainCode.Add("\tpop ax");
                        mainCode.Add("\txor dx, dx");
                        mainCode.Add("\tdiv bx");
                        mainCode.Add("\tpush ax");
                    }
                }
                else if (token.key == 'D')
                {
                    for (int i = 0; i < GetWord(token, D).Length; i++)
                    {
                        Scan(false);
                    }
                }
                else
                    Scan(false);
        }

        static void F()
        {
            Token token = Scan(true);
            if (token.key == 'R' && (token.value == 3 || token.value == 4 || token.value == 7 || token.value == 8))
            {
                Error("F", 13);
            }
            if (token.key == 'R' && token.value == 6)
            {
                Error("F", 8);
            }
            if (token.key == 'R' && token.value == 5)
            {
                counter++;
                E();
                token = Scan(true);
                if (!(token.key == 'R' && token.value == 6))
                {
                    Error("F", 9);
                }
                counter--;
                token = Scan(true);
                if (token.key == 'R' && token.value == 11)
                {
                    Scan(false);
                }
                else if(token.key == 'R' && (token.value == 3 || token.value == 4 || token.value == 7 || token.value == 8))
                {
                    Scan(false);
                    token.value = -1;
                }
            }
            if (!(token.key == 'I' || token.key == 'C' || token.key == 'R' && (token.value == 11 || token.value == -1)))
            {
                Error("F", 13);
            }
            if (token.key == 'C')
            {
                mainCode.Add("\tmov ax, " + Convert.ToInt32(GetWord(token, C)));
                mainCode.Add("\tpush ax");
            }
            else if (token.key == 'I')
            {
                int temp = 1;
                for (int i = 0; i < ids.Count; i++)
                {
                    if (ids[i].word.Equals(GetWord(token, I)))
                    {
                        temp = ids[i].type;
                        break;
                    }                       
                }
                switch (temp)
                {
                    case 0:
                        {
                            mainCode.Add("\tmov di, 0");
                            mainCode.Add("\tmov ax, 0");
                            mainCode.Add("\tmov al, " + GetWord(token, I) + "[di]");
                            mainCode.Add("\tpush ax");
                        }
                        break;
                    case 1:
                        {
                            mainCode.Add("\tmov di, 0");
                            mainCode.Add("\tmov ax, " + GetWord(token, I) + "[di]");
                            mainCode.Add("\tpush ax");
                        }
                        break;
                    case 2:
                        Error("F", 13);
                        break;
                }
            }
        }


        static void EL()
        {
            TL();
            Token token = Scan(true);
            if (token.key == 'D' && token.value == 5) // ||
            {
                TL();
                mainCode.Add("\tpop bx");
                mainCode.Add("\tpop ax");
                mainCode.Add("\tor ax, bx");
                mainCode.Add("\tpush ax");
            }
            else
                Scan(false);
        }
        static void TL()
        {
            FL();
            Token token = Scan(true);
            if (token.key == 'D' && token.value == 6) // &&
            {
                FL();
                mainCode.Add("\tpop bx");
                mainCode.Add("\tpop ax");
                mainCode.Add("\tand ax, bx");
                mainCode.Add("\tpush ax");
            }
            else if (token.key == 'D')
            {
                for (int i = 0; i < GetWord(token, D).Length; i++)
                {
                    Scan(false);
                }
            }
            else
                Scan(false);
        }
        static void FL()
        {
            Token token = Scan(true);
            string trueMark, falseMark;
            if ((token.key == 'R' && token.value == 6)) // (
            {
                Error("FL", 9);
            }
            if (token.key == 'R' && token.value == 5)
            {
                counter++;
                EL();
                token = Scan(true);
                if (!(token.key == 'R' && token.value == 6)) // (
                {
                    Error("FL", 9);
                }
                counter--;
                token = Scan(true);
                if (token.key == 'R' && token.value != 11) // ;
                {
                    Scan(false);
                    token.value = -1;
                }
                else if (token.key == 'D')
                {
                    Scan(false);
                    Scan(false);
                    token.value = -1;
                }
                else
                {
                    Scan(false);
                }
            }
            if (counter != 0)
            {
                if (!(token.key == 'I' || token.key == 'C' || token.key == 'K' || token.key == 'R'))
                {
                    Error("FL", 20);
                }
                if (token.key == 'R' && token.value == 12) // !
                {
                    FL();
                    mainCode.Add("\tpop ax");
                    mainCode.Add("\tnot ax");
                    mainCode.Add("\tpush ax");
                }
                else if (token.key == 'K' && (token.value == 9 || token.value == 16))
                {
                    switch (token.value)
                    {
                        case 9: mainCode.Add("\tpush 0"); break; // false
                        case 16: mainCode.Add("\tpush 1"); break; // true
                    }
                }
                else if (token.key == 'I' || token.key == 'C')
                {
                    if (token.key == 'I')
                        for (int i = 0; i < GetWord(token, I).Length; i++)
                        {
                            Scan(false);
                        }
                    else
                        for (int i = 0; i < GetWord(token, C).Length; i++)
                        {
                            Scan(false);
                        }
                    E();
                    Z();
                    E();
                    mainCode.Add("\tpop bx");
                    mainCode.Add("\tpop ax");
                    mainCode.Add("\tcmp ax, bx");

                    trueMark = generateAsmMark();
                    falseMark = generateAsmMark();

                    switch (cmpType)
                    {
                        case 0: mainCode.Add("\tje " + trueMark); break; // ==
                        case 1: mainCode.Add("\tjb " + trueMark); break; // <
                        case 2: mainCode.Add("\tja " + trueMark); break; // >
                        case 3: mainCode.Add("\tjbe " + trueMark); break; // <=
                        case 4: mainCode.Add("\tjae " + trueMark); break; // >=
                        case 7: mainCode.Add("\tjne " + trueMark); break; // !=
                    }

                    mainCode.Add("\tpush 0");
                    mainCode.Add("\tjmp " + falseMark);
                    mainCode.Add(trueMark + ":");
                    mainCode.Add("\tpush 1");
                    mainCode.Add(falseMark + ":");
                }
            }
            else if (!(token.key == 'K' && (token.value == 9 || token.value == 16) || (token.key == 'D' && token.value == -1) ||
                    token.key == 'R' && (token.value == 11 || token.value == -1))) // false || true, ; , all D, all R
            {
                Error("FL", 13);
            }
        }

        static void Z()
        {
            Token token = Scan(true);
            if (!(token.key == 'R' && (token.value == 1 || token.value == 2) ||
                token.key == 'D' && (token.value == 0 || token.value == 3 || token.value == 4 || token.value == 7))) // >, <, ==, <=, >=, !=
            {
                Error("ZN", 13);
            }
            else
            {
                cmpType = token.value;
            }
        }

        static string GetWord(Token token, List<string> list)
        {
            string keyWord = "";
            for (int i = 0; i < list.Count; i++)
            {
                if (token.value == i)
                {
                    keyWord = list[i];
                    break;
                }
            }
            return keyWord;
        }

        static void Progr() // начало программы 
        {
            Token token = Scan(true);
            if (!(token.key == 'K' && token.value == 0))
            {
                Error("Progr", 12);
            }
            else
            {
                startCode.Add("\t.8086");
                startCode.Add("\t.model small");
                startCode.Add("\t.stack 100h");
                startCode.Add("\t.data");
                mainCode.Add(".code");
                token = Scan(true);
                if (!(token.key == 'I'))
                {
                    Error("Progr", 0);
                }
                else
                {

                    token = Scan(true);
                    if (!(token.key == 'R' && token.value == 11)) // ;
                    {
                        Error("Progr", 1);
                    }
                    else
                    {
                        token = Scan(true);
                        if (!(token.key == 'K' && (token.value == 21 || token.value == 22)))
                        {
                            Error("Progr", 7);
                        }
                        else
                        {
                            if (token.key == 'K' && token.value == 21)
                            {
                                dataCode.Add("; support data");
                                dataCode.Add("\t@buffer   db    6");
                                dataCode.Add("\tblength   db    (?)");
                                dataCode.Add("\t@buf      db    256 DUP (?)");
                                dataCode.Add("\terr_msg   db    \"Input error, try again\", 0Dh, 0Ah, \"$\"");
                                dataCode.Add("\t@true     db    \"true\"");
                                dataCode.Add("\t@false     db    \"false\"");
                                dataCode.Add("\t@@true     db    \"true$\"");
                                dataCode.Add("\t@@false     db    \"false$\"");
                                dataCode.Add("\tclrf   db   0Dh, 0Ah, \"$\"");
                                dataCode.Add("\toutput  db  6 DUP (?), \"$\"");
                                dataCode.Add("; using data");
                                Declare();
                                token = Scan(true);
                                if (!(token.key == 'K' && token.value == 22))
                                {
                                    Error("Progr", 7);
                                }
                                else
                                {
                                    mainCode.Add("main :");
                                    mainCode.Add("\tmov ax, @data");
                                    mainCode.Add("\tmov ds, ax");
                                    mainCode.Add("\tmov es, ax");
                                    Body();
                                    End();
                                    mainCode.Add("\tmov ax, 4C00h");
                                    mainCode.Add("\tint 21h");
                                    mainCode.Add("end main");
                                }
                            }
                            else if (token.key == 'K' && token.value == 22)
                            {
                                mainCode.Add("main :");
                                mainCode.Add("\tmov ax, @data");
                                mainCode.Add("\tmov ds, ax");
                                mainCode.Add("\tmov es, ax");
                                Body();
                                End();
                                mainCode.Add("\tmov ax, 4C00h");
                                mainCode.Add("\tint 21h");
                                mainCode.Add("end main");
                            }
                            else
                            {
                                Error("Progr", 7);
                            }
                        }
                    }
                }
            }
        }


        static void End() //конец программы
        {
            Token token = Scan(true);
            if (!(token.key == 'K' && token.value == 23)) //end
            {
                Error("End", 5);
            }
            else
            {
                token = Scan(true);
                if (!(token.key == 'K' && (token.value == 23) || (token.key == 'R' && token.value == 9)))
                {
                    Error("End", 2);
                }
            }
        }

        static void Declare()
        {
            Token token = Scan(true);
            Word w;
            if (!(token.key == 'I'))
            {
                Error("Declare", 0);
            }
            else
            {
                w.word = GetWord(token, I);
                w.type = 0;
                w.mas = false;
                words.Add(w);
                token = Scan(true);
                if (token.key == 'R' && token.value == 10)
                {
                    Declare();
                }
                else if (!(token.key == 'R' && token.value == 15)) //:
                {
                    Error("Declare", 2);
                }
                else if (token.key == 'R' && token.value == 15)
                {
                    VarType();
                    foreach (Word ws in words)
                        AddData(ws);
                    words.Clear();
                }
                token = Scan(true);
                if (token.key == 'I')
                {
                    for (int i = 0; i < GetWord(token, I).Length; i++)
                    {
                        Scan(false);
                    }
                    Declare();
                }
                else if (token.key == 'K' && token.value == 22)
                {
                    for (int i = 0; i < GetWord(token, K).Length; i++)
                    {
                        Scan(false);
                    }
                }
                else
                {
                    Error("Declare", 13);
                }
            }
        }

        static void VarType()
        {
            Token token = Scan(true);
            if (token.key == 'K' && token.value == 24) // array
            {
                for (int i = 0; i < words.Count; i++)
                {
                    Word temp;
                    temp.word = words[i].word;
                    temp.mas = true;
                    temp.type = words[i].type;
                    words.RemoveAt(i);
                    words.Insert(i, temp);
                }
                FArray();
            }
            else if (token.key == 'K' && (token.value == 2 || token.value == 5 || token.value == 12))
            {
                for (int i = 0; i < GetWord(token, K).Length; i++)
                {
                    Scan(false);
                }
                Basic();
            }
            else
            {
                Error("Type", 5);
            }
        }


        static void FArray()
        {
            Token token = Scan(true);
            if (!(token.key == 'R' && token.value == 18))//[
            {
                Error("Array", 14);
            }
            if (token.key == 'R' && token.value == 18)
            {
                token = Scan(true);
                if (!(token.key == 'C')) // 1234..
                {
                    Error("Array", 6);
                }
                masLenght = Convert.ToInt32(GetWord(token, C));
                token = Scan(true);
                if (!(token.key == 'R' && token.value == 19)) //]
                {
                    Error("Array", 15);
                }
                token = Scan(true);
                if (!(token.key == 'K' && token.value == 25)) // of
                {
                    Error("Array", 17);
                }
                token = Scan(true);
                if (token.key == 'K' && (token.value == 2 || token.value == 5 || token.value == 12))
                {
                    for (int i = 0; i < GetWord(token, K).Length; i++)
                    {
                        Scan(false);
                    }
                    Basic();
                }
                else
                {
                    Error("Array", 10);
                }
            }
        }

        static void Basic()
        {
            Token token = Scan(true);
            if (token.value == 2) // bool
            {
                for (int i = 0; i < words.Count; i++)
                {
                    Word temp;
                    temp.word = words[i].word;
                    temp.mas = words[i].mas;
                    temp.type = 0;
                    words.RemoveAt(i);
                    words.Insert(i, temp);
                }
            }
            else if (token.value == 12) // int
            {
                for (int i = 0; i < words.Count; i++)
                {
                    Word temp;
                    temp.word = words[i].word;
                    temp.mas = words[i].mas;
                    temp.type = 1;
                    words.RemoveAt(i);
                    words.Insert(i, temp);
                }
            }
            else if (token.value == 5) // char
            {
                for (int i = 0; i < words.Count; i++)
                {
                    Word temp;
                    temp.word = words[i].word;
                    temp.mas = words[i].mas;
                    temp.type = 2;
                    words.RemoveAt(i);
                    words.Insert(i, temp);
                }
            }
            token = Scan(true);
            if (!(token.key == 'R' && token.value == 11))
            {
                Error("Declare", 1);
            }
        }

        static void Body()
        {
            Token token = Scan(true);
            if (!(token.key == 'R' && token.value == 16)) // {
            {
                Error("Main", 3);
            }
            if (token.key == 'R' && token.value == 16)
            {
                body++;
                Operators();
                token = Scan(true);
                if (token.key == 'I' || token.key == 'K' && (token.value == 10 || token.value == 11 || token.value == 14 || token.value == 18 || token.value == 19))
                {
                    switch (token.key)
                    {
                        case 'I':
                            for (int i = 0; i < GetWord(token, I).Length; i++)
                            {
                                Scan(false);
                            }
                            break;
                        case 'K':
                            for (int i = 0; i < GetWord(token, K).Length; i++)
                            {
                                Scan(false);
                            }
                            break;
                    }
                    Operators();
                }
                if (!(token.key == 'R' && token.value == 17))
                {
                    Error("Body", 4);
                }
                body--;
            }
        }

        static void Operators()
        {
            Token token = Scan(true);
            if (token.key == 'R' && token.value == 17)
            {

            }
            else if (!(token.key == 'I' || token.key == 'K'))
            {
                Error("Body operators", 18);
            }
            if (body != 0)
            {
                if (!(token.key == 'I' || token.key == 'K' && (token.value == 10 || token.value == 11 || token.value == 14 || token.value == 18 || token.value == 19 || token.value == 23) || token.key == 'R' && token.value == 17))
                {
                    Error("Body operators", 18);
                }
                else if (token.key == 'I')
                {
                    identificator = GetWord(token, I);
                    type = false;
                    if (Assign())
                    {
                        Scan(false);
                        Scan(false);
                        token = Scan(true);
                    }
                    if (token.key == 'D' && token.value == 14)
                    {
                        token = Scan(true);
                        if (!(token.key == 'I' || token.key == 'C' || token.key == 'L' || (token.key == 'K' && (token.value == 9 || token.value == 16)) || (token.key == 'R' && token.value == 5)))
                        {
                            Error("Let", 13);
                        }
                        else if (token.key == 'L')
                        {
                            for (int i = 0; i < ids.Count; i++)
                            {
                                if (ids[i].word.Equals(identificator) && ids[i].type == 2)
                                {
                                    mainCode.Add("; let char");
                                    char[] s = GetWord(token, L).ToArray();
                                    mainCode.Add("\tpush \"" + s[1] + "\"");
                                    mainCode.Add("\tpop ax");
                                    mainCode.Add("\tmov " + identificator + ", al");
                                    type = true;
                                    break;
                                }
                            }
                            if (!type)
                            {
                                Error("Let literal", 21);
                            }
                            token = Scan(true);
                        }
                        else if (token.key == 'C' || token.key == 'I' || ( token.key == 'R' && token.value == 5))
                        {
                            switch (token.key)
                            {
                                case 'I':
                                    for (int i = 0; i < GetWord(token, I).Length; i++)
                                    {
                                        Scan(false);
                                    }
                                    break;
                                case 'C':
                                    for (int i = 0; i < GetWord(token, C).Length; i++)
                                    {
                                        Scan(false);
                                    }
                                    break;
                                case 'R':
                                    Scan(false);
                                    break;
                            }
                            for (int i = 0; i < ids.Count; i++)
                            {
                                if (ids[i].word.Equals(identificator) && ids[i].type == 1)
                                {
                                    mainCode.Add("; let ariphmetics");
                                    E();
                                    mainCode.Add("\tpop ax");
                                    mainCode.Add("\tmov " + identificator + ", ax");
                                    type = true;
                                    break;
                                }
                            }
                            if (!type)
                            {
                                Error("Let ariphmetics", 21);
                            }
                            token = Scan(true);
                        }
                        else if (token.key == 'K' && (token.value == 9 || token.value == 16))
                        {
                            for (int i = 0; i < ids.Count; i++)
                            {
                                if (ids[i].word.Equals(identificator) && ids[i].type == 0)
                                {
                                    mainCode.Add("; let bool");
                                    if (token.value == 9) // false
                                    {
                                        mainCode.Add("\tpush 0");
                                        mainCode.Add("\tpop ax");
                                        mainCode.Add("\tmov " + identificator + ", al");
                                    }
                                    else //true
                                    {
                                        mainCode.Add("\tpush 1");
                                        mainCode.Add("\tpop ax");
                                        mainCode.Add("\tmov " + identificator + ", al");
                                    }
                                    type = true;
                                    break;
                                }
                            }
                            if (!type)
                            {
                                Error("Let bool", 21);
                            }
                            token = Scan(true);
                        }
                        if (body == 0) token = Scan(true);
                        if (!(token.key == 'R' && token.value == 11))
                        {
                            Error("Body operators", 1);
                        }
                    }
                }
                else if (token.key == 'K' && token.value == 11) //if
                {
                    mainCode.Add("; if()");
                    EL();
                    string thenMark = generateAsmMark();
                    string elseMark = generateAsmMark();
                    mainCode.Add("\tpop ax");
                    mainCode.Add("\tcmp ax, 0");
                    mainCode.Add("\tjz " + thenMark);
                    mainCode.Add("; if() then");
                    Loop();
                    mainCode.Add("\tjmp " + elseMark);
                    mainCode.Add(thenMark + ":");
                    token = Scan(true);
                    if (token.key == 'K' && token.value == 8)
                    {
                        mainCode.Add("; if() else");
                        Loop();
                    }
                    else if (token.key == 'I' || token.key == 'K' && (token.value == 10 || token.value == 11 || token.value == 14 || token.value == 18 || token.value == 19))
                    {
                        switch (token.key)
                        {
                            case 'I':
                                for (int i = 0; i < GetWord(token, I).Length; i++)
                                {
                                    Scan(false);
                                }
                                break;
                            case 'K':
                                for (int i = 0; i < GetWord(token, K).Length; i++)
                                {
                                    Scan(false);
                                }
                                break;
                        }
                        Operators();
                    }
                    else if (token.key == 'K' && token.value == 23)
                    {
                        for (int i = 0; i < GetWord(token, K).Length; i++)
                        {
                            Scan(false);
                        }
                    }
                    else
                    {
                        Scan(false);
                    }
                    mainCode.Add(elseMark + ":");
                }
                else if (token.key == 'K' && token.value == 18) //while
                {
                    mainCode.Add("; while()");
                    string whileMark = generateAsmMark();
                    string endMark = generateAsmMark();
                    mainCode.Add(whileMark + ":");
                    EL();
                    mainCode.Add("\tpop ax");
                    mainCode.Add("\tcmp ax, 0");
                    mainCode.Add("\tjz " + endMark);
                    Loop();
                    mainCode.Add("\tjmp " + whileMark);
                    mainCode.Add(endMark + ":");
                }
                else if (token.key == 'K' && token.value == 14) //read
                {
                    mainCode.Add("; readln()");
                    ReadF();
                    mainCode.Add("\tlea   dx, clrf");
                    mainCode.Add("\tmov   ah, 9");
                    mainCode.Add("\tint   21h");
                }
                else if (token.key == 'K' && token.value == 19) //write
                {
                    mainCode.Add("; writeln()");
                    WriteF();
                    mainCode.Add("\tlea   dx, clrf");
                    mainCode.Add("\tmov   ah, 9");
                    mainCode.Add("\tint   21h");
                }
                else if (token.key == 'K' && token.value == 23) // end
                {
                    for (int i = 0; i < GetWord(token, K).Length; i++)
                    {
                        Scan(false);
                    }
                }
            }
            token = Scan(true);
            if (token.key == 'I' || token.key == 'K' && (token.value == 10 || token.value == 11 || token.value == 14 || token.value == 18 || token.value == 19 || token.value == 23))
            {
                switch (token.key)
                {
                    case 'I':
                        for (int i = 0; i < GetWord(token, I).Length; i++)
                        {
                            Scan(false);
                        }
                        break;
                    case 'K':
                        for (int i = 0; i < GetWord(token, K).Length; i++)
                        {
                            Scan(false);
                        }
                        break;
                }
                Operators();
            }
            else if (token.key == 'R' && token.value == 17)
            {
                Scan(false);
            }
        }

        static void ReadF()
        {
            Token token = Scan(true);
            if (!(token.key == 'R' && token.value == 5)) //(
            {
                Error("Read", 8);
            }
            else
            {
                counter++;
                token = Scan(true);
                identificator = GetWord(token, I);
                type = false;
                if (!(token.key == 'I' || (token.key == 'R' && token.value == 6)))
                {
                    Error("Read", 0);
                }
                for (int i = 0; i < ids.Count; i++)
                {
                    if (ids[i].word.Equals(identificator))
                    {                       
                        switch (ids[i].type)
                        {
                            case 0:  //bool
                                ReadBool(ids[i].word);
                                break;
                            case 1://int
                                ReadInt(ids[i].word);
                                break;
                            case 2://char
                                ReadChar(ids[i].word);
                                break;
                        }
                        type = true;
                        break;
                    }
                }
                if (!type)
                {
                    Error("ReadF", 21);
                }
                token = Scan(true);
                while (!(token.key == 'R' && token.value == 6)) //)
                {
                    if (!(token.key == 'R' && token.value == 10)) //,
                    {
                        Error("Read", 19);
                    }
                    token = Scan(true);
                    identificator = GetWord(token, I);
                    type = false;
                    if (!(token.key == 'I'))
                    {
                        Error("Read", 0);
                    }
                    for (int i = 0; i < ids.Count; i++)
                    {
                        if (ids[i].word.Equals(identificator))
                        {
                            switch (ids[i].type)
                            {
                                case 0:  //bool
                                    ReadBool(ids[i].word);
                                    break;
                                case 1://int
                                    ReadInt(ids[i].word);
                                    break;
                                case 2://char
                                    ReadChar(ids[i].word);
                                    break;
                            }
                            type = true;
                            break;
                        }
                    }
                    if (!type)
                    {
                        Error("ReadF", 21);
                    }
                    token = Scan(true);
                }
                token = Scan(true);
                if (!(token.key == 'R' && token.value == 11)) //;
                {
                    Error("Read", 1);
                }
                else
                    counter--;
            }
        }

        static void ReadBool(string id)
        {
            mainCode.Add("; read bool()");
            string lstart = generateAsmMark();
            string l4 = generateAsmMark();
            string l5 = generateAsmMark();
            string le = generateAsmMark();
            string lt = generateAsmMark();
            string lend = generateAsmMark();
            string lerror = generateAsmMark();
            mainCode.Add(lstart + ":");
            mainCode.Add("\tmov ah, 0Ah");
            mainCode.Add("\tlea dx, @buffer");
            mainCode.Add("\tint 21h");
            mainCode.Add("\tcmp blength, 4");
            mainCode.Add("\tje " + l4);
            mainCode.Add("\tcmp blength, 5");
            mainCode.Add("\tje " + l5);
            mainCode.Add("\tjmp " + lerror);
            mainCode.Add(l4 + ":");
            mainCode.Add("\tlea si, @true");
            mainCode.Add("\tlea di, @buf");
            mainCode.Add("\tmov cx, 4");
            mainCode.Add("\trepe cmpsb");
            mainCode.Add("\tjz " + le);
            mainCode.Add("\tjmp " + lerror);
            mainCode.Add(l5 + ":");
            mainCode.Add("\tlea si, @false");
            mainCode.Add("\tlea di, @buf");
            mainCode.Add("\tmov cx, 5");
            mainCode.Add("\trepe cmpsb");
            mainCode.Add("\tjz " + le);
            mainCode.Add("\tjmp " + lerror);
            mainCode.Add(le + ":");
            mainCode.Add("\tcmp @buf[0], \"t\"");
            mainCode.Add("\tje " + lt);
            mainCode.Add("\tpush 0");
            mainCode.Add("\tjmp " + lend);
            mainCode.Add(lt + ":");
            mainCode.Add("\tpush 1");
            mainCode.Add("\tjmp " + lend);
            mainCode.Add(lerror + ":");
            mainCode.Add("\tlea dx, err_msg");
            mainCode.Add("\tmov ah, 9");
            mainCode.Add("\tint 21h");
            mainCode.Add("\tjmp " + lstart);
            mainCode.Add(lend + ":");
            mainCode.Add("\tpop ax");
            mainCode.Add("\tmov di, 0");
            mainCode.Add("\tmov " + id + "[di], al");
        }

        static void ReadInt(string id)
        {
            mainCode.Add("; read int()");
            string lstart = generateAsmMark();
            string lMark = generateAsmMark();
            string lend = generateAsmMark();
            string lerror = generateAsmMark();
            mainCode.Add(lstart + ":");
            mainCode.Add("\tmov ah, 0Ah");
            mainCode.Add("\tlea dx, @buffer");
            mainCode.Add("\tint 21h");
            mainCode.Add("\tmov ax, 0");
            mainCode.Add("\tmov cx, 0");
            mainCode.Add("\tmov cl, byte ptr[blength]");
            mainCode.Add("\tmov bx, cx");
            mainCode.Add(lMark + ":");
            mainCode.Add("\tdec bx");
            mainCode.Add("\tmov al, @buf[bx]");
            mainCode.Add("\tcmp al, \"9\"");
            mainCode.Add("\tja " + lerror);
            mainCode.Add("\tcmp al, \"0\"");
            mainCode.Add("\tjb " + lerror);
            mainCode.Add("\tloop " + lMark);
            mainCode.Add("\tmov cl, byte ptr[blength]");
            mainCode.Add("\tmov di, 0");
            mainCode.Add("\tmov ax, 0");
            lMark = generateAsmMark();
            mainCode.Add(lMark + ":");
            mainCode.Add("\tmov bl, @buf[di]");
            mainCode.Add("\tinc di");
            mainCode.Add("\tsub bl, 30h");
            mainCode.Add("\tadd ax, bx");
            mainCode.Add("\tmov si, ax");
            mainCode.Add("\tmov bx, 10");
            mainCode.Add("\tmul bx");
            mainCode.Add("\tloop " + lMark);
            mainCode.Add("\tmov ax, si");
            mainCode.Add("\tpop di");           
            mainCode.Add("\tjmp " + lend);
            mainCode.Add(lerror + ":");
            mainCode.Add("\tlea dx, err_msg");
            mainCode.Add("\tmov ah, 9");
            mainCode.Add("\tint 21h");
            mainCode.Add("\tjmp " + lstart);
            mainCode.Add(lend + ":");
            mainCode.Add("\tmov di, 0");
            mainCode.Add("\tmov " + id + ", ax");
        }

        static void ReadChar(string id)
        {
            mainCode.Add("; read char()");
            mainCode.Add("\tmov ah, 0Ah");
            mainCode.Add("\tlea dx, @buffer");
            mainCode.Add("\tint 21h");
            mainCode.Add("\txor dx, dx");
            mainCode.Add("\tmov dl, @buf[0]");
            mainCode.Add("\tpop di");
            mainCode.Add("\tmov " + id + ", dl");
        }

        static void WriteF()
        {
            Token token = Scan(true);
            if (!(token.key == 'R' && token.value == 5)) //(
            {
                Error("Write", 8);
            }
            else
            {
                counter++;
                token = Scan(true);
                if (!(token.key == 'I' || token.key == 'L' || (token.key == 'R' && token.value == 6)))
                {
                    Error("Write", 0);
                }
                else if(token.key == 'L')
                {
                    WriteLiteral(GetWord(token, L).Replace("\"", ""));
                }
                else if(token.key == 'I')
                {
                    identificator = GetWord(token, I);
                    type = false;
                    for (int i = 0; i < ids.Count; i++)
                    {
                        if (ids[i].word.Equals(identificator))
                        {
                            for (int j = 0; j < GetWord(token, I).Length; j++)
                            {
                                Scan(false);
                            }
                            switch (ids[i].type)
                            {
                                case 0:  //bool
                                    {                                       
                                        E();
                                        WriteBool();
                                    }
                                    break;
                                case 1://int
                                    {
                                        E();
                                        WriteInt();
                                    }
                                    break; 
                                case 2://char
                                    {
                                        WriteChar(ids[i].word);
                                        Scan(true);
                                    }
                                    break; 
                            }
                            type = true;
                            break;
                        }
                    }
                    if(!type)
                    {
                        Error("WriteF", 21);
                    }
                }
                token = Scan(true);
                while (!(token.key == 'R' && token.value == 6)) //)
                {
                    if(!(token.key == 'R' && token.value == 10)) //,
                    {
                        Error("Write", 19);
                    }
                    token = Scan(true);
                    if (token.key == 'L')
                    {
                        WriteLiteral(GetWord(token, L).Replace("\"", ""));
                    }
                    else if (token.key == 'I')
                    {
                        identificator = GetWord(token, I);
                        type = false;
                        for (int i = 0; i < ids.Count; i++)
                        {
                            if (ids[i].word.Equals(identificator))
                            {
                                for (int j = 0; j < GetWord(token, I).Length; j++)
                                {
                                    Scan(false);
                                }
                                switch (ids[i].type)
                                {
                                    case 0://bool
                                        {
                                            E();
                                            WriteBool();
                                        }
                                        break; 
                                    case 1://int
                                        {
                                            E();
                                            WriteInt();
                                        }
                                        break; 
                                    case 2://char
                                        {
                                            WriteChar(ids[i].word);
                                            Scan(true);
                                        }
                                        break; 
                                }
                                type = true;
                                break;
                            }
                        }
                        if (!type)
                        {
                            Error("WriteF", 21);
                        }
                    }
                    else if (!(token.key == 'I'))
                    {
                        Error("Write", 0);
                    }
                    token = Scan(true);
                }
                token = Scan(true);
                if (!(token.key == 'R' && token.value == 11)) //;
                {
                    Error("Write", 1);
                }
                else
                    counter--;
            }
        }

        static void WriteInt()
        {
            mainCode.Add("; write int()");
            string sMark = generateAsmMark();
            mainCode.Add("\tpop ax");
            mainCode.Add("\tmov bx, 10");
            mainCode.Add("\tmov di, 0");
            mainCode.Add("\tmov si, ax");
            mainCode.Add("\tcmp ax, 0");
            mainCode.Add("\tjns " + sMark);
            mainCode.Add("\tneg si");
            mainCode.Add("\tmov ah, 2");
            mainCode.Add("\tmov dl, \"-\"");
            mainCode.Add("\tint 21h");
            mainCode.Add("\tmov ax, si");
            mainCode.Add(sMark + ":");
            mainCode.Add("\tmov dx, 0");
            mainCode.Add("\tdiv bx");
            mainCode.Add("\tadd dl, 30h");
            mainCode.Add("\tmov output[di], dl");
            mainCode.Add("\tinc di");
            mainCode.Add("\tcmp al, 0");
            mainCode.Add("\tjnz " + sMark);
            mainCode.Add("\tmov cx, di");
            mainCode.Add("\tdec di");
            mainCode.Add("\tmov ah, 2");
            sMark = generateAsmMark();
            mainCode.Add(sMark + ":");
            mainCode.Add("\tmov dl, output[di]");
            mainCode.Add("\tdec di");
            mainCode.Add("\tint 21h");
            mainCode.Add("\tloop " + sMark);           
        }


        static void WriteLiteral(string line)
        {
            mainCode.Add("; write literal()");
            string textp = generateAsmMark();
            dataCode.Add(textp + "\t db \"" + line + "$\"");
            mainCode.Add("\tlea dx, " + textp);
            mainCode.Add("\tmov ah, 9");
            mainCode.Add("\tint 21h");
        }

        static void WriteChar(string id)
        {
            mainCode.Add("; write char()");
            mainCode.Add("\tmov di, 0");
            mainCode.Add("\tmov ax, 0");
            mainCode.Add("\tmov al, " + id + "[di]");
            mainCode.Add("\tmov dl, al");
            mainCode.Add("\tmov ah, 2");
            mainCode.Add("\tint 21h");
        }

        static void WriteBool()
        {
            mainCode.Add("; write bool()");
            string l0 = generateAsmMark();
            string l1 = generateAsmMark();
            mainCode.Add("\tpop ax");
            mainCode.Add("\tcmp ax, 0");
            mainCode.Add("\tje " + l0);
            mainCode.Add("\tlea dx, @@true");
            mainCode.Add("\tjmp " + l1);
            mainCode.Add(l0 + ":");
            mainCode.Add("\tlea dx, @@false");
            mainCode.Add(l1 + ":");
            mainCode.Add("\tmov ah, 9");
            mainCode.Add("\tint 21h");
        }

        static void Loop()
        {
            Token token = Scan(true);
            if (!(token.key == 'R' && (token.value == 11 || token.value == 16))) //{
            {
                Error("Loop", 3);
            }
            if (token.key == 'R' && token.value == 16)
            {
                loop++;
                Operators();
                token = Scan(true);
                if (!(token.key == 'R' && token.value == 17)) // }
                {
                    Error("Loop", 4);
                }
                loop--;
            }
        }

        static bool Assign()
        {
            Token token = Scan(true);
            if (!(token.key == 'D' && token.value == 14)) // :=
            {
                Error("Assign", 16);
                return false;
            }
            else
            {
                return true;
            }
        }

        static void AddData(Word w)
        {
            if (w.mas)
            {
                switch (w.type)
                {
                    case 0: dataCode.Add(w.word + "\tdb\t" + masLenght + " dup " + "(?)"); break;
                    case 1: dataCode.Add(w.word + "\tdw\t" + masLenght + " dup " + "(?)"); break;
                    case 2: dataCode.Add(w.word + "\tdb\t" + masLenght + " dup " + "(?)"); break;
                }
            }
            else
            {
                switch (w.type)
                {
                    case 0: dataCode.Add(w.word + "\tdb\t" + "(?)"); break;
                    case 1: dataCode.Add(w.word + "\tdw\t" + "(?)"); break;
                    case 2: dataCode.Add(w.word + "\tdb\t" + "(?)"); break;
                }
            }
            ids.Add(w);
        }


        static void Error(string FuncName, int key)
        {
            Console.WriteLine("Функция: " + FuncName + "\n\t");
            switch (key)
            {
                case 0:
                    Console.WriteLine("Ожидался идентификатор !");
                    break;
                case 1:
                    Console.WriteLine("Ожидалось \";\"!");
                    break;
                case 2:
                    Console.WriteLine("Ожидалось \":\"!");
                    break;
                case 3:
                    Console.WriteLine("Ожидалось \"{\"!");
                    break;
                case 4:
                    Console.WriteLine("Ожидалось \"}\"!");
                    break;
                case 5:
                    Console.WriteLine("Ожидался end!");
                    break;
                case 6:
                    Console.WriteLine("Ожидалось число!");
                    break;
                case 7:
                    Console.WriteLine("Ожидался main!");
                    break;
                case 8:
                    Console.WriteLine("Ожидалось \"(\"!");
                    break;
                case 9:
                    Console.WriteLine("Ожидалось \")\"!");
                    break;
                case 10:
                    Console.WriteLine("Ожидался тип данных int, bool, char!");
                    break;
                case 11:
                    Console.WriteLine("Ожидался оператор или \"}\"!");
                    break;
                case 12:
                    Console.WriteLine("Ожидалось \"program\"!");
                    break;
                case 13:
                    Console.WriteLine("Неверное выражение!");
                    break;
                case 14:
                    Console.WriteLine("Ожидалось \"[\"!");
                    break;
                case 15:
                    Console.WriteLine("Ожидалось \"]\"!");
                    break;
                case 16:
                    Console.WriteLine("Ожидалось \":=\"!");
                    break;
                case 17:
                    Console.WriteLine("Ожидалось \"of\"!");
                    break;
                case 18:
                    Console.WriteLine("Ожидался оператор или ключевое слово!");
                    break;
                case 19:
                    Console.WriteLine("Ожидалось \",\"!");
                    break;
                case 20:
                    Console.WriteLine("Ожидался идентификатор или ключевое слово!");
                    break;
                case 21:
                    Console.WriteLine("Ошибка типа данных идентификатора, или идентификатора не существует!");
                    break;
            }
            Thread.Sleep(10000);
            Environment.Exit(0);
        }

        static void Main(string[] args)
        {
            Progr();

            Translator();

            Console.WriteLine("Всё отлично!");         

            Console.WriteLine(" Весь текстовый файл:");
            foreach (string rows in str)
            {
                Console.WriteLine("\t" + rows);
            }

            Console.WriteLine("\n" + " Идентификаторы:");

            foreach (string identity in I)
            {
                Console.Write("\t" + " I" + I.IndexOf(identity) + ": " + identity + "\n");
            }

            Console.WriteLine("\n" + " Цифры:");

            foreach (string nums in C)
            {
                Console.Write("\t" + " C" + C.IndexOf(nums) + ": " + nums + "\n");
            }

            Console.WriteLine("\n" + " Разделители:");

            foreach (string sep in my_R_Separ.Distinct())
            {
                Console.Write("\t" + " R" + R.IndexOf(sep) + ": " + sep + "\n");
            }

            Console.WriteLine("\n" + " Двойные разделители:");

            foreach (string sep in my_Doub_Separ.Distinct())
            {
                Console.Write("\t" + " D" + D.IndexOf(sep) + ": " + sep + "\n");
            }

            Console.WriteLine("\n" + " Текст в кавычках:");

            foreach (string text in L)
            {
                Console.Write("\t" + " L" + L.IndexOf(text) + ": " + text + "\n");
            }

            Console.WriteLine("\n" + " Ключевые слова:");

            foreach (string keys in my_K.Distinct())
            {
                Console.Write("\t" + " K" + K.IndexOf(keys) + ": " + keys + "\n");
            }

            Console.WriteLine("\n" + " Комментарии:");

            foreach (string comms in Comments)
            {
                Console.Write("\t" + " Comms " + Comments.IndexOf(comms) + ": " + comms + "\n");
            }

            Console.ReadKey();
        }
    }
}
