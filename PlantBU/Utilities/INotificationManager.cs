using System;
using System.Collections.Generic;
using System.Text;

namespace PlantBU.Utilities
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'INotificationManager'
    public interface INotificationManager
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'INotificationManager'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'INotificationManager.NotificationReceived'
        event EventHandler NotificationReceived;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'INotificationManager.NotificationReceived'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'INotificationManager.Initialize()'
        void Initialize();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'INotificationManager.Initialize()'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'INotificationManager.SendNotification(string, string, DateTime?)'
        void SendNotification(string title, string message, DateTime? notifyTime = null);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'INotificationManager.SendNotification(string, string, DateTime?)'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'INotificationManager.ReceiveNotification(string, string)'
        void ReceiveNotification(string title, string message);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'INotificationManager.ReceiveNotification(string, string)'
    }
}
