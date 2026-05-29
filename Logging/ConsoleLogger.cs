using DocEditor.Interfaces;

namespace DocEditor.Logging;

internal sealed class ConsoleLogger : ILogger
{
  public void Info(string message)
  {
    ArgumentNullException.ThrowIfNull(message);
    Console.WriteLine(message);
  }
}
