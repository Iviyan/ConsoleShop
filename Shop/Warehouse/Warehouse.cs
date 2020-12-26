using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shop
{
    public class Warehouse
    {
        private string path;
        public string Path
        {
            get => path;
            set
            {
                path = value;
                Name = path.Substring(path.LastIndexOf('\\') + 1);
            }
        }
        public string Name { get; private set; }
        public List<string> Products { get; set; }
        public Warehouse(string folder)
        {
            Path = folder;
            UpdateProducts();
        }
        public void AddProduct(Product product)
        {
            Products.Add(product.Name);
            product.SaveToFile(GetProductFilePath(product.Name));
        }
        public void UpdateProducts() => Products = Directory.EnumerateFiles(Path, "*.dat").Select(p => Helper.ExtractFileNameWithotExtension(p)).ToList();
        public string GetProductFilePath(string name) => $@"{Path}\{name}.dat";
        public Product GetProduct(string name)
            => new Product(GetProductFilePath(name));
        public void RemoveProduct(string name)
        {
            if (Products.Remove(name))
                File.Delete(GetProductFilePath(name));
        }
        public bool RenameProduct(string name, string newName)
        {
            if (Products.Contains(newName)) return false;
            Products[Products.IndexOf(name)] = newName;
            File.Move(GetProductFilePath(name), GetProductFilePath(newName));
            return true;
        }
    }
}
