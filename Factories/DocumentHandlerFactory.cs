using DocEditor.Interfaces;

namespace DocEditor.Factories;

internal sealed class DocumentHandlerFactory : KeyedRegistry<IDocumentHandler>, IDocumentHandlerFactory
{
  public DocumentHandlerFactory Register(string format, Func<IDocumentHandler> creator)
  {
    this.AddEntry(format, creator);
    return this;
  }

  public IDocumentHandler Create(string format)
  {
    return this.Resolve(format);
  }

  protected override string BuildNotFoundMessage(string key)
  {
    return $"Формат '{key}' не зарегистрирован в DocumentHandlerFactory.";
  }
}
