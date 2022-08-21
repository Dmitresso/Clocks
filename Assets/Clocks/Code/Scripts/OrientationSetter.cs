#region USING
using System.Collections;
using NRTW.Code.Scripts.Utils;
using UnityEngine;
#endregion


namespace NRTW.Code.Scripts {
    public class OrientationSetter : MonoBehaviour {
        #region FIELDS
        private static GameManager gameManager;
        #endregion
        
        
        
        #region BUILTIN METHODS
        private void Awake() {
            gameManager = GetComponent<GameManager>();
            if (gameManager == null) gameManager = FindObjectOfType<GameManager>();

            if (gameManager == null) {
                Debug.LogWarning("[OrientationSetter] GameManager not found!");
                return;
            }
            StartCoroutine(SetupInitialOrientation());
        }
        #endregion

        

        #region CUSTOM METHODS
        private static IEnumerator SetupInitialOrientation() {
            if (PlayerPrefsUtils.HasOrientation()) {
                if (PlayerPrefsUtils.IsPortrait()) {
                    SetupPortraitOrientation();
                    if (Screen.orientation != ScreenOrientation.Portrait) {
                        Screen.orientation = ScreenOrientation.Portrait;
                        yield return new WaitForSeconds(3);
                        gameManager.LoadScene();
                    }
                }
                else {
                    SetupLandscapeOrientation();
                    if (Screen.orientation != ScreenOrientation.LandscapeLeft) {
                        Screen.orientation = ScreenOrientation.LandscapeLeft;
                        yield return new WaitForSeconds(3);
                        gameManager.LoadScene();
                    }
                }
                
                yield break;
            }

            if (Screen.orientation == ScreenOrientation.Portrait) {
                SetupPortraitOrientation();
            }
            else {
                SetupLandscapeOrientation();
            }

            gameManager.LoadScene();
        }


        private static void SetupPortraitOrientation() {
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
        }


        private static void SetupLandscapeOrientation() {
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
        }
        #endregion
    }
}