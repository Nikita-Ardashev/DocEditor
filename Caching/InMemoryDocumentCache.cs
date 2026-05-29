using DocEditor.Interfaces;
using DocEditor.Models;

namespace DocEditor.Caching;

internal sealed class InMemoryDocumentCache : IDocumentCache
{
  private readonly Dictionary<Guid, Document> store = new();
  private readonly ILogger logger;

  public InMemoryDocumentCache(ILogger logger)
  {
    ArgumentNullException.ThrowIfNull(logger);
    this.logger = logger;
  }

  public Document? Find(Guid id)
  {
    if (this.store.TryGetValue(id, out Document? found))
    {
      this.logger.Info($"  [Cache] HIT для {id}");
      return found;
    }
    this.logger.Info($"  [Cache] MISS для {id}");
    return null;
  }

  public void Set(Document document)
  {
    ArgumentNullException.ThrowIfNull(document);
    this.store[document.Id] = document;
    this.logger.Info($"  [Cache] SET для {document.Id}");
  }

  public void Remove(Guid id)
  {
    if (this.store.Remove(id))
    {
      this.logger.Info($"  [Cache] REMOVE для {id}");
    }
  }
}
