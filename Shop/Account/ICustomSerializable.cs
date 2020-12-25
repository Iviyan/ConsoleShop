using System.IO;

namespace Shop
{
    interface ICustomSerializable
    {
        void Export(BinaryWriter writer);
    }
}
