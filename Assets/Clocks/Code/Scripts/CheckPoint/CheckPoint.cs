#region USING

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion


namespace NRTW.Code.Scripts.CheckPoint {
    [RequireComponent(typeof(BoxCollider2D))]
    public class CheckPoint : MonoBehaviour {
        #region FIELDS
        private int id;
        private bool idIsSet;
        
        private bool
            enteredClockwise,
            exitedClockwise,
            enteredCounterсlockwise,
            exitedCounterclockwise,
            passedClockwise,
            passedCounterclockwise; 

        private BoxCollider2D c;
        private RectTransform t;

        private CheckPointSystem cps;
        #endregion


        
        #region PROPERTIES
        public BoxCollider2D Collider => c;

        public int ID {
            get => id;
            set {
                if (idIsSet) {
                    Debug.LogAssertion($"Impossible to set CheckPoint id to {value}. Id is already set to {id}.");
                    return;
                }
                id = value;
                idIsSet = true;
            }
        }

        public bool PassedClockwise {
            get => passedClockwise;
            private set {
                passedClockwise = value;
                cps.Pass(id, true);
            }
        }

        public bool PassedCounterclockwise {
            get => passedCounterclockwise;
            private set {
                passedCounterclockwise = value;
                cps.Pass(id, false);
            }
        }
        #endregion



        #region BUILTIN METHODS
        private void Awake() {
            c = GetComponent<BoxCollider2D>();
            t = GetComponent<RectTransform>();
            cps = GetComponentInParent<CheckPointSystem>();
        }


        private void OnTriggerEnter2D(Collider2D other) {
            if (IsClockwise(other)) {
                enteredClockwise = true;
                if (enteredCounterсlockwise && passedCounterclockwise) {
                    enteredCounterсlockwise = exitedCounterclockwise = passedCounterclockwise = false;
                }
            }
            else {
                enteredCounterсlockwise = true;
                if (enteredClockwise && passedClockwise) {
                    enteredClockwise = exitedClockwise = passedClockwise = false;
                }
            }
        }
        
        
        private void OnTriggerExit2D(Collider2D other) {
            if (!IsClockwise(other)) {
                exitedClockwise = true;
                if (enteredClockwise) {
                    PassedClockwise = true;
                }
            }
            else {
                exitedCounterclockwise = true;
                if (enteredCounterсlockwise) {
                    PassedCounterclockwise = true;
                }
            }
        }
        #endregion



        #region CUSTOM METHODS
        public void Reset() {
            enteredClockwise =
                exitedClockwise =
                    passedClockwise = 
                        enteredCounterсlockwise = 
                            exitedCounterclockwise = 
                                passedCounterclockwise = false;
        }


        private bool IsClockwise(Collider2D collision) {
            var t1 = collision.transform;
            var targetDirection = t.position - t1.position;
            var isClockwise = Vector2.Dot(t1.right, targetDirection) > 0;
            return isClockwise;
        }
        #endregion
    }
}