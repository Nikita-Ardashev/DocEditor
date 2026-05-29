namespace DocEditor.Interfaces;

/// <summary>
/// Фабрика обработчиков документов по строковому ключу формата.
/// </summary>
internal interface IDocumentHandlerFactory
{
  /// <summary>
  /// Создаёт новый экземпляр обработчика для указанного формата.
  /// </summary>
  /// <param name="format">Формат документа (например, <c>"fm1"</c>; точка в начале допустима).</param>
  /// <exception cref="KeyNotFoundException">
  /// Если формат не зарегистрирован.
  /// </exception>
  IDocumentHandler Create(string format);
}
