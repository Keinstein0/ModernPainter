using ModernPainter.Painter;
using ModernPainter.Painter.Data;
using ModernPainter.Painter.Writer;
using ModernPainter.Painter.Writer.ConsoleWriter;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ModernPainter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IWriter writer = new ConsoleWriter();

            ModernPainter.Painter.ModernPainter painter = new(writer);

            var rect = painter.GetFrame();
            painter.FillRectangle(rect, new Color("004a00"));
            painter.Update();
            Thread.Sleep(10000);
        }


        private static void YTClient()
        {
            // Configuration
            Console.Write("Enter youtube url: ");
            string videoUrl = Console.ReadLine();


            int targetFps = 24;
            int maxDurationSeconds = 30;

            string outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "extracted_frames");
            string tempVideoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp_video.mp4");

            // Setup and clean previous runs
            SetupDirectories(outputDir, tempVideoPath);

            // 1. Fetch the video using yt-dlp
            Console.WriteLine("Fetching video from YouTube...");
            DownloadVideo(videoUrl, tempVideoPath);

            // 2. Slice the video into PNG frames via ffmpeg
            Console.WriteLine($"Slicing first {maxDurationSeconds} seconds into frames...");
            ExtractFrames(tempVideoPath, outputDir, targetFps, maxDurationSeconds);

            // 3. Pre-load all frames into memory (The Performance Fix)
            Console.WriteLine("Loading frames into RAM cache...");

            // Get files and sort them numerically to ensure correct playback order
            string[] frameFiles = Directory.GetFiles(outputDir, "frame_*.png")
                                           .OrderBy(f => f)
                                           .ToArray();

            ModernImage[] frameCache = new ModernImage[frameFiles.Length];
            for (int i = 0; i < frameFiles.Length; i++)
            {
                frameCache[i] = new ModernImage(frameFiles[i]);
            }

            Console.WriteLine($"Successfully cached {frameCache.Length} frames. Starting playback...");
            System.Threading.Thread.Sleep(1000); // Brief pause to let user see status

            Console.Write("Enter to start playback ");
            Console.ReadLine();
            while (true)
            {



                // 4. Playback Loop
                Console.Clear();
                ModernPainter.Painter.ModernPainter painter = new(new ConsoleWriter());
                Stopwatch playbackClock = Stopwatch.StartNew();

                while (true)
                {
                    painter.Clear(new Color("#000000"));
                    var screenRect = painter.GetFrame();

                    double elapsedSeconds = playbackClock.Elapsed.TotalSeconds;

                    // Using 0-based indexing now to align perfectly with the array
                    int targetFrameIndex = (int)(elapsedSeconds * targetFps);

                    // Check if we still have frames left in our RAM cache
                    if (targetFrameIndex < frameCache.Length)
                    {
                        // Instant RAM lookup - no disk reads, no allocations!
                        ModernImage currentFrame = frameCache[targetFrameIndex];
                        painter.BlitImage(currentFrame, screenRect);
                    }
                    else
                    {
                        // Out of frames, loop or stop
                        break;
                    }

                    // Overlay telemetry stats
                    string debugText = $"Engine FPS: {painter.FPS} | Cached Frame: {targetFrameIndex}/{frameCache.Length - 1} | Time: {elapsedSeconds:F2}s";
                    painter.WriteText(new Vector2D(0, 0), debugText, new Color(255, 255, 255));

                    painter.Update();
                }

                Console.Clear();
                Console.WriteLine("Playback finished.");
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
                Arguments = $"-f \"bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]\" -o \"{outputPath}\" \"{url}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };
            using (Process process = Process.Start(psi)) process?.WaitForExit();
        }

        private static void ExtractFrames(string videoPath, string outputDir, int fps, int duration)
        {
            string framePattern = Path.Combine(outputDir, "frame_%04d.png");
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-ss 00:00:00 -i \"{videoPath}\" -t {duration} -vf \"fps={fps}\" \"{framePattern}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };
            using (Process process = Process.Start(psi)) process?.WaitForExit();
        }
    }
}