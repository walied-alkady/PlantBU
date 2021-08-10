using System;
using System.Net.NetworkInformation;

namespace PlantBU.Utilities
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Network'
    public static class Network
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Network'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Network.IsConnectedToInternet()'
        public static bool IsConnectedToInternet()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Network.IsConnectedToInternet()'
        {
            string host = "www.google.com";
            bool result = false;
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(host, 3000);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex) { }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            return result;
        }
    }
}
