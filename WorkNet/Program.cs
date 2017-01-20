using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;

namespace WorkNet
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    static class Keyboard
    {
        static CultureInfo rus = new CultureInfo(0x0419), eng = new CultureInfo(0x0409);
        public static bool ruslayout = false;

        public static string NumberSeparator
        {
            get { return NumberFormatInfo.CurrentInfo.NumberDecimalSeparator; }
        }

        public static string DateSeparator
        {
            get { return DateTimeFormatInfo.CurrentInfo.DateSeparator; }
        }

        public static void Rus()
        {
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(rus);
            ruslayout = true;
        }

        public static void Eng()
        {
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(eng);
            ruslayout = false;
        }

        public static void Change()
        {
            if (ruslayout) Eng(); else Rus();
        }
    }
}