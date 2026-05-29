# Паттерны и принципы в проекте DocEditor

Краткая карта применённых паттернов и принципов SOLID с указанием конкретных файлов.

## Принципы SOLID

| Принцип                                  | Где применён                                                                                                                                                                                                                                                                                                                                                                                     |
| ---------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Принцип единственной ответственности** | [Document](Models/Document.cs) — только данные; [InMemoryDocumentCache](Caching/InMemoryDocumentCache.cs) — только кеш; [FileSystemStorageProvider](Storage/FileSystemStorageProvider.cs) — только I/O; [ConsoleLogger](Logging/ConsoleLogger.cs) — только вывод; [DocumentService](Services/DocumentService.cs) — только оркестрация                                                            |
| **Принцип открытости-закрытости**        | Добавление нового формата (`.fm3`) = новый класс-наследник [DocumentHandlerBase](Abstracts/DocumentHandlerBase.cs) + одна строка `.Register(...)` в [Program.cs](Program.cs). Код [DocumentService](Services/DocumentService.cs) не изменяется                                                                                                                                                   |
| **Принцип подстановки Лисков**           | [Fm1DocumentHandler](DocumentHandlers/Fm1DocumentHandler.cs) и [Fm2DocumentHandler](DocumentHandlers/Fm2DocumentHandler.cs) взаимозаменяемы через [IDocumentHandler](Interfaces/IDocumentHandler.cs); [ProcessWatcher](DocumentHandlers/Watchers/ProcessWatcher.cs) и [ComWatcher](DocumentHandlers/Watchers/ComWatcher.cs) — через [IEditorProcessWatcher](Interfaces/IEditorProcessWatcher.cs) |
| **Принцип разделения интерфейсов**       | [IDocumentHandler](Interfaces/IDocumentHandler.cs) отделён от [IEditorProcessWatcher](Interfaces/IEditorProcessWatcher.cs); [IDocumentCache](Interfaces/IDocumentCache.cs) не наследует [IStorageProvider](Interfaces/IStorageProvider.cs), потому что у них разная семантика «не найдено»                                                                                                       |
| **Принцип инверсии зависимостей**        | [DocumentService](Services/DocumentService.cs), [DocumentHandlerBase](Abstracts/DocumentHandlerBase.cs), [CachedStorageProvider](Storage/CachedStorageProvider.cs) — все зависимости через конструктор и только через интерфейсы. Никаких `new ConcreteClass()` внутри бизнес-логики                                                                                                             |

## Разделение команды и запроса

[IEditorProcessWatcher](Interfaces/IEditorProcessWatcher.cs) разделяет:

- **Запрос** — `bool IsRunning { get; }` — чистый запрос без побочных эффектов
- **Команда** — `void Poll()` — продвигает внутреннее состояние

Реализация в [WatcherBase](DocumentHandlers/Watchers/WatcherBase.cs).

## Паттерны проектирования

### Стратегия

| Где                                                                                                                                                                                   | Зачем                                                                                   |
| ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------- |
| [IDocumentHandler](Interfaces/IDocumentHandler.cs) → [Fm1DocumentHandler](DocumentHandlers/Fm1DocumentHandler.cs), [Fm2DocumentHandler](DocumentHandlers/Fm2DocumentHandler.cs)       | Подменять способ работы с редактором в зависимости от формата документа                 |
| [IStorageProvider](Interfaces/IStorageProvider.cs) → [FileSystemStorageProvider](Storage/FileSystemStorageProvider.cs), [InMemoryStorageProvider](Storage/InMemoryStorageProvider.cs) | Подменять тип хранилища (файлы / память / БД / S3)                                      |
| [IEditorProcessWatcher](Interfaces/IEditorProcessWatcher.cs) → [ProcessWatcher](DocumentHandlers/Watchers/ProcessWatcher.cs), [ComWatcher](DocumentHandlers/Watchers/ComWatcher.cs)   | Подменять способ отслеживания (по процессу / через COM) —**Стратегия внутри Стратегии** |

### Шаблонный метод

| Где                                                     | Что фиксируется                                                                                                              |
| ------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| [DocumentHandlerBase](Abstracts/DocumentHandlerBase.cs) | Каркас `Open → WaitForCompletion → Close` + общий цикл опроса. Наследники реализуют только `OpenEditor` и `ReleaseResources` |
| [WatcherBase](DocumentHandlers/Watchers/WatcherBase.cs) | Алгоритм опроса со счётчиком и разделением команды/запроса. Наследники реализуют только `FormatProbeMessage`                 |
| [KeyedRegistry&lt;T&gt;](Factories/KeyedRegistry.cs)    | Алгоритм регистрации/резолва ключей + нормализация. Наследники реализуют только `BuildNotFoundMessage`                       |

### Фабричный метод

| Где                                                                                                                              | Зачем                                                                        |
| -------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------- |
| [IDocumentHandlerFactory](Interfaces/IDocumentHandlerFactory.cs) → [DocumentHandlerFactory](Factories/DocumentHandlerFactory.cs) | Создание обработчика по строковому ключу формата без знания конкретного типа |
| [IStorageProviderFactory](Interfaces/IStorageProviderFactory.cs) → [StorageProviderFactory](Factories/StorageProviderFactory.cs) | Создание хранилища по строковому ключу типа                                  |

Обе фабрики используют общий реестр `Dictionary<string, Func<TProduct>>` — рецепты создания, не готовые объекты. Каждый `Create` производит новый экземпляр.

### Декоратор

[CachedStorageProvider](Storage/CachedStorageProvider.cs) реализует [IStorageProvider](Interfaces/IStorageProvider.cs) и оборачивает другой [IStorageProvider](Interfaces/IStorageProvider.cs), добавляя cache-aside кеширование через [IDocumentCache](Interfaces/IDocumentCache.cs). Снаружи неотличим от обычного провайдера — [DocumentService](Services/DocumentService.cs) о кеше не знает.

### Корень композиции

[Program.cs](Program.cs) — единственное место в проекте, где живут `new` для конкретных типов. Собирает граф зависимостей: логгер → кеш → фабрика хранилищ → хранилище (file + декоратор) → фабрика обработчиков → сервис.

### Внедрение через конструктор

Все классы получают зависимости через конструктор:

- [DocumentService](Services/DocumentService.cs) ← `IStorageProvider`, `IDocumentHandlerFactory`, `ILogger`
- [DocumentHandlerBase](Abstracts/DocumentHandlerBase.cs) ← `IEditorProcessWatcher`, `ILogger`
- [CachedStorageProvider](Storage/CachedStorageProvider.cs) ← `IStorageProvider`, `IDocumentCache`

