using System;
using System.IO;

namespace Shop
{
    public class HR : Employee, ICustomSerializable, IUI
    {

        public HR(string login, string password, string firstName, string lastName, string patronymic, DateTime birthday, string[] educations, ushort workExperience, string placeOfWork)
            : base(login, password, AccountType.HR, firstName, lastName, patronymic, birthday, educations, workExperience, placeOfWork) { }
        public HR(string login, string password, BinaryReader reader) : base(login, password, AccountType.HR, reader) { }
        public HR(BinaryReader reader) : base(AccountType.HR, reader) { }
        public HR() : base(AccountType.HR) { }

        //public override void Export(BinaryWriter writer) => base.Export(writer);

        public void UI(Settings settings)
        {
            new HRUI(this, settings);
        }
    }
}
