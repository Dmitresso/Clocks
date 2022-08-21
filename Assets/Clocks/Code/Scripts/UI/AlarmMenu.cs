#region USING
using System;
using System.Collections;
using NRTW.Code.Scripts.Clocks;
using NRTW.Code.Scripts.OperationsProvider;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
#endregion


namespace NRTW.Code.Scripts.UI {
    public class AlarmMenu : MonoBehaviour {
        #region FIELDS
        [Header("Controls")]
        [SerializeField] private ClockInputField inputField;
        [SerializeField] private RectTransform tip;
        [SerializeField] private RectTransform alarmT;
        [SerializeField] private Button setAlarmButton;
        [SerializeField] private Button removeAlarmButton;
        [SerializeField] private TMP_Text alarmTime;
        [SerializeField] private AudioClip alarmSound;
        
        [SerializeField, Range(5, 20)] private float showTipTime = 5;

        private bool tipShown, compareTimeCoroutineIsRunning;
        private int d, h, m, s;

        private DateTime alarm;
        private BaseClock clock;
        private AudioSource audio;
        
        
        private readonly YieldInstruction waitForOneSec = new WaitForSeconds(1);
        #endregion



        #region PROPERTIES
        public bool AlarmIsSet { get; private set; }

        public RectTransform AlarmT => alarmT;

        private string FinalTime {
            set => alarmTime.text = value;
        }
        

        public int D {
            get => d;
            set {
                d = value;
            }
        }
        
        public int H {
            get => h;
            set {
                h = value;
            }
        }
        
        public int M {
            get => m;
            set {
                m = value;
            }
        }
        
        public int S {
            get => s;
            set {
                s = value;
            }
        }
        #endregion
        
        


        public Button stBtn;
        public AndroidOperationsProvider aop;
        private void OnSTBtnClick() {
            aop = GetComponent<AndroidOperationsProvider>();
            aop.SynchronizeSystemTime();
        }

        
        #region BUILTIN METHODS
        private void OnEnable() {
            setAlarmButton.onClick.AddListener(OnSetAlarmButtonClick);
            removeAlarmButton.onClick.AddListener(OnRemoveAlarmButtonClick);
            inputField.inputFieldPointerEnter += OnInputFieldPointerEntered;
            tip.gameObject.SetActive(false);
            
            stBtn.onClick.AddListener(OnSTBtnClick);
        }


        private void OnDisable() {
            setAlarmButton.onClick.RemoveListener(OnSetAlarmButtonClick);
            removeAlarmButton.onClick.RemoveListener(OnRemoveAlarmButtonClick);
        }


        private void Awake() {
            clock = FindObjectOfType<BaseClock>();
            audio = GetComponent<AudioSource>();
        }


        private void Start() {
            alarmT.gameObject.SetActive(false);
            h = m = s = -1;
        }
        #endregion
        


        #region CUSTOM METHODS
        public void Prepare() {
            if (h == -1) h = clock.Time.Hour;
            if (m == -1) m = clock.Time.Minute;
            if (s == -1) s = clock.Time.Second;
            
            var dt = clock.Time.Date + new TimeSpan(d, h, m, s);
            FinalTime = dt.ToString("HH:mm:ss");
            alarm = DateTime.Compare(dt, clock.Time) < 0 ? dt.AddDays(1) : dt; 
        }

        
        public void ResetAlarm() {
            d = h = m = s = -1;
            alarm = DateTime.Now;
            if (compareTimeCoroutineIsRunning) StopCoroutine(nameof(CompareTime));
        }
        
        
        private void SetAlarm() {
            if (compareTimeCoroutineIsRunning) StopCoroutine(nameof(CompareTime));
            StartCoroutine(CompareTime(alarm));
        }


        private void OnSetAlarmButtonClick() {
            SetAlarm();
            Debug.Log("Alarm was set to: " + alarm);
            
            AlarmIsSet = true;
            setAlarmButton.enabled = false;
            setAlarmButton.interactable = false;
        }

        
        private void OnRemoveAlarmButtonClick() {
            AlarmIsSet = false;
            setAlarmButton.enabled = true;
            setAlarmButton.interactable = true;
            ResetAlarm();
            alarmT.gameObject.SetActive(false);
        }


        private void OnInputFieldPointerEntered() {
            StartCoroutine(ShowTip(showTipTime));
        }


        private IEnumerator CompareTime(DateTime alarm) {
            compareTimeCoroutineIsRunning = true;
            alarm = RoundToSecond(alarm);
            
            while (compareTimeCoroutineIsRunning) {
                if (DateTime.Compare(alarm, RoundToSecond(clock.Time)) == 0) {
                    Debug.Log("Alarm!");
                    compareTimeCoroutineIsRunning = false;
                }
                yield return waitForOneSec;
            }
            
            audio.PlayOneShot(alarmSound);
            yield return new WaitForSeconds(alarmSound.length);
        }
        
        
        private IEnumerator ShowTip(float time) {
            tip.gameObject.SetActive(true);
            yield return new WaitForSeconds(time);
            tip.gameObject.SetActive(false);
            inputField.inputFieldPointerEnter -= OnInputFieldPointerEntered;
        }
        
        
        private static DateTime RoundToSecond(DateTime dt) {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }
        #endregion
    }
}