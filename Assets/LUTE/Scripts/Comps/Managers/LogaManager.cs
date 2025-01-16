using LoGaCulture.LUTE;
using LoGaCulture.LUTE.Logs;
using UnityEngine;

[RequireComponent(typeof(SoundManager))]
[RequireComponent(typeof(CameraManager))]
[RequireComponent(typeof(StateManager))]
[RequireComponent(typeof(GlobalVariables))]
[RequireComponent(typeof(LogManager))]
[RequireComponent(typeof(ConnectionManager))]
[RequireComponent(typeof(MapLayerChanger))]
#if UNITY_5_3_OR_NEWER
[RequireComponent(typeof(SaveManager))]
[RequireComponent(typeof(SaveLog))]
#endif
public class LogaManager : MonoBehaviour
{
    // Singleton pattern that uses volatile to ensure that assignment to the instance variable completes before the instance variable can be accessed.
    volatile static LogaManager instance;
    static bool applicationIsQuitting = false;
    // Singleton pattern that uses a private constructor to prevent instantiation of a class from other classes
    static readonly object padlock = new object();

    public SoundManager SoundManager { get; private set; }
    public CameraManager CameraManager { get; private set; }
    public StateManager StateManager { get; private set; }
    public GlobalVariables GlobalVariables { get; private set; }
    public LogManager LogManager { get; private set; }
    public ConnectionManager ConnectionManager { get; private set; }
    public MapLayerChanger MapLayerChanger { get; private set; }
#if UNITY_5_3_OR_NEWER
    public SaveManager SaveManager { get; private set; }
    public SaveLog SaveLog { get; private set; }

#endif

    private void Awake()
    {
        SoundManager = GetComponent<SoundManager>();
        CameraManager = GetComponent<CameraManager>();
        StateManager = GetComponent<StateManager>();
        GlobalVariables = GetComponent<GlobalVariables>();
        LogManager = GetComponent<LogManager>();
        ConnectionManager = GetComponent<ConnectionManager>();
        MapLayerChanger = GetComponent<MapLayerChanger>();
#if UNITY_5_3_OR_NEWER
        SaveManager = GetComponent<SaveManager>();
        SaveLog = GetComponent<SaveLog>();
#endif
    }

    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    void OnDestroy()
    {
        applicationIsQuitting = true;
    }

    public static LogaManager Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(LogaManager) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }
            // Double-checked locking pattern which (once the instance exists) avoids locking each time the method is invoked
            if (instance == null)
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        GameObject logaManager = new GameObject("LogaManager");
                        DontDestroyOnLoad(logaManager);
                        instance = logaManager.AddComponent<LogaManager>();
                    }
                }
            }
            return instance;
        }
    }
}
