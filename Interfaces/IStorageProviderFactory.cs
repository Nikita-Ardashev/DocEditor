namespace DocEditor.Interfaces;

/// <summary>
/// Фабрика хранилищ документов по строковому ключу типа хранилища.
/// </summary>
internal interface IStorageProviderFactory
{
  /// <summary>
  /// Создаёт новый экземпляр хранилища указанного типа.
  /// </summary>
  /// <param name="storageType">Тип хранилища, например <c>"file"</c> или <c>"memory"</c>.</param>
  /// <exception cref="KeyNotFoundException">
  /// Если тип хранилища не зарегистрирован.
  /// </exception>
  IStorageProvider Create(string storageType);
}
