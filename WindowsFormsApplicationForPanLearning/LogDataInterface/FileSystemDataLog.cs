using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogDataInterface
{

    public class Data
    {
        // what is Id?
        public string Id { get; set; }
        public string Value { get; set; }
    }

    public class LogData
    {
        private readonly List<Data> _datas = new List<Data>();
        public List<Data> getDatas()
        {
            return _datas;
        }

        public Data this[int pos]
        {
            get
            {
                return _datas[pos];
            }

        }
    }

    /// <summary>
    /// Represents a Log object.
    /// </summary>
    public class FileSystemDataLog
    {

        public const string DateTimeIndexType = "date time";
        public const string MeasuredDepthIndexType = "measured depth";

        /// <summary>
        /// Set Index LogCurveInfo mnemonic to  DATETIME, should match corresponding IndexCurve
        /// element value if indexType element is 'date time'
        /// </summary>
        public const string DateTimeIndexMnemonic = "DATETIME";
        /// <summary>
        /// Set Index LogCurveInfo mnemonic to  Mdepth, should match corresponding IndexCurve
        /// element value if indexType element is 'measured depth'
        /// </summary>

        public const string MeasuredDepthIndexMnemonic = "Mdepth";

        private string _indexType = DateTimeIndexType;

        public string WellName { get; set; }
        public string WellboreName { get; set; }
        public string Name { get; set; }

        public string LogId;
        public string WellboreId;
        public string WellId;
        public string Id;


        public string Description { get; set; }
        public string ServiceCompany { get; set; }
        public bool ObjectGrowing { get; set; }
        public int DataRowCount { get; set; }

        public DateTimeOffset StartDateTimeIndex { get; set; }
        public DateTimeOffset EndDateTimeIndex { get; set; }

        // defaults to 'date time'
        public string IndexType
        {
            get { return _indexType; }
            set
            {
                validateIndexType(value);
                _indexType = value;
            }
        }

        public IndexCurve IndexCurve { get; set; }
        public string NullValue { get; set; }

        private IList<ILogCurveInfo> _logCurveInfos = new List<ILogCurveInfo>();

        private LogData _logData = new LogData();

        public FileSystemDataLog()
        {
        }

        private void validateIndexType(string value)
        {

            if (!(value.Equals(FileSystemDataLog.MeasuredDepthIndexType) || value.Equals(FileSystemDataLog.DateTimeIndexType)))
            {
                throw new ArgumentOutOfRangeException("indexType must be either " + FileSystemDataLog.DateTimeIndexType + " or " + FileSystemDataLog.MeasuredDepthIndexType);
            }


        }
    }


}
