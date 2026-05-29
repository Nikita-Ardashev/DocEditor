using DocEditor.Interfaces;

namespace DocEditor.DocumentHandlers.Watchers;

internal sealed class ComWatcher : WatcherBase
{
  private readonly string comServerName;

  public ComWatcher(string comServerName, ILogger logger)
    : this(comServerName, DefaultMaxPolls, logger) { }

  public ComWatcher(string comServerName, int maxPolls, ILogger logger)
    : base(maxPolls, logger)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(comServerName);
    this.comServerName = comServerName;
  }

  protected override string FormatProbeMessage(bool running) =>
    $"  [ComWatcher] Опрос COM-интерфейса '{this.comServerName}.IsBusy': {(running ? "true" : "false")}";
}
