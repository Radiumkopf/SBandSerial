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
        Data = 0,
        Ack = 1
    }
    
    internal class SatelliteTCPPacket
    {

        public char symbol { get; set; }
        public PacketType Type { get; set; }
        public byte id { get; set; }
        public short number { get; set; }
        public byte size { get; set; }
        public byte[] data { get; set; }

        public SatelliteTCPPacket()
        {

        }
        public SatelliteTCPPacket(char _symbol, byte _id, short _number, byte _size, byte[] _data)
        {
            this.Type = PacketType.Data;
            this.symbol = _symbol;
            this.id = _id;
            this.number = _number;
            this.size = _size;
            this.data = _data;

        }

        public byte[] ToByteArray()
        {
            List<byte> fullPackage = new List<byte>();


            fullPackage.Add((byte)symbol);
            fullPackage.Add((byte)Type);
            fullPackage.Add(id);
            fullPackage.AddRange(BitConverter.GetBytes(number));
            fullPackage.Add(size);
            fullPackage.AddRange(data);

            return fullPackage.ToArray();
        }
    }
    
}
