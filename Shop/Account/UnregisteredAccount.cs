using System.IO;

namespace Shop
{
    public struct UnregisteredAccount : ICustomSerializable
    {
        public string Login;
        public string Password;
        public string FirstName;
        public string LastName;
        public string Patronymic;


        public UnregisteredAccount(string login, string password, string firstName, string lastName, string patronymic)
        {
            Login = login;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
        }

        public UnregisteredAccount(BinaryReader reader)
        {
            string filePath = (reader.BaseStream as FileStream).Name;
            Login = Helper.ExtractFileNameWithotExtension(filePath);

            Password = reader.ReadString();
            FirstName = reader.ReadString();
            LastName = reader.ReadString();
            Patronymic = reader.ReadString();
        }

        public void Export(BinaryWriter writer)
        {
            writer.Write(Password);
            writer.Write(FirstName);
            writer.Write(LastName);
            writer.Write(Patronymic);
        }
    }
}
