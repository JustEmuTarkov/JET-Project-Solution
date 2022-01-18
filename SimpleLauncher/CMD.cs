using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLauncher
{
    internal class CMD
    {
        internal void Write(string text, bool inOneLine = true, string prefix = "")
        {
            if (inOneLine)
            {
                Console.WriteLine($"{prefix} {text}");
            }
        }
        internal void Clear() 
        {
            Console.Clear();
        }
        internal void Write(int menuId, string text, string prefix = ">", bool inOneLine = true)
        {
            Write(menuId, text, inOneLine, prefix);
        }
        internal void Write(int menuId, string text, bool inOneLine = true, string prefix = ">")
        {
            if (inOneLine)
            {
                Console.WriteLine($"{prefix} {menuId} {text}");
            }
        }
        internal void WaitForInput(string text = "")
        {
            if (text != "")
                Write(text);
            Console.ReadKey();
        }
    }
}
