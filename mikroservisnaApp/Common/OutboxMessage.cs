using Common.StrucniDogadjajDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public enum Status
    {
        NotProcessed,
        Processed
    }

    public enum OperationEvent
    {
        Created,
        Read,
        Updated,
        Deleted,
    }

    public class OutboxMessage
    {
        public int Id { get; set; }
        public string Payload { get; set; } = "";
        public DateTime SentTime { get; set; } = DateTime.UtcNow;
        public Status Status { get; set; } = Status.NotProcessed;
        public OperationEvent Event { get; set; }

    }
}
