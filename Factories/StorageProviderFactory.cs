using DocEditor.Interfaces;

namespace DocEditor.Factories;

internal sealed class StorageProviderFactory : KeyedRegistry<IStorageProvider>, IStorageProviderFactory
{
  public StorageProviderFactory Register(string storageType, Func<IStorageProvider> creator)
  {
    this.AddEntry(storageType, creator);
    return this;
  }

  public IStorageProvider Create(string storageType) => this.Resolve(storageType);

  protected override string BuildNotFoundMessage(string key) =>
    $"Хранилище '{key}' не зарегистрировано в StorageProviderFactory.";
}
