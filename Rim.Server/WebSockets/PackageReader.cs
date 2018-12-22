using System;

namespace Rim.Server.WebSockets
{
    public enum SocketOpCode : byte
    {
        Continue = 0x00,
        UTF8 = 0x01,
        Binary = 0x02,
        Terminate = 0x08,
        Ping = 0x09,
        Pong = 0x0A
    }

    public enum SocketLength : byte
    {
        Shorter126,
        Int16,
        Int64
    }

    public enum ReadingStatus : byte
    {
        OpCode,
        MaskingAndLength,
        Length,
        Key,
        Body
    }

    public class PackageReader
    {

        #region Properties

        public bool Fin { get; private set; }
        public SocketOpCode OpCode { get; private set; }
        public SocketLength LengthType { get; private set; }
        private byte[] LengthBytes { get; set; }
        private int LengthBytesRead { get; set; }

        public bool Masking { get; private set; }
        public int PayloadLength { get; private set; }
        private int BodyReadIndex { get; set; }

        private byte[] Key { get; set; }
        private int KeyBytesRead { get; set; }

        public byte[] Payload { get; set; }

        public ReadingStatus Status { get; private set; } = ReadingStatus.OpCode;

        public bool IsReady { get; private set; }

        #endregion


        #region Read 

        public int Read(byte[] buffer, int offset, int count)
        {
            int read = offset;
            while (read < count)
            {
                if (Status == ReadingStatus.Body)
                {
                    byte[] readbuf = new byte[count - read];
                    Array.Copy(buffer, read, readbuf, 0, readbuf.Length);
                    read += ReadBody(readbuf);
                    return read - offset;
                }

                ReadHeader(buffer[read]);
                read++;
                if (read >= count)
                    return read - offset;

            }

            return read - offset;
        }

        public bool ReadHeader(byte data)
        {
            if (Status == ReadingStatus.OpCode)
                return ReadFinAndOpCode(data);

            if (Status == ReadingStatus.MaskingAndLength)
                return ReadMaskingAndLength(data);

            if (Status == ReadingStatus.Length)
                return ReadLength(data);

            if (Status == ReadingStatus.Key)
                return ReadMaskingKey(data);

            return false;
        }

        public int ReadBody(byte[] data)
        {
            if (Payload == null)
            {
                Payload = new byte[PayloadLength];
                BodyReadIndex = 0;
            }

            int left = Payload.Length - BodyReadIndex;
            int read = data.Length > left ? left : data.Length;

            Array.Copy(data, 0, Payload, BodyReadIndex, read);
            BodyReadIndex += read;

            if (BodyReadIndex >= Payload.Length)
            {
                if (Masking)
                {
                    for (int i = 0; i < Payload.Length; i++)
                        Payload[i] = (byte)(Payload[i] ^ Key[i % 4]);
                }

                IsReady = true;
            }

            return read;
        }

        #endregion

        #region Header

        private bool ReadFinAndOpCode(byte data)
        {
            byte opcode = data;
            if (opcode > 128)
            {
                Fin = true;
                opcode -= 128;
            }
            else
                Fin = false;

            if (opcode == 0x00 || opcode == 0x01 || opcode == 0x02 || opcode == 0x08 || opcode == 0x09 || opcode == 0x0A)
                OpCode = (SocketOpCode)opcode;

            Status = ReadingStatus.MaskingAndLength;
            return true;
        }

        private bool ReadMaskingAndLength(byte data)
        {
            int plen = data;
            if (plen > 127)
            {
                Masking = true;
                Key = new byte[4];
                plen -= 128;
            }
            else
                Masking = false;

            if (plen < 126)
            {
                PayloadLength = plen;
                LengthType = SocketLength.Shorter126;

                Status = !Masking ? ReadingStatus.Body : ReadingStatus.Key;

                if (Status != ReadingStatus.Key && OpCode != SocketOpCode.UTF8 && OpCode != SocketOpCode.Binary && OpCode != SocketOpCode.Continue)
                    IsReady = true;
            }
            else if (plen == 126)
            {
                LengthType = SocketLength.Int16;
                Status = ReadingStatus.Length;
                LengthBytes = new byte[2];
                LengthBytesRead = 0;
            }
            else if (plen == 127)
            {
                LengthType = SocketLength.Int64;
                Status = Masking ? ReadingStatus.MaskingAndLength : ReadingStatus.Length;
                LengthBytes = new byte[8];
                LengthBytesRead = 0;
            }

            return true;
        }

        private bool ReadLength(byte data)
        {
            LengthBytes[LengthBytes.Length - LengthBytesRead - 1] = data;
            LengthBytesRead++;
            if (LengthBytesRead >= LengthBytes.Length)
            {
                if (LengthType == SocketLength.Int16)
                    PayloadLength = BitConverter.ToUInt16(LengthBytes, 0);
                else if (LengthType == SocketLength.Int64)
                    PayloadLength = (int)BitConverter.ToUInt64(LengthBytes, 0);

                Status = Masking ? ReadingStatus.Key : ReadingStatus.Body;
            }

            return true;
        }

        private bool ReadMaskingKey(byte data)
        {
            Key[KeyBytesRead] = data;
            KeyBytesRead++;

            if (KeyBytesRead == 4)
            {
                Status = ReadingStatus.Body;
                if (OpCode != SocketOpCode.UTF8 && OpCode != SocketOpCode.Binary && OpCode != SocketOpCode.Continue)
                    IsReady = true;
            }

            return false;
        }

        #endregion

    }

}
