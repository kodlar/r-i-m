using System;
using System.IO;
using System.Text;

namespace Rim.Server.WebSockets
{
    public class PackageWriter
    {

        public static byte[] CreateUTF8(string message)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteByte(0x81);
                byte[] msg = Encoding.UTF8.GetBytes(message);

                if (msg.Length < 126)
                    ms.WriteByte((byte)msg.Length);
                else if (msg.Length <= UInt16.MaxValue)
                {
                    ms.WriteByte(126);
                    ushort len = (ushort)msg.Length;
                    byte[] lenbytes = BitConverter.GetBytes(len);
                    ms.Write(new[] { lenbytes[1], lenbytes[0] }, 0, 2);
                }
                else
                {
                    ms.WriteByte(127);
                    ulong len = (ulong)msg.Length;
                    byte[] lb = BitConverter.GetBytes(len);
                    ms.Write(new[] { lb[7], lb[6], lb[5], lb[4], lb[3], lb[2], lb[1], lb[0] }, 0, 8);
                }

                ms.Write(msg, 0, msg.Length);
                return ms.ToArray();
            }

        }

        public static byte[] CreateBinary(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteByte(0x82);

                if (data.Length < 126)
                    ms.WriteByte((byte)data.Length);
                else
                {
                    ms.WriteByte(126);
                    short len = (short)data.Length;
                    byte[] lenbytes = BitConverter.GetBytes(len);
                    ms.Write(new[] { lenbytes[1], lenbytes[0] }, 0, 2);
                }

                ms.Write(data, 0, data.Length);
                return ms.ToArray();
            }
        }

        public static byte[] CreatePing()
        {
            byte[] result = { 0x89, 0x00 };
            return result;
        }

    }
}
