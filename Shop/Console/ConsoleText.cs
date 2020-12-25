using System;

namespace Shop
{
    public class ConsoleText
    {
        public int X1 { get; private set; }
        public int Y1 { get; private set; }
        public int X2 { get; private set; }
        public int Y2 { get; private set; }

        private string text;
        public string Text
        {
            get => text;
            set
            {
                if (text == value) return;
                text = value;
                Update();
            }
        }
        
        private bool center;
        public bool Center
        {
            get => center;
            set
            {
                center = value;
                Update();
            }
        }


        public ConsoleText(int x1, int y1, int x2, int y2, string text = "", bool center = false, bool write = true)
        {
            (X1, Y1, X2, Y2) = (x1, y1, x2, y2);
            this.text = text;
            this.center = center;
            if (write) Write();
        }
        public ConsoleText(int x1, int y1, int height, string text = "", bool center = false) : this(x1, y1, Console.WindowWidth, y1 + height - 1, text, center) { }
        public void SetArea(int x1, int y1, int x2, int y2, bool rewrite = true)
        {
            (X1, Y1, X2, Y2) = (x1, y1, x2, y2);
            if (rewrite) Update();
        }

        public void Write()
        {
            int width = X2 - X1;
            int height = Y2 - Y1 + 1;
            int length = width * height;
            ReadOnlySpan<char> text_ = text.AsSpan();
            if (text_.Length > length) text_ = text_.Slice(0, length);
            else if (text_.Length < length)
            {
                if (Center)
                {
                    int mod = text_.Length % width; //длина последней строки
                    int div = text_.Length / width;
                    if (mod > 0)
                        text_ = (div > 0 ? text_.Slice(0, width * div).ToString() : "")
                            + new string(' ', (width - mod) / 2)
                            + text_.Slice(width * div, mod).ToString();

                }
            }
            for (int h = 0; h < height - 1; h++)
            {
                Console.SetCursorPosition(X1, Y1 + h);
                Console.Write(text_.Slice(width * h, width).ToString());
            }
            Console.SetCursorPosition(X1, Y1 + height - 1);
            Console.Write(text_.Slice(width * (height - 1), text_.Length - (width * (height - 1))).ToString());
        }
        public void Clear()
        {
            int width = X2 - X1 + 1;
            int height = Y2 - Y1 + 1;
            if (Text.Length > width * height) ConsoleHelper.ClearArea(X1, Y1, X2, Y2);
            else ConsoleHelper.ClearArea(X1, Y1, X2, (int)Math.Ceiling(Text.Length / (double)width));
        }

        public void Update()
        {
            Clear();
            Write();
        }

    }
}
