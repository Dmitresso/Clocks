#region USING
using System;
#endregion


namespace NRTW.Code.Scripts.Interfaces {
    public interface INetworkOperationsProvider {
        #region INTERFACE METHODS
        public DateTime GetNetworkTime(string url);
        
        public uint SwapEndianness(ulong x);
        
        public bool Ping(string url = "google.com");
        #endregion
    }
}