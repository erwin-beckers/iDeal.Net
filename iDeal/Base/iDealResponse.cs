using System;

namespace iDeal.Base
{
    public abstract class iDealResponse
    {
        public int AcquirerId { get; protected set; }

        public string createDateTimestamp { get; protected set; }

        public DateTime createDateTimestampLocalTime { get { return DateTime.Parse(createDateTimestamp); } }
    }
}
