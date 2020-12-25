using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Shop.Account;

namespace Shop
{
    public static class UIFunctions
    {
        public static bool ChangePassword(ref string password)
        {
            try
            {
                while (true)
                {
                    password = ConsoleHelper.Input_ex("Пароль: ", password);
                    if (ValidatePassword(password, out string msg)) break;
                    else Console.WriteLine($"!> {msg}");
                }
                return true;
            }
            catch (OperationCanceledException) { }
            return false;
        }

        public static bool WorkExperienceTryParse(string s, out ushort months)
        {
            int pos = s.IndexOf('.');

            if (pos >= 0 && pos != s.Length - 1)
            {
                bool res = ushort.TryParse(s.Substring(0, pos), out ushort years);
                res &= ushort.TryParse(s.Substring(pos + 1), out ushort months_);
                months = res ? (ushort)(months_ + years * 12) : 0;
                return res;
            }
            else
            {
                bool res = ushort.TryParse(s, out ushort years);
                months = (ushort)(years * 12);
                return res;
            }
        }

        static string Years(int y) => y switch
        {
            int when (y % 100 is >= 11 and <= 20) || y % 10 >= 5 => "лет",
            int when y % 10 is >= 2 and <= 4 => "года",
            int when y % 10 == 1 => "год",
            _ => throw new ArgumentException()
        };
        static string Months(int m) => m switch
        {
            int when (m % 100 is >= 11 and <= 20) || m % 10 >= 5 => "месяцев",
            int when m % 10 is >= 2 and <= 4 => "месяца",
            int when m % 10 == 1 => "месяц",
            _ => throw new ArgumentException()
        };

        /*static string Months(int m)
        {
            if (m % 100 >= 11 && m % 100 <= 20 || m % 10 >= 5) return "месяцев";
            if (m % 10 == 1) return "месяц";
            if (m % 10 >= 2 && m % 10 <= 4) return "месяца";
            throw new ArgumentException();
        }*/
        public static string WorkExperienceToString(ushort months)
        {
            if (months == 0) return "нет";
            int years = months / 12;
            int months_ = months % 12;
            if (years == 0) return $"{months_} {Months(months_)}";
            if (months_ == 0) return $"{years} {Years(years)}";
            return $"{years} {Years(years)} и {months_} {Months(months_)}";

        }

        public static void InputLogPass(Account account, Settings settings)
        {
            while (true)
            {
                account.Login = ConsoleHelper.Input_ex("Логин: ");
                if (ValidateLogin(account.Login, out string msg)) break;
                else Console.WriteLine($"!> {msg}");

                if (settings.FindAccount(account.Login) == null)
                    break;
                else
                    Console.WriteLine($"!> Такой логин уже существует");
            }
            InputPassword(account);
        }

        public static void InputPassword(Account account, bool update = false)
        {
            while (true)
            {
                account.Password = ConsoleHelper.Input_ex("Пароль: ", update ? account.Password : "");
                if (ValidatePassword(account.Password, out string msg)) break;
                else Console.WriteLine($"!> {msg}");
            }
        }
        public static void InputFIO(Employee employee, bool update = false)
        {
            if (update)
                (employee.LastName, employee.FirstName, employee.Patronymic) = InputFIO(employee.LastName, employee.FirstName, employee.Patronymic);
            else
                (employee.LastName, employee.FirstName, employee.Patronymic) = InputFIO();
        }
        public static (string F, string I, string O) InputFIO(string F = null, string I = null, string O = null)
        {
            (string F, string I, string O) FIO;
            while (true)
            {
                FIO.F = ConsoleHelper.Input_ex("Фамилия: ", F ?? "");
                if (FIO.F.Length > 0) break;
                else Console.WriteLine($"!> Фамилия не может быть пустой");
            }
            while (true)
            {
                FIO.I = ConsoleHelper.Input_ex("Имя: ", I ?? "");
                if (FIO.I.Length > 0) break;
                else Console.WriteLine($"!> Имя не может быть пустым");
            }
            while (true)
            {
                FIO.O = ConsoleHelper.Input_ex("Отчество: ", O ?? "");
                if (FIO.O.Length > 0) break;
                else Console.WriteLine($"!> Отчество не может быть пустым"); // TODO: сможет, но не сегодня
            }
            return FIO;
        }

        /// <exception cref="OperationCanceledException">esc -> throw</exception>
        public static void InputEmployeeData(Employee employee, bool FIO = true)
        {
            if (FIO) InputFIO(employee);

            DateTime birthday;
            while (true)
                if (!DateTime.TryParse(ConsoleHelper.Input_ex("Дата рождения: "), out birthday))
                    Console.WriteLine("Ошибка ввода, попробуйте ещё раз");
                else
                    if (ValidateEmployeeBirthday(birthday, out string msg))
                    break;
                else
                    Console.WriteLine(msg);
            employee.Birthday = birthday;

            Console.WriteLine("Образования: (пустая строка - закончить ввод)");
            List<string> educations = new();
            string education;
            for (; ; )
            {
                education = ConsoleHelper.Input_ex(" > ");
                if (education != "")
                    educations.Add(education);
                else
                    break;
            }
            employee.Educations = educations.ToArray();

            ushort workExperience;
            while (!WorkExperienceTryParse(ConsoleHelper.Input_ex("Опыт работы (1.4 - 1 год и 4 месяца): "), out workExperience))
                Console.WriteLine("Ошибка ввода, попробуйте ещё раз");
            employee.WorkExperience = workExperience;

            string placeOfWork = ConsoleHelper.Input_ex("Место работы (пустая строка = главный офис): ");
            employee.PlaceOfWork = placeOfWork == "" ? "Главный офис" : placeOfWork;
        }

        public static string GetAccountTypeName(AccountType type) =>
            type switch
            {
                AccountType.Admin => "Администратор",
                AccountType.HR => "Сотрудник отдела кадров",
                AccountType.WarehouseManager => "Складовщик",
                AccountType.Cashier => "Кассир",
                AccountType.Accountant => "Бухгалтер",
                AccountType.Customer => "Покупатель",
                _ => throw new ArgumentException(message: "invalid enum value", paramName: nameof(type))
            };

        public static void WriteAccountInfo(Account account, bool password = false)
        {
            Console.WriteLine($" Должность: {GetAccountTypeName(account.Type)}");
            Console.WriteLine($" Логин: {account.Login}");
            if (password) Console.WriteLine($" Пароль: {account.Password}");
            switch (account)
            {
                case Employee employee:
                    Console.WriteLine($" ФИО: {GetFullName(employee)}");
                    Console.WriteLine($" Дата Рождения: {employee.Birthday.ToString("d")}");
                    Console.WriteLine(" Образования:");
                    foreach (string education in employee.Educations)
                        Console.WriteLine($" > {education}");
                    Console.WriteLine($" Опыт работы: {WorkExperienceToString(employee.WorkExperience)}");
                    Console.WriteLine($" Место работы: {employee.PlaceOfWork}");
                    break;
                case Customer customer:
                    Console.WriteLine($" E-mail: {customer.Email}");
                    break;
            }
        }
        public static void WriteUnregisteredAccountInfo(UnregisteredAccount uAccount, bool password = false)
        {
            Console.WriteLine($" Логин: {uAccount.Login}");
            if (password) Console.WriteLine($" Пароль: {uAccount.Password}");
            Console.WriteLine($" ФИО: {uAccount.LastName} {uAccount.FirstName} {uAccount.Patronymic}");
        }

        public static void UpdateLogPass(Account account, Settings settings)
        {
            string _login = account.Login;
            while (true)
            {
                account.Login = ConsoleHelper.Input_ex("Логин: ", account.Login);
                if (ValidateLogin(account.Login, out string msg)) break;
                else Console.WriteLine($"!> {msg}");

                if (account.Login != _login)
                {
                    if (settings.FindAccount(account.Login) == null)
                    {
                        settings.RenameAccount(_login, account.Login, account.Type);
                        break;
                    }
                    else
                        Console.WriteLine($"!> Такой логин уже существует");
                }
            }
            InputPassword(account, true);
        }
        public static void UpdateEmployeeData(Employee employee)
        {
            InputFIO(employee, true);

            DateTime birthday;
            while (true)
                if (!DateTime.TryParse(ConsoleHelper.Input_ex("Дата рождения: ", employee.Birthday.ToString("d")), out birthday))
                    Console.WriteLine("Ошибка ввода, попробуйте ещё раз");
                else
                    if (ValidateEmployeeBirthday(birthday, out string msg))
                    break;
                else
                    Console.WriteLine(msg);
            employee.Birthday = birthday;

            employee.Educations = ConsoleHelper.Input_ex("Образования: (разделять ;): ", String.Join(';', employee.Educations))
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()).ToArray();

            ushort workExperience;
            while (!WorkExperienceTryParse(ConsoleHelper.Input_ex("Опыт работы: ", $"{employee.WorkExperience / 12}.{employee.WorkExperience % 12}"), out workExperience))
                Console.WriteLine("Ошибка ввода, попробуйте ещё раз");
            employee.WorkExperience = workExperience;

            string placeOfWork = ConsoleHelper.Input_ex("Место работы (пустая строка = главный офис): ", employee.PlaceOfWork);
            employee.PlaceOfWork = placeOfWork == "" ? "Главный офис" : placeOfWork;
        }
        public static void UpdateCustomerData(Customer customer)
        {
            while (true)
            {
                customer.Email = ConsoleHelper.Input_ex("E-mail: ", customer.Email);
                if (CheckEmail(customer.Email)) break; // TODO: Сделать отправку письма
                else Console.WriteLine($"!> Ошибка проверки Email");
            }
        }
        public static void UpdateAccountData(Account account, Settings settings, bool logpass = false)
        {
            if (logpass) UpdateLogPass(account, settings);
            switch (account)
            {
                case Employee employee: UpdateEmployeeData(employee); break;
                case Customer customer: UpdateCustomerData(customer); break;
            }
        }

        public static bool ShowAccountEditMenu(Account account, Settings settings, bool asAdmin = false)
        {
            ConsoleSelect menu = new ConsoleSelect(
                new string[] { "Назад", "Редактировать", "Перевести на другую должность" }
            );

            int startY = menu.ContentHeight + 2;

            Console.SetCursorPosition(0, startY);

            WriteAccountInfo(account, asAdmin);

            int selectedIndex = menu.Choice();

            switch (selectedIndex)
            {
                case 1:
                    try
                    {
                        Console.Clear();
                        UpdateAccountData(account, settings, asAdmin);
                        Console.Clear();
                        return true;
                    }
                    catch (OperationCanceledException) { }

                    break;
                case 2:
                    Console.Clear();
                    Console.WriteLine($"Текущая должность: {GetAccountTypeName(account.Type)}\n");
                    Console.WriteLine("Выберите новую должность (esc - отмена):");

                    menu.StartY = Console.CursorTop;
                    menu.Update(employeeMenu);
                    EmployeeMenu selectedMenu = (EmployeeMenu)menu.Choice(
                        (ConsoleKeyInfo key, int selectedIndex) => (key.Key == ConsoleKey.Escape) ? (int)EmployeeMenu.Back : null
                        );
                    if (selectedMenu != EmployeeMenu.Back)
                        settings.ChangeJob(account.Login, account.Type, Enum.Parse<AccountType>(Enum.GetName(selectedMenu)));

                    break;
            }

            Console.Clear();
            return false;
        }

        enum EmployeeMenu : byte
        {
            [Description("Сотрудник отдела кадров")]
            HR,
            [Description("Складовщик")]
            WarehouseManager,
            [Description("Кассир")]
            Cashier,
            [Description("Бухгалтер")]
            Accountant,
            [Description("Назад")]
            Back
        }

        static string[] employeeMenu = EnumHelper.GetArrayFromEnum<EmployeeMenu>();

        public static Employee ShowUnregisteredAccountEditMenu(UnregisteredAccount uAccount, bool asAdmin = false)
        {
            ConsoleSelect menu = new ConsoleSelect(
                new string[] { "Назад", "Преобразовать" }
            );

            int startY = menu.ContentHeight + 2;

            Console.SetCursorPosition(0, startY);

            WriteUnregisteredAccountInfo(uAccount, asAdmin);

            int selectedIndex = menu.Choice();

            if (selectedIndex == 1)
            {
                try
                {
                    Console.Clear();
                    menu.Update(employeeMenu);
                    EmployeeMenu selectedMenu = (EmployeeMenu)menu.Choice(
                        (ConsoleKeyInfo key, int selectedIndex) => (key.Key == ConsoleKey.Escape) ? (int)EmployeeMenu.Back : null
                        );
                    if (selectedMenu != EmployeeMenu.Back)
                    {
                        Employee employee = selectedMenu switch
                        {
                            EmployeeMenu.HR => new HR(),
                            EmployeeMenu.WarehouseManager => new WarehouseManager(),
                            EmployeeMenu.Cashier => new Cashier(),
                            EmployeeMenu.Accountant => new Accountant(),
                            _ => throw new ArgumentException(message: "invalid enum value", paramName: nameof(selectedMenu))
                        };

                        menu.Clear();

                        Console.WriteLine($"Должность: {GetAccountTypeName(employee.Type)}");
                        InputEmployeeData(employee, false);
                        Console.Clear();

                        employee.Login = uAccount.Login;
                        employee.Password = uAccount.Password;
                        employee.FirstName = uAccount.FirstName;
                        employee.LastName = uAccount.LastName;
                        employee.Patronymic = uAccount.Patronymic;

                        return employee;
                    }
                }
                catch (OperationCanceledException) { }
            }

            Console.Clear();
            return null;
        }

        public static bool ShowDismissedEmployeeEditMenu(Account account, bool asAdmin = false)
        {
            ConsoleSelect menu = new ConsoleSelect(
                new string[] { "Назад", "Восстановить" }
            );

            int startY = menu.ContentHeight + 2;

            Console.SetCursorPosition(0, startY);

            WriteAccountInfo(account, asAdmin);

            int selectedIndex = menu.Choice();

            if (selectedIndex == 1)
            {
                Console.Clear();
                return true;
            }

            Console.Clear();
            return false;
        }

        public static void ShowUnregisteredAccountMenu(string login, Settings settings, bool asAdmin = false)
        {
            UnregisteredAccount uAccount = settings.LoadUnregisteredAccount(login);

            Employee employee = ShowUnregisteredAccountEditMenu(uAccount, asAdmin);
            if (employee != null)
            {
                settings.RemoveUnregisteredAccount(login);
                settings.AddAccount(employee);
            }
        }
        public static void ShowDismissedAccountMenu(string login, Settings settings, bool asAdmin = false)
        {
            Account account = settings.LoadDismissedAccount(login);

            if (ShowDismissedEmployeeEditMenu(account, asAdmin))
                settings.RestoreEmployee(login);
        }

        public static string GetInitials(Employee e) => $"{e.LastName} {e.FirstName[0]}.{e.Patronymic[0]}.";
        public static string GetFullName(Employee e) => $"{e.LastName} {e.FirstName} {e.Patronymic}";

        static Regex regexEmail = new Regex(@"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", RegexOptions.Compiled);
        public static bool CheckEmail(string email) => regexEmail.IsMatch(email);


        static Regex regexLogin = new Regex(@"^[a-zA-Z0-9_]+$", RegexOptions.Compiled);
        static string specialCharacters = "!@#$%^&*()-_+=;:,./?|`~[]{}<>";
        public static bool IsLetter(char c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z');
        public static bool IsCapitalLetter(char c) => c is >= 'A' and <= 'Z';

        //ClassEditor<Account>.ValidateFunc
        public static bool ValidateLogin(string login, out string msg)
        {
            try
            {
                if (login.Length == 0)
                    throw new FormatException("Логин не должен быть пустым");
                if (!regexLogin.IsMatch(login))
                    throw new FormatException("Логин должен состоять из латинский букв и цифр");
            }
            catch (FormatException ex)
            {
                msg = ex.Message;
                return false;
            }
            msg = ""; return true;
        }
        public static bool ValidatePassword(string password, out string msg)
        {
            try
            {
                if (password.Length < 8)
                    throw new FormatException("Пароль должен состоять как минимум из 8 символов");

                int specialCharacterCount = 0,
                    digits = 0,
                    capitalLetters = 0;

                foreach (char c in password)
                {
                    bool isLetter = IsLetter(c);
                    bool isDigit = Char.IsDigit(c);
                    bool isSC = specialCharacters.Contains(c);
                    if (!(isLetter || isDigit || isSC))
                        throw new FormatException("Пароль должен содержать только символы латинского алфавита, цифры, !@#$%^&*()-_+=;:,./?|`~[]{}<>");

                    if (IsCapitalLetter(c)) capitalLetters++;
                    if (isDigit) digits++;
                    if (isSC) specialCharacterCount++;
                }
                if (digits < 3)
                    throw new FormatException("Пароль должен содержать как минимум 3 цифры");
                if (specialCharacterCount < 2)
                    throw new FormatException("Пароль должен содержать как минимум 2 спецсимвола");
                if (capitalLetters < 3)
                    throw new FormatException("Пароль должен содержать как минимум 3 заглавные буквы не подряд");

                int firstCapitalLetterIndex = -1;
                for (int i = 0; i < password.Length; i++)
                    if (IsCapitalLetter(password[i])) { firstCapitalLetterIndex = i; break; }

                if (!(capitalLetters >= 3 && Helper.RepetionOfSymbolPredicate(password, c => IsCapitalLetter(c)) != capitalLetters))
                    throw new FormatException("Пароль должен содержать как минимум 3 заглавные буквы не подряд");
            }
            catch (FormatException ex)
            {
                msg = ex.Message;
                return false;
            }
            msg = ""; return true;
        }
        public static bool ValidateEmployeeBirthday(DateTime birthday, out string msg)
        {
            try
            {
                if (birthday > DateTime.Today)
                    throw new FormatException("Дата рождения не может быть больше текущей даты");
                if (birthday < new DateTime(1960, 1, 1))
                    throw new FormatException("Дата рождения должна быть не меньше 1960");
                if (birthday.AddYears(18) > DateTime.Today)
                    throw new FormatException("Возраст сотрудника должен быть не меньше 18 лет");
            }
            catch (FormatException ex)
            {
                msg = ex.Message;
                return false;
            }
            msg = ""; return true;
        }


        enum WarehouseMenu : byte
        {
            [Description("Товары")]
            Products,
            [Description("Добавить товар")]
            AddProduct,
            [Description("Переименовать склад")]
            Rename,
            [Description("Назад")]
            Back
        }
        static string[] warehouseMenu = EnumHelper.GetArrayFromEnum<WarehouseMenu>();
        enum ProductMenu : byte
        {
            [Description("Назад")]
            Back,
            [Description("Изменить имя")]
            EditName,
            [Description("Изменить цену")]
            EditCost,
            [Description("Изменить количество")]
            EditCount,
            [Description("Изменить срок годности")]
            EditExpirationDate,
            [Description("Изменить категорию")]
            EditCategory
        }
        static string[] productMenu = EnumHelper.GetArrayFromEnum<ProductMenu>();

        public static void WriteProductInfo(Product product)
        {
            Console.WriteLine($" Имя: {product.Name}");
            Console.WriteLine($" Цена: {product.Cost}");
            Console.WriteLine($" Кол-во: {product.Count}");
            Console.WriteLine($" Срок годности: {product.ExpirationDate.ToString("d")}");
            Console.WriteLine($" Категория: {product.Category}");
        }

        public static void ShowWarehouseMenu(string warehouseName, Settings settings)
        {
            ConsoleSelect menu = new ConsoleSelect(
                new string[] { },
                startY: 2,
                write: false
            );
            ConsoleText Title = new(0, 0, 1, "", true);

            Warehouse warehouse = new Warehouse(settings.GetWarehousePatch(warehouseName));
            for (; ; )
            {
                Title.Text = $"Склад - {warehouse.Name}";
                string[] unregisteredAccountsList = settings.GetUnregisteredAccountsList();

                menu.Update(warehouseMenu);
                WarehouseMenu selectedMenu = (WarehouseMenu)menu.Choice(
                    (ConsoleKeyInfo key, int selectedIndex) => (key.Key == ConsoleKey.Escape) ? (int)WarehouseMenu.Back : null
                    );
                if (selectedMenu == WarehouseMenu.Back) break;

                menu.Clear();

                switch (selectedMenu)
                {
                    case WarehouseMenu.Products:
                        {
                            for (int selectedIndex = 0; ;)
                            {
                                Title.Text = $"Склад - {warehouse.Name} - товары";
                                menu.Update(warehouse.Products.Append("Назад").ToArray());

                                selectedIndex = menu.Choice(selectedIndex,
                                    (ConsoleKeyInfo key, int selectedIndex) =>
                                    {
                                        if (key.Key == ConsoleKey.Escape) return -1;
                                        if (key.Key == ConsoleKey.Delete && selectedIndex < menu.Choices.Count - 1)
                                        {
                                            warehouse.RemoveProduct(warehouse.Products[selectedIndex]);
                                            menu.Choices.RemoveAt(selectedIndex);
                                        }
                                        return null;
                                    });
                                if (selectedIndex == menu.Choices.Count - 1 || selectedIndex == -1) break;


                                Product product = warehouse.GetProduct(warehouse.Products[selectedIndex]);
                                Title.Text = $"Склад - {warehouse.Name} - {product.Name}";

                                menu.Update(productMenu);
                                for (ProductMenu selectedProductMenu; ;)
                                {
                                    Title.Write();
                                    menu.Write();

                                    Console.SetCursorPosition(0, menu.ContentHeight + 3);
                                    WriteProductInfo(product);

                                    selectedProductMenu = (ProductMenu)menu.Choice(
                                        (ConsoleKeyInfo key, int selectedIndex) => (key.Key == ConsoleKey.Escape) ? (int)ProductMenu.Back : null
                                        );

                                    Console.Clear();
                                    if (selectedProductMenu == ProductMenu.Back) break;

                                    try
                                    {
                                        switch (selectedProductMenu)
                                        {
                                            case ProductMenu.EditName:
                                                while (true)
                                                {
                                                    string name = ConsoleHelper.Input_ex("Имя товара: ", product.Name);
                                                    if (name.Length > 0) { product.Name = name; break; }
                                                    else Console.WriteLine($"!> Имя товара не может быть пустым");
                                                }
                                                break;
                                            case ProductMenu.EditCost:
                                                while (true)
                                                {
                                                    if (!int.TryParse(ConsoleHelper.Input_ex("Цена товара: ", product.Cost.ToString()), out int cost))
                                                        Console.WriteLine($"!> Ошибка ввода");
                                                    else
                                                    { product.Cost = cost; break; }
                                                }
                                                break;
                                            case ProductMenu.EditCount:
                                                while (true)
                                                {
                                                    if (!int.TryParse(ConsoleHelper.Input_ex("Кол-во товара: ", product.Count.ToString()), out int count))
                                                        Console.WriteLine($"!> Ошибка ввода");
                                                    else
                                                    { product.Count = count; break; }
                                                }
                                                break;
                                            case ProductMenu.EditExpirationDate:
                                                while (true)
                                                {
                                                    if (!DateTime.TryParse(ConsoleHelper.Input_ex("Срок годности товара: ", product.ExpirationDate.ToString("d")), out DateTime expirationDate))
                                                        Console.WriteLine($"!> Ошибка ввода");
                                                    else
                                                    { product.ExpirationDate = expirationDate; break; }
                                                }
                                                break;
                                            case ProductMenu.EditCategory:
                                                while (true)
                                                {
                                                    string categoty = ConsoleHelper.Input_ex("Категория товара: ", product.Category);
                                                    if (categoty.Length > 0) { product.Category = categoty; break; }
                                                    else Console.WriteLine($"!> Категория товара не может быть пустой");
                                                }
                                                break;
                                        }
                                    }
                                    catch (OperationCanceledException) { }
                                    Console.Clear();
                                }
                            }
                        }
                        break;
                    case WarehouseMenu.AddProduct:
                        {
                            Title.Text = $"Склад - {warehouse.Name} - Добавление продукта";
                            Console.SetCursorPosition(0, 2);

                            try
                            {
                                Product product = new();
                                while (true)
                                {
                                    string name = ConsoleHelper.Input_ex("Имя товара: ");
                                    if (name.Length > 0) { product.Name = name; break; }
                                    else Console.WriteLine($"!> Имя товара не может быть пустым");
                                }
                                while (true)
                                {
                                    if (!int.TryParse(ConsoleHelper.Input_ex("Цена товара: "), out int cost))
                                        Console.WriteLine($"!> Ошибка ввода");
                                    else
                                    { product.Cost = cost; break; }
                                }
                                while (true)
                                {
                                    if (!int.TryParse(ConsoleHelper.Input_ex("Кол-во товара: "), out int count))
                                        Console.WriteLine($"!> Ошибка ввода");
                                    else
                                    { product.Count = count; break; }
                                }
                                while (true)
                                {
                                    if (!DateTime.TryParse(ConsoleHelper.Input_ex("Срок годности товара: "), out DateTime expirationDate))
                                        Console.WriteLine($"!> Ошибка ввода");
                                    else
                                    { product.ExpirationDate = expirationDate; break; }
                                }
                                while (true)
                                {
                                    string categoty = ConsoleHelper.Input_ex("Категория товара: ");
                                    if (categoty.Length > 0) { product.Category = categoty; break; }
                                    else Console.WriteLine($"!> Категория товара не может быть пустой");
                                }

                                warehouse.AddProduct(product);
                            }
                            catch (OperationCanceledException) { }
                        }
                        break;
                    case WarehouseMenu.Rename:
                        {
                            Title.Text = "Изменение имени склада";
                            Console.SetCursorPosition(0, 2);
                            try
                            {
                                string newWarehouseName = ConsoleHelper.Input_ex("Новое имя склада: ", warehouse.Name);

                                settings.RenameWarehouse(warehouse.Name, newWarehouseName);
                                warehouse.Path = settings.GetWarehousePatch(newWarehouseName);
                            }
                            catch (OperationCanceledException) { }
                        }
                        break;
                }
                Console.Clear();
            }
            menu.Clear();
        }
    }
}
