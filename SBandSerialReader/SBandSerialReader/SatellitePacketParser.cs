using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBandSerialReader
{
    internal static class SatellitePacketParser
    {

        public static FileTransferPacket Parse(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            try
            {
                int index = 0;

                if (bytes.Length < 6)
                    throw new InvalidDataException("Пакет слишком короткий.");

                //if (bytes[index++] != (byte)'#')
                //    throw new InvalidDataException("Неверный стартовый символ.");

                FileTransferPacket packet = new FileTransferPacket();

                packet.Type = (PacketType)bytes[index++];
                packet.id = bytes[index++];
                packet.number = BitConverter.ToInt16(bytes, index);
                index += 2;

                byte size = bytes[index++];
                packet.size = size;

                if (bytes.Length != index + size)
                    throw new InvalidDataException("Размер пакета не соответствует заголовку.");

                packet.data = new byte[size];

                Array.Copy(bytes, index, packet.data, 0, size);

                return packet;
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Ошибка разбора пакета.", ex);
            }
        }
    }
}
