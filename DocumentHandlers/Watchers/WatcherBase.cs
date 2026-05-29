using DocEditor.Interfaces;

namespace DocEditor.DocumentHandlers.Watchers;

/// <summary>
/// Общая Template-Method-реализация эмулирующего watcher-а:
/// хранит счётчик опросов, реализует CQS (<see cref="IsRunning"/> — чистый запрос, <see cref="Poll"/> — команда),
/// формат сообщения о проверке делегирует наследникам.
/// </summary>
internal abstract class WatcherBase : IEditorProcessWatcher
{
  internal const int DefaultMaxPolls = 3;

  private readonly int maxPolls;
  private readonly ILogger logger;
  private int currentPoll;

  protected WatcherBase(int maxPolls, ILogger logger)
  {
    ArgumentNullException.ThrowIfNull(logger);
    this.maxPolls = maxPolls;
    this.logger = logger;
    this.currentPoll = 0;
  }

  public bool IsRunning => this.currentPoll < this.maxPolls;

  public void Poll()
  {
    if (this.currentPoll < this.maxPolls)
    {
      this.currentPoll++;
    }
    this.logger.Info(this.FormatProbeMessage(this.IsRunning));
  }

  /// <summary>Шаг шаблона: построить сообщение о результате очередного опроса.</summary>
  protected abstract string FormatProbeMessage(bool running);
}
