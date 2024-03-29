﻿using System;
using System.Diagnostics;

namespace PSDFile
{
    [DebuggerDisplay("Layer Info: { key }")]
    public class RawLayerInfo : LayerInfo
    {
        private string signature;
        public override string Signature => signature;

        private string key;
        public override string Key => key;

        public byte[] Data { get; private set; }

        public RawLayerInfo(string key, string signature = "8BIM")
        {
            this.signature = signature;
            this.key = key;
        }

        public RawLayerInfo(PsdBinaryReader reader, string signature, string key,
            long dataLength)
        {
            this.signature = signature;
            this.key = key;

            Util.CheckByteArrayLength(dataLength);
            Data = reader.ReadBytes((int)dataLength);
        }

        protected override void WriteData(PsdBinaryWriter writer)
        {
            writer.Write(Data);
        }
    }
}
