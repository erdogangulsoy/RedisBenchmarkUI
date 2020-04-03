using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RedisBenchmarkUI.Hubs
{
    public class StreamHub : Hub
    {
        ChannelWriter<string> writer = null;
        public ChannelReader<string> RunCommand(string cmd)
        {
            var channel = Channel.CreateUnbounded<string>();
            writer = channel.Writer;

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            var isLinux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            if (isLinux)
            {
                process.StartInfo.FileName = "/bin/bash";
                process.StartInfo.Arguments = $"-c \"{cmd.Replace("\"", "\\\"")}\"";
            }
            else
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c {cmd}";
            }

            process.OutputDataReceived += ReadOutputHandler;
          
          
            process.Exited += Process_Exited;


            process.Start();
            process.BeginOutputReadLine();

            return channel.Reader;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            writer.TryComplete();
        }

       
        private async void ReadOutputHandler(object sendingProcess, DataReceivedEventArgs data)
        {
            if (!String.IsNullOrEmpty(data.Data))
            {
                Debug.WriteLine(data.Data);
                await writer.WriteAsync(data.Data);
                await Task.Delay(500);
                if (((Process)sendingProcess).HasExited)
                {
                    writer.TryComplete();
                }
            }
        }
    }
}
