using DocEditor.Models;

namespace DocEditor.Interfaces;

/// <summary>
/// Стратегия персистентного хранилища документов.
/// Реализации не должны кешировать — за кеш отвечает <see cref="IDocumentCache"/>
/// и Decorator <c>CachedStorageProvider</c>.
/// </summary>
internal interface IStorageProvider
{
  /// <summary>
  /// Возвращает документ по идентификатору.
  /// </summary>
  /// <param name="id">Идентификатор документа.</param>
  /// <returns>Найденный документ.</returns>
  /// <exception cref="KeyNotFoundException">
  /// Если документ отсутствует в хранилище.
  /// </exception>
  Document Read(Guid id);

  /// <summary>
  /// Сохраняет (создаёт или обновляет) документ.
  /// </summary>
  /// <param name="document">Документ для сохранения. Не <c>null</c>.</param>
  void Save(Document document);

  /// <summary>
  /// Удаляет документ. Если документа нет — операция считается успешной.
  /// </summary>
  /// <param name="id">Идентификатор документа.</param>
  void Delete(Guid id);
}
