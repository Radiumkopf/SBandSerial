using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBandSerialReader
{
	class TlmPacket
	{
		public const int SIZE = 17;

		public float Temperature1 { get; private set; }
		public float Temperature2 { get; private set; }
		public float BatteryV { get; private set; }

		public float[] PvPower { get; private set; } = new float[3];

		public float AngularRate { get; private set; }
		public float MagFieldAbs { get; private set; }

		public float BatChargePower { get; private set; }
		public float BatDischargePower { get; private set; }

		public byte ResetCounter { get; private set; }
		public uint StatusFlags { get; private set; }

		public static TlmPacket Parse(byte[] data)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			if (data.Length < SIZE)
				throw new ArgumentException($"Data length must be at least {SIZE} bytes");

			var packet = new TlmPacket();

			int offset = 0;

			packet.Temperature1 = Decode8(data[offset++], -40.0f, 85.0f);
			packet.Temperature2 = Decode8(data[offset++], -40.0f, 85.0f);
			packet.BatteryV = Decode8(data[offset++], 2.0f, 5.0f);

			for (int i = 0; i < 3; i++)
				packet.PvPower[i] = Decode8(data[offset++], 0.0f, 5000.0f);

			packet.AngularRate = Decode16(ReadUInt16BE(data, ref offset), 0.0f, 500.0f);
			packet.MagFieldAbs = Decode16(ReadUInt16BE(data, ref offset), 0.0f, 1000.0f);

			packet.BatChargePower = Decode8(data[offset++], 0.0f, 5000.0f);
			packet.BatDischargePower = Decode8(data[offset++], 0.0f, 5000.0f);

			packet.ResetCounter = data[offset++];

			packet.StatusFlags = ReadUInt32BE(data, ref offset);

			return packet;
		}

        public byte[] ToBytes()
        {
            byte[] buffer = new byte[40];

            int offset = 0;

            WriteFloat(buffer, ref offset, Temperature1);
            WriteFloat(buffer, ref offset, Temperature2);
            WriteFloat(buffer, ref offset, BatteryV);

            WriteFloat(buffer, ref offset, PvPower[0]);
            WriteFloat(buffer, ref offset, PvPower[1]);
            WriteFloat(buffer, ref offset, PvPower[2]);

            WriteFloat(buffer, ref offset, AngularRate);
            WriteFloat(buffer, ref offset, MagFieldAbs);

            WriteFloat(buffer, ref offset, BatChargePower);
            WriteFloat(buffer, ref offset, BatDischargePower);

            return buffer;
        }


        private static void WriteFloat(byte[] buffer, ref int offset, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            buffer[offset++] = bytes[0];
            buffer[offset++] = bytes[1];
            buffer[offset++] = bytes[2];
            buffer[offset++] = bytes[3];
        }

        // =========================
        // Decode (обратный encode)
        // =========================

        private static float Decode8(byte v, float min, float max)
		{
			float norm = v / 255.0f;
			return norm * (max - min) + min;
		}

		private static float Decode16(ushort v, float min, float max)
		{
			float norm = v / 65535.0f;
			return norm * (max - min) + min;
		}

		// =========================
		// Big-endian чтение
		// =========================

		private static ushort ReadUInt16BE(byte[] data, ref int offset)
		{
			ushort value = (ushort)((data[offset] << 8) | data[offset + 1]);
			offset += 2;
			return value;
		}

		private static uint ReadUInt32BE(byte[] data, ref int offset)
		{
			uint value =
				((uint)data[offset] << 24) |
				((uint)data[offset + 1] << 16) |
				((uint)data[offset + 2] << 8) |
				data[offset + 3];

			offset += 4;
			return value;
		}
	}
}
