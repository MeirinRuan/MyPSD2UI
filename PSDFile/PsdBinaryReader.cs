﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSDFile
{
    public class PsdBinaryReader : IDisposable
    {
        private BinaryReader reader;
        private Encoding encoding;

        public Stream BaseStream => reader.BaseStream;

        public PsdBinaryReader(Stream stream, PsdBinaryReader reader)
            : this(stream, reader.encoding)
        {
        }

        public PsdBinaryReader(Stream stream, Encoding encoding)
        {
            this.encoding = encoding;

            // ReadPascalString and ReadUnicodeString handle encoding explicity.
            // BinaryReader.ReadString() is never called, so it is constructed with
            // ASCII encoding to make accidental usage obvious.
            reader = new BinaryReader(stream, Encoding.ASCII);
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            return reader.ReadBytes(count);
        }

        public bool ReadBoolean()
        {
            return reader.ReadBoolean();
        }

        public Int16 ReadInt16()
        {
            var val = reader.ReadInt16();
            unsafe
            {
                Util.SwapBytes((byte*)&val, 2);
            }
            return val;
        }

        public Int32 ReadInt32()
        {
            var val = reader.ReadInt32();
            unsafe
            {
                Util.SwapBytes((byte*)&val, 4);
            }
            return val;
        }

        public Int64 ReadInt64()
        {
            var val = reader.ReadInt64();
            unsafe
            {
                Util.SwapBytes((byte*)&val, 8);
            }
            return val;
        }

        public UInt16 ReadUInt16()
        {
            var val = reader.ReadUInt16();
            unsafe
            {
                Util.SwapBytes((byte*)&val, 2);
            }
            return val;
        }

        public UInt32 ReadUInt32()
        {
            var val = reader.ReadUInt32();
            unsafe
            {
                Util.SwapBytes((byte*)&val, 4);
            }
            return val;
        }

        public UInt64 ReadUInt64()
        {
            var val = reader.ReadUInt64();
            unsafe
            {
                Util.SwapBytes((byte*)&val, 8);
            }
            return val;
        }

        /// <summary>
        /// Read padding to get to the byte multiple for the block.
        /// </summary>
        /// <param name="startPosition">Starting position of the padded block.</param>
        /// <param name="padMultiple">Byte multiple that the block is padded to.</param>
        public void ReadPadding(long startPosition, int padMultiple)
        {
            // Pad to specified byte multiple
            var totalLength = reader.BaseStream.Position - startPosition;
            var padBytes = Util.GetPadding((int) totalLength, padMultiple);
            ReadBytes(padBytes);
        }

        public Rectangle ReadRectangle()
        {
            var rect = new Rectangle();
            rect.Y = ReadInt32();
            rect.X = ReadInt32();
            rect.Height = ReadInt32() - rect.Y;
            rect.Width = ReadInt32() - rect.X;
            return rect;
        }

        /// <summary>
        /// Read a fixed-length ASCII string.
        /// <returns></returns>
        public string ReadAsciiChars(int count)
        {
            var bytes = reader.ReadBytes(count);
            var s = Encoding.ASCII.GetString(bytes);
            return s;
        }

        /// <summary>
        /// Read a Pascal string using the specified encoding.
        /// </summary>
        /// <param name="padMultiple">Byte multiple that the Pascal string is padded to.</param>
        /// <returns></returns>
        public string ReadPascalString(int padMultiple)
        {
            var startPosition = reader.BaseStream.Position;

            byte stringLength = ReadByte();
            var bytes = ReadBytes(stringLength);
            ReadPadding(startPosition, padMultiple);

            // Default decoder uses best-fit fallback, so it will not throw any
            // exceptions if unknown characters are encountered.
            var str = encoding.GetString(bytes);
            return str;
        }

        public string ReadUnicodeString()
        {
            var numChars = ReadInt32();
            var length = 2 * numChars;
            var data = ReadBytes(length);
            var str = Encoding.BigEndianUnicode.GetString(data, 0, length);

            return str;
        }

        #region IDisposable

        private bool disposed = false;

        ~PsdBinaryReader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (disposed)
                return;

            if (disposing)
            {
                if (reader != null)
                {
                    // BinaryReader.Dispose() is protected.
                    reader.Close();
                    reader = null;
                }
            }

            disposed = true;
        }

        #endregion
    }
}
