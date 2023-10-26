using UnityEngine;

/// <summary>
/// Singleton class that holds persistent data and handles transitions;
/// </summary>
public partial class GameManager : MonoBehaviour {

    /// <summary> Internal reference to the active GameManager; </summary>
    private static GameManager _instance;
    /// <summary> Public instance reference to the active GameManager; </summary>
    public static GameManager Instance { get => _instance; }

    /// <summary> Index of the level scene currently loaded; </summary>
    public static int CurrLevel { get { return 2; } set { } }

    void Awake() {
        /// Initialize Singleton;
        if (_instance != null) {
            Destroy(gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
    }
}