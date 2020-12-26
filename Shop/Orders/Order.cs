using System;
using System.Collections.Generic;
using System.IO;

namespace Shop
{
    public class Order : ICustomSerializable
    {
        public string Id { get; set; }
        public string CustomerLogin { get; set; }
        public DateTime Date { get; set; }
        public IList<(string product, int count, int cost)> Items { get; set; }
        //Cost в будущем убрать

        public Order(string id, string customerLogin, DateTime date, IList<(string product, int count, int cost)> items)
        {
            Id = id;
            CustomerLogin = customerLogin;
            Date = date;
            Items = items;

        }
        public Order(BinaryReader reader)
        {
            Load(reader);
        }
        public Order(string filePath)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
                Load(reader);
        }
        void Load(BinaryReader reader)
        {
            Id = Helper.ExtractFileNameWithotExtension((reader.BaseStream as FileStream).Name);
            CustomerLogin = reader.ReadString();
            Date = reader.ReadDateTime();
            int itemsInOrder = reader.ReadInt32();
            Items = new (string product, int count, int cost)[itemsInOrder];
            for (int i = 0; i < itemsInOrder; i++)
                Items[i] = (reader.ReadString(), reader.ReadInt32(), reader.ReadInt32());
            

        }
        public Order() { }

        public void Export(BinaryWriter writer)
        {
            writer.Write(CustomerLogin);
            writer.Write(Date);
            writer.Write(Items.Count);
            foreach ((string product, int count, int cost) in Items)
            {
                writer.Write(product);
                writer.Write(count);
                writer.Write(cost);
            }
        }

        public void SaveToFile(string filePath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
                Export(writer);
        }
    }
}
