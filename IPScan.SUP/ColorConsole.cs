using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IPScan.SUP
{
    /// <summary>
    /// Additional text rendering tool
    /// </summary>
    public static class ColorConsole
    {
        /// <summary>
        /// Rendering the color field
        /// </summary>
        /// <param name="text">Text of the field</param>
        /// <param name="width">Width of the field</param>
        /// <param name="colorSections">Color sections</param>
        public static void Field(string text, int width = 0, params ColorSection[] colorSections)
        {
            var commonText = text;

            // add remaining spaces
            if (width >= text.Length)
            {
                commonText += new String(' ', width - text.Length);
            }

            Write(commonText, colorSections);
        }

        /// <summary>
        /// Rendering the color field line
        /// </summary>
        /// <param name="text">Text of the field</param>
        /// <param name="width">Width of the field</param>
        /// <param name="colorSections">Color sections</param>
        public static void FieldLine(string text, int width = 0, params ColorSection[] colorSections)
        {
            Field(text, width, colorSections);
            Console.WriteLine();
        }

        /// <summary>
        /// Rendering the color fields
        /// </summary>
        /// <param name="textCollection">Text collection</param>
        /// <param name="width">Width of the fields</param>
        /// <param name="colorSections">Color sections</param>
        public static void Fields(ICollection<string> textCollection, int width = 0, params ColorSection[] colorSections)
        {
            foreach (var text in textCollection)
            {
                Field(text, width, colorSections);
            }
        }

        /// <summary>
        /// Rendering the color fields line
        /// </summary>
        /// <param name="textCollection">Text collection</param>
        /// <param name="width">Width of the fields</param>
        /// <param name="colorSections">Color sections</param>
        public static void FieldsLine(ICollection<string> textCollection, int width = 0, params ColorSection[] colorSections)
        {
            Fields(textCollection, width, colorSections);
            Console.WriteLine();
        }

        /// <summary>
        /// Rendering the color text
        /// </summary>
        /// <param name="text">Text of the field</param>
        /// <param name="colorSections">Color sections</param>
        public static void Write(string text, params ColorSection[] colorSections)
        {
            // default colors
            var foregroundColor = Console.ForegroundColor;
            var backgroundColor = Console.BackgroundColor;

            // result* sections
            var commonSections = new List<ColorSection>();

            // split textField into ColorSections
            if (colorSections.Length != 0)
            {
                var sortedSections = colorSections.ToList();
                sortedSections.Sort((s, p) => text.IndexOf(s.Section));

                var lastIndexOfSection = 0;
                foreach (var section in sortedSections)
                {
                    // set default colors by empty* section
                    if (String.IsNullOrEmpty(section.Section))
                    {
                        foregroundColor = section.Foreground;
                        backgroundColor = section.Background;
                        continue;
                    }

                    var indexOfSection = text.IndexOf(section.Section);
                    if (indexOfSection == -1) continue;

                    // before section...
                    var before = text.Substring(lastIndexOfSection, indexOfSection - lastIndexOfSection);
                    var beforeSection = new ColorSection(backgroundColor, foregroundColor, before);

                    commonSections.Add(beforeSection);
                    commonSections.Add(section);

                    lastIndexOfSection = indexOfSection + section.Section.Length;
                }

                // ...after section
                var after = text.Substring(lastIndexOfSection);
                var afterSection = new ColorSection(backgroundColor, foregroundColor, after);

                commonSections.Add(afterSection);
            }
            // or create only one default ColorSection
            else
            {
                var section = new ColorSection(backgroundColor, foregroundColor, text);
                commonSections.Add(section);
            }

            // render
            foreach (var section in commonSections)
            {
                Console.BackgroundColor = section.Background;
                Console.ForegroundColor = section.Foreground;

                Console.Write(section.Section);
            }

            Console.ResetColor();
        }

        /// <summary>
        /// Rendering the color text line
        /// </summary>
        /// <param name="text">Text of the field</param>
        /// <param name="colorSections">Color sections</param>
        public static void WriteLine(string text, params ColorSection[] colorSections)
        {
            Write(text, colorSections);
            Console.WriteLine();
        }

        /// <summary>
        /// Rendering text of loading for example "Please wait..."
        /// </summary>
        public static void Loader(string text, Func<bool> predicate, int pause = 100, int dotCount = 3, params ColorSection[] colorSections)
        {
            var dots = 0;
            var dynamicText = text;

            while (predicate.Invoke())
            {
                dynamicText += new String('.', dots);
                dots++;

                Write(dynamicText, colorSections);
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

        /// <summary>
        /// Deleting last symbols at terminal
        /// </summary>
        /// <param name="count">Count of symbols</param>
        private static void WriteBackspace(int count)
        {
            for (int c = 0; c < count; c++)
            {
                Console.Write("\b \b");
            }
        }
    }

    public class ColorSection
    {
        public string Section { get; set; }
        public ConsoleColor Background { get; set; }
        public ConsoleColor Foreground { get; set; }

        public ColorSection(ConsoleColor background = ConsoleColor.Black, ConsoleColor foreground = ConsoleColor.White, string section = "")
        {
            Section = section;
            Background = background;
            Foreground = foreground;
        }
    }
}
