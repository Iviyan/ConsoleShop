using System;
using System.IO;

namespace Shop
{
    public class Accountant : Employee, ICustomSerializable, IUI
    {

        public Accountant(string login, string password, string firstName, string lastName, string patronymic, DateTime birthday, string[] educations, ushort workExperience, string placeOfWork)
            : base(login, password, AccountType.Accountant, firstName, lastName, patronymic, birthday, educations, workExperience, placeOfWork) { }
        public Accountant(string login, string password, BinaryReader reader) : base(login, password, AccountType.Accountant, reader) { }
        public Accountant(BinaryReader reader) : base(AccountType.Accountant, reader) { }
        public Accountant() : base(AccountType.Accountant) { }

        //public override void Export(BinaryWriter writer) => base.Export(writer);

        public void UI(Settings settings)
        {
            //new AccountantUI(this, settings);
        }
    }
}
