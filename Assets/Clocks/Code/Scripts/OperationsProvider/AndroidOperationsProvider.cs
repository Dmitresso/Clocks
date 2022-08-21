#region USING

using System.Collections;
using System.Collections.Generic;
using NRTW.Code.Scripts.Android;
using UnityEngine;
using UnityEngine.Android;

#endregion


namespace NRTW.Code.Scripts.OperationsProvider {
    public class AndroidOperationsProvider : SystemOperationsProvider {
        #region FIELDS
        private AndroidJavaClass timeManager;

        private bool synchronizeSystemTimeCoroutineIsRunning;
        #endregion



        #region CONST FIELDS
        private const string setTimePermission = "com.google.android.things.permission.SET_TIME";
        private const string timeManagerClass = "TimeManager";
        private const string setAutoTimeEnabledMethod = "setAutoTimeEnabled";
        #endregion



        #region BUILTIN METHODS
        private void Awake() {
            timeManager = new AndroidJavaClass(timeManagerClass);
        }


        private void Start() {
            
        }
        #endregion

        
        
        #region OVERRIDEN METHODS
        public override void SynchronizeSystemTime() {
            if (synchronizeSystemTimeCoroutineIsRunning) StopCoroutine(nameof(SynchronizeSystemTimeCoroutine));
            StartCoroutine(nameof(SynchronizeSystemTimeCoroutine));
        }


        private IEnumerator SynchronizeSystemTimeCoroutine() {
            var enabled = false;
            timeManager.Call(setAutoTimeEnabledMethod, enabled);
            yield return new WaitForEndOfFrame();
            timeManager.Call(setAutoTimeEnabledMethod, !enabled);
        }
        #endregion
    }
}