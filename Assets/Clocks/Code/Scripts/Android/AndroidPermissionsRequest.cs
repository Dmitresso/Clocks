#region USING
using UnityEngine;
using UnityEngine.Android;
#endregion


namespace NRTW.Code.Scripts.Android {
    public class AndroidPermissionsRequest {
        #region BUILTIN METHODS
        private void Start() {
            if (Permission.HasUserAuthorizedPermission(Permission.Microphone)) {
                // The user authorized use of the microphone.
            }
            else {
                var useCallbacks = false;
                if (!useCallbacks)
                {
                    // We do not have permission to use the microphone.
                    // Ask for permission or proceed without the functionality enabled.
                    Permission.RequestUserPermission(Permission.Microphone);
                }
                else
                {
                    var callbacks = new PermissionCallbacks();
                    callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
                    callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                    callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
                    Permission.RequestUserPermission(Permission.Microphone, callbacks);
                }
            }
        }
        #endregion



        #region CUSTOM METHODS
        public void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName) {
            Debug.Log($"{permissionName} PermissionDeniedAndDontAskAgain");
        }

        
        public void PermissionCallbacks_PermissionGranted(string permissionName) {
            Debug.Log($"{permissionName} PermissionCallbacks_PermissionGranted");
        }

        
        public void PermissionCallbacks_PermissionDenied(string permissionName) {
            Debug.Log($"{permissionName} PermissionCallbacks_PermissionDenied");
        }
        #endregion
    }
}