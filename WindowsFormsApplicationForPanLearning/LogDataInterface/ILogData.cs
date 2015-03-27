using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace LogDataInterface
{
    public enum LogTypeEnum
    {
        TimeBased = 0, DepthBased = 1,
    }

    public struct ChannelStatistics
    {
        public double MinValue;
        public double MaxValue;

        public bool ContainsNulls;
        public bool NullChannel;
    }

    public interface ILogData
    {
        string LogStoreFolder { get; set; }
        string CurrentLogStoreFolder { get; set; }
        LogTypeEnum LogType { get; set; }
        DateTimeOffset[] TimeBasedIndexChannel { get; }
        double[] OffsetChannel { get; }
        double[] DepthBasedIndexChannel { get; }
        List<string> Channels { get; }
        List<String> ToolSections { get; }
        DateTimeOffset LogStartTime { get; }
        DateTimeOffset LogEndTime { get; }
        int LogVersionNumber { get; set; }
        double LogStartDepth { get; }
        double LogEndDepth { get; }
        int Length { get; }
        FileSystemLogParameters LogParameters { get; }
        ChannelData GetChannelData(string mnemonic);
        List<ChannelData> GetChannelData(List<string> mnemonics);
        List<ChannelData> GetChannelData(List<string> mnemonics, DateTimeOffset startTime, DateTimeOffset endTime);
        List<ChannelData> GetChannelData(List<String> mnemonics, double startOffset, double endOffset);
        ChannelData GetChannelData(string mnemonic, DateTimeOffset startTime, DateTimeOffset endTime);
        ChannelData GetChannelData(String mnemonic, double startOffset, double endOffset);
        FileSystemLogInfo GetChannelInfo(string mnemonic);
        List<FileSystemLogInfo> GetChannelInfo(List<string> mnemonic);
        double[,] GetChannelWaveforms(string mnemonic);
        double[] GetChannel(string mnemonic);
        double[] GetChannel(string mnemonic, DateTimeOffset startTime, DateTimeOffset endTime);
        double[] GetChannel(string mnemonic, double startDepth, double endDepth);
        double[] GetChannelByOffsets(string mnemonic, double startOffsetTime, double endOffsetTime);

        void DoActionInOuputStream(string fileName, Action<Stream> action);
        void DoActionInInputStream(string fileName, Action<Stream> action);
        void StoreIndexChannelValues(Stream s, DateTimeOffset[] values);
        ChannelStatistics StoreChannelValues(Stream s, double[] curveValues);
        double[] ReadChannelValues(Stream s);
        DateTimeOffset[] ReadIndexChannelValues(Stream s);
    }
}
