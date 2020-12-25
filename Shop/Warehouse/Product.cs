using System;
using System.IO;

namespace Shop
{
    public class Product : ICustomSerializable
    {
        public string Name { get; set; }
        public int Cost { get; set; }
        public int Count { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Category { get; set; }

        public Product(string name, int count, DateTime expirationDate, string category, int cost)
        {
            Name = name;
            Cost = cost;
            Count = count;
            ExpirationDate = expirationDate;
            Category = category;
            
        }
        public Product(BinaryReader reader)
        {
            Load(reader);
        }
        public Product(string filePath)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
                Load(reader);
        }
        void Load(BinaryReader reader)
        {
            Name = reader.ReadString();
            Cost = reader.ReadInt32();
            Count = reader.ReadInt32();
            ExpirationDate = reader.ReadDateTime();
            Category = reader.ReadString();
        }
        public Product() { }

        public void Export(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Cost);
            writer.Write(Count);
            writer.Write(ExpirationDate);
            writer.Write(Category);
        }

        public void SaveToFile(string filePath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
                Export(writer);
        }
    }
}
