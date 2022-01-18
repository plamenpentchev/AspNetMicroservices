using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Events
{
    /// <summary>
    /// Base class for all rabbotmq events
    /// All queue messages will derive from this base class
    /// </summary>
    public class IntegrationBaseEvent
    {
        /// <summary>
        /// correaltion id to track the rabbitmq queue names
        /// </summary>
        public Guid Id { get; private set; }
        public DateTime CreationDate { get; private set; }

        public IntegrationBaseEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public IntegrationBaseEvent(Guid id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }
    }
}
