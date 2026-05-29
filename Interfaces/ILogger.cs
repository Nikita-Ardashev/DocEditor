namespace DocEditor.Interfaces;

/// <summary>
/// Абстракция логирования. Имя описывает роль (журналирование сообщений),
/// а не реализацию (Console / File / NoOp).
/// </summary>
internal interface ILogger
{
  /// <summary>
  /// Записывает информационное сообщение.
  /// </summary>
  /// <param name="message">Текст сообщения. Не должен быть <c>null</c>.</param>
  void Info(string message);
}
