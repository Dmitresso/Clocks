namespace NRTW.Code.Scripts.OperationsProvider {
    public class WindowsOperationsProvider : SystemOperationsProvider {
        private readonly string cmd = "cmd.exe";
        private readonly string update = "w32TM /config /update";
        private readonly string resync = "w32TM /resync";
        
        
        public override void SynchronizeSystemTime() {
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                FileName = cmd,
                Arguments = update
            };
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}