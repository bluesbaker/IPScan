using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPScan.Supports
{
    /// <summary>
    /// Additional rendering tools
    /// </summary>
    public static class ConsoleRender
    {
        /// <summary>
        /// Rendering a color field
        /// </summary>
        public static void Field(string textField, ConsoleColor bgColor = ConsoleColor.Black, ConsoleColor fgColor = ConsoleColor.White, int fieldWidth = 0)
        {
            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = fgColor;

            Console.Write(textField);

            if (fieldWidth >= textField.Length)
            {
                // add remaining spaces
                var spaces = new String(' ', fieldWidth - textField.Length);
                Console.Write(spaces);
            }

            Console.ResetColor();
        }

        /// <summary>
        /// Rendering text of loading for example "Please wait..."
        /// </summary>
        public static void Loader(string text, Func<bool> predicate, int pause = 100, int dotCount = 3)
        {
            var dots = 0;
            var dynamicText = text;

            while (predicate.Invoke())
            {
                dynamicText += new String('.', dots);
                dots++;

                Console.Write(dynamicText);
                Thread.Sleep(pause);

                // clear dots
                if (dots >= dotCount)
                {
                    dots = 0;
                }

                WriteBackspace(dynamicText.Length);
                dynamicText = text;
            }           
        }

        #region Tools
        // delete last symbols at terminal
        private static void WriteBackspace(int count)
        {
            for (int c = 0; c < count; c++)
            {
                Console.Write("\b \b");
            }
        }
        #endregion
    }
}
