namespace DocEditor.Factories;

/// <summary>
/// Общая база для фабрик, регистрирующих создателей продуктов по строковому ключу.
/// Гарантирует одинаковую семантику ключей (нормализация, регистронезависимость)
/// и единое сообщение об ошибке во всех фабриках проекта.
/// </summary>
/// <typeparam name="TProduct">Тип создаваемого продукта.</typeparam>
internal abstract class KeyedRegistry<TProduct>
  where TProduct : class
{
  private readonly Dictionary<string, Func<TProduct>> registry = new(StringComparer.OrdinalIgnoreCase);

  protected void AddEntry(string key, Func<TProduct> creator)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(key);
    ArgumentNullException.ThrowIfNull(creator);
    this.registry[NormalizeKey(key)] = creator;
  }

  protected TProduct Resolve(string key)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(key);
    if (!this.registry.TryGetValue(NormalizeKey(key), out Func<TProduct>? creator))
    {
      throw new KeyNotFoundException(this.BuildNotFoundMessage(key));
    }
    return creator();
  }

  /// <summary>Шаг шаблона: текст исключения, специфичный для конкретной фабрики.</summary>
  protected abstract string BuildNotFoundMessage(string key);

  private static string NormalizeKey(string key) => key.TrimStart('.').ToLowerInvariant();
}
