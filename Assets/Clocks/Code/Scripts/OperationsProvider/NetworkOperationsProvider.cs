#region USING
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NRTW.Code.Scripts.Interfaces;
using Debug = UnityEngine.Debug;
using Ping = System.Net.NetworkInformation.Ping;
#endregion


namespace NRTW.Code.Scripts.OperationsProvider {
    public class NetworkOperationsProvider : INetworkOperationsProvider {
        #region FIELDS
        private Ping ping = new ();
        private int timeout = 1000;
        private byte[] buffer = new byte[32];
        private TimeZoneInfo timeZone = TimeZoneInfo.Local;
        #endregion
        
        
        
        #region CONST FIELDS
        private const int ntpServerPort = 123;
        private const int connectionTimeout = 3000;
        #endregion
        
        
        
        #region METHODS
        public DateTime GetAverageTime(List<string> hosts) {
            if (hosts == null) {
                Debug.LogAssertion("[NetworkOperationsProvider] Hosts SO is empty! Clocks will use actual system time.");
                return DateTime.Now;
            }
            
            switch (hosts.Capacity) {
                case 0:
                    return DateTime.UtcNow;
                case 1:
                    return GetNetworkTime(hosts[1]);
                default:
                    var times = hosts.Select(GetNetworkTime).ToList();

                    var tmp = times.Sum(t => t.Ticks / (double) times.Count);
                    var time = new DateTime((long) tmp);
                    return time;
            }
        }
        #endregion


        
        #region INTERFACE METHODS
        public DateTime GetNetworkTime(string url) {
            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            // Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(url).AddressList;


            // The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], ntpServerPort);
            // NTP uses UDP

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
                socket.Connect(ipEndPoint);

                // Stops code hang if NTP is blocked
                socket.ReceiveTimeout = connectionTimeout;     

                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
            }

            // Offset to get to the "Transmit Timestamp" field (time at which the reply 
            // departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            // Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            // Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            // Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = intPart * 1000 + fractPart * 1000 / 0x100000000L;

            // **UTC** time
            var networkDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((long)milliseconds) + timeZone.BaseUtcOffset;
            return networkDateTime;
        }
        
        
        // stackoverflow.com/a/3294698/162671
        public uint SwapEndianness(ulong x) {
            return (uint) (((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }
        
        
        public bool Ping(string url = "google.com") {
            PingReply reply;
            try { reply = ping.Send(url, timeout, buffer); }
            catch (PingException e) { return false; }
            catch (SocketException e) { return false; }
            return reply?.Status == IPStatus.Success;
        }
        #endregion
    }
}