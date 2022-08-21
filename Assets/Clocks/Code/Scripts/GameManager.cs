#region USING
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion


namespace NRTW.Code.Scripts {
    public class GameManager : MonoBehaviour {
        #region FIELDS
        #endregion
        
        
        
        #region BUILTIN METHODS
        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        
        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        
        private void Awake() {
            SetupDebug();
        }
        #endregion

        

        #region CUSTOM METHODS
        public void LoadScene(string sceneName = "Dev") {
            SceneManager.LoadSceneAsync(sceneName);
        }
        

        public void ExitApp() {
            #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
            #endif
            Application.Quit();
        }


        private void SetupDebug() {
            Debug.unityLogger.logEnabled = Debug.isDebugBuild;
        }
        
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
            
        }
        #endregion
    }
}