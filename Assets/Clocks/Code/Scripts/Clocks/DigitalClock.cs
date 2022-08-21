#region USING
using TMPro;
using UnityEngine;
#endregion


namespace NRTW.Code.Scripts.Clocks {
    public class DigitalClock : BaseClock {
        #region FIELDS
        [Header("Ciphers")]
        [SerializeField] private TMP_Text ciphers;
        #endregion



        #region BUILTIN METHODS
        private void OnEnable() {
            updateEvent.AddListener(UpdateCiphers);
        }
        
        
        private void OnDisable() {
            updateEvent.RemoveAllListeners();
            StopAllCoroutines();
        }
        #endregion
        
        
        
        #region CUSTOM METHODS
        protected override void ProcessHandles() {
            updateEvent?.Invoke();
        }
        
        
        private void UpdateCiphers() {
            ciphers.text = Time.ToString("HH:mm:ss");
        }
        #endregion
    }
}