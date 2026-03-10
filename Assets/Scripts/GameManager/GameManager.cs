using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Sahne İsimleri")]
    public string menuSceneName = "MainMenu"; 
    public string gameOverSceneName = "GameOverScene";

    [Header("Oyun Ayarları")]
    public float gameDuration = 60f;
    public float timeBonusFromBasket = 10f;
    
    [Header("Durum")]
    public bool gameActive = false;
    private float timer;
    private int score;
    
    private int destructionCombo = 0;
    private float lastBreakTime = 0f;
    public float comboTimeout = 3f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start() 
    { 
        StartGame(); 
    }

    void Update()
    {
        if (!gameActive) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 0;
            EndGame();
        }
    }

    public void StartGame()
    {
        score = 0;
        timer = gameDuration;
        destructionCombo = 0;
        gameActive = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameUI();
            UIManager.Instance.UpdateScore(score); 
        }
    }

    public void OnObjectBroken(int points)
    {
        if (!gameActive) return;
        AddScore(points); 

        float now = Time.time;
        if (now - lastBreakTime < comboTimeout) destructionCombo++;
        else destructionCombo = 0;
        
        lastBreakTime = now;
        
        if (destructionCombo > 0 && destructionCombo % 5 == 0)
        {
            if (UIManager.Instance != null) 
                UIManager.Instance.SpawnTimerPopup("RAGE COMBO!"); 
        }
    }

    public void OnBasketScored()
    {
        if (!gameActive) return;
        AddTime(timeBonusFromBasket);
        AddScore(50); 
        if (UIManager.Instance != null)
            UIManager.Instance.SpawnTimerPopup("+" + timeBonusFromBasket + "s");
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SpawnScorePopup(amount);
            UIManager.Instance.UpdateScore(score); 
        }
    }

    public void AddTime(float seconds)
    {
        if (!gameActive) return;
        timer += seconds;
    }

    public void EndGame()
    {
        gameActive = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerPrefs.SetInt("LastScore", score);
        
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        
        SceneManager.LoadScene(gameOverSceneName);
    }

    public void QuitGame() { Application.Quit(); }
    public int GetScore() => score;
    public float GetRemainingTime() => timer;
}