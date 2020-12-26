using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Shop
{
    class Program
    {
        enum AuthMenu : byte
        {
            [Description("Авторизация")]
            Auth,
            [Description("Создать аккаунт")]
            CreateAccount,
            [Description("Выход")]
            Exit
        }
        static string[] authMenu = EnumHelper.GetArrayFromEnum<AuthMenu>();


        static void Main(string[] args)
        {
            Settings settings = new Settings();

            ConsoleSelect menu = new ConsoleSelect(
                authMenu,
                write: false
            );

            Account acc;
            while (true)
            {
                menu.Write();
                AuthMenu selectedMenu = (AuthMenu)menu.Choice();
                menu.Clear();
                switch (selectedMenu)
                {
                    case AuthMenu.Auth:
                        {
                            while (true)
                            {
                                ConsoleHelper.WriteCenter("Авторизация");
                                Console.SetCursorPosition(0, 2);
                                try
                                {
                                    string login = ConsoleHelper.Input_ex("Логин: ");
                                    string pass = ConsoleHelper.Input_ex("Пароль: "); // TODO: ***

                                    acc = settings.TryLogin(login, pass, out string errorMsg);

                                    if (acc == null)
                                    {
                                        Console.WriteLine($"\n!> {errorMsg}");
                                        Console.ReadKey();
                                    }
                                    Console.Clear();
                                    if (acc != null) ((IUI)acc).UI(settings);
                                }
                                catch (OperationCanceledException) { break; }
                                Console.Clear();
                            }
                            Console.Clear();
                        }
                        break;
                    case AuthMenu.CreateAccount:
                        {
                            while (true)
                            {
                                ConsoleHelper.WriteCenter("Создание аккаунта");
                                Console.SetCursorPosition(0, 2);
                                string login = "",
                                    pass = "",
                                    email = "";

                                try
                                {
                                    while (true)
                                    {
                                        login = ConsoleHelper.Input_ex("Логин: ");
                                        if (!UIFunctions.ValidateLogin(login, out string msg))
                                        {
                                            Console.WriteLine($"!> {msg}");
                                            continue;
                                        }

                                        if (settings.FindAccount(login) == null)
                                            break;
                                        else
                                            Console.WriteLine($"!> Такой логин уже существует");
                                    }

                                    while (true)
                                    {
                                        pass = ConsoleHelper.Input_ex("Пароль: ");
                                        if (UIFunctions.ValidatePassword(pass, out string msg)) break;
                                        else Console.WriteLine($"!> {msg}");
                                    }
                                    while (true)
                                    {
                                        email = ConsoleHelper.Input_ex("Email: ");
                                        if (UIFunctions.CheckEmail(email)) break;
                                        else Console.WriteLine($"!> Ошибка ввода");
                                    }

                                    Customer customer = new(login, pass, email);

                                    settings.AddAccount(customer);
                                    break;
                                }
                                catch (OperationCanceledException) { break; }
                            }
                            Console.Clear();
                        }
                        break;
                    case AuthMenu.Exit: return;
                }

            }
        }
    }
}
