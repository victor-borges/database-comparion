using DatabaseComparison.Data;
using DatabaseComparison.Data.Entities;
using DatabaseComparison.Requests.HeartRate;
using DatabaseComparison.Responses.HeartRate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DatabaseComparison.Controllers;

[ApiController]
public class HeartRateController : ControllerBase
{
    private readonly MySqlDbContext _mySqlDbContext;
    private readonly PostgresDbContext _postgresDbContext;
    private readonly IMongoCollection<HeartRateMonitor<Guid>> _heartRateCollection;

    public HeartRateController(IConfiguration configuration, MySqlDbContext mySqlDbContext, PostgresDbContext postgresDbContext)
    {
        _mySqlDbContext = mySqlDbContext;
        _postgresDbContext = postgresDbContext;

        var clientSettings = MongoClientSettings.FromConnectionString(configuration.GetConnectionString("MongoDB"));
        clientSettings.LinqProvider = LinqProvider.V3;
        
        var mongoClient = new MongoClient(clientSettings);
        var mongoDatabase = mongoClient.GetDatabase("databasecomparion");
        
        _heartRateCollection = mongoDatabase.GetCollection<HeartRateMonitor<Guid>>("heart_rate_monitors");
        
        var registeredAtIndexKeysDefinition = Builders<HeartRateMonitor<Guid>>.IndexKeys.Ascending(hearRateMonitor => hearRateMonitor.RegisteredAt);
        var smartWatchIdIndexKeysDefinition = Builders<HeartRateMonitor<Guid>>.IndexKeys.Ascending(hearRateMonitor => hearRateMonitor.SmartWatchId);
        _heartRateCollection.Indexes.CreateOne(new CreateIndexModel<HeartRateMonitor<Guid>>(registeredAtIndexKeysDefinition));
        _heartRateCollection.Indexes.CreateOne(new CreateIndexModel<HeartRateMonitor<Guid>>(smartWatchIdIndexKeysDefinition));
    }

    [HttpGet]
    [Route("mysql")]
    public IAsyncEnumerable<HeartRateValuesModel> GetMySql(
        [FromQuery(Name = "smart_watch_slug")] string smartWatchSlug,
        [FromQuery(Name = "start")] DateTimeOffset start,
        [FromQuery(Name = "end")] DateTimeOffset end)
    {
        return GetRelational<MySqlDbContext, long>(smartWatchSlug, start, end, _mySqlDbContext);
    }
    
    [HttpPost]
    [Route("mysql")]
    public Task<IEnumerable<HeartRateMonitorResponse>> PostMySql(HeartRateMonitorRequest request)
    {
        return PostRelational<MySqlDbContext, long>(request, _mySqlDbContext);
    }
    
    [HttpGet]
    [Route("postgres")]
    public IAsyncEnumerable<HeartRateValuesModel> GetPostgres(
        [FromQuery(Name = "smart_watch_slug")] string smartWatchSlug,
        [FromQuery(Name = "start")] DateTimeOffset start,
        [FromQuery(Name = "end")] DateTimeOffset end)
    {
        return GetRelational<PostgresDbContext, Guid>(smartWatchSlug, start, end, _postgresDbContext);
    }

    [HttpPost]
    [Route("postgres")]
    public Task<IEnumerable<HeartRateMonitorResponse>> PostPostgres(HeartRateMonitorRequest request)
    {
        return PostRelational<PostgresDbContext, Guid>(request, _postgresDbContext);
    }

    [HttpGet]
    [Route("mongo")]
    public IEnumerable<HeartRateValuesModel> GetMongo(
        [FromQuery(Name = "smart_watch_slug")] string smartWatchSlug,
        [FromQuery(Name = "start")] DateTimeOffset start,
        [FromQuery(Name = "end")] DateTimeOffset end)
    {
        return _heartRateCollection.AsQueryable()
            .Where(hrm =>
                hrm.SmartWatchId == smartWatchSlug
                && hrm.RegisteredAt >= start
                && hrm.RegisteredAt <= end)
            .Select(hrm => new HeartRateValuesModel
            {
                RegisteredAt = hrm.RegisteredAt,
                AverageHeartRate = hrm.Average,
                MaxHeartRate = hrm.Max,
                MinHeartRate = hrm.Min
            });
    }
    
    [HttpPost]
    [Route("mongo")]
    public IEnumerable<HeartRateMonitorResponse> PostMongo(HeartRateMonitorRequest request)
    {
        var (maxRegisterDate, minRegisterDate) = GetMinMaxRegisterDate(request);
        minRegisterDate = minRegisterDate.AddMinutes(-30);
        maxRegisterDate = maxRegisterDate.AddMinutes(30);
        
        var existingRegisterDateTimes = _heartRateCollection.AsQueryable()
            .Where(hrm =>
                hrm.SmartWatchId == request.SmartWatchSlug
                && hrm.RegisteredAt >= minRegisterDate
                && hrm.RegisteredAt <= maxRegisterDate)
            .Select(hrm => hrm.RegisteredAt)
            .ToList();

        var heartRateMonitors = CreateHeartRateMonitors<Guid>(request, existingRegisterDateTimes).ToList();
        var registeredDateTimes = heartRateMonitors.Select(hrm => hrm.RegisteredAt).ToHashSet();

        var invalidOrExistingDateTimes = GetInvalidOrExistingDateTimes(request, registeredDateTimes);
        
        _heartRateCollection.InsertManyAsync(heartRateMonitors);

        return heartRateMonitors.Select(hrm => new HeartRateMonitorResponse
        {
            Id = hrm.RegisteredAt,
            Status = 1
        }).Concat(invalidOrExistingDateTimes.Select(invalidOrExistingDate => new HeartRateMonitorResponse
        {
            Id = invalidOrExistingDate,
            Status = 0
        }));
    }

    private IAsyncEnumerable<HeartRateValuesModel> GetRelational<TContext, TKey>(
        string smartWatchSlug,
        DateTimeOffset start,
        DateTimeOffset end,
        TContext dbContext)
        where TContext : DbContext
        where TKey : struct
    {
        return dbContext.Set<HeartRateMonitor<TKey>>()
            .Where(hrm =>
                hrm.SmartWatchId == smartWatchSlug
                && hrm.RegisteredAt >= start.ToUniversalTime()
                && hrm.RegisteredAt <= end.ToUniversalTime())
            .Select(hrm => new HeartRateValuesModel
            {
                RegisteredAt = hrm.RegisteredAt,
                AverageHeartRate = hrm.Average,
                MaxHeartRate = hrm.Max,
                MinHeartRate = hrm.Min
            })
            .AsAsyncEnumerable();
    }
    
    private async Task<IEnumerable<HeartRateMonitorResponse>> PostRelational<TContext, TKey>(
        HeartRateMonitorRequest request,
        TContext dbContext)
        where TContext : DbContext
        where TKey : struct
    {
        var (maxRegisterDate, minRegisterDate) = GetMinMaxRegisterDate(request);
        minRegisterDate = minRegisterDate.AddMinutes(-30);
        maxRegisterDate = maxRegisterDate.AddMinutes(30);

        var existingRegisterDateTimes = dbContext.Set<HeartRateMonitor<TKey>>()
            .Where(hrm =>
                hrm.SmartWatchId == request.SmartWatchSlug
                && hrm.RegisteredAt >= minRegisterDate
                && hrm.RegisteredAt <= maxRegisterDate)
            .Select(hrm => hrm.RegisteredAt)
            .ToList();

        var heartRateMonitors = CreateHeartRateMonitors<TKey>(request, existingRegisterDateTimes).ToList();
        var registeredDateTimes = heartRateMonitors.Select(hrm => hrm.RegisteredAt).ToHashSet();

        var invalidOrExistingDateTimes = GetInvalidOrExistingDateTimes(request, registeredDateTimes);
        
        await dbContext.Set<HeartRateMonitor<TKey>>().AddRangeAsync(heartRateMonitors);
        await dbContext.SaveChangesAsync();

        return heartRateMonitors.Select(hrm => new HeartRateMonitorResponse
        {
            Id = hrm.RegisteredAt,
            Status = 1
        }).Concat(invalidOrExistingDateTimes.Select(invalidOrExistingDate => new HeartRateMonitorResponse
        {
            Id = invalidOrExistingDate,
            Status = 0
        }));
    }
    
    private static (DateTimeOffset maxRegisterDate, DateTimeOffset minRegisterDate) GetMinMaxRegisterDate(HeartRateMonitorRequest request)
    {
        var maxRegisterDate = request.HeartRateMonitor.Max(model => model.HeartRateValues.Max(value => value.RegisteredAt));
        var minRegisterDate = request.HeartRateMonitor.Max(model => model.HeartRateValues.Min(value => value.RegisteredAt));
        
        return (maxRegisterDate, minRegisterDate);
    }

    private static IEnumerable<HeartRateMonitor<TKey>> CreateHeartRateMonitors<TKey>(
        HeartRateMonitorRequest request,
        List<DateTimeOffset> existingRegisterDateTimes)
        where TKey : struct
    {
        return
            from model in request.HeartRateMonitor
            from value in model.HeartRateValues
            where !existingRegisterDateTimes.Any(existing =>
                existing < value.RegisteredAt.AddMinutes(30)
                && existing > value.RegisteredAt.AddMinutes(-30))
            select new HeartRateMonitor<TKey>
            {
                SmartWatchId = request.SmartWatchSlug,
                RegisteredAt = value.RegisteredAt.ToUniversalTime(),
                Average = value.AverageHeartRate,
                Max = value.MaxHeartRate,
                Min = value.MinHeartRate
            };
    }
    
    private static IEnumerable<DateTimeOffset> GetInvalidOrExistingDateTimes(HeartRateMonitorRequest request, HashSet<DateTimeOffset> registeredDateTimes)
    {
        return request.HeartRateMonitor
            .SelectMany(model => model.HeartRateValues.Select(value => value.RegisteredAt))
            .Where(date => !registeredDateTimes.Contains(date));
    }
}
