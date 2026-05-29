using DocEditor.Abstracts;
using DocEditor.Interfaces;

namespace DocEditor.DocumentHandlers;

internal sealed class Fm1DocumentHandler : DocumentHandlerBase
{
  private const string ExecutableName = "Fm1Editor.exe";

  public Fm1DocumentHandler(IEditorProcessWatcher watcher, ILogger logger)
    : base(watcher, logger) { }

  protected override string EditorName => "Fm1Editor";

  protected override void OpenEditor(string filePath)
  {
    this.Logger.Info($"  [Fm1] Запуск процесса {ExecutableName} \"{filePath}\"...");
  }

  protected override void ReleaseResources()
  {
    this.Logger.Info($"  [Fm1] Процесс {ExecutableName} завершён, ресурсы освобождены.");
  }
}
