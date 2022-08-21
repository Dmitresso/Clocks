#region USING
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NRTW.Code.Scripts;
using NRTW.Code.Scripts.Interfaces;
using NRTW.Code.Scripts.OperationsProvider;
using UnityEngine;
#endregion


namespace NRTW.Code.ScriptableObjects {
    public class Hosts : ScriptableObject {
        #region FIELDS
        [SerializeField] private List<string> pingHosts;
        [SerializeField] private List<string> ntpHosts;
        
        
        [HideInInspector] public List<string> pingHostsValidated = new();
        [HideInInspector] public List<string> ntpHostsValidated = new();

        
        private INetworkOperationsProvider netops = new NetworkOperationsProvider();
        #endregion



        #region CONST FIELDS
        private static readonly Regex urlRegex = new(
            @"^((ht|f)tp(s?)\:\/\/)?(?<host>[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*))?$");
        #endregion



        #region BUILTIN METHODS
        private void OnEnable() {
            ValidateHosts();
        }


        private void OnDisable() {
            pingHostsValidated.Clear();
            ntpHostsValidated.Clear();
        }
        #endregion



        #region CUSTOM METHODS
        private void ValidateHosts() {
            foreach (var host in pingHosts) ValidateHost(host, pingHostsValidated);
            foreach (var host in ntpHosts) ValidateHost(host, ntpHostsValidated);
        }


        private void ValidateHost(string host, List<string> list) {
            if (string.IsNullOrEmpty(host)) return;
            var match = urlRegex.Match(host);
            if (!match.Success) return;

            var str = match.Groups["host"].ToString();
            var finalHost = str.Contains("www.") ? str[4..] : str;

            var ping = netops.Ping();
            if (ping) list.Add(finalHost);
        }
        #endregion
    }
}