using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IRO.Samples.TelegramBotWithAsp
{
    public static class HostSetup
    {
        public static void Setup(string port)
        {
            //Closing previous instances.
            try
            {
                var oldProcesses = Process.GetProcesses()
                    .Where(p =>
                    {
                        try
                        {
                            return p.MainModule.FileName.Contains("ngrok.exe");
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    }).ToList();
                foreach (var proc in oldProcesses)
                {
                    proc.Kill();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Ngrok error: " + ex.ToString());
            }

            var process = new Process();
            process.StartInfo.FileName = "cmd";
            process.StartInfo.Arguments = $" /c call ngrok http -host-header=\"localhost:{port}\" {port} & pause";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            AppDomain.CurrentDomain.ProcessExit += (s, a) =>
            {
                process.Kill();
            };
            var outputStr=process.StandardOutput.ReadLine();
            outputStr+=process.StandardOutput.ReadLine();
            outputStr+=process.StandardOutput.ReadLine();
            outputStr+=process.StandardOutput.ReadLine();
            if (outputStr.Contains("Tunnel session failed"))
            {
                throw new Exception("Ngrok tunnel session failed. Probably you have opened connections.");
            }
            process.WaitForExit();
        }
    }
}
