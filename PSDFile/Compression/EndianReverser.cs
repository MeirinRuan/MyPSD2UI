﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSDFile.Compression
{
    public class EndianReverser : ImageData
    {
        private ImageData imageData;

        protected override bool AltersWrittenData => true;

        public EndianReverser(ImageData imageData) : base(imageData.Size, imageData.BitDepth)
        {
            this.imageData = imageData;
        }

        internal override void Read(byte[] buffer)
        {
            imageData.Read(buffer);

            var numPixels = Size.Width * Size.Height;
            if (numPixels == 0)
            {
                return;
            }
            Util.SwapByteArray(BitDepth, buffer, 0, numPixels);
        }

        public override Byte[] ReadCompressed()
        {
            return imageData.ReadCompressed();
        }

        internal override void WriteInternal(byte[] array)
        {
            // Reverse endianness before passing on to underlying compressor
            if (array.Length > 0)
            {
                var numPixels = array.Length / BytesPerRow * Size.Width;
                Util.SwapByteArray(BitDepth, array, 0, numPixels);
            }

            imageData.Write(array);
        }
    }
}
