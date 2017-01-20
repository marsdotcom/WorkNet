using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace WorkNet
{
    public enum Token_ID
    {
        number,
        calendarhours,
        calendardays,
        tabelhours,
        tabeldays,
        hoursforweekend,
        daysforweekend,
        rate,
        hourrate,
        monthrate,
        salary,       
        lbr,
        rbr,
        mul,
        div,
        add,
        sub        
    }

    public class Token
    {
        public static string[] apparences =
        {
            "число",                //=0
            "часы по календарю",
            "дни оп календарю",
            "часы по табелю",
            "дни по табелю",
            "часы за выходние",
            "дни за выходние",
            "тариф",
            "часовая ставка",
            "месячная ставка",      
            "оклад",                //=10
            "(",
            ")",
             "*",
            "/",
            "+",
            "-"                     //=16
        };

        public float value;
        public Token_ID Ti;

        public Token()
        {
        }

        public Token(Token_ID Ti, float value)
        {
            this.Ti = Ti;
            this.value = value;
        }

        public override string  ToString()
        {
 	        if (Ti == Token_ID.number) 
                return value.ToString();
            else 
                return apparences[(int)Ti];
        }

    }

    public class Expression
    {
        List<Token> ListofTokens = new List<Token>();

        List<Token> tokens;

        int[] paramis = new int[11];

        int balansofbrackets = 0;

        Token_ID LastTi;

        public int Count
        {
            get { return ListofTokens.Count; }
        }    

        bool IsOparation(Token_ID Ti)
        {
            return (Ti >= Token_ID.mul && Ti <= Token_ID.sub);
        }

        bool IsParam(Token_ID Ti)
        {
            return Ti <= Token_ID.salary;
        }

        public bool Append(Token T)
        {
            if (IsOparation(T.Ti))
            {
                if (ListofTokens.Count == 0) return false;
                if (IsOparation(LastTi)) return false;
                if (LastTi == Token_ID.lbr) return false;
            }

            if (IsParam(T.Ti))
            {
                if (ListofTokens.Count > 0)
                {
                    if (IsParam(LastTi)) return false;
                    if (LastTi == Token_ID.rbr) return false;
                }
                paramis[(int)T.Ti]++;
            }

            if (T.Ti == Token_ID.lbr)
            {
                balansofbrackets++;
                if (ListofTokens.Count > 0)
                {
                    if (IsParam(LastTi)) return false;
                    if (LastTi == Token_ID.rbr) return false;
                }
            }

            if (T.Ti == Token_ID.rbr)
            {                
                if (ListofTokens.Count == 0) return false;
                if (IsOparation(LastTi)) return false;
                if (LastTi == Token_ID.lbr) return false;
                if (balansofbrackets == 0) return false;
                balansofbrackets--;
            }

            ListofTokens.Add(T);
            LastTi = T.Ti;
            return true;
        }

        public void Remove()
        {
            if (ListofTokens.Count == 0) return;
            if (LastTi == Token_ID.lbr) balansofbrackets--;
            if (LastTi == Token_ID.rbr) balansofbrackets++;
            if (IsParam(LastTi))
                paramis[(int)LastTi]--;
            ListofTokens.RemoveAt(ListofTokens.Count - 1);
            if (ListofTokens.Count > 0)
            LastTi = ListofTokens[ListofTokens.Count - 1].Ti;
        }

        public override string ToString()
        {
            StringBuilder S = new StringBuilder();
            foreach (Token t in ListofTokens)
                S.Append(t.ToString()).Append(" ");
            return S.ToString();
        }

        public bool Test()
        {
            if (balansofbrackets != 0) return false;
            if (IsOparation(LastTi)) return false;
            return true;
        }

        int ParamsCount()
        {
            int r = 0;
            for (int i = 1; i < paramis.Length;i++ )
                if (paramis[i] > 0) r++;
            return r;
        }

        public bool Evalute(ref float result, params float[] data)
        {
            if (data.Length < ParamsCount()) return false;

            if (ListofTokens.Count == 0)
            {
                result = 0;
                return true;
            }

            tokens = new List<Token>();
            tokens.Add(new Token(Token_ID.lbr, 0));

            foreach (Token TT in ListofTokens)
            {
                if (IsParam(TT.Ti) && TT.Ti != Token_ID.number) TT.value = data[(int)TT.Ti - 1];
                tokens.Add(new Token(TT.Ti, TT.value));
            }

            tokens.Add(new Token(Token_ID.rbr, 0));

            if (!F(0, tokens.Count-1)) return false;
            result = tokens[0].value;
            return true;
        }

        bool F(int a, int b)
        {
            float r;
            tokens.RemoveAt(a);
            b--;
            tokens.RemoveAt(b);
            b--;
            int i = a;
            int n = b;
            int br = 0;

            while (i < n)
            {
                if (tokens[i].Ti == Token_ID.lbr)
                {
                    int j = i;
                    br = 1;
                    while (j < n && br > 0)
                    {
                        j++;
                        if (tokens[j].Ti == Token_ID.rbr) br--;
                        else
                            if (tokens[j].Ti == Token_ID.lbr) br++;
                    }
                    if (!F(i, j)) return false;
                    else n -= j - i;
                }

                i++;
            }


            i = a + 1;

            while (i < n)
            {
                if (tokens[i].Ti == Token_ID.mul || tokens[i].Ti == Token_ID.div)
                {
                    if (tokens[i].Ti == Token_ID.mul)
                        r = tokens[i - 1].value * tokens[i + 1].value;
                    else
                    {
                        if (tokens[i + 1].value == 0)
                        {
                            return false;
                        }
                        r = tokens[i - 1].value / tokens[i + 1].value;
                    }

                    tokens[i - 1].value = r;
                    tokens.RemoveRange(i, 2);
                    n -= 2;
                    i -= 2;
                }
                i++;
            }

            i = a + 1;

            while (i < n)
            {
                if (tokens[i].Ti == Token_ID.add || tokens[i].Ti == Token_ID.sub)
                {
                    if (tokens[i].Ti == Token_ID.add)
                        r = tokens[i - 1].value + tokens[i + 1].value;
                    else
                        r = tokens[i - 1].value - tokens[i + 1].value;

                    tokens[i - 1].value = r;
                    tokens.RemoveRange(i, 2);
                    n -= 2;
                    i -= 2;
                }
                i++;
            }

            return true;
            
        }


        public string SaveToString()
        {
            StringBuilder S = new StringBuilder();
            foreach (Token T in ListofTokens)
            {
                if (IsParam(T.Ti) && T.Ti != Token_ID.number)
                    S.Append("@").Append((int)T.Ti).Append("@");
                else if (T.Ti == Token_ID.number)
                    S.Append("#").Append(T.value).Append("#");
                else
                    S.Append(T.ToString());
            }

            return S.ToString();
        }

        void Clear()
        {
            ListofTokens.Clear();
            for (int i = 0; i < paramis.Length; i++) paramis[i] = 0;
            balansofbrackets = 0;
            tokens = null;
        }

        public void LoadFromString(string S)
        {
            Clear();
            int j;
            for (int i = 0; i < S.Length; i++)
            {
                if (S[i] == '@')
                {
                    j = i + 1;
                    while (S[j] != '@') { j++; }
                    string s = S.Substring(i + 1, j - i - 1);
                    this.Append(new Token((Token_ID)Convert.ToInt32(s),0));
                    i = j;
                }else
                    if (S[i] == '#')
                    {
                        j = i + 1;
                        while (S[j] != '#') { j++; }
                        string s = S.Substring(i + 1, j - i - 1);
                        this.Append(new Token(Token_ID.number, Convert.ToSingle(s)));
                        i = j;
                    }
                    else
                    {
                        Token T = new Token();
                        switch (S[i])
                        {
                            case '(': T.Ti = Token_ID.lbr; break;
                            case ')': T.Ti = Token_ID.rbr; break;
                            case '*': T.Ti = Token_ID.mul; break;
                            case '/': T.Ti = Token_ID.div; break;
                            case '+': T.Ti = Token_ID.add; break;
                            case '-': T.Ti = Token_ID.sub; break;
                        }

                        this.Append(T);

                    }
            }
        }

        public static string ExpStrtoText(string str)
        {
            int j;
            StringBuilder S = new StringBuilder();

            for (int i = 0; i <str.Length; i++)
            {
                if (str[i] == '@')
                {
                    j = i + 1;
                    while (str[j] != '@') { j++; }
                    string s = str.Substring(i + 1, j - i - 1);
                    S.Append(Token.apparences[Convert.ToInt32(s)]).Append(' ');
                    i = j;
                }
                else
                    if (str[i] == '#')
                    {
                        j = i + 1;
                        while (str[j] != '#') { j++; }
                        string s = str.Substring(i + 1, j - i - 1);
                        S.Append(s).Append(' ');
                        i = j;
                    }
                    else
                    {
                        S.Append(str[i]).Append(' ');
                    }
            }          

            return S.ToString();
        }

    }

    public class Expressions
    {
        Expression ExpSalary;
        Expression ExpSalaryOff;
        Expression Exp;
        bool which;

        public Expressions()
        {
            ExpSalary = new Expression();
            ExpSalaryOff = new Expression();
            Exp = ExpSalary;
            which = true;
        }

        public bool Which
        {
            get { return which; }
        }

        public void Switch()
        {
            which = !which;
            Switch(which);
        }

        public void Switch(bool b)
        {
            which = b;
            if (b)
                Exp = ExpSalary;
            else
                Exp = ExpSalaryOff;                        
        }

        public int Count
        {
            get { return Exp.Count; }
        }

        public bool Append(Token T)
        {
            return Exp.Append(T);
        }

        public void Remove()
        {
            Exp.Remove();
        }

        public override string ToString()
        {
            return Exp.ToString();
        }

        public bool Test()
        {
            return Exp.Test();
        }

        public bool Evalute(ref float salary,ref float salaryoff, params float[] data)
        {
            return (ExpSalary.Evalute(ref salary, data) && ExpSalaryOff.Evalute(ref salaryoff, data));
        }

        public string SaveToString()
        {
            return ExpSalary.SaveToString() + ";" + ExpSalaryOff.SaveToString();
        }

        public void LoadFromString(string str)
        {
            int i = str.IndexOf(';');
            if (i < 0) return;
            string s1 = str.Remove(i);
            string s2 = str.Substring(++i);
            ExpSalary.LoadFromString(s1);
            ExpSalaryOff.LoadFromString(s2);
        }

        public static string ExpStrToText(string str)
        {
            int i = str.IndexOf(';');
            if (i < 0) return null;
            string s1 = str.Remove(i);
            string s2 = str.Substring(++i);

            return "Зарплата    = " + Expression.ExpStrtoText(s1) + Environment.NewLine + Environment.NewLine +
                   "За выходние = " + Expression.ExpStrtoText(s2);
        }


    }

}
