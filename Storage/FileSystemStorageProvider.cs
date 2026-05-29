using System.Text.Json;
using DocEditor.Interfaces;
using DocEditor.Models;

namespace DocEditor.Storage;

internal sealed class FileSystemStorageProvider : IStorageProvider
{
  #region Константы

  private const string FileExtension = ".json";

  #endregion

  #region Поля и свойства

  private readonly string baseDirectory;
  private readonly ILogger logger;

  #endregion

  #region Методы

  public Document Read(Guid id)
  {
    string path = this.PathFor(id);
    this.logger.Info($"  [FileStorage] Чтение {path}");
    if (!File.Exists(path))
    {
      throw new FileNotFoundException($"Документ {id} не найден в файловом хранилище.", path);
    }

    string json = File.ReadAllText(path);
    Document? document = JsonSerializer.Deserialize<Document>(json);
    if (document is null)
    {
      throw new InvalidDataException($"Не удалось десериализовать документ из файла {path}.");
    }
    return document;
  }

  public void Save(Document document)
  {
    ArgumentNullException.ThrowIfNull(document);
    string path = this.PathFor(document.Id);
    this.logger.Info($"  [FileStorage] Запись {path}");
    string json = JsonSerializer.Serialize(document);
    File.WriteAllText(path, json);
  }

  public void Delete(Guid id)
  {
    string path = this.PathFor(id);
    this.logger.Info($"  [FileStorage] Удаление {path}");
    if (File.Exists(path))
    {
      File.Delete(path);
    }
  }

  private string PathFor(Guid id) => Path.Combine(this.baseDirectory, $"{id}{FileExtension}");

  #endregion

  #region Конструкторы

  public FileSystemStorageProvider(string baseDirectory, ILogger logger)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(baseDirectory);
    ArgumentNullException.ThrowIfNull(logger);
    this.baseDirectory = baseDirectory;
    this.logger = logger;
    Directory.CreateDirectory(this.baseDirectory);
  }

  #endregion
}
