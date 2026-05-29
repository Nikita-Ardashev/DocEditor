using DocEditor.Interfaces;
using DocEditor.Models;

namespace DocEditor.Storage;

internal sealed class InMemoryStorageProvider : IStorageProvider
{
  private readonly Dictionary<Guid, Document> store = new();
  private readonly ILogger logger;

  public InMemoryStorageProvider(ILogger logger)
  {
    ArgumentNullException.ThrowIfNull(logger);
    this.logger = logger;
  }

  public Document Read(Guid id)
  {
    this.logger.Info($"  [InMemoryStorage] Чтение документа {id}");
    if (!this.store.TryGetValue(id, out Document? doc))
    {
      throw new KeyNotFoundException($"Документ {id} не найден в InMemoryStorage.");
    }
    return Snapshot(doc);
  }

  public void Save(Document document)
  {
    ArgumentNullException.ThrowIfNull(document);
    this.logger.Info($"  [InMemoryStorage] Сохранение документа {document.Id} ({document.Name}.{document.Format})");
    this.store[document.Id] = Snapshot(document);
  }

  public void Delete(Guid id)
  {
    this.logger.Info($"  [InMemoryStorage] Удаление документа {id}");
    this.store.Remove(id);
  }

  private static Document Snapshot(Document source) => source with { Content = (byte[])source.Content.Clone() };
}
