using System;
using System.Collections.Generic;
using System.IO;

namespace LogDataInterface
{
    public class ChannelData
    {
        private const double NullValue = -999.25;

        public double[] ElapsedTimes { get; set; }
        public double[] Values { get; set; }

        public ChannelData()
        { }

        public ChannelData(double[] offsets, double[] values)
        {
            if (offsets == null || values == null) return;

            List<double> selectedTimes = new List<double>(offsets.Length);
            List<double> selectedValues = new List<double>(values.Length);

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == NullValue || double.IsNaN(values[i])) continue;
                selectedTimes.Add(offsets[i]);
                selectedValues.Add(values[i]);
            }

            ElapsedTimes = selectedTimes.ToArray();
            Values = selectedValues.ToArray();
        }

        public override string ToString()
        {
            return "Num Values = " + ((ElapsedTimes == null) ? "null" : (ElapsedTimes.Length.ToString()));
        }

        public byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter binaryWriter = new BinaryWriter(ms);

                binaryWriter.Write(ElapsedTimes.Length);

                byte[] byteValues = new byte[ElapsedTimes.Length * sizeof(double)];
                Buffer.BlockCopy(ElapsedTimes, 0, byteValues, 0, ElapsedTimes.Length * sizeof(double));
                binaryWriter.Write(byteValues);

                binaryWriter.Write(Values.Length);

                byteValues = new byte[Values.Length * sizeof(double)];
                Buffer.BlockCopy(Values, 0, byteValues, 0, Values.Length * sizeof(double));
                binaryWriter.Write(byteValues);

                return ms.GetBuffer();
            }
        }

        public static ChannelData FromBytes(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                ChannelData channelData = new ChannelData();
                BinaryReader binaryReader = new BinaryReader(ms);

                int numTimes = binaryReader.ReadInt32();

                byte[] byteValues = new byte[numTimes * sizeof(double)];
                binaryReader.Read(byteValues, 0, numTimes * sizeof(double));
                channelData.ElapsedTimes = new double[numTimes];
                Buffer.BlockCopy(byteValues, 0, channelData.ElapsedTimes, 0, numTimes * sizeof(double));

                int numValues = binaryReader.ReadInt32();

                byteValues = new byte[numValues * sizeof(double)];
                binaryReader.Read(byteValues, 0, numValues * sizeof(double));
                channelData.Values = new double[numValues];
                Buffer.BlockCopy(byteValues, 0, channelData.Values, 0, numValues * sizeof(double));

                return channelData;
            }

        }
    }
}
