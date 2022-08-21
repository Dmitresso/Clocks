#region USING
using System;
using NRTW.Code.Scripts.CheckPoint;
using NRTW.Code.Scripts.Clocks;
using NRTW.Code.Scripts.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#endregion


namespace NRTW.Code.Scripts.Arrows {
    public delegate void EnumPropertyChanged(ArrowType newValue);
    public class Arrow : DragAndRotate {
        #region FIELDS
        [SerializeField] private ArrowType arrowType = ArrowType.Second;
        [SerializeField] private CheckPointSystem cps;
        
        private UnityEvent calculateTime = new ();
        private event EnumPropertyChanged arrowTypeChanged;
        
        private RectTransform t;
        private AnalogClock clock;

        private int step;
        private bool isClockwise;
        private float endAngle, previousAngle, stickAngle;
        #endregion



        #region CONST FIELDS
        private const float dragSpeed = 2f;
        #endregion



        #region PROPERTIES
        public BoxCollider2D Collider { get; private set; }

        private ArrowType ArrowType {
            get => arrowType;
            set {
                arrowType = value;
                arrowTypeChanged?.Invoke(value);
            }
        }
        #endregion



        #region BUILTIN METHODS
        private void OnValidate() {
            ArrowType = arrowType;
        }
        
        
        protected void Awake() {
            base.Awake();
            t = GetComponent<RectTransform>();
            Collider = GetComponentInChildren<BoxCollider2D>();
            clock = GetComponentInParent<AnalogClock>();
        }


        private void Start() {
            SwitchArrowType(arrowType);
        }


        private void OnEnable() {
            arrowTypeChanged += SwitchArrowType;
            SwitchBehavior(OnBeginDragEvent, OnBeginDragCallback);
            SwitchBehavior(OnDragEvent, OnDragCallback);
            SwitchBehavior(OnEndDragEvent, OnEndDragCallback);
        }


        private void OnDisable() {
            base.OnDisable();
            arrowTypeChanged -= SwitchArrowType;
        }
        #endregion



        #region CUSTOM METHODS
        private void OnBeginDragCallback(PointerEventData eventData) {
            clock.Process = false;
            cps.CollidersActive = true;
        }

        
        private void OnDragCallback(PointerEventData eventData) {
            if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(t, eventData.position, eventData.pressEventCamera, out var mousePosition)) return;
            var targetDirection = mousePosition - t.position;
            var angle = Mathf.Atan2(targetDirection.x, targetDirection.y) *- Mathf.Rad2Deg;
            // isClockwise = Vector2.Dot(t.right, targetDirection) > 0;

            var targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            t.localRotation = Quaternion.RotateTowards(t.rotation, targetRotation, dragSpeed);
        }
        
        
        private void OnEndDragCallback(PointerEventData eventData) {
            Stick();
            endAngle = stickAngle;
            
            clock.Wait();
            calculateTime?.Invoke();
            
            clock.alarm.Prepare();
            cps.CollidersActive = false;
        }


        private void Stick() {
            var rotation = t.localRotation.eulerAngles.z;
            var floor = (int) rotation / step;
            var rest =  rotation % step;

            var position = floor * step;
            if (rest >= step * 0.5) position += step;
            stickAngle = position;
            t.localRotation = Quaternion.Euler(0, 0, position);
        }

        
        private void CalculateTime_S() {
            var second = (360 - endAngle) / clock.MSStep;
            clock.alarm.S = (int) second;
        }
        
        
        private void CalculateTime_M() {
            var minute = (360 - endAngle) / clock.MSStep;
            clock.alarm.M = (int) minute;
        }
        
        
        private void CalculateTime_H() {
            var isPM = clock.IsPM;
            var floor = cps.Log[0] / 2;
            var rest = cps.Log[0] % 2;
            isPM = Math.Abs(rest) == 0 ? isPM : !isPM;
            
            var hour = (360 - endAngle) / clock.HStep + (isPM ? 12 : 0);
            if ((int) hour == 24) hour = 0;
            clock.alarm.H = (int) hour;

            var day = floor;
            if (floor < 0) day = 0; // prevent setting alarms to past day
            if (clock.AllowNextDayAlarmOnly && day > 1) day = 1;
            clock.alarm.D = day;
            if (clock.IsPM && rest > 0) clock.alarm.D++;
        }

        
        private void SwitchArrowType(ArrowType arrowType) {
            calculateTime.RemoveAllListeners();

            switch (arrowType) {
                case ArrowType.Second:
                    step = clock.MSStep;
                    calculateTime.AddListener(CalculateTime_S);
                    break;
                case ArrowType.Minute:
                    step = clock.MSStep;
                    calculateTime.AddListener(CalculateTime_M);
                    break;
                case ArrowType.Hour:
                    step = clock.HStep;
                    calculateTime.AddListener(CalculateTime_H);
                    break;
            }
        }
        #endregion
    }
}