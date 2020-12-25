using System.IO;

namespace Shop
{
    public abstract class Account : ICustomSerializable
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public enum AccountType : byte
        {
            Admin = 1,
            HR,
            WarehouseManager,
            Cashier,
            Accountant,
            Customer
        }
        public AccountType Type { get; set; }

        public Account(string login, string password, AccountType type)
        {
            Login = login;
            Password = password;
            Type = type;
        }

        public Account(BinaryReader reader, AccountType type)
        {
            string filePath = (reader.BaseStream as FileStream).Name;
            Login = Helper.ExtractFileNameWithotExtension(filePath);

            Password = reader.ReadString();
            Type = type;
        }

        public Account(AccountType type) => Type = type;

        public virtual void Export(BinaryWriter writer)
        {
            writer.Write((byte)Type);
            writer.Write(Password);
        }
    }
}
