using System.Diagnostics;
using System.ServiceProcess;

namespace TradeSync.Desktop.Logic
{
    public class ServiceAdmin
    {
        // Ім'я служби має співпадати з тим, що вказано в Service/Program.cs
        private const string ServiceName = "TradeSyncService";

        public string GetStatus()
        {
            try
            {
                using (var sc = new ServiceController(ServiceName))
                {
                    return sc.Status.ToString(); // "Running", "Stopped" тощо
                }
            }
            catch
            {
                return "Not Installed";
            }
        }

        public void Start()
        {
            using (var sc = new ServiceController(ServiceName))
            {
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }
            }
        }

        public void Stop()
        {
            using (var sc = new ServiceController(ServiceName))
            {
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                }
            }
        }

        public void Install(string exePath)
        {
            if (!File.Exists(exePath)) throw new FileNotFoundException("Файл сервісу не знайдено", exePath);

            // sc.exe вимагає пробіл після "binPath="
            RunScCommand($"create {ServiceName} binPath= \"{exePath}\" start= auto");
        }

        public void Uninstall()
        {
            // Спочатку зупиняємо, якщо запущено
            try { Stop(); } catch { }
            RunScCommand($"delete {ServiceName}");
        }

        private void RunScCommand(string args)
        {
            var psi = new ProcessStartInfo("sc.exe", args)
            {
                UseShellExecute = true,
                Verb = "runas", // Обов'язково: запит прав Адміністратора
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };

            var p = Process.Start(psi);
            if (p != null) p.WaitForExit();
        }
    }
}