using DocEditor.Abstracts;
using DocEditor.Interfaces;

namespace DocEditor.DocumentHandlers;

internal sealed class Fm2DocumentHandler : DocumentHandlerBase
{
  private const string ComProgId = "Fm2Editor.Application";

  public Fm2DocumentHandler(IEditorProcessWatcher watcher, ILogger logger)
    : base(watcher, logger) { }

  protected override string EditorName => "Fm2Editor";

  protected override void OpenEditor(string filePath)
  {
    this.Logger.Info($"  [Fm2] Создание COM-объекта {ComProgId}...");
    this.Logger.Info($"  [Fm2] Вызов app.OpenDocument(\"{filePath}\")...");
  }

  protected override void ReleaseResources()
  {
    this.Logger.Info("  [Fm2] Вызов Marshal.ReleaseComObject(app).");
    this.Logger.Info("  [Fm2] COM-объект Fm2Editor освобождён.");
  }
}
