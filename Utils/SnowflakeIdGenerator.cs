using System;
using System.Threading;

namespace Genzy.Base.Utils
{
    public class SnowflakeOptions
    {
        /// <summary>
        /// Node ID (0 - 1023), tương ứng worker/machine
        /// </summary>
        public long NodeId { get; set; }

        /// <summary>
        /// Custom epoch (Unix time milliseconds), ví dụ: 2025-01-01
        /// </summary>
        public long Epoch { get; set; } = new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)).ToUnixTimeMilliseconds();

        /// <summary>
        /// Bit length của sequence (mặc định 12)
        /// </summary>
        public int SequenceBits { get; set; } = 12;

        /// <summary>
        /// Bit length của nodeId (mặc định 10)
        /// </summary>
        public int NodeBits { get; set; } = 10;
    }

    public class SnowflakeIdGenerator
    {
        private readonly long _nodeId;
        private readonly long _epoch;
        private readonly int _sequenceBits;
        private readonly int _nodeBits;
        private readonly long _maxSequence;
        private readonly long _maxNodeId;
        private long _lastTimestamp = -1L;
        private long _sequence = 0L;
        private readonly object _lock = new object();

        private readonly int _nodeShift;
        private readonly int _timestampShift;

        public SnowflakeIdGenerator(SnowflakeOptions options)
        {
            _nodeId = options.NodeId;
            _epoch = options.Epoch;
            _sequenceBits = options.SequenceBits;
            _nodeBits = options.NodeBits;

            _maxSequence = (1L << _sequenceBits) - 1;
            _maxNodeId = (1L << _nodeBits) - 1;

            if (_nodeId < 0 || _nodeId > _maxNodeId)
                throw new ArgumentException($"NodeId must be between 0 and {_maxNodeId}");

            _nodeShift = _sequenceBits;
            _timestampShift = _sequenceBits + _nodeBits;
        }

        public ulong NextId()
        {
            lock (_lock)
            {
                var timestamp = CurrentTimeMillis();
                if (timestamp < _lastTimestamp)
                    throw new InvalidOperationException($"Clock moved backwards. Refusing to generate id for {_lastTimestamp - timestamp}ms");

                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & _maxSequence;
                    if (_sequence == 0)
                    {
                        // Sequence overflow, đợi tới timestamp tiếp theo
                        timestamp = WaitNextMillis(timestamp);
                    }
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;

                return (ulong)((timestamp - _epoch) << _timestampShift | (_nodeId << _nodeShift) | _sequence);
            }
        }

        private long WaitNextMillis(long lastTimestamp)
        {
            var timestamp = CurrentTimeMillis();
            while (timestamp <= lastTimestamp)
            {
                Thread.SpinWait(1); // hoặc Thread.Sleep(0) để giảm CPU
                timestamp = CurrentTimeMillis();
            }
            return timestamp;
        }

        private static long CurrentTimeMillis() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
