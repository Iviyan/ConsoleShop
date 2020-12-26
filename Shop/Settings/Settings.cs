using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Shop.Account;

namespace Shop
{
    public class Settings
    {
        public string FolderName { get; }
        public const string accountsFolderName = "Accounts";
        public const string accounts_HRFolderName = "HR";
        public const string accounts_WarehouseManagerFolderName = "WarehouseManager";
        public const string accounts_CashierFolderName = "Cashier";
        public const string accounts_AccountantFolderName = "Accountant";
        public const string accounts_CustomerFolderName = "Customer";
        public const string accounts_DismissedFolderName = "Dismissed";
        public const string accounts_UnregisteredFolderName = "Unregistered";
        
        public const string warehousesFolderName = "Warehouses";

        public const string ordersFolderName = "Orders";
        public const string orders_CompletedFolderName = "Completed orders";
        public const string orders_CancelledFolderName = "Cancelled orders";
        
        public const string chequesFolderName = "Cheques";
        public string AccountsFolderPath { get; }
        public string Accounts_HRFolderPath { get; }
        public string Accounts_WarehouseManagerFolderPath { get; }
        public string Accounts_CashierFolderPath { get; }
        public string Accounts_AccountantFolderPath { get; }
        public string Accounts_CustomerFolderPath { get; }
        public string Accounts_DismissedFolderPath { get; }
        public string Accounts_UnregisteredFolderPath { get; }
        public string[] AccountsFoldersList { get; }
        public string WarehousesFolderPath { get; }
        public string OrdersFolderPath { get; }
        public string ComletedOrdersFolderPath { get; }
        public string CancelledOrdersFolderPath { get; }
        public string ChequesFolderPath { get; }
        public string GetAccountFilePatch(string login, AccountType type) => @$"{GetFolderPathForAccountType(type)}\{login}.dat";
        public string GetDismissedAccountFilePatch(string login) => @$"{Accounts_DismissedFolderPath}\{login}.dat";
        public string GetUnredisteredAccountFilePatch(string login) => @$"{Accounts_UnregisteredFolderPath}\{login}.dat";
        public string GetWarehousePatch(string name) => @$"{WarehousesFolderPath}\{name}";
        public string GetProductPatch(string warehouseName, string productName) => @$"{WarehousesFolderPath}\{warehouseName}\{productName}.dat";
        public string GetOrderFilePatch(string name) => @$"{OrdersFolderPath}\{name}.dat";
        public string GetCompleteOrderFilePatch(string name) => @$"{ComletedOrdersFolderPath}\{name}.dat";
        public string GetCancelledOrderFilePatch(string name) => @$"{CancelledOrdersFolderPath}\{name}.dat";
        public string GetChequeFilePatch(string name) => @$"{ChequesFolderPath}\{name}.txt";
        public BinaryReader GetFileReader(string filePath, FileMode fileMode = FileMode.Open) => new BinaryReader(File.Open(filePath, fileMode));
        public BinaryWriter GetFileWriter(string filePath, FileMode fileMode = FileMode.Truncate) => new BinaryWriter(File.Open(filePath, fileMode));
        public BinaryReader GetAccountFileReader(string login, AccountType type, FileMode fileMode = FileMode.Open) => new BinaryReader(File.Open(GetAccountFilePatch(login, type), fileMode));
        public BinaryWriter GetAccountFileWriter(string login, AccountType type, FileMode fileMode = FileMode.Truncate) => new BinaryWriter(File.Open(GetAccountFilePatch(login, type), fileMode));

        public const string DefaultAdminLogin = "admin";
        public const string DefaultAdminPassword = "admin";

        private List<string> subjects = new List<string>();
        public string[] Subjects { get => subjects.ToArray(); }

        public Settings(
            string folderName = "Shop"
            )
        {
            FolderName = folderName;
            AccountsFolderPath = $@"{folderName}\{accountsFolderName}";
            Accounts_HRFolderPath = $@"{AccountsFolderPath}\{accounts_HRFolderName}";
            Accounts_WarehouseManagerFolderPath = $@"{AccountsFolderPath}\{accounts_WarehouseManagerFolderName}";
            Accounts_CashierFolderPath = $@"{AccountsFolderPath}\{accounts_CashierFolderName}";
            Accounts_AccountantFolderPath = $@"{AccountsFolderPath}\{accounts_AccountantFolderName}";
            Accounts_CustomerFolderPath = $@"{AccountsFolderPath}\{accounts_CustomerFolderName}";
            Accounts_DismissedFolderPath = $@"{AccountsFolderPath}\{accounts_DismissedFolderName}";
            Accounts_UnregisteredFolderPath = $@"{AccountsFolderPath}\{accounts_UnregisteredFolderName}";
            AccountsFoldersList = new string[]
            {
                AccountsFolderPath,
                Accounts_HRFolderPath,
                Accounts_WarehouseManagerFolderPath,
                Accounts_CashierFolderPath,
                Accounts_AccountantFolderPath,
                Accounts_CustomerFolderPath
            };
            
            WarehousesFolderPath = $@"{folderName}\{warehousesFolderName}";
            OrdersFolderPath = $@"{folderName}\{ordersFolderName}";
            ComletedOrdersFolderPath = $@"{folderName}\{orders_CompletedFolderName}";
            CancelledOrdersFolderPath = $@"{folderName}\{orders_CancelledFolderName}";
            ChequesFolderPath = $@"{folderName}\{chequesFolderName}";

            if (!Directory.Exists(FolderName)) Directory.CreateDirectory(FolderName);

            if (!Directory.Exists(AccountsFolderPath)) Directory.CreateDirectory(AccountsFolderPath);
            foreach (string path in AccountsFoldersList)
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (!Directory.Exists(Accounts_DismissedFolderPath)) Directory.CreateDirectory(Accounts_DismissedFolderPath);
            if (!Directory.Exists(Accounts_UnregisteredFolderPath)) Directory.CreateDirectory(Accounts_UnregisteredFolderPath);

            if (!File.Exists(GetAccountFilePatch(DefaultAdminLogin, AccountType.Admin)))
                AddAccount(new Admin(DefaultAdminLogin, DefaultAdminPassword));
            
            if (!Directory.Exists(WarehousesFolderPath)) Directory.CreateDirectory(WarehousesFolderPath);
            if (!Directory.Exists(OrdersFolderPath)) Directory.CreateDirectory(OrdersFolderPath);
            if (!Directory.Exists(ComletedOrdersFolderPath)) Directory.CreateDirectory(ComletedOrdersFolderPath);

            if (!Directory.Exists(ChequesFolderPath)) Directory.CreateDirectory(ChequesFolderPath);

        }

        public string GetFolderPathForAccountType(AccountType type) =>
            type switch
            {
                AccountType.Admin => AccountsFolderPath,
                AccountType.HR => Accounts_HRFolderPath,
                AccountType.WarehouseManager => Accounts_WarehouseManagerFolderPath,
                AccountType.Cashier => Accounts_CashierFolderPath,
                AccountType.Accountant => Accounts_AccountantFolderPath,
                AccountType.Customer => Accounts_CustomerFolderPath,
                _ => throw new ArgumentException(message: "invalid enum value", paramName: nameof(type))
            };

        public void AddAccount(Account account)
        {
            using (BinaryWriter writer = GetFileWriter(GetAccountFilePatch(account.Login, account.Type), FileMode.CreateNew))
                account.Export(writer);
        }
        public void UpdateAccount(Account account)
        {
            using (BinaryWriter writer = GetFileWriter(GetAccountFilePatch(account.Login, account.Type), FileMode.Truncate))
                account.Export(writer);
        }
        public void DismissEmployee(string login, AccountType type) =>
            File.Move(GetAccountFilePatch(login, type), GetDismissedAccountFilePatch(login));

        public void RestoreEmployee(string login)
        {
            string filePath = GetDismissedAccountFilePatch(login);
            string newFilePath;
            using (var reader = GetFileReader(filePath))
                newFilePath = GetAccountFilePatch(login, (AccountType)reader.ReadByte());

            File.Move(filePath, newFilePath);
        }
        public void ChangeJob(string login, AccountType type, AccountType newJob)
        {
            Account account = LoadAccount(login, type);
            account.Type = newJob;

            string filePath = GetAccountFilePatch(login, type);
            File.Delete(filePath);

            AddAccount(account);
        }

        public void RemoveAccount(string login) =>
            File.Delete(GetDismissedAccountFilePatch(login));

        public void RenameAccount(string oldLogin, string newLogin, AccountType type)
        {
            File.Move(GetAccountFilePatch(oldLogin, type), GetAccountFilePatch(newLogin, type));
        }

        public void AddUnregisteredAccount(UnregisteredAccount uAccount)
        {
            using (BinaryWriter writer = GetFileWriter(GetUnredisteredAccountFilePatch(uAccount.Login), FileMode.CreateNew))
                uAccount.Export(writer);
        }
        public void RemoveUnregisteredAccount(string login) =>
            File.Delete(GetUnredisteredAccountFilePatch(login));

        public Account LoadAccount(string login, AccountType type)
        {
            using (BinaryReader reader = GetAccountFileReader(login, type))
                switch ((AccountType)reader.ReadByte())
                {
                    case AccountType.Admin: return new Admin(reader);
                    case AccountType.HR: return new HR(reader);
                    case AccountType.WarehouseManager: return new WarehouseManager(reader);
                    case AccountType.Cashier: return new Cashier(reader);
                    case AccountType.Accountant: return new Accountant(reader);
                    case AccountType.Customer: return new Customer(reader);
                }
            throw new FormatException("Wrong account type");
        }
        public UnregisteredAccount LoadUnregisteredAccount(string login)
        {
            using (BinaryReader reader = GetFileReader(GetUnredisteredAccountFilePatch(login)))
                return new UnregisteredAccount(reader);
        }
        public Account LoadDismissedAccount(string login)
        {
            using (BinaryReader reader = GetFileReader(GetDismissedAccountFilePatch(login)))
                return LoadAccount(reader);
        }
        public Account LoadAccount(BinaryReader reader) => LoadAccount(reader, reader.ReadByte(), Helper.ExtractFileNameWithotExtension((reader.BaseStream as FileStream).Name), reader.ReadString());
        public Account LoadAccount(BinaryReader reader, byte type_, string login, string pass)
        {
            AccountType type = (AccountType)type_;
            switch (type)
            {
                case AccountType.Admin: return new Admin(login, pass);
                case AccountType.HR: return new HR(login, pass, reader);
                case AccountType.WarehouseManager: return new WarehouseManager(login, pass, reader);
                case AccountType.Cashier: return new Cashier(login, pass, reader);
                case AccountType.Accountant: return new Accountant(login, pass, reader);
                case AccountType.Customer: return new Customer(login, pass, reader);
            }
            throw new FormatException("Wrong account type");
        }

        public Account TryLogin(string login, string password, out string errorMsg)
        {
            bool accountExists = false;
            FileStream fileStream = null;
            foreach (string folderPath in AccountsFoldersList)
            {
                try
                {
                    fileStream = File.Open($@"{folderPath}\{login}.dat", FileMode.Open);
                    accountExists = true;
                }
                catch (FileNotFoundException) { continue; }
                catch (Exception ex)
                {
                    errorMsg = $"Неизвестная ошибка: {ex.Message}";
                    return null;
                }

            }
            if (!accountExists)
            {
                errorMsg = "Аккаунта не существует";
                return null;
            }

            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                byte type = reader.ReadByte();
                if (reader.ReadString() != password)
                {
                    errorMsg = "Неверный пароль";
                    return null;
                }
                errorMsg = String.Empty;
                return LoadAccount(reader, type, login, password);
            }

        }
        public string[] GetAccountsList(AccountType type) =>
            Directory.EnumerateFiles(GetFolderPathForAccountType(type)).Select(fn => Helper.ExtractFileNameWithotExtension(fn)).ToArray();
        public string[] GetAccountsList() =>
            Directory.EnumerateFiles(AccountsFolderPath, "*.dat", SearchOption.AllDirectories).Select(fn => Helper.ExtractFileNameWithotExtension(fn)).ToArray();

        public string[] GetDismissedAccountsList() =>
            Directory.EnumerateFiles(Accounts_DismissedFolderPath, "*.dat").Select(fn => Helper.ExtractFileNameWithotExtension(fn)).ToArray();
        public string[] GetUnregisteredAccountsList() =>
            Directory.EnumerateFiles(Accounts_UnregisteredFolderPath, "*.dat").Select(fn => Helper.ExtractFileNameWithotExtension(fn)).ToArray();

        /// <returns>file path or null</returns>
        public string FindAccount(string login)
        {
            string[] files = Directory.GetFiles(AccountsFolderPath, login + ".dat", SearchOption.AllDirectories);
            if (files.Length == 0) return null;
            else return files[0];
        }

        public void AddWarehouse(string name) =>
            Directory.CreateDirectory(GetWarehousePatch(name));
        public void RemoveWarehouse(string name) =>
            Directory.Delete(GetWarehousePatch(name), true);
        public void RenameWarehouse(string name, string newName) =>
            Directory.Move(GetWarehousePatch(name), GetWarehousePatch(newName));

        public string[] GetWarehousesList() =>
            Directory.EnumerateDirectories(WarehousesFolderPath)
            .Select(p => p.Substring(p.LastIndexOf('\\') + 1))
            .ToArray();
        public bool WarehouseExists(string name) =>
            Directory.Exists(GetWarehousePatch(name));

        public string[] GetProductsList() =>
            new SortedSet<string>(
                Directory.EnumerateFiles(WarehousesFolderPath, "*.dat", SearchOption.AllDirectories)
                .Select(fn => Helper.ExtractFileNameWithotExtension(fn))
                ).ToArray();

        public string[] GetOrdersList() =>
            Directory.EnumerateFiles(OrdersFolderPath, "*.dat").Select(fn => Helper.ExtractFileNameWithotExtension(fn)).ToArray();
        public void SaveOrder(Order order) =>
            order.SaveToFile(GetOrderFilePatch(order.Id));
        public void RemoveOrder(string name) =>
            Directory.Delete(GetOrderFilePatch(name));
        public void CompleteOrder(string name) =>
            Directory.Move(GetOrderFilePatch(name), GetCompleteOrderFilePatch(name));
        public void CancelOrder(string name) =>
            Directory.Move(GetOrderFilePatch(name), GetCancelledOrderFilePatch(name));

        public string[] FindProductFilePaths(string name) =>
            Directory.GetFiles(WarehousesFolderPath, $"{name}.dat", SearchOption.AllDirectories);

        public void WriteCheque(string name, string text) =>
            File.WriteAllText(GetChequeFilePatch(name), text);
    }
}