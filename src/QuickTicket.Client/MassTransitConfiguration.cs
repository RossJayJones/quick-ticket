using System;

namespace QuickTicket.Client
{
    public class MassTransitConfiguration
    {
        public const string SectionName = "MassTransit";

        public string Url { get; set; }
        
        public string KeyName { get; set; }

        public string SharedAccessKey { get; set; }

        public TimeSpan TokenTimeToLive { get; set; } = TimeSpan.FromDays(1);
    }
}