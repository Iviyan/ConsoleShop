using Shop;
using System;
using System.ComponentModel;
using static Shop.Account;

namespace Shop
{
    public class WarehouseManagerUI
    {
        WarehouseManager CurrentAccount;
        Settings settings;

        ConsoleText Title;
        enum MainMenu
        {
            [Description("Склады")]
            Warehouses,
            [Description("Сменить пароль")]
            ChangePassword,
            [Description("Выход из аккаунта")]
            LogOut,
            [Description("Выход")]
            Exit
        }
        string[] mainMenu = EnumHelper.GetArrayFromEnum<MainMenu>();

        public WarehouseManagerUI(WarehouseManager warehouseManager, Settings settings)
        {
            this.settings = settings;
            CurrentAccount = warehouseManager;

            Title = new(0, 0, 1, "", true);

            ShowMainMenu();
            Console.Clear();
        }

        void ShowMainMenu()
        {
            ConsoleSelect menu = new ConsoleSelect(
                mainMenu,
                startY: 2,
                write: false
            );

            MainMenu selectedMenu = 0;
            for (; ; )
            {
                Console.SetCursorPosition(0, 0);
                Title.Text = $"Заведующий складом - { UIFunctions.GetFullName(CurrentAccount)}";
                Title.Write();
                menu.Write();
                selectedMenu = (MainMenu)menu.Choice((int)selectedMenu);

                Title.Clear();
                menu.Clear();
                Console.SetCursorPosition(0, 0);

                switch (selectedMenu)
                {
                    case MainMenu.Warehouses:
                        if (settings.WarehouseExists(CurrentAccount.PlaceOfWork))
                        {
                            menu.Clear();
                            Title.Clear();
                            UIFunctions.ShowWarehouseMenu(CurrentAccount.PlaceOfWork, settings);
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("!> Склада, указанного как место работы сотрудника не существует");
                            Console.ReadKey();
                            Console.Clear();
                        }
                        break;
                    case MainMenu.ChangePassword:
                        string pass = CurrentAccount.Password;

                        if (UIFunctions.ChangePassword(ref pass))
                        {
                            CurrentAccount.Password = pass;
                            settings.UpdateAccount(CurrentAccount);
                        }
                        Console.Clear();
                        break;
                    case MainMenu.LogOut: return;
                    case MainMenu.Exit: Environment.Exit(0); return;
                }
            }
        }
    }
}