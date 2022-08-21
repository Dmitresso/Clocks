#region USING
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NRTW.Code.Scripts.Arrows;
using NRTW.Code.Scripts.CheckPoint;
using NRTW.Code.Scripts.Enums;
using NRTW.Code.Scripts.UI;
using UnityEngine;
#endregion


namespace NRTW.Code.Scripts.Clocks {
    public delegate void EnumPropertyChanged(ArrowStep newValue);
    public class AnalogClock : BaseClock {
        #region FIELDS
        [Header("Arrows")]
        [SerializeField] private Arrow secondsArrow;
        [SerializeField] private Arrow minutesArrow;
        [SerializeField] private Arrow hoursArrow;
        [SerializeField] private ArrowStep secondsArrowStep = ArrowStep.Continuous;
        [SerializeField, Range(0.02f, 0.1f)] private float lerpSpeed = 0.02f;
        [SerializeField, Range(5f, 10f)] private float waitForRunTime;
        [SerializeField] private bool allowNextDayAlarmOnly = true;

        private event EnumPropertyChanged secondsArrowStepChanged;
        private RectTransform secondsArrowT, minutesArrowT, hoursArrowT;
        private bool waitCoroutineIsRunning;

        private CheckPointSystem cps;
        #endregion



        #region CONST FIELDS
        private const int msStep = 6, hStep = 30;
        #endregion
        

        
        #region PROPERTIES
        public bool IsPM => Time.ToString("tt", CultureInfo.InvariantCulture).Equals("PM");
        public bool AllowNextDayAlarmOnly => allowNextDayAlarmOnly;
        
        public int MSStep => msStep;
        public int HStep => hStep;
        
        
        private ArrowStep SecondsArrowStep {
            get => secondsArrowStep;
            set {
                secondsArrowStep = value;
                secondsArrowStepChanged?.Invoke(value);
            }
        }
        #endregion
        
        

        #region BUILTIN METHODS
        private void OnValidate() {
            SecondsArrowStep = secondsArrowStep;
        }
        

        private void Awake() {
            base.Awake();
            secondsArrowT = secondsArrow.GetComponent<RectTransform>();
            minutesArrowT = minutesArrow.GetComponent<RectTransform>();
            hoursArrowT = hoursArrow.GetComponent<RectTransform>();
            cps = GetComponentInChildren<CheckPointSystem>();
        }


        private void Start() {
            base.Start();
            SwitchSecondsArrowStep(secondsArrowStep);
            
            Physics2D.IgnoreCollision(hoursArrow.Collider, minutesArrow.Collider);
            Physics2D.IgnoreCollision(hoursArrow.Collider, secondsArrow.Collider);
            Physics2D.IgnoreCollision(minutesArrow.Collider, secondsArrow.Collider);
        }
        
        
        private void OnEnable() {
            secondsArrowStepChanged += SwitchSecondsArrowStep;
            updateEvent.AddListener(MinutesArrowRun);
            updateEvent.AddListener(HoursArrowRun);
        }
        
        
        private void OnDisable() {
            secondsArrowStepChanged -= SwitchSecondsArrowStep;
            updateEvent.RemoveAllListeners();
            StopAllCoroutines();
        }
        #endregion
        
        

        #region CUSTOM METHODS
        public void Wait() {
            if (waitCoroutineIsRunning) StopCoroutine(nameof(WaitForRun));
            StartCoroutine(nameof(WaitForRun));
        }
        

        protected override void ProcessHandles() {
            updateEvent?.Invoke();
        }
        

        private void SecondsArrowContinuousRun() {
            secondsArrowT.rotation = Quaternion.Lerp(secondsArrowT.rotation, Quaternion.Euler(0, 0, - Time.Second * 6), lerpSpeed);
        }


        private void SecondsArrowSeparatedRun() {
            secondsArrowT.localRotation = Quaternion.Euler(0, 0, - Time.Second * msStep);
        }


        private void MinutesArrowRun() {
            minutesArrowT.localRotation = Quaternion.Euler(0, 0, - Time.Minute * msStep);
        }


        private void HoursArrowRun() {
            hoursArrowT.localRotation = Quaternion.Euler(0, 0, - Time.Hour * hStep);
        }


        private IEnumerator WaitForRun() {
            waitCoroutineIsRunning = true;
            alarm.AlarmT.gameObject.SetActive(true);
            yield return new WaitForSeconds(waitForRunTime);
            
            Process = true;
            waitCoroutineIsRunning = false;
            if (!alarm.AlarmIsSet) alarm.AlarmT.gameObject.SetActive(false);
            
            cps.Reset();
            alarm.ResetAlarm();
        }


        private void SwitchSecondsArrowStep(ArrowStep arrowStep) {
            switch (arrowStep) {
                case ArrowStep.Continuous:
                    updateEvent.RemoveListener(SecondsArrowSeparatedRun);
                    updateEvent.AddListener(SecondsArrowContinuousRun);
                    break;
                case ArrowStep.Separated:
                    updateEvent.RemoveListener(SecondsArrowContinuousRun);
                    updateEvent.AddListener(SecondsArrowSeparatedRun);
                    break;
            }
        }
        #endregion
    }
}