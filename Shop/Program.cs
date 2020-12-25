using System;
using System.IO;
using System.Linq;

namespace Shop
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings settings = new Settings();

            Account acc;
            while (true)
            {
                ConsoleHelper.WriteCenter("Авторизация");
                Console.SetCursorPosition(0, 2);
                Console.Write("Логин: ");
                string login = Console.ReadLine();
                Console.Write("Пароль: "); // TODO: ***
                string pass = Console.ReadLine();

                acc = settings.TryLogin(login, pass, out string errorMsg);

                if (acc == null)
                {
                    Console.WriteLine($"\n!> {errorMsg}");
                    Console.ReadKey();
                }
                Console.Clear();
                if (acc != null) ((IUI)acc).UI(settings);
                Console.Clear();
            }
        }
    }
}
