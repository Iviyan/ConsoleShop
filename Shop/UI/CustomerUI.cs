using Shop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static Shop.Account;

namespace Shop
{
    public class CustomerUI
    {
        Customer CurrentAccount;
        Settings settings;

        ConsoleText Title;
        enum MainMenu
        {
            [Description("Магазин")]
            Shop,
            [Description("Корзина")]
            Cart,
            [Description("Сменить пароль")]
            ChangePassword,
            [Description("Выход из аккаунта")]
            LogOut,
            [Description("Выход")]
            Exit
        }
        string[] mainMenu = EnumHelper.GetArrayFromEnum<MainMenu>();

        List<(string product, int count, int cost)> Cart = new();

        public CustomerUI(Customer customer, Settings settings)
        {
            this.settings = settings;
            CurrentAccount = customer;

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
                Title.Text = $"Покупатель - {CurrentAccount.Login}";
                Title.Write();
                menu.Write();
                selectedMenu = (MainMenu)menu.Choice((int)selectedMenu);

                Title.Clear();
                menu.Clear();
                Console.SetCursorPosition(0, 0);

                switch (selectedMenu)
                {
                    case MainMenu.Shop:
                        ShowShopMenu();
                        break;
                    case MainMenu.Cart:
                        ShowCartMenu();
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

        enum ProductMenu
        {
            [Description("Назад")]
            Back,
            [Description("Добавить в корзину")]
            AddToCart
        }
        string[] productMenu = EnumHelper.GetArrayFromEnum<ProductMenu>();

        public void ShowShopMenu()
        {
            ConsoleSelect menu = new ConsoleSelect(
                new string[] { },
                startY: 2,
                write: false
            );

            for (; ; )
            {
                Title.Text = "Товары";
                Title.Write();
                string[] ProductsList = settings.GetProductsList();

                menu.Update(ProductsList.Add("Назад"));
                int selectedIndex = menu.Choice(
                    ConsoleSelect.AllowEsc
                );
                if (selectedIndex == menu.Choices.Count - 1 || selectedIndex == -1) break;

                string[] productFilePaths = settings.FindProductFilePaths(ProductsList[selectedIndex]);
                if (productFilePaths.Length == 0)
                {
                    Console.WriteLine("Ошибка поиска товара");
                    continue;
                }
                Product product = new(productFilePaths[0]);

                Title.Text = $"Товар - {product.Name}";
                menu.Update(productMenu);

                Console.SetCursorPosition(0, menu.StartY + menu.ContentHeight + 1);

                UIFunctions.WriteProductInfo(product);


                ProductMenu selectedMenu = (ProductMenu)menu.Choice(
                    (ConsoleKeyInfo key, int selectedIndex) => (key.Key == ConsoleKey.Escape) ? (int)ProductMenu.Back : null
                    );

                if (selectedMenu == ProductMenu.Back) break;

                switch (selectedMenu)
                {
                    case ProductMenu.AddToCart:
                        {
                            Console.Clear();
                            int count = 0;
                            try
                            {
                                Console.Clear();
                                while (true)
                                {
                                    if (!int.TryParse(ConsoleHelper.Input_ex("Кол-во товара: ", "1"), out count))
                                        Console.WriteLine($"!> Ошибка ввода");
                                    else
                                        break;
                                }
                            }
                            catch (OperationCanceledException) { }
                            Console.Clear();

                            if (count > 0)
                                Cart.Add((product.Name, count, product.Cost));
                        }
                        break;
                }

            }
            Console.Clear();
        }

        enum CartItemMenu
        {
            [Description("Назад")]
            Back,
            [Description("Изменить количество")]
            ChangeCount
        }
        string[] cartItemMenu = EnumHelper.GetArrayFromEnum<CartItemMenu>();
        void ShowCartMenu()
        {
            ConsoleSelect menu = new ConsoleSelect(
                new string[] { },
                startY: 2,
                write: false
            );

            for (; ; )
            {
                Title.Text = "Корзина (del - удалить)";
                Title.Write();

                var tmenu = Cart.Select(p => $"{p.product} ~ {p.count} * {p.cost}Р. = {p.count * p.cost}Р.")
                    .Append($"Итого: {Cart.Aggregate(0, (sum, p) => sum += p.cost * p.count)}Р.")
                    .Append("Отправить")
                    .Append("Назад");
                if (Cart.Count == 0) menu.Disable(1, false);
                menu.Update(tmenu.ToArray());
                int selectedIndex = menu.Choice(
                    (ConsoleKeyInfo key, int selectedIndex) =>
                    {
                        if (key.Key == ConsoleKey.Escape) return -1;
                        if (key.Key == ConsoleKey.Delete && selectedIndex < menu.Choices.Count - 2)
                        {
                            Cart.RemoveAt(selectedIndex);
                            menu.Choices.RemoveAt(selectedIndex);
                        }
                        return null;
                    }
                );
                menu.EnableAll(false);

                if (selectedIndex == menu.Choices.Count - 1 || selectedIndex == -1) break;
                if (selectedIndex == menu.Choices.Count - 3) continue;

                if (selectedIndex == menu.Choices.Count - 2) // Отправить
                {
                    Order order = new(DateTime.Now.ToString("G").Replace(':', '.'), CurrentAccount.Login, DateTime.Now, Cart);
                    settings.SaveOrder(order);
                    Cart.Clear();
                    break;
                }

                Title.Text = $"Корзина - {Cart[selectedIndex].product}";
                menu.Clear();
                Console.SetCursorPosition(0, 2);

                try
                {
                    int newCount = 1;
                    while (true)
                    {
                        if (!int.TryParse(ConsoleHelper.Input_ex("Кол-во товара (0 - удалить): ", Cart[selectedIndex].count.ToString()), out newCount))
                            Console.WriteLine($"!> Ошибка ввода");
                        else
                            break;
                    }
                    var titem = Cart[selectedIndex];
                    Cart[selectedIndex] = (titem.product, newCount, titem.cost);

                }
                catch (OperationCanceledException) { }
                Console.Clear();
            }
            menu.Clear();
        }
    }
}