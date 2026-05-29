using DocEditor.Interfaces;

namespace DocEditor.Abstracts;

/// <summary>
/// Шаблонный (Template Method) базовый класс для обработчиков документов.
/// Фиксирует общий каркас: логирование + опрос watcher-а;
/// специфика открытия и освобождения ресурсов выносится в абстрактные хуки.
/// </summary>
internal abstract class DocumentHandlerBase : IDocumentHandler
{
  #region Константы

  private const int WaitIterationDelayMs = 300;

  #endregion

  #region Поля и свойства

  private readonly IEditorProcessWatcher watcher;

  /// <summary>Логгер, доступный наследникам для специфичных сообщений.</summary>
  protected ILogger Logger { get; }

  /// <summary>Имя редактора для отображения в логах.</summary>
  protected abstract string EditorName { get; }

  #endregion

  #region Методы

  public void Open(string filePath)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
    this.Logger.Info($"[{this.EditorName}] Открытие документа: {filePath}");
    this.OpenEditor(filePath);
  }

  public void WaitForCompletion()
  {
    int attempt = 1;
    while (true)
    {
      this.watcher.Poll();
      if (!this.watcher.IsRunning)
      {
        break;
      }
      this.Logger.Info($"[{this.EditorName}] Ожидание завершения работы (попытка {attempt})...");
      Thread.Sleep(WaitIterationDelayMs);
      attempt++;
    }
    this.Logger.Info($"[{this.EditorName}] Работа приложения завершена.");
  }

  public void Close()
  {
    this.Logger.Info($"[{this.EditorName}] Освобождение ресурсов...");
    this.ReleaseResources();
  }

  /// <summary>Шаг шаблона: запустить редактор для указанного файла.</summary>
  protected abstract void OpenEditor(string filePath);

  /// <summary>Шаг шаблона: освободить ресурсы, специфичные для редактора.</summary>
  protected abstract void ReleaseResources();

  #endregion

  #region Конструкторы

  protected DocumentHandlerBase(IEditorProcessWatcher watcher, ILogger logger)
  {
    ArgumentNullException.ThrowIfNull(watcher);
    ArgumentNullException.ThrowIfNull(logger);
    this.watcher = watcher;
    this.Logger = logger;
  }

  #endregion
}
