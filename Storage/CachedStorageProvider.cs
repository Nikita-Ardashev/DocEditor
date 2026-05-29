using DocEditor.Interfaces;
using DocEditor.Models;

namespace DocEditor.Storage;

/// <summary>
/// Decorator поверх произвольного <see cref="IStorageProvider"/>: добавляет
/// прозрачное cache-aside кеширование через <see cref="IDocumentCache"/>.
/// </summary>
internal sealed class CachedStorageProvider : IStorageProvider
{
  #region Поля и свойства

  private readonly IStorageProvider inner;
  private readonly IDocumentCache cache;

  #endregion

  #region Методы

  public Document Read(Guid id)
  {
    Document? cached = this.cache.Find(id);
    if (cached is not null)
    {
      return cached;
    }
    Document document = this.inner.Read(id);
    this.cache.Set(document);
    return document;
  }

  public void Save(Document document)
  {
    ArgumentNullException.ThrowIfNull(document);
    this.inner.Save(document);
    this.cache.Set(document);
  }

  public void Delete(Guid id)
  {
    this.cache.Remove(id);
    this.inner.Delete(id);
  }

  #endregion

  #region Конструкторы

  public CachedStorageProvider(IStorageProvider inner, IDocumentCache cache)
  {
    ArgumentNullException.ThrowIfNull(inner);
    ArgumentNullException.ThrowIfNull(cache);
    this.inner = inner;
    this.cache = cache;
  }

  #endregion
}
