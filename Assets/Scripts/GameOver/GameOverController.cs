using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverController : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;

    public string gameSceneName = "GameLevel"; // Oyun sahnenin adı
    public string menuSceneName = "MainMenu"; // Menü sahnenin adı

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // GameManager'ın kaydettiği skoru çekiyoruz
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if(finalScoreText != null) 
            finalScoreText.text = "SKOR: " + lastScore;
            
        if(highScoreText != null)
            highScoreText.text = "EN YÜKSEK: " + highScore;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}