using System;
using System.Collections.Generic;
using System.Text;

namespace PlantBU.Utilities
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'NotificationEventArgs'
    public class NotificationEventArgs : EventArgs
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'NotificationEventArgs'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'NotificationEventArgs.Title'
        public string Title { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'NotificationEventArgs.Title'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'NotificationEventArgs.Message'
        public string Message { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'NotificationEventArgs.Message'
    }
}
