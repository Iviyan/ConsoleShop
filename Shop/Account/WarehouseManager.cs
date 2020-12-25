using System;
using System.IO;

namespace Shop
{
    public class WarehouseManager : Employee, ICustomSerializable, IUI
    {

        public WarehouseManager(string login, string password, string firstName, string lastName, string patronymic, DateTime birthday, string[] educations, ushort workExperience, string placeOfWork)
            : base(login, password, AccountType.WarehouseManager, firstName, lastName, patronymic, birthday, educations, workExperience, placeOfWork) { }
        public WarehouseManager(string login, string password, BinaryReader reader) : base(login, password, AccountType.WarehouseManager, reader) { }
        public WarehouseManager(BinaryReader reader) : base(AccountType.WarehouseManager, reader) { }
        public WarehouseManager() : base(AccountType.WarehouseManager) { }

        //public override void Export(BinaryWriter writer) => base.Export(writer);

        public void UI(Settings settings)
        {
           new WarehouseManagerUI(this, settings);
        }
    }
}
