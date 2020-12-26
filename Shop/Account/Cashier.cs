using System;
using System.IO;

namespace Shop
{
    public class Cashier : Employee, ICustomSerializable, IUI
    {

        public Cashier(string login, string password, string firstName, string lastName, string patronymic, DateTime birthday, string[] educations, ushort workExperience, string placeOfWork)
            : base(login, password, AccountType.Cashier, firstName, lastName, patronymic, birthday, educations, workExperience, placeOfWork) { }
        public Cashier(string login, string password, BinaryReader reader) : base(login, password, AccountType.Cashier, reader) { }
        public Cashier(BinaryReader reader) : base(AccountType.Cashier, reader) { }
        public Cashier() : base(AccountType.Cashier) { }

        //public override void Export(BinaryWriter writer) => base.Export(writer);

        public void UI(Settings settings)
        {
            new CashierUI(this, settings);
        }
    }
}
