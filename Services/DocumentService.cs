using DocEditor.Interfaces;
using DocEditor.Models;

namespace DocEditor.Services;

/// <summary>
/// Оркестратор алгоритма просмотра документа. Не знает о конкретных форматах
/// и типах хранилищ — только о контрактах <see cref="IStorageProvider"/> и
/// <see cref="IDocumentHandlerFactory"/>.
/// </summary>
internal sealed class DocumentService
{
  #region Поля и свойства

  private readonly IStorageProvider storage;
  private readonly IDocumentHandlerFactory handlerFactory;
  private readonly ILogger logger;

  #endregion

  #region Методы

  /// <summary>
  /// Выполняет полный цикл просмотра документа: загрузка, открытие, ожидание,
  /// сохранение изменений, очистка временного файла и ресурсов редактора.
  /// </summary>
  /// <param name="id">Идентификатор документа.</param>
  public void ViewDocument(Guid id)
  {
    this.logger.Info(string.Empty);
    this.logger.Info($"=== Просмотр документа {id} ===");

    Document document = this.LoadDocument(id);
    string tempPath = this.SaveToTempFile(document);
    IDocumentHandler handler = this.handlerFactory.Create(document.Format);

    try
    {
      this.RunEditor(handler, tempPath);
      this.PersistChanges(document, tempPath);
    }
    finally
    {
      this.Cleanup(handler, tempPath);
    }

    this.logger.Info($"=== Просмотр документа {id} завершён ===");
  }

  private Document LoadDocument(Guid id)
  {
    Document document = this.storage.Read(id);
    this.logger.Info($"Загружен документ \"{document.Name}\" формата .{document.Format}");
    return document;
  }

  private string SaveToTempFile(Document document)
  {
    string tempPath = Path.Combine(Path.GetTempPath(), $"{document.Id}.{document.Format}");
    File.WriteAllBytes(tempPath, document.Content);
    this.logger.Info($"Документ сохранён в локальный файл: {tempPath}");
    return tempPath;
  }

  private void RunEditor(IDocumentHandler handler, string tempPath)
  {
    handler.Open(tempPath);
    handler.WaitForCompletion();
  }

  private void PersistChanges(Document document, string tempPath)
  {
    Document updated = document with { Content = File.ReadAllBytes(tempPath) };
    this.storage.Save(updated);
    this.logger.Info("Документ сохранён обратно в хранилище.");
  }

  private void Cleanup(IDocumentHandler handler, string tempPath)
  {
    handler.Close();
    if (File.Exists(tempPath))
    {
      File.Delete(tempPath);
      this.logger.Info($"Временный файл удалён: {tempPath}");
    }
  }

  #endregion

  #region Конструкторы

  public DocumentService(IStorageProvider storage, IDocumentHandlerFactory handlerFactory, ILogger logger)
  {
    ArgumentNullException.ThrowIfNull(storage);
    ArgumentNullException.ThrowIfNull(handlerFactory);
    ArgumentNullException.ThrowIfNull(logger);
    this.storage = storage;
    this.handlerFactory = handlerFactory;
    this.logger = logger;
  }

  #endregion
}
