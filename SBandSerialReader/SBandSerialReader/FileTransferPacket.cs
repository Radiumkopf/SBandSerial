using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SBandSerialReader
{
    public enum PacketType : byte
    {
        TimeSet = 0x0A,
        FileSending = 0x0B,
        FileRequesting = 0x0C,
        B = 0x0D,

        TimeSetAck = 0x1A,
        FileSendingAck = 0x1B,
        FileRequestingAck = 0x1C,
        FileRequestingLast = 0x2C,
        AddressChanging = 0xAC
    }

    internal class FileTransferPacket
    {

        public PacketType Type { get; set; }
        public byte id { get; set; }
        public short number { get; set; }
        public byte size { get; set; }
        public byte[] data { get; set; }

        public FileTransferPacket()
        {

        }
        public FileTransferPacket(PacketType type, byte _id, short _number, byte _size, byte[] _data)
        {
            this.Type = type;

            this.id = _id;
            this.number = _number;
            this.size = _size;
            this.data = _data;

        }

        public byte[] ToByteArray()
        {
            List<byte> fullPackage = new List<byte>();


            fullPackage.Add((byte)Type);
            fullPackage.Add(id);
            fullPackage.AddRange(BitConverter.GetBytes(number));
            fullPackage.Add(size);
            fullPackage.AddRange(data);

            return fullPackage.ToArray();
        }

    }
}
