using ModernPainter.Core.Painter.Reader;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModernPainter.Console.Reader
{
    internal class ConsoleTrace : KeyboardTrace
    {
        private CancellationTokenSource _cts;
        private Task _readerTask;
        private bool _isEnded;

        public async Task Start()
        {
            if (_readerTask != null) return; // Prevent double starting

            _cts = new CancellationTokenSource();
            
            // Spin up the background reader loop
            _readerTask = Task.Run(() => ReadLoop(_cts.Token));
            
            await Task.CompletedTask;
        }

        private void ReadLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Check if a key was pressed without blocking execution endlessly
                if (System.Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = System.Console.ReadKey(intercept: true);
                    
                    // Call Update to process the key press
                    Update(keyInfo);
                }
                else
                {
                    // Small sleep to prevent high CPU usage in the polling loop
                    Thread.Sleep(10);
                }
            }
        }

        public void Update(ConsoleKeyInfo keyInfo)
        {
            // Filter out NULL / dead characters that terminals send on paste
            if (keyInfo.KeyChar == '\0') return;

            // Handle Newlines cleanly (pasted text often sends \r or \n)
            if (keyInfo.Key == ConsoleKey.Enter || keyInfo.KeyChar == '\r' || keyInfo.KeyChar == '\n')
            {
                // Decide how you want to store newlines, or append a space/newline safely:
                Content += "\n";
                return;
            }

            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (Content.Length > 0)
                {
                    Content = Content.Substring(0, Content.Length - 1);
                }
                return;
            }

            // Ignore other control keys (Ctrl combinations, escape codes, etc.)
            if (char.IsControl(keyInfo.KeyChar))
            {
                return;
            }

            Content += keyInfo.KeyChar;
        }

        public override void End()
        {
            if (_isEnded) return;
            _isEnded = true;

            // Signal the loop to stop and clean up task resources
            _cts?.Cancel();
            try
            {
                _readerTask?.Wait(200); // Give the task a brief window to complete cleanly
            }
            catch (AggregateException) { /* Handle cancellation exception on wait */ }

            _cts?.Dispose();
        }
    }
}