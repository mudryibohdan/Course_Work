using System.Text.Json;
using System.Text.Json.Serialization;
using Wallet.Domain.Abstractions;
using Wallet.Domain.Exceptions;
using Wallet.Domain.Repositories;

namespace Wallet.DataAccess.Json;

public class JsonRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IIdentifiable
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _mutex = new(1, 1);
    private readonly JsonSerializerOptions _serializerOptions;

    public JsonRepository(string filePath, JsonSerializerOptions? serializerOptions = null)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _serializerOptions = serializerOptions ?? CreateDefaultOptions();
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await ExecuteAsync(async list =>
        {
            if (list.Any(e => e.Id == entity.Id))
            {
                throw new DataStoreException("Сутність з таким ідентифікатором вже існує.");
            }

            list.Add(entity);
        }, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(list =>
        {
            var index = list.FindIndex(e => e.Id == id);
            if (index >= 0)
            {
                list.RemoveAt(index);
            }
        }, cancellationToken);
    }

    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await ReadAsync(cancellationToken);
        return list.AsReadOnly();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var list = await ReadAsync(cancellationToken);
        return list.FirstOrDefault(e => e.Id == id);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await ExecuteAsync(list =>
        {
            var index = list.FindIndex(e => e.Id == entity.Id);
            if (index == -1)
            {
                throw new DataStoreException("Сутність для оновлення не знайдено.");
            }

            list[index] = entity;
        }, cancellationToken);
    }

    protected async Task ExecuteAsync(Func<List<TEntity>, Task> updater, CancellationToken cancellationToken)
    {
        await _mutex.WaitAsync(cancellationToken);
        try
        {
            var list = await ReadInternalAsync(cancellationToken);
            await updater(list);
            await WriteInternalAsync(list, cancellationToken);
        }
        catch (IOException ex)
        {
            throw new DataStoreException("Помилка доступу до файлу даних.", ex);
        }
        finally
        {
            _mutex.Release();
        }
    }

    protected async Task ExecuteAsync(Action<List<TEntity>> updater, CancellationToken cancellationToken)
    {
        await ExecuteAsync(list =>
        {
            updater(list);
            return Task.CompletedTask;
        }, cancellationToken);
    }

    protected async Task<List<TEntity>> ReadAsync(CancellationToken cancellationToken)
    {
        await _mutex.WaitAsync(cancellationToken);
        try
        {
            return await ReadInternalAsync(cancellationToken);
        }
        catch (IOException ex)
        {
            throw new DataStoreException("Помилка читання файлу даних.", ex);
        }
        finally
        {
            _mutex.Release();
        }
    }

    private async Task<List<TEntity>> ReadInternalAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return new List<TEntity>();
        }

        await using var stream = File.OpenRead(_filePath);
        if (stream.Length == 0)
        {
            return new List<TEntity>();
        }

        var entities = await JsonSerializer.DeserializeAsync<List<TEntity>>(stream, _serializerOptions, cancellationToken);
        return entities ?? new List<TEntity>();
    }

    private async Task WriteInternalAsync(List<TEntity> entities, CancellationToken cancellationToken)
    {
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, entities, _serializerOptions, cancellationToken);
    }

    private static JsonSerializerOptions CreateDefaultOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        return options;
    }
}

