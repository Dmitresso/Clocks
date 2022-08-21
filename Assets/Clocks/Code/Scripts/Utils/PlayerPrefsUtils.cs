using UnityEngine;

namespace NRTW.Code.Scripts.Utils {
    public static class PlayerPrefsUtils {
        #region FIELDS
        private const string Orientation = "Orientation";
        #endregion

        
        
        #region METHODS
        public static bool IsPortrait() {
            return PlayerPrefs.GetInt(Orientation, 1) == 1;
        }

        
        public static bool HasOrientation() {
            return PlayerPrefs.HasKey(Orientation);
        }

        
        public static void SavePortrait(bool isPortrait) {
            PlayerPrefs.SetInt(Orientation, isPortrait ? 1 : 0);
        }
        #endregion
    }
}