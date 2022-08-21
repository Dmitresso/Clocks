#region USING
using UnityEngine;
#endregion


namespace NRTW.Code.Scripts.OperationsProvider {
    public class SystemOperationsProvider : MonoBehaviour {
        #region FIELDS
        protected string packageName = "com." + companyName + "." + productName;
        #endregion
        
        
        
        #region CONST FIELDS
        private const string companyName = "WheelApps";
        private const string productName = "Clocks";
        #endregion


        
        #region CUSTOM METHODS
        public virtual void SynchronizeSystemTime() {}
        #endregion
    }
}