using Shop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using static Shop.Account;

namespace Shop
{
    public class CashierUI
    {
        Cashier CurrentAccount;
        Settings settings;

        ConsoleText Title;
        enum MainMenu
        {
            [Description("Заказы")]
            Orders,
            [Description("Сменить пароль")]
            ChangePassword,
            [Description("Выход из аккаунта")]
            LogOut,
            [Description("Выход")]
            Exit
        }
        string[] mainMenu = EnumHelper.GetArrayFromEnum<MainMenu>();

        public CashierUI(Cashier warehouseManager, Settings settings)
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
                Title.Text = $"Кассир - { UIFunctions.GetFullName(CurrentAccount)}";
                Title.Write();
                menu.Write();
                selectedMenu = (MainMenu)menu.Choice((int)selectedMenu);

                Title.Clear();
                menu.Clear();
                Console.SetCursorPosition(0, 0);

                switch (selectedMenu)
                {
                    case MainMenu.Orders:
                        ShowOrdersMenu();
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

        enum OrderMenu
        {
            [Description("Проверить наличие товаров и оформить")]
            CheckAndDecision,
            [Description("Отменить заказ")]
            CancelOrder,
            [Description("Назад")]
            Back
        }
        string[] orderMenu = EnumHelper.GetArrayFromEnum<OrderMenu>();

        public void ShowOrdersMenu()
        {
            ConsoleSelect menu = new ConsoleSelect(
                new string[] { },
                startY: 2,
                write: false
            );

            for (; ; )
            {
                Title.Text = "Заказы";
                Title.Write();
                string[] ordersList = settings.GetOrdersList();

                menu.Update(ordersList.Add("Назад"));
                int selectedIndex = menu.Choice(
                    ConsoleSelect.AllowEsc
                );
                if (selectedIndex == menu.Choices.Count - 1 || selectedIndex == -1) break;

                Order order = new(settings.GetOrderFilePatch(ordersList[selectedIndex]));

                Title.Text = $"Заказ - {order.Id}";
                menu.Update(orderMenu);

                Console.SetCursorPosition(0, menu.StartY + menu.ContentHeight + 1);
                Console.WriteLine($" Id: {order.Id}");
                Console.WriteLine($" Покупатель: {order.CustomerLogin}");
                Console.WriteLine($" Дата заказа: {order.Date}");

                bool check = CheckOrderProducts(order.Items, out var list);

                int sumCost = 0;

                if (check)
                {
                    Console.WriteLine(" Товары:");
                    foreach ((string product, int count, string warehouseName, int cost) in list)
                    {
                        Console.WriteLine($" > {product} ~ {count} * {cost} = {cost * count} Р.");
                        sumCost += cost * count;
                    }
                }
                else
                {
                    Console.WriteLine(" Часть товаров не была найдена на складах, оформить заказ невозможно");
                    Console.WriteLine(" Товары:");

                    foreach ((string product, int count, int cost) in order.Items)
                    {
                        Console.WriteLine($" > {product} ~ {count} * {cost} = {cost * count} Р.");
                        sumCost += cost * count;
                    }
                    menu.Disable((int)OrderMenu.CheckAndDecision);
                }
                Console.WriteLine($" Итого: {sumCost} Р.");

                OrderMenu selectedMenu = (OrderMenu)menu.Choice(
                    (ConsoleKeyInfo key, int selectedIndex) => (key.Key == ConsoleKey.Escape) ? (int)OrderMenu.Back : null
                    );

                if (!check) menu.Enable((int)OrderMenu.CheckAndDecision);

                if (selectedMenu == OrderMenu.Back) break;

                switch (selectedMenu)
                {
                    case OrderMenu.CheckAndDecision:
                        {
                            foreach ((string productName, int count, string warehouseName, int cost) in list)
                            {
                                string productPath = settings.GetProductPatch(warehouseName, productName);
                                Product product = new(productPath);
                                product.Count -= count;
                                product.SaveToFile(productPath);
                            }

                            using (StreamWriter fwriter = new(File.Open(settings.GetChequeFilePatch(order.Id), FileMode.Create)))
                            {
                                fwriter.WriteLine($"Id: {order.Id}");
                                fwriter.WriteLine($"Покупатель: {order.CustomerLogin}");
                                fwriter.WriteLine($"Дата заказа: {order.Date}");
                                fwriter.WriteLine("Товары:");
                                foreach ((string product, int count, string warehouseName, int cost) in list)
                                {
                                    fwriter.WriteLine($"> {product} ~ {count} * {cost} = {cost * count} Р.");
                                }
                                fwriter.WriteLine($" Итого: {sumCost} Р.");
                            }

                            // Send Email with settings.GetChequeFilePatch(order.Id) to customer

                            Console.Clear();
                            menu.Choices.RemoveAt(selectedIndex);

                            settings.CompleteOrder(order.Id);
                        }
                        break;
                    case OrderMenu.CancelOrder:
                        {
                            settings.CancelOrder(order.Id);
                        }
                        break;
                }

            }
            Console.Clear();
        }

        // Можно было сделать через GetFiles
        bool CheckOrderProducts(IList<(string product, int count, int cost)> items, out IList<(string product, int count, string warehouseName, int cost)> list)
        {
            list = new (string product, int count, string warehouseName, int cost)[items.Count];

            List<(int index, string product)> tItems = new(items.Count);
            for (int i = 0; i < items.Count; i++)
                tItems.Add((i, items[i].product));

            foreach (string warehouseName in settings.GetWarehousesList())
            {
                Warehouse warehouse = new(settings.GetWarehousePatch(warehouseName));
                for (int i = 0; i < tItems.Count; i++)
                //foreach ((int index, string productName) in tItems)
                {
                    (int index, string productName) = tItems[i];
                    if (warehouse.Products.Contains(productName))
                    {
                        int requiredCount = items[index].count;
                        Product product = warehouse.GetProduct(productName);
                        if (product.Count >= requiredCount && product.Cost == items[index].cost)
                        {
                            list[index] = (productName, requiredCount, warehouseName, product.Cost);
                            tItems.RemoveAt(i--);
                        }
                    }
                }
                if (tItems.Count == 0) break;
            }

            return tItems.Count == 0;
        }
    }
}