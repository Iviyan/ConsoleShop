using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Shop
{
    public class ConsoleSelect
    {
        private int startX;
        public int StartX
        {
            get => startX;
            set
            {
                startX = value;
            }
        }
        private int startY;
        public int StartY
        {
            get => startY;
            set
            {
                startY = value;
            }
        }
        private int maxWidth;
        public int MaxWidth
        {
            get => (maxWidth == 0) ? Console.WindowWidth : maxWidth;
            set
            {
                if (value > 2 || value == 0)
                    maxWidth = value;
                else
                    throw new Exception("The max width must be >2 or 0");
            }
        }
        public int CurrentMaxWidth { get; private set; }
        public int CurrentHeight { get; private set; } = 0;
        public int ContentHeight { get; private set; }
        public ObservableCollection<string> Choices { get; private set; }
        private HashSet<int> Disabled;
        private int interval;
        public int Interval
        {
            get => interval;
            set
            {
                interval = value;
            }
        }


        private int selectedIndex;
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                if (value < 0 || value > Choices.Count) return;
                Select(selectedIndex, ' ', ' ');
                selectedIndex = value;
                Select(selectedIndex);
            }
        }

        (int top, int width, int height)[] selHelper;

        public ConsoleSelect(string[] choices, int selectedIndex = 0, int interval = 0, int startX = 0, int startY = 0, int maxWidth = 0, int[] disabled = null, bool write = true)
        {
            Choices = new ObservableCollection<string>(choices);
            Choices.CollectionChanged += Choices_CollectionChanged;

            this.selectedIndex = selectedIndex;
            this.interval = interval;
            this.startX = startX;
            this.startY = startY;
            this.maxWidth = maxWidth;

            if (disabled != null)
                Disabled = new(disabled);
            else
                Disabled = new();

            CurrentMaxWidth = 0;

            Calculate();
            if (write) Write();
        }

        public void Update(string[] choices, bool clear = true)
        {
            if (clear) Clear();
            selectedIndex = 0;
            Choices = new ObservableCollection<string>(choices);
            Choices.CollectionChanged += Choices_CollectionChanged;

            Calculate();
            Write();
        }

        private void Choices_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Replace:
                    //Edit(e.NewStartingIndex, e.NewItems[0] as string);
                    string newText = e.NewItems[0] as string;

                    var sh_ = selHelper[e.NewStartingIndex];
                    int lineHeght = GetLineHeight(e.NewStartingIndex);
                    if (sh_.height == lineHeght)
                    {
                        int newWidth = sh_.height > 1 ? MaxWidth - 2 : newText.Length;
                        ConsoleHelper.ClearArea(StartX + 1, StartY + sh_.top, StartX + sh_.width, StartY + sh_.top + sh_.height - 1);
                        selHelper[e.NewStartingIndex].width = newWidth;
                        WriteLine(newText, sh_.top);
                        if (CurrentMaxWidth < newWidth) CurrentMaxWidth = newWidth;
                    }
                    else
                    {
                        Clear(e.NewStartingIndex, Choices.Count - 1);

                        Calculate(e.NewStartingIndex);
                        Write(e.NewStartingIndex);

                        if (SelectedIndex >= e.NewStartingIndex) Select(SelectedIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        Clear(e.OldStartingIndex, Choices.Count);
                        Calculate(e.OldStartingIndex);
                        Write(e.OldStartingIndex);
                        if (SelectedIndex >= e.OldStartingIndex)
                            if (SelectedIndex == Choices.Count)
                            {
                                if (selectedIndex > 0)
                                {
                                    selectedIndex--;
                                    Select(SelectedIndex);
                                }
                            }
                            else
                                Select(SelectedIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                    {
                        if (!(e.NewStartingIndex == Choices.Count - 1))
                            Clear(e.NewStartingIndex, Choices.Count - 2);

                        Calculate(e.NewStartingIndex);
                        Write(e.NewStartingIndex);

                        if (e.NewStartingIndex >= SelectedIndex)
                            Select(SelectedIndex);
                    }
                    break;
            }
        }

        int GetLineHeight(int length, int maxLength) => (int)Math.Ceiling(length / (double)maxLength);
        int GetLineHeight(int index) => GetLineHeight(Choices[index].Length, MaxWidth - 2);

        void Calculate(int startIndex = 0)
        {
            if (Choices.Count == 0)
            {
                selHelper = new (int, int, int)[0];
                return;
            }

            int line = 0;
            if (startIndex > 0 && selHelper != null)
            {
                if (startIndex < selHelper.Length)
                    line = selHelper[startIndex].top;
                else if (startIndex == selHelper.Length)
                {
                    var sh = selHelper[startIndex - 1];
                    line = sh.top + sh.height + interval;
                }
            }

            if (selHelper != null)
            {
                if (Choices.Count != selHelper.Length)
                    Array.Resize(ref selHelper, Choices.Count);
            }
            else
                selHelper = new (int top, int width, int height)[Choices.Count];


            for (int i = startIndex; i < Choices.Count; i++)
            {
                int lineHeight = GetLineHeight(i);
                int lineWidth = lineHeight > 1 ? MaxWidth - 2 : Choices[i].Length;
                selHelper[i] =
                    (
                        line,
                        lineWidth,
                        lineHeight
                    );
                if (CurrentMaxWidth < lineWidth) CurrentMaxWidth = lineWidth;
                line += lineHeight + interval;
            }
            ContentHeight = line - interval;
        }

        int WriteLine(string text, int top)
        {
            string[] lines = Helper.Split(text, MaxWidth - 2);

            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(StartX + 1, StartY + top + i);
                bool disable = Disabled.Contains(i);
                if (disable)
                    using (new UseConsoleColor(ConsoleColor.DarkGray))
                        Console.Write(lines[i]);
                else
                    Console.Write(lines[i]);
            }
            return lines.Length;
        }
        public void Write(int startIndex = 0)
        {
            if (startIndex >= Choices.Count) return;
            //Helper.mb($"=> {PageCount}");

            int line = selHelper[startIndex].top;
            for (int i = startIndex; i < Choices.Count; i++)
            {
                line += interval + WriteLine(Choices[i], line);
            }
            CurrentHeight = ContentHeight;
        }
        public void Clear()
        {
            if (CurrentHeight == 0) return;
            ConsoleHelper.ClearArea(StartX, StartY, StartX + CurrentMaxWidth + 1, StartY + CurrentHeight);
            Console.SetCursorPosition(0, StartY);
            CurrentHeight = 0;
        }
        public void Clear(int startIndex, int endIndex)
        {
            var sh1 = selHelper[startIndex];
            var sh2 = selHelper[endIndex];
            ConsoleHelper.ClearArea(StartX, StartY + sh1.top, StartX + CurrentMaxWidth + 1, StartY + sh2.top + sh2.height);
        }

        public void Select(int sel, char cStart = '>', char cEnd = '<')
        {
            if (Choices.Count <= 0) return;
            var sh = selHelper[sel];
            for (int i = 0; i < sh.height; i++)
            {
                Console.SetCursorPosition(StartX, StartY + sh.top + i);
                Console.Write(cStart);
                Console.SetCursorPosition(StartX + sh.width + 1, StartY + sh.top + i);
                Console.Write(cEnd);

            }
        }

        public void Disable(int index, bool rewrite = true)
        {
            if (!Disabled.Contains(index))
            {
                Disabled.Add(index);
                if (rewrite) WriteLine(Choices[index], selHelper[index].top);
            }
        }
        public void EnableAll(bool rewrite = true)
        {
            Disabled.Clear();
            if (rewrite) Write();
        }
        public void Enable(int index, bool rewrite = true)
        {
            if (Disabled.Contains(index))
            {
                Disabled.Remove(index);
                if (rewrite) WriteLine(Choices[index], selHelper[index].top);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>null -> continue, int -> return int</returns>
        public delegate int? PressKey(ConsoleKeyInfo key, int selectedIndex);
        public static int? AllowEsc(ConsoleKeyInfo key, int selectedIndex) => (key.Key == ConsoleKey.Escape) ? -1 : null;

        public int Choice(PressKey onPressKey, bool rewrite = false) => Choice(0, onPressKey, rewrite);
        public int Choice(int selectIndex = 0, PressKey onPressKey = null, bool rewrite = false)
        {
            if (Choices.Count == 0) { Console.ReadKey(true); return -1; }//throw new Exception("The list of choices is empty");
            if (rewrite) Write();

            SelectedIndex = selectIndex;
            bool onPressKeyEventExists = onPressKey != null;

            if (Choices.Count == 0) throw new Exception("Count of choices must be >0");

            ConsoleKeyInfo info;
            while (true)
            {
                info = Console.ReadKey(true);
                switch (info.Key)
                {
                    case ConsoleKey.DownArrow:
                        if (SelectedIndex + 1 < Choices.Count) SelectedIndex++;
                        else SelectedIndex = 0;
                        break;
                    case ConsoleKey.UpArrow:
                        if (SelectedIndex > 0) SelectedIndex--;
                        else SelectedIndex = Choices.Count - 1;
                        break;
                    case ConsoleKey.Enter:
                        if (!Disabled.Contains(SelectedIndex)) return SelectedIndex;
                        break;
                    default:
                        if (onPressKeyEventExists)
                        {
                            int? r = onPressKey(info, selectedIndex);
                            if (r != null) return (int)r;
                        }
                        break;
                }
            }
        }
    }
}