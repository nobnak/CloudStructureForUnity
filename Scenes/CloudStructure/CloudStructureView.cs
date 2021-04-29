using CloudStructures;
using CloudStructures.Converters;
using CloudStructures.Structures;
using MessagePack;
using System.Text;
using UnityEngine;

public class CloudStructureView : MonoBehaviour {

    public const string KEY_TEST = "k_data";

    [SerializeField]
    protected string server = "127.0.0.1";

    protected Rect windowSize = new Rect(10, 10, 300, 300);

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
    private void OnGUI() {
        windowSize = GUILayout.Window(GetInstanceID(), windowSize, OnWindow, name);
    }
    private void Update() {
    }
    #endregion

    #region member
    protected void OnWindow(int id) {
        var conn = redis.GetConnection();
        GUI.enabled = conn != null && conn.IsConnected;

        using (new GUILayout.VerticalScope()) {
            if (GUILayout.Button("Set")) {
                var data = new Creature() {
                    id = Time.frameCount,
                    position_x = Mathf.FloorToInt(10f * Random.value),
                    position_y = Mathf.FloorToInt(10f * Random.value),
                    birthTime = System.DateTimeOffset.Now.Ticks
                };
                Debug.Log($"Set. {data}");
                redisData.SetAsync(data).Wait();
            }
            if (GUILayout.Button("Get")) {
                redisData.GetAsync().ContinueWith(t => {
                    if (!t.IsFaulted) {
                        var data = t.Result;
                        Debug.Log($"Get. {data}");
                    }
                }).Wait();
            }
        }

        GUI.DragWindow();
    }
    #endregion

    #region definition
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

#region interafce

#region object
        public override string ToString() {
            var tmp = new StringBuilder();
            tmp.Append($"<{GetType().Name} : ");
            tmp.Append($"id={id}");
            tmp.Append($", position=({position_x},{position_y})");
            tmp.Append($", birth_time={birthTime}>");
            return tmp.ToString();
        }
#endregion

#endregion
    }
#endregion
}
