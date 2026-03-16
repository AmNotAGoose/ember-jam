using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    Player player;

    public RawImage themedImage1;
    public RawImage themedImage2;
    public GameObject pauseMenu;

    private bool isPaused = false;
    private static InGameUI instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isPaused = false;
        if (pauseMenu != null) pauseMenu.SetActive(false);

        string name = scene.name;
        if (name.StartsWith("Menu")) return;

        Color color1 = Color.white;
        Color color2 = Color.white;

        if (name.StartsWith("Strawberry"))
        {
            color1 = Hex("#FF9999");
            color2 = Hex("#FFB3B3");
        }
        else if (name.StartsWith("Lemon"))
        {
            color1 = Hex("#B06901");
            color2 = Hex("#FFFFB3");
        }
        else if (name.StartsWith("Blueberry"))
        {
            color1 = Hex("#9999FF");
            color2 = Hex("#B3B3FF");
        }

        if (themedImage1 != null) themedImage1.color = color1;
        if (themedImage2 != null) themedImage2.color = color2;
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.P)) return;
        if (SceneManager.GetActiveScene().name.StartsWith("Menu")) return;
        if (pauseMenu == null) return;

        TogglePause();
    }

    public void GoHome()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1.0f;
        player.isPaused = false;
    }

    public void TogglePause()
    {
        player = FindFirstObjectByType<Player>();

        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        player.isPaused = isPaused;
        Time.timeScale = !isPaused ? 1.0f : 0.0f;

        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var src in audioSources)
        {
            if (isPaused) src.Pause();
            else src.UnPause();
        }
    }
}