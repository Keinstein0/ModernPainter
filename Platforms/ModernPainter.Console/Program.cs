using ModernPainter.Console;
using ModernPainter.Console.Writer;
using ModernPainter.Core;
using ModernPainter.Core.Painter.Writer;

IWriter writer = new ConsoleWriter();

App engine = new App(writer);
ModernPainter.Core.Painter.ModernPainter painter = new(writer);

await engine.RunApp(painter);