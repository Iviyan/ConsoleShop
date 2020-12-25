using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Shop
{
    public static class Helper
    {
        public static string[] Split(string str, int length)
        {
            if (str.Length <= length) return new string[] { str };

            List<string> res = new List<string>();
            for (int i = 0; i < str.Length; i += length)
            {
                if (i + length <= str.Length)
                    res.Add(str.Substring(i, length));
                else
                    res.Add(str.Substring(i));
            }

            return res.ToArray();
        }

        public static string ExtractFileNameWithotExtension(string filePath)
        {
            int pos1 = filePath.LastIndexOf('\\');
            int pos2 = filePath.LastIndexOf('.');
            int start = pos1 == -1 ? 0 : pos1 + 1;
            int len = pos2 == -1 ? filePath.Length - start : filePath.Length - start - (filePath.Length - pos2);
            return filePath.Substring(start, len);
        }
        public static int RepetionOfSymbolPredicate(string s, Predicate<char> predicate , int start = 0)
        {
            int count = 0;
            for (; start < s.Length; start++)
                if (predicate(s[start])) count++;
                else break;
            return count;
        }
        /*static PropertyInfo InfoOf<T>(Expression<Func<T>> ex)
        {
            return (PropertyInfo)((MemberExpression)ex.Body).Member;
        }*/

        public static string ArrayToStr<T>(T[] arr)
        {
            if (arr.Length == 0) return "[]";
            string s = "[";
            foreach (T i in arr) s += i.ToString() + ", ";
            return s.Substring(0, s.Length - 2) + "]";
        }

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);

        public static int MessageBox(string msg, string caption, int type) => MessageBox(IntPtr.Zero, msg, caption, type);
        public static int MessageBox(string msg, string caption) => MessageBox(IntPtr.Zero, msg, caption, 0);
        public static int mb(params object[] msg) => MessageBox(IntPtr.Zero, msg.Aggregate("", (string acc, object str) => acc += str.ToString()), "", 0);
    }
}
