using System;
using System.Diagnostics;


namespace NRTW.Code.Scripts {
    public static class Timer {
        #region FIELDS
        private static Stopwatch stopwatch = new ();
        #endregion


        
        #region METHODS
        public static void Start() {
            stopwatch.Start();
        }


        public static void Stop() {
            stopwatch.Stop();
        }


        public static TimeSpan GetTime() {
            var time = stopwatch.Elapsed;
            stopwatch.Reset();
            return time;
        }
        #endregion
    }
}