using UnityEngine;

public partial class GameManager : MonoBehaviour {

    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }

    public static int CurrLevel { get; private set; }

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