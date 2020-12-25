using System;
using System.IO;

namespace Shop
{
    public abstract class Employee : Account
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public DateTime Birthday { get; set; }
        public string[] Educations { get; set; }
        /// <summary>In months</summary>
        public ushort WorkExperience { get; set; }
        public string PlaceOfWork { get; set; }
        public Employee(string login, string password, AccountType type, string firstName, string lastName, string patronymic, DateTime birthday, string[] educations, ushort workExperience, string placeOfWork) : base(login, password, type)
        {
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
            Birthday = birthday;
            Educations = educations;
            WorkExperience = workExperience;
            PlaceOfWork = placeOfWork;
        }
        public Employee(string login, string password, AccountType type, BinaryReader reader) : base(login, password, type)
        {
            Load(reader);
        }
        public Employee(AccountType type, BinaryReader reader) : base(reader, type)
        {
            Load(reader);
        }
        public Employee(AccountType type) : base(type) { }
        void Load(BinaryReader reader)
        {
            FirstName = reader.ReadString();
            LastName = reader.ReadString();
            Patronymic = reader.ReadString();
            Birthday = reader.ReadDateTime();
            Educations = reader.ReadStringArray();
            WorkExperience = reader.ReadUInt16();
            PlaceOfWork = reader.ReadString();
        }

        public override void Export(BinaryWriter writer)
        {
            base.Export(writer);
            writer.Write(FirstName);
            writer.Write(LastName);
            writer.Write(Patronymic);
            writer.Write(Birthday);
            writer.Write(Educations);
            writer.Write(WorkExperience);
            writer.Write(PlaceOfWork);
        }
    }
}
