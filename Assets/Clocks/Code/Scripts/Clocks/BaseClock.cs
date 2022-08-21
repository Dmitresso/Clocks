#region USING
using System;
using System.Collections;
using System.Text.RegularExpressions;
using NRTW.Code.ScriptableObjects;
using NRTW.Code.Scripts.OperationsProvider;
using NRTW.Code.Scripts.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
#endregion


namespace NRTW.Code.Scripts.Clocks {
    [DisallowMultipleComponent]
    public abstract class BaseClock : MonoBehaviour {
        #region FIELDS
        [SerializeField] private Hosts hosts;
        [SerializeField] private bool synchronizeClockTime = true;
        [SerializeField] private bool updateClockTime = true;
        [SerializeField] private bool process = true;

        [Tooltip("Update time interval is set in seconds")]
        [SerializeField] protected float updateTimeInterval = 3600f;
        
        public AlarmMenu alarm;
        
        
        protected UnityEvent updateEvent = new ();
        protected NetworkOperationsProvider netops = new ();
        protected SystemOperationsProvider sysops;
        
        
        private readonly YieldInstruction waitForOneSec = new WaitForSeconds(1);
        #endregion



        #region PROPERTIES
        public DateTime Time { get; private set; }
        
        public bool Process {
            get => process;
            set {
                process = value;
                if (process) ActualizeTime();
            }
        }
        #endregion
        
        

        #region BUILTIN METHODS
        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }


        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        
        protected void Awake() {
            alarm = FindObjectOfType<AlarmMenu>();
        }

        
        protected void Start() {
            StartCoroutine(SynchronizeTime(updateTimeInterval));
            StartCoroutine(UpdateTime());
        }
        
        
        protected void Update() {
            if (process) ProcessHandles();
        }
        #endregion



        #region CUSTOM METHODS
        protected abstract void ProcessHandles();


        protected virtual IEnumerator SynchronizeTime(float updateInterval) {
            while (synchronizeClockTime) {
                //Timer.Start();
                //var time = netops.GetAverageTime(hosts.ntpHostsValidated);
                //Timer.Stop();
                //Time = time + Timer.GetTime();

                ActualizeTime();
                yield return new WaitForSeconds(updateInterval);
            }
        }


        protected virtual IEnumerator UpdateTime() {
            while (updateClockTime) {
                Time = Time.AddSeconds(1);
                yield return waitForOneSec;
            }
        }


        protected void ActualizeTime() {
            Time = netops.GetAverageTime(hosts.ntpHostsValidated);
        }


        private void Init() {
            sysops = SystemInfo.operatingSystem switch {
                var x when Regex.IsMatch(x, "Android") => new AndroidOperationsProvider(),
                var y when Regex.IsMatch(y, "Windows") => new WindowsOperationsProvider(),
                _ => new SystemOperationsProvider()
            };
        }


        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
            Init();
        }
        #endregion
    }
}