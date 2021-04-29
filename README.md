# CloudStructureForUnity
CloudStructure (O/R mapper for [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis)) combined with dependencies for Unity

- Use a modified [CloudStructure](https://github.com/nobnak/CloudStructures) for
  - making it IDisposable
  - serializing with MessagePack v2
  - keeping connection for pub/sub

## Usage
```csharp
protected RedisConnection redis;
protected RedisString<Creature> redisData;

#region unity
private void OnEnable() {
    redis = new RedisConnection(new RedisConfig(name, server), new MessagePackConverter());
    redisData = new RedisString<Creature>(redis, KEY_TEST, null);
}
private void OnDisable() {
    if (redis != null) {
        redis.Dispose();
        redis = null;
    }
}

protected void Somewhere() {
    var data = new Creature() {
        id = Time.frameCount,
        position_x = Mathf.FloorToInt(10f * Random.value),
        position_y = Mathf.FloorToInt(10f * Random.value),
        birthTime = System.DateTimeOffset.Now.Ticks
    };
    redisData.SetAsync(data).Wait();

    redisData.GetAsync().ContinueWith(t => {
        if (!t.IsFaulted) {
            var data = t.Result;
            Debug.Log($"Get. {data}");
        }
    }).Wait();
}

[MessagePackObject]
public class Creature {
    [Key(0)]
    public int id;
    [Key(1)]
    public float position_x;
    [Key(2)]
    public float position_y;
    [Key(3)]
    public long birthTime;

    [IgnoreMember]
    public Vector2 position { 
        get => new Vector2(position_x, position_y);  
        set {
            position_x = value.x;
            position_y = value.y;
        }
    }
}
```
