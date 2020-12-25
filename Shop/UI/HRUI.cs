using Shop;
using System;
using System.ComponentModel;
using static Shop.Account;

namespace Shop
{
    public class HRUI
    {
        HR CurrentAccount;
        Settings settings;

        ConsoleText Title;
        enum MainMenu
        {
            [Description("Сотрудники")]
            Staff,
            [Description("Незарегистрированные аккаунты")]
            UnregisteredAccounts,
            [Description("Уволенные сотрудники")]
            DismissedEmployees,
            //
            [Description("Сменить пароль")]
            ChangePassword,
            [Description("Выход из аккаунта")]
            LogOut,
            [Description("Выход")]
            Exit
        }
        string[] mainMenu = EnumHelper.GetArrayFromEnum<MainMenu>();

        public HRUI(HR hr, Settings settings)
        {
            this.settings = settings;
            CurrentAccount = hr;

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
                Title.Text = $"Отдел кадров - { UIFunctions.GetFullName(CurrentAccount)}";
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
                    case MainMenu.UnregisteredAccounts:
                        ShowUnregisteredAccountsMenu();
                        break;
                    case MainMenu.DismissedEmployees:
                        ShowDismissedAccountsMenu();
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
                    (ConsoleKeyInfo key, int selectedIndex) => (key.Key == ConsoleKey.Escape) ? (int)AccountsMenu.Back : null
                    );

                if (selectedMenu == AccountsMenu.Back) break;

                AccountType aType;
                (aType, Title.Text) = selectedMenu switch
                {
                    AccountsMenu.HR => (AccountType.HR, "Сотрудники отдела кадров (del - уволить)"),
                    AccountsMenu.WarehouseManager => (AccountType.WarehouseManager, "Сотрудники склада (del - уволить)"),
                    AccountsMenu.Cashier => (AccountType.Cashier, "Кассиры (del - уволить)"),
                    AccountsMenu.Accountant => (AccountType.Accountant, "Бухгалтеры (del - уволить)"),
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
                        ShowAccountMenu(accountsList[selectedIndex], aType);
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

            if (UIFunctions.ShowAccountEditMenu(account, settings))
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
            Title.Text = "Незарегистрированные аккаунты";

            for (; ; )
            {
                Title.Write();
                string[] unregisteredAccountsList = settings.GetUnregisteredAccountsList();

                menu.Update(unregisteredAccountsList.Add("Назад"));
                int selectedIndex = menu.Choice(ConsoleSelect.AllowEsc);
                if (selectedIndex == menu.Choices.Count - 1) break;

                if (selectedIndex >= 0)
                {
                    menu.Clear();
                    Title.Clear();
                    UIFunctions.ShowUnregisteredAccountMenu(unregisteredAccountsList[selectedIndex], settings);
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
            Title.Text = "Уволенные сотрудники";

            for (; ; )
            {
                Title.Write();
                string[] dismissedAccountsList = settings.GetDismissedAccountsList();

                menu.Update(dismissedAccountsList.Add("Назад"));
                int selectedIndex = menu.Choice(ConsoleSelect.AllowEsc);
                if (selectedIndex == menu.Choices.Count - 1) break;

                if (selectedIndex >= 0)
                {
                    menu.Clear();
                    Title.Clear();
                    UIFunctions.ShowDismissedAccountMenu(dismissedAccountsList[selectedIndex], settings);
                }
                else
                    break;
            }
            menu.Clear();
        }
    }
}