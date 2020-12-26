using System.IO;

namespace Shop
{
    public class Customer : Account, ICustomSerializable, IUI
    {
        public string Email { get; set; }
        public Customer(string login, string password, string email) : base(login, password, AccountType.Customer)
        {
            Email = email;
        }
        public Customer(string login, string password, BinaryReader reader) : base(login, password, AccountType.Customer)
        {
            Email = reader.ReadString();
        }
        public Customer(BinaryReader reader) : base(reader, AccountType.Customer)
        {
            Email = reader.ReadString();
        }
        public Customer() : base(AccountType.Customer) { }

        public override void Export(BinaryWriter writer)
        {
            base.Export(writer);
            writer.Write(Email);
        }
        public void UI(Settings settings)
        {
            new CustomerUI(this, settings);
        }
    }
}