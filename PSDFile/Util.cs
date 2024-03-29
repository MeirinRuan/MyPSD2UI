﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace PSDFile
{
    public static class Util
    {
        public const string LayerGroupDivider = "</Layer group>";

        [DebuggerDisplay("Top = {Top}, Bottom = {Bottom}, Left = {Left}, Right = {Right}")]
        public struct RectanglePosition
        {
            public int Top { get; set; }
            public int Bottom { get; set; }
            public int Left { get; set; }
            public int Right { get; set; }
        }

        public static Rectangle IntersectWith(this Rectangle thisRect, Rectangle rect)
        {
            thisRect.Intersect(rect);
            return thisRect;
        }

        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.Width / 2 + rect.X, rect.Height / 2 + rect.Y);

        }
        public static void AlignCenter(this Rectangle rect, Rectangle original)
        {
            var r1c = rect.Center();
            var r2c = original.Center();
            rect.Offset(r2c.X - r1c.X, r2c.Y - r1c.Y);
        }

        public static Layer MakeImageLayer(this PsdFile psd, Bitmap bmp, string name, int x = 0, int y = 0)
        {
            Layer psdLayer = new Layer(psd);
            // Set layer metadata
            psdLayer.Name = name;
            psdLayer.Rect = new Rectangle(new Point(x, y), bmp.Size);
            psdLayer.BlendModeKey = PsdBlendMode.Normal;
            psdLayer.Opacity = 255;
            psdLayer.Visible = true;
            psdLayer.Masks = new MaskInfo();
            psdLayer.BlendingRangesData = new BlendingRanges(psdLayer);
            psdLayer.SetBitmap(bmp, ImageReplaceOption.KeepCenter, psd.ImageCompression);
            return psdLayer;
        }

        public static Layer MakeSectionLayers(this PsdFile psd, string name, out Layer dividerLayer, bool isOpen = false)
        {
            Layer headLayer = new Layer(psd)
            {
                Name = name,
                Rect = new Rectangle(),
                BlendModeKey = PsdBlendMode.Normal,
                Opacity = 255,
                Visible = true,
                Masks = new MaskInfo()
            };
            // Set layer metadata
            headLayer.BlendingRangesData = new BlendingRanges(headLayer);
            headLayer.AdditionalInfo.Add(new LayerSectionInfo()
            {
                SectionType = isOpen ? LayerSectionType.OpenFolder : LayerSectionType.ClosedFolder
            });

            dividerLayer = new Layer(psd)
            {
                Name = LayerGroupDivider,
                Rect = new Rectangle(),
                BlendModeKey = PsdBlendMode.Normal,
                Opacity = 255,
                Visible = true,
                Masks = new MaskInfo(),
                BlendingRangesData = new BlendingRanges(headLayer)
            };
            // Set layer metadata
            dividerLayer.AdditionalInfo.Add(new LayerSectionInfo()
            {
                SectionType = LayerSectionType.SectionDivider
            });

            return headLayer;
        }

        /////////////////////////////////////////////////////////////////////////// 

        /// <summary>
        /// Fills a buffer with a byte value.
        /// </summary>
        unsafe static public void Fill(byte* ptr, byte* ptrEnd, byte value)
        {
            while (ptr < ptrEnd)
            {
                *ptr = value;
                ptr++;
            }
        }

        unsafe static public void Invert(byte* ptr, byte* ptrEnd)
        {
            while (ptr < ptrEnd)
            {
                *ptr = (byte)(*ptr ^ 0xff);
                ptr++;
            }
        }

        /// <summary>
        /// Reverses the endianness of a 2-byte word.
        /// </summary>
        unsafe static public void SwapBytes2(byte* ptr)
        {
            byte byte0 = *ptr;
            *ptr = *(ptr + 1);
            *(ptr + 1) = byte0;
        }

        /// <summary>
        /// Reverses the endianness of a 4-byte word.
        /// </summary>
        unsafe static public void SwapBytes4(byte* ptr)
        {
            byte byte0 = *ptr;
            byte byte1 = *(ptr + 1);

            *ptr = *(ptr + 3);
            *(ptr + 1) = *(ptr + 2);
            *(ptr + 2) = byte1;
            *(ptr + 3) = byte0;
        }

        /// <summary>
        /// Reverses the endianness of a word of arbitrary length.
        /// </summary>
        unsafe static public void SwapBytes(byte* ptr, int nLength)
        {
            for (long i = 0; i < nLength / 2; ++i)
            {
                byte t = *(ptr + i);
                *(ptr + i) = *(ptr + nLength - i - 1);
                *(ptr + nLength - i - 1) = t;
            }
        }

        public static void SwapByteArray(int bitDepth, byte[] byteArray, int startIndex, int count)
        {
            switch (bitDepth)
            {
                case 1:
                case 8:
                    break;
                case 16:
                    SwapByteArray2(byteArray, startIndex, count);
                    break;
                case 32:
                    SwapByteArray4(byteArray, startIndex, count);
                    break;
               default:
                    throw new Exception("Byte-swapping implemented only for 16 and 32 bit depths.");
            }
        }

        /// <summary>
        /// Reverses the endianness of 2-byte words in a byte array.
        /// </summary>
        /// <param name="byteArray">Byte array containing the sequence on which to swap endianness</param>
        /// <param name="startIndex">Byte index of the first word to swap</param>
        /// <param name="count">Number of words to swap</param>
        public static void SwapByteArray2(byte[] byteArray, int startIndex, int count)
        {
            int endIdx = startIndex + count * 2;
            if (byteArray.Length < endIdx)
                throw new IndexOutOfRangeException();

            unsafe
            {
                fixed (byte* arrayPtr = &byteArray[0])
                {
                    byte* ptr = arrayPtr + startIndex;
                    byte* endPtr = arrayPtr + endIdx;
                    while (ptr < endPtr)
                    {
                        SwapBytes2(ptr);
                        ptr += 2;
                    }
                }
            }
        }

        /// <summary>
        /// Reverses the endianness of 4-byte words in a byte array.
        /// </summary>
        /// <param name="byteArray">Byte array containing the sequence on which to swap endianness</param>
        /// <param name="startIndex">Byte index of the first word to swap</param>
        /// <param name="count">Number of words to swap</param>
        public static void SwapByteArray4(byte[] byteArray, int startIndex, int count)
        {
            int endIdx = startIndex + count * 4;
            if (byteArray.Length < endIdx)
                throw new IndexOutOfRangeException();

            unsafe
            {
                fixed (byte* arrayPtr = &byteArray[0])
                {
                    byte* ptr = arrayPtr + startIndex;
                    byte* endPtr = arrayPtr + endIdx;
                    while (ptr < endPtr)
                    {
                        SwapBytes4(ptr);
                        ptr += 4;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the number of bytes required to store a row of an image
        /// with the specified bit depth.
        /// </summary>
        /// <param name="size">The size of the image in pixels.</param>
        /// <param name="bitDepth">The bit depth of the image.</param>
        /// <returns>The number of bytes needed to store a row of the image.</returns>
        public static int BytesPerRow(Size size, int bitDepth)
        {
            switch (bitDepth)
            {
                case 1:
                    return (size.Width + 7) / 8;
                default:
                    return size.Width * BytesFromBitDepth(bitDepth);
            }
        }

        /// <summary>
        /// Round the integer to a multiple.
        /// </summary>
        public static int RoundUp(int value, int multiple)
        {
            if (value == 0)
                return 0;

            if (Math.Sign(value) != Math.Sign(multiple))
            {
                throw new ArgumentException(
                  $"{nameof(value)} and {nameof(multiple)} cannot have opposite signs.");
            }

            var remainder = value % multiple;
            if (remainder > 0)
            {
                value += (multiple - remainder);
            }
            return value;
        }

        /// <summary>
        /// Get number of bytes required to pad to the specified multiple.
        /// </summary>
        public static int GetPadding(int length, int padMultiple)
        {
            if ((length < 0) || (padMultiple < 0))
                throw new ArgumentException();

            var remainder = length % padMultiple;
            if (remainder == 0)
                return 0;

            var padding = padMultiple - remainder;
            return padding;
        }

        /// <summary>
        /// Returns the number of bytes needed to store a single pixel of the
        /// specified bit depth.
        /// </summary>
        public static int BytesFromBitDepth(int depth)
        {
            switch (depth)
            {
                case 1:
                case 8:
                    return 1;
                case 16:
                    return 2;
                case 32:
                    return 4;
                default:
                    throw new ArgumentException("Invalid bit depth.");
            }
        }

        public static short MinChannelCount(this PsdColorMode colorMode)
        {
            switch (colorMode)
            {
                case PsdColorMode.Bitmap:
                case PsdColorMode.Duotone:
                case PsdColorMode.Grayscale:
                case PsdColorMode.Indexed:
                case PsdColorMode.Multichannel:
                    return 1;
                case PsdColorMode.Lab:
                case PsdColorMode.RGB:
                    return 3;
                case PsdColorMode.CMYK:
                    return 4;
            }

            throw new ArgumentException("Unknown color mode.");
        }

        /// <summary>
        /// Verify that the offset and count will remain within the bounds of the
        /// buffer.
        /// </summary>
        /// <returns>True if in bounds, false if out of bounds.</returns>
        public static bool CheckBufferBounds(byte[] data, int offset, int count)
        {
            if (offset < 0)
                return false;
            if (count < 0)
                return false;
            if (offset + count > data.Length)
                return false;

            return true;
        }

        public static void CheckByteArrayLength(long length)
        {
            if (length < 0)
            {
                throw new Exception("Byte array cannot have a negative length.");
            }
            if (length > 0x7fffffc7)
            {
                throw new OutOfMemoryException(
                  "Byte array cannot exceed 2,147,483,591 in length.");
            }
        }

        /// <summary>
        /// Writes a message to the debug console, indicating the current position
        /// in the stream in both decimal and hexadecimal formats.
        /// </summary>
        [Conditional("DEBUG")]
        public static void DebugMessage(Stream stream, string message)
        {
            Debug.WriteLine($"0x{stream.Position:x}, {stream.Position}, {message}");
        }
    }

    /// <summary>
    /// Fixed-point decimal, with 16-bit integer and 16-bit fraction.
    /// </summary>
    public class UFixed16_16
    {
        public UInt16 Integer { get; set; }
        public UInt16 Fraction { get; set; }

        public UFixed16_16(UInt16 integer, UInt16 fraction)
        {
            Integer = integer;
            Fraction = fraction;
        }

        /// <summary>
        /// Split the high and low words of a 32-bit unsigned integer into a
        /// fixed-point number.
        /// </summary>
        public UFixed16_16(UInt32 value)
        {
            Integer = (UInt16)(value >> 16);
            Fraction = (UInt16)(value & 0x0000ffff);
        }

        public UFixed16_16(double value)
        {
            if (value >= 65536.0) throw new OverflowException();
            if (value < 0) throw new OverflowException();

            Integer = (UInt16)value;

            // Round instead of truncate, because doubles may not represent the
            // fraction exactly.
            Fraction = (UInt16)((value - Integer) * 65536 + 0.5);
        }

        public static implicit operator double(UFixed16_16 value)
        {
            return (double)value.Integer + value.Fraction / 65536.0;
        }

    }

}
