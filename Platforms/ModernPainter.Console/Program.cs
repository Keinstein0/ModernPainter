using ModernPainter.Console;
using ModernPainter.Console.Reader;
using ModernPainter.Console.Writer;
using ModernPainter.Core;
using ModernPainter.Core.Painter.Reader;
using ModernPainter.Core.Painter.Writer;

IWriter writer = new ConsoleWriter();
IReader reader = new ConsoleReader();

ModernPainter.Core.Painter.ModernPainter painter = new(writer, reader);

App engine = new App();
await engine.RunApp(painter);