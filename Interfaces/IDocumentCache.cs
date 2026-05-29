using DocEditor.Models;

namespace DocEditor.Interfaces;

/// <summary>
/// Кеш документов. В отличие от <see cref="IStorageProvider"/> допускает «промах» — это нормальное состояние.
/// </summary>
internal interface IDocumentCache
{
  /// <summary>
  /// Возвращает документ из кеша или <c>null</c>, если его там нет.
  /// </summary>
  /// <param name="id">Идентификатор документа.</param>
  Document? Find(Guid id);

  /// <summary>
  /// Помещает документ в кеш. Если запись с таким Id уже есть — перезаписывает.
  /// </summary>
  /// <param name="document">Документ для кеширования.</param>
  void Set(Document document);

  /// <summary>
  /// Удаляет документ из кеша. Если документа нет — операция игнорируется.
  /// </summary>
  /// <param name="id">Идентификатор документа.</param>
  void Remove(Guid id);
}
