using System;
using System.ComponentModel;
using static Shop.Account;

namespace Shop
{
    public class AdminUI
    {
        Admin CurrentAccount;
        Settings settings;

        ConsoleText Title;
        enum MainMenu
        {
            [Description("Сотрудники")]
            Staff,
            [Description("Добавить аккаунт")]
            AddAccount,
            [Description("Незарегистрированные аккаунты")]
            UnregisteredAccounts,
            [Description("Уволенные сотрудники")]
            DismissedEmployees,
            [Description("Склады")]
            Warehouses,
            [Description("Добавить склад")]
            AddWarehouse,
            //
            [Description("Сменить пароль")]
            ChangePassword,
            [Description("Выход из аккаунта")]
            LogOut,
            [Description("Выход")]
            Exit
        }
        string[] mainMenu = EnumHelper.GetArrayFromEnum<MainMenu>();

        public AdminUI(Admin account, Settings settings)
        {
            this.settings = settings;
            CurrentAccount = account;

            Title = new(0, 0, 1, "Администратор", true);

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
                Title.Text = "Администратор";
                Title.Write();
                menu.Write();
                selectedMenu = (MainMenu)menu.Choice((int)selectedMenu);

                Title.Clear();
                menu.Clear();
                Console.SetCursorPosition(0, 0);

                switch (selectedMenu)
                {
                    case MainMenu.Staff:
                        ShowAccountsMenu();
                        break;
                    case MainMenu.AddAccount:
                        ShowAccountAddMenu();
                        break;
                    case MainMenu.UnregisteredAccounts:
                        ShowUnregisteredAccountsMenu();
                        break;
                    case MainMenu.DismissedEmployees:
                        ShowDismissedAccountsMenu();
                        break;
                    case MainMenu.Warehouses:
                        ShowWarehousesMenu();
                        break;
                    case MainMenu.AddWarehouse:
                        ShowAddWarehouseMenu();
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

        enum AccountsMenu : byte
        {
            [Description("Отдел кадров")]
            HR,
            [Description("Складовщики")]
            WarehouseManager,
            [Description("Кассиры")]
            Cashier,
            [Description("Бухгалтеры")]
            Accountant,
            [Description("Покупатели")]
            Customer,
            [Description("Назад")]
            Back
        }
        string[] accountsMenu = EnumHelper.GetArrayFromEnum<AccountsMenu>();

        void ShowAccountsMenu()
        {
            ConsoleSelect menu = new ConsoleSelect(
                accountsMenu,
                startY: 2
            );

            AccountsMenu selectedMenu = 0;
            for (; ; )
            {
                Title.Text = "Аккаунты";
                string[] accountsList = null;

                selectedMenu = (AccountsMenu)menu.Choice(
                    (int)selectedMenu,
                    (ConsoleKeyInfo key, int selectedIndex) => (key.Key == ConsoleKey.Escape) ? (int)AccountsMenu.Back : null);

                if (selectedMenu == AccountsMenu.Back) break;

                AccountType aType;
                (aType, Title.Text) = selectedMenu switch
                {
                    AccountsMenu.HR => (AccountType.HR, "Сотрудники отдела кадров (del - уволить)"),
                    AccountsMenu.WarehouseManager => (AccountType.WarehouseManager, "Сотрудники склада (del - уволить)"),
                    AccountsMenu.Cashier => (AccountType.Cashier, "Кассиры (del - уволить)"),
                    AccountsMenu.Accountant => (AccountType.Accountant, "Бухгалтеры (del - уволить)"),
                    AccountsMenu.Customer => (AccountType.Customer, "Покупатели (del - удалить)"),
                    _ => throw new ArgumentException(),
                };

                for (int selectedIndex = 0; ;)
                {
                    Title.Write();
                    accountsList = settings.GetAccountsList(aType);
                    menu.Update(accountsList.Add("Назад"));
                    selectedIndex = menu.Choice(
                        selectedIndex,
                        (ConsoleKeyInfo key, int selectedIndex) =>
                        {
                            if (key.Key == ConsoleKey.Escape) return -1;
                            if (key.Key == ConsoleKey.Delete && selectedIndex < menu.Choices.Count - 1)
                            {
                                settings.DismissEmployee(accountsList[selectedIndex], aType);
                                menu.Choices.RemoveAt(selectedIndex);
                            }
                            return null;
                        }
                    );
                    if (selectedIndex == menu.Choices.Count - 1) break;

                    if (selectedIndex >= 0)
                    {
                        menu.Clear();
                        ShowAccountMenu(menu.Choices[selectedIndex], aType);
                    }
                    else
                        break;
                }

                menu.Update(accountsMenu);
            }
            menu.Clear();
        }

        void ShowAccountMenu(string login, AccountType type)
        {
            Account account = settings.LoadAccount(login, type);

            Title.Clear();

            if (UIFunctions.ShowAccountEditMenu(account, settings, true))
                if (login == account.Login)
                    settings.UpdateAccount(account);
                else
                {
                    settings.RenameAccount(login, account.Login, type);
                    settings.UpdateAccount(account);
                }
        }

        void ShowUnregisteredAccountsMenu()
        {
            ConsoleSelect menu = new ConsoleSelect(
                new string[] { },
                startY: 2,
                write: false
            );
            Title.Text = "Незарегистрированные аккаунты (del - удалить)";

            for (; ; )
            {
                Title.Write();
                string[] unregisteredAccountsList = settings.GetUnregisteredAccountsList();

                menu.Update(unregisteredAccountsList.Add("Назад"));
                int selectedIndex = menu.Choice(
                    (ConsoleKeyInfo key, int selectedIndex) =>
                    {
                        if (key.Key == ConsoleKey.Escape) return -1;
                        if (key.Key == ConsoleKey.Delete && selectedIndex < menu.Choices.Count - 1)
                        {
                            settings.RemoveUnregisteredAccount(unregisteredAccountsList[selectedIndex]);
                            menu.Choices.RemoveAt(selectedIndex);
                        }
                        return null;
                    }
                );
                if (selectedIndex == menu.Choices.Count - 1) break;

                if (selectedIndex >= 0)
                {
                    menu.Clear();
                    Title.Clear();
                    UIFunctions.ShowUnregisteredAccountMenu(menu.Choices[selectedIndex], settings, true);
                }
                else
                    break;
            }
            menu.Clear();
        }
        void ShowDismissedAccountsMenu()
        {
            ConsoleSelect menu = new ConsoleSelect(
                new string[] { },
                startY: 2,
                write: false
            );
            Title.Text = "Уволенные сотрудники (del - удалить)";

            for (; ; )
            {
                Title.Write();
                string[] dismissedAccountsList = settings.GetDismissedAccountsList();

                menu.Update(dismissedAccountsList.Add("Назад"));
                int selectedIndex = menu.Choice(
                    (ConsoleKeyInfo key, int selectedIndex) =>
                    {
                        if (key.Key == ConsoleKey.Escape) return -1;
                        if (key.Key == ConsoleKey.Delete && selectedIndex < menu.Choices.Count - 1)
                        {
                            settings.RemoveAccount(dismissedAccountsList[selectedIndex]);
                            menu.Choices.RemoveAt(selectedIndex);
                        }
                        return null;
                    }
                );
                if (selectedIndex == menu.Choices.Count - 1) break;

                if (selectedIndex >= 0)
                {
                    menu.Clear();
                    Title.Clear();
                    UIFunctions.ShowDismissedAccountMenu(menu.Choices[selectedIndex], settings);
                }
                else
                    break;
            }
            menu.Clear();
        }


        enum AccountAddMenu : byte
        {
            [Description("Пустой аккаунт")]
            EmptyAccount,
            [Description("Сотрудник отдела кадров")]
            HR,
            [Description("Назад")]
            Back
        }
        string[] accountAddMenu = EnumHelper.GetArrayFromEnum<AccountAddMenu>();
        void ShowAccountAddMenu()
        {
            ConsoleSelect menu = new ConsoleSelect(
                accountAddMenu,
                startY: 2
            );
            Title.Text = "Добавление аккаунта";

            AccountAddMenu selectedMenu = (AccountAddMenu)menu.Choice(
                (ConsoleKeyInfo key, int selectedIndex) =>
                {
                    if (key.Key == ConsoleKey.Escape) return (int)AccountAddMenu.Back;
                    return null;
                }
            );
            menu.Clear();

            if (selectedMenu != AccountAddMenu.Back)
            {
                switch (selectedMenu)
                {
                    case AccountAddMenu.EmptyAccount:
                        {
                            Title.Text = "Добавление пустого аккаунта";
                            Console.SetCursorPosition(0, 2);
                            try
                            {
                                UnregisteredAccount uAccount;
                                while (true)
                                {
                                    uAccount.Login = ConsoleHelper.Input_ex("Логин: ");
                                    if (!UIFunctions.ValidateLogin(uAccount.Login, out string msg))
                                    {
                                        Console.WriteLine($"!> {msg}");
                                        continue;
                                    }

                                    if (settings.FindAccount(uAccount.Login) == null)
                                        break;
                                    else
                                        Console.WriteLine($"!> Такой логин уже существует");
                                }
                                while (true)
                                {
                                    uAccount.Password = ConsoleHelper.Input_ex("Пароль: ");
                                    if (UIFunctions.ValidatePassword(uAccount.Password, out string msg)) break;
                                    else Console.WriteLine($"!> {msg}");
                                }

                                (uAccount.LastName, uAccount.FirstName, uAccount.Patronymic) = UIFunctions.InputFIO();

                                settings.AddUnregisteredAccount(uAccount);
                            }
                            catch (OperationCanceledException) { }
                        }
                        break;
                    case AccountAddMenu.HR:
                        {
                            Title.Text = "Добавление сотрудника отдела кадров";
                            HR hr = new HR("", "", "", "", "", DateTime.Today, new string[] { }, 1, "");

                            Console.SetCursorPosition(0, 2);
                            try
                            {
                                UIFunctions.InputLogPass(hr, settings);
                                UIFunctions.InputEmployeeData(hr);

                                settings.AddAccount(hr);
                            }
                            catch (OperationCanceledException) { }
                        }
                        break;
                }
                Console.Clear();
            }
        }

        void ShowWarehousesMenu()
        {
            ConsoleSelect menu = new ConsoleSelect(
                new string[] { },
                startY: 2,
                write: false
            );
            Title.Text = "Склады (del - удалить)";

            for (; ; )
            {
                Title.Write();
                string[] warehousesList = settings.GetWarehousesList();

                menu.Update(warehousesList.Add("Назад"));
                int selectedIndex = menu.Choice(
                    (ConsoleKeyInfo key, int selectedIndex) =>
                    {
                        if (key.Key == ConsoleKey.Escape) return -1;
                        if (key.Key == ConsoleKey.Delete && selectedIndex < menu.Choices.Count - 1)
                        {
                            settings.RemoveWarehouse(warehousesList[selectedIndex]);
                            menu.Choices.RemoveAt(selectedIndex);
                        }
                        return null;
                    }
                );
                if (selectedIndex == menu.Choices.Count - 1) break;

                if (selectedIndex >= 0)
                {
                    menu.Clear();
                    Title.Clear();
                    UIFunctions.ShowWarehouseMenu(menu.Choices[selectedIndex], settings);
                }
                else
                    break;
            }
            menu.Clear();
        }
        void ShowAddWarehouseMenu()
        {
            Title.Text = "Добавление склада";
            Console.SetCursorPosition(0, 2);
            try
            {
                string warehouseName;
                while (true)
                {
                    warehouseName = ConsoleHelper.Input_ex("Имя склада: ");
                    if (warehouseName.Length == 0)
                    {
                        Console.WriteLine($"!> Имя склада не может быть пустым");
                        continue;
                    }
                    if (settings.WarehouseExists(warehouseName))
                    {
                        Console.WriteLine($"!> Склад с таким именем уже существует");
                        continue;
                    }
                    break;

                }

                settings.AddWarehouse(warehouseName);
            }
            catch (OperationCanceledException) { }

            Console.Clear();
        }
    }
}