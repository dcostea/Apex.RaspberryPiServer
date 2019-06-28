using System;

namespace Apex.RaspberryPiServer.Helpers
{
    public static class ConsoleHelper
    {
        public static void WriteHeader(string line)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void HighlightLine(string line)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(line);
            Console.ResetColor();
        }
    }
}
