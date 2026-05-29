namespace DocEditor.Interfaces;

/// <summary>
/// Стратегия работы с приложением-редактором для одного формата документа.
/// Жизненный цикл строго: <see cref="Open"/> → <see cref="WaitForCompletion"/> → <see cref="Close"/>.
/// </summary>
internal interface IDocumentHandler
{
  /// <summary>
  /// Открывает документ в редакторе.
  /// </summary>
  /// <param name="filePath">Полный путь к временному файлу с содержимым.</param>
  void Open(string filePath);

  /// <summary>
  /// Блокирует выполнение до завершения работы редактора (пользователь закрыл документ).
  /// </summary>
  void WaitForCompletion();

  /// <summary>
  /// Освобождает ресурсы редактора (для COM — обязательно).
  /// Метод должен быть идемпотентен.
  /// </summary>
  void Close();
}
