#region USING

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#endregion


namespace NRTW.Code.Scripts.CheckPoint {
    public class CheckPointSystem : MonoBehaviour {
        #region FIELDS
        private List<CheckPoint> checkPoints = new ();
        private List<int> log = new ();
        
        private int rotations, negativeRotations, positiveRotations;
        #endregion



        #region PROPERTIES
        public IList<int> Log => log.AsReadOnly();
        public bool CollidersActive { set { foreach (var cp in checkPoints) cp.Collider.enabled = value; } }
        public int Rotations => rotations;
        #endregion


        
        #region BUILTIN METHODS
        private void Awake() {
            if (checkPoints.Capacity == 0) checkPoints.AddRange(GetComponentsInChildren<CheckPoint>().ToList());
            for (var i = 0; i < checkPoints.Capacity; i++) {
                checkPoints[i].ID = i;
                log.Add(0);
            }
        }


        private void Start() {
            CollidersActive = false;
        }
        #endregion

        

        #region CUSTOM METHODS
        public void Reset() {
            foreach (var cp in checkPoints) cp.Reset();
            for (var i = 0; i < log.Capacity; i++) log[i] = 0;
            rotations = negativeRotations = positiveRotations = 0;
        }
        
        
        public void Pass(int checkPointId, bool isClockwise) {
            switch (isClockwise) {
                case true:
                    log[checkPointId]++;
                    break;
                case false:
                    log[checkPointId]--;
                    break;
            }

            CountRotations();
        }

        
        private void CountRotations() {
            if (log.All(i => i <= 0)) negativeRotations = log.Max();
            if (log.All(i => i >= 0)) positiveRotations = log.Min();
            
            if (negativeRotations != 0) rotations = negativeRotations;
            else if (positiveRotations != 0) rotations = positiveRotations;
            else rotations = 0;
        }
        #endregion
    }
}