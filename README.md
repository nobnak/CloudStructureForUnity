# CloudStructureForUnity
CloudStructure (O/R mapper for [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis)) combined with dependencies for Unity

- [CloudStructure](https://github.com/nobnak/CloudStructures) is modified for
  - making it IDisposable
  - serializing with MessagePack v2
  - keeping connection for pub/sub

# Usage
## Init
```csharp
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

RedisConnection redis = new RedisConnection(new RedisConfig(name, server), new MessagePackConverter());
RedisString<Creature> redisData = new RedisString<Creature>(redis, KEY_TEST, null);
```

## Dispose
```csharp
redis.Dispose();
```

## Send
```csharp
Creature data;
redisData.SetAsync(data).Wait();
```

## Get
```csharp
redisData.GetAsync().ContinueWith(t => {
  if (!t.IsFaulted) {
      var data = t.Result;
      Debug.Log($"Get. {data}");
  }
}).Wait();
```
