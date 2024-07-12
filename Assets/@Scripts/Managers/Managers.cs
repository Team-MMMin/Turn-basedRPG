using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }
    static bool s_init = false;

    #region Contents
    ObjectManager _object = new ObjectManager();
    MapManager _map = new MapManager();
    GameManager _game = new GameManager();
    public static ObjectManager Object { get { return Instance?._object; } }
    public static MapManager Map { get { return Instance?._map; } }
    public static GameManager Game { get { return Instance?._game; } }
    #endregion

    #region Core
    ResourceManager _resource = new ResourceManager();
    PoolManager _pool = new PoolManager();
    SceneManagerEX _scene = new SceneManagerEX();
    UIManager _ui = new UIManager();
    DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    public static ResourceManager Resource { get { return Instance?._resource; } }
    public static PoolManager Pool { get { return Instance?._pool; } }
    public static SceneManagerEX Scene {  get { return Instance?._scene; } }
    public static UIManager UI { get { return Instance?._ui; } }
    public static DataManager Data { get { return Instance?._data; } }
    public static InputManager Input { get { return Instance?._input; } }
    #endregion

    public static void Init()
    {
        if (s_init == false)
        {
            s_init = true;

            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject() { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
        }
    }

    public static void Clear()
    {
        UI.Clear();
        Scene.Clear();
        Pool.Clear();
        Input.Clear();
    }
}
