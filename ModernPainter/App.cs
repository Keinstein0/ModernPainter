using ModernPainter.Core.Painter.Data;
using ModernPainter.Core.Painter.Writer;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModernPainter.Core
{
    public class App
    {
        private readonly IWriter _writer;

        public App(IWriter writer)
        {
            _writer = writer;
        }

        public async Task RunApp(Painter.ModernPainter painter)
        {
            // Made this awaitable to handle the application flow cleanly
            ModernImage img = new ModernImage("C:\\Users\\alex\\OneDrive\\Bilder\\Screenshots 1\\Screenshot 2026-06-22 091920.png");
            var r = painter.GetFrame();

            //painter.BlitImage(img, r);
            //painter.Update();
            //Thread.Sleep(10000);

            await YTClient(painter);
        }

        private static async Task YTClient(Painter.ModernPainter painter)
        {
            // Configuration
            Console.Write("Enter youtube url: ");
            string videoUrl = Console.ReadLine();

            int targetFps = 24;
            int maxDurationSeconds = 3000;

            // --- THE SIZE FIX ---
            // Set a target width. ffmpeg will auto-scale the height to maintain the aspect ratio.
            // Adjust this based on your canvas capabilities (e.g., 480, 640, 1280)
            int targetWidth = 640;

            string outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "extracted_frames");
            string tempVideoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp_video.mp4");

            // Setup and clean previous runs
            SetupDirectories(outputDir, tempVideoPath);

            // 1. Fetch the video using yt-dlp
            Console.WriteLine("Fetching video from YouTube...");
            DownloadVideo(videoUrl, tempVideoPath);

            // 2. Slice AND downscale the video into PNG frames via ffmpeg
            Console.WriteLine($"Slicing and resizing first {maxDurationSeconds} seconds into frames...");
            ExtractAndResizeFrames(tempVideoPath, outputDir, targetFps, maxDurationSeconds, targetWidth);

            // Get files and sort them numerically
            string[] frameFiles = Directory.GetFiles(outputDir, "frame_*.png")
                                           .OrderBy(f => f)
                                           .ToArray();

            if (frameFiles.Length == 0)
            {
                Console.WriteLine("No frames extracted. Exiting.");
                return;
            }

            Console.Write("Enter to start playback ");
            Console.ReadLine();

            while (true)
            {
                Console.Clear();

                // Limit buffer size to 2 seconds of video
                int maxBufferSize = targetFps * 2;
                var frameQueue = new ConcurrentQueue<(int Index, ModernImage Image)>();
                var bufferThrottle = new SemaphoreSlim(maxBufferSize, maxBufferSize);
                var cts = new CancellationTokenSource();

                // Background worker to stream lightweight frames from disk
                Task processingTask = Task.Run(async () =>
                {
                    try
                    {
                        for (int i = 0; i < frameFiles.Length; i++)
                        {
                            await bufferThrottle.WaitAsync(cts.Token);
                            var img = new ModernImage(frameFiles[i]);
                            frameQueue.Enqueue((i, img));
                        }
                    }
                    catch (OperationCanceledException) { }
                });

                // --- THE EMPTINESS FIX: BUFFER PRIMING ---
                // Wait until the buffer has a comfortable head start before starting the clock
                Console.WriteLine("Priming frame buffer...");
                int primeTarget = Math.Min(maxBufferSize, frameFiles.Length) / 2;
                while (frameQueue.Count < primeTarget && !processingTask.IsCompleted)
                {
                    await Task.Delay(30);
                }

                Stopwatch playbackClock = Stopwatch.StartNew();
                (int Index, ModernImage Image) currentFrame = default;

                // 4. Playback Loop
                while (true)
                {
                    painter.Clear(new Color("#000000"));
                    var screenRect = painter.GetFrame();

                    double elapsedSeconds = playbackClock.Elapsed.TotalSeconds;
                    int targetFrameIndex = (int)(elapsedSeconds * targetFps);

                    // Out of frames, stop playback loop
                    if (targetFrameIndex >= frameFiles.Length)
                    {
                        break;
                    }

                    // 1. Drop skipped frames if system falls behind
                    while (frameQueue.TryPeek(out var nextFrame) && nextFrame.Index < targetFrameIndex)
                    {
                        if (frameQueue.TryDequeue(out var skipped))
                        {
                            bufferThrottle.Release();
                            (skipped.Image as IDisposable)?.Dispose();
                        }
                    }

                    bool frameUpdated = false;
                    (int Index, ModernImage Image) latestAvailableFrame = default;

                    // Dequeue frames until we find the closest one to our target index
                    while (frameQueue.TryPeek(out var nextFrame) && nextFrame.Index <= targetFrameIndex)
                    {
                        if (frameQueue.TryDequeue(out var taken))
                        {
                            bufferThrottle.Release();

                            // If we already picked up a frame in this loop iteration, dispose it immediately 
                            // because 'taken' is newer and a better match for our current time.
                            if (latestAvailableFrame.Image != null)
                            {
                                (latestAvailableFrame.Image as IDisposable)?.Dispose();
                            }

                            latestAvailableFrame = taken;
                            frameUpdated = true;
                        }
                    }

                    if (frameUpdated)
                    {
                        (currentFrame.Image as IDisposable)?.Dispose();
                        currentFrame = latestAvailableFrame;
                    }

                    // 3. Render the frame
                    if (currentFrame.Image != null)
                    {
                        painter.BlitImage(currentFrame.Image, screenRect);
                    }

                    // Overlay telemetry stats
                    string debugText = $"Engine FPS: {painter.FPS} | Buffer: {frameQueue.Count}/{maxBufferSize} | Frame: {targetFrameIndex}/{frameFiles.Length - 1} | Time: {elapsedSeconds:F2}s";
                    painter.WriteText(new Vector2D(0, 0), debugText, new Color(255, 255, 255));

                    painter.Update();
                }

                // --- CLEANUP ---
                cts.Cancel();
                await processingTask;

                while (frameQueue.TryDequeue(out var leftover))
                {
                    (leftover.Image as IDisposable)?.Dispose();
                }
                (currentFrame.Image as IDisposable)?.Dispose();

                Console.Clear();
                Console.WriteLine("Playback finished. Press Enter to replay...");
                Console.ReadLine();
            }
        }

        private static void SetupDirectories(string outputDir, string videoPath)
        {
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, true);
            Directory.CreateDirectory(outputDir);
            if (File.Exists(videoPath)) File.Delete(videoPath);
        }

        private static void DownloadVideo(string url, string outputPath)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                // Changes: 
                // 1. "worst" or "worstvideo" ensures a small file size (usually 360p/480p).
                // 2. No '+' sign means it grabs a single pre-merged file, completely skipping the heavy muxing phase.
                Arguments = $"-f \"worstvideo[ext=mp4]/worst[ext=mp4]\" -o \"{outputPath}\" \"{url}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true // Changed to true to keep your console clean
            };
            using (Process process = Process.Start(psi)) process?.WaitForExit();
        }

        // Updated to use ffmpeg's video filter scaling feature
        private static void ExtractAndResizeFrames(string videoPath, string outputDir, int fps, int duration, int width)
        {
            string framePattern = Path.Combine(outputDir, "frame_%04d.png");
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                // 'scale=640:-1' tells ffmpeg to force width to 640 and auto-calculate height to prevent stretching
                Arguments = $"-ss 00:00:00 -i \"{videoPath}\" -t {duration} -vf \"scale={width}:-1,fps={fps}\" \"{framePattern}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };
            using (Process process = Process.Start(psi)) process?.WaitForExit();
        }
    }
}