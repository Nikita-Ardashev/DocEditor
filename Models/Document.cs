namespace DocEditor.Models;

/// <summary>
/// Документ, перемещаемый между хранилищем и редактором.
/// Immutable: для замены содержимого используйте выражение <c>with</c>.
/// </summary>
internal sealed record Document
{
  /// <summary>Уникальный идентификатор документа.</summary>
  public Guid Id { get; init; }

  /// <summary>Человеко-читаемое имя документа (без расширения).</summary>
  public string Name { get; init; } = string.Empty;

  /// <summary>Формат документа без точки, например <c>"fm1"</c>.</summary>
  public string Format { get; init; } = string.Empty;

  /// <summary>Бинарное содержимое документа.</summary>
  public byte[] Content { get; init; } = [];
}
