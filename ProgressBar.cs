using System;
using System.Text;
using System.Threading;

namespace State_Downloader
{
    /// <summary>
    /// An ASCII progress bar
    /// <see cref="https://gist.github.com/DanielSWolf/0ab6a96899cc5377bf54"/>
    /// </summary>
    public sealed class ProgressBar : IDisposable, IProgress<double>
    {
        private int BlockCount => Math.Max(narrowProgressBar ? Console.WindowWidth / 2 - (appendText.Length + prependText.Length) : Console.WindowWidth - (appendText.Length + prependText.Length), 10); //20;
        private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 8);
        private const string animation = @"|/-\";

        private readonly Timer timer;

        private double currentProgress = 0;
        private string currentText = string.Empty;
        private string prependText = string.Empty;
        private string appendText = string.Empty;
        private bool disposed = false;
        private int animationIndex = 0;
        private bool narrowProgressBar = false;

        public ProgressBar(bool narrowProgressBar = false)
        {
            timer = new Timer(TimerHandler);
            this.narrowProgressBar = narrowProgressBar;

            // A progress bar is only for temporary display in a console window.
            // If the console output is redirected to a file, draw nothing.
            // Otherwise, we'll end up with a lot of garbage in the target file.
            if (!Console.IsOutputRedirected)
            {
                ResetTimer();
            }
        }

        public void Report(double decimalPercent)
        {
            // Make sure value is in [0..1] range
            decimalPercent = Math.Max(0, Math.Min(1, decimalPercent));
            Interlocked.Exchange(ref currentProgress, decimalPercent);
        }

        public void Report(double decimalPercent, string prependText = "", string appendText = "")
        {
            // Make sure value is in [0..1] range
            decimalPercent = Math.Max(0, Math.Min(1, decimalPercent));
            Interlocked.Exchange(ref currentProgress, decimalPercent);
            Interlocked.Exchange(ref this.prependText, prependText);
            Interlocked.Exchange(ref this.appendText, appendText);
        }

        private void TimerHandler(object? state)
        {
            lock (timer)
            {
                if (disposed) return;

                int count = BlockCount;
                int progressBlockCount = (int)(currentProgress * count);
                int percent = (int)(currentProgress * 100);
                string text = $"[{new string('#', progressBlockCount)}{new string('-', count - progressBlockCount)}] {percent,3}% {animation[animationIndex++ % animation.Length]}";
                UpdateText(text);

                ResetTimer();
            }
        }

        private void UpdateText(string text)
        {
            // Backtrack to the first differing character
            StringBuilder outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', currentText.Length);

            // Output new suffix
            outputBuilder
                .Append(prependText)
                .Append(text)
                .Append(appendText);

            // If the new text is shorter than the old one: delete overlapping characters
            int overlapCount = currentText.Length - (prependText.Length + text.Length + appendText.Length);
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }



            Console.Write(outputBuilder);
            currentText = prependText + text + appendText;
        }

        private void ResetTimer()
        {
            timer.Change(animationInterval, TimeSpan.FromMilliseconds(-1));
        }

        public void Dispose()
        {
            lock (timer)
            {
                disposed = true;
                UpdateText(string.Empty);
            }
        }

    }
}
