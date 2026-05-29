using System.Text;
using DocEditor.Caching;
using DocEditor.DocumentHandlers;
using DocEditor.DocumentHandlers.Watchers;
using DocEditor.Factories;
using DocEditor.Interfaces;
using DocEditor.Logging;
using DocEditor.Models;
using DocEditor.Services;
using DocEditor.Storage;

Console.OutputEncoding = Encoding.UTF8;

ILogger logger = new ConsoleLogger();

string storageDir = Path.Combine(AppContext.BaseDirectory, "storage");
IDocumentCache cache = new InMemoryDocumentCache(logger);

StorageProviderFactory storageFactory = new StorageProviderFactory()
  .Register("file", () => new CachedStorageProvider(new FileSystemStorageProvider(storageDir, logger), cache))
  .Register("memory", () => new InMemoryStorageProvider(logger));

IStorageProvider storage = storageFactory.Create("file");

DocumentHandlerFactory handlerFactory = new DocumentHandlerFactory()
  .Register("fm1", () => new Fm1DocumentHandler(new ProcessWatcher("fm1Editor.exe", logger), logger))
  .Register("fm2", () => new Fm2DocumentHandler(new ComWatcher("Fm2Editor.Application", logger), logger));

Document fm1Doc = new()
{
  Id = Guid.NewGuid(),
  Name = "report",
  Format = "fm1",
  Content = Encoding.UTF8.GetBytes("Содержимое документа .fm1 (исходное)"),
};

Document fm2Doc = new()
{
  Id = Guid.NewGuid(),
  Name = "specification",
  Format = "fm2",
  Content = Encoding.UTF8.GetBytes("Содержимое документа .fm2 (исходное)"),
};

storage.Save(fm1Doc);
storage.Save(fm2Doc);

DocumentService service = new(storage, handlerFactory, logger);

service.ViewDocument(fm1Doc.Id);
service.ViewDocument(fm2Doc.Id);

logger.Info(string.Empty);
logger.Info("--- Повторный просмотр (должен попасть в кэш) ---");
service.ViewDocument(fm1Doc.Id);
