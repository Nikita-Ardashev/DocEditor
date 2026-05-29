using DocEditor.Interfaces;

namespace DocEditor.DocumentHandlers.Watchers;

internal sealed class ProcessWatcher : WatcherBase
{
  private readonly string processName;

  public ProcessWatcher(string processName, ILogger logger)
    : this(processName, DefaultMaxPolls, logger) { }

  public ProcessWatcher(string processName, int maxPolls, ILogger logger)
    : base(maxPolls, logger)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(processName);
    this.processName = processName;
  }

  protected override string FormatProbeMessage(bool running) =>
    $"  [ProcessWatcher] Проверка процесса '{this.processName}' в памяти: {(running ? "найден" : "не найден")}";
}
