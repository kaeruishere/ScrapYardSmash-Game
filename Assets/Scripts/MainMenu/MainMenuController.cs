using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için şart!

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName = "GameLevel"; // Oyun sahnenin adı neyse buraya yazarsın

    // "OYNA" butonuna bağlayacağız
    public void PlayGame()
    {
        // 1. Yöntem: İsme göre sahne yükle (Daha garanti)
        SceneManager.LoadScene(gameSceneName);

        // 2. Yöntem: Sıradaki sahneyi yükle (Build Settings sırasına göre)
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // "ÇIKIŞ" butonuna bağlayacağız
    public void QuitGame()
    {
        Debug.Log("Oyundan çıkılıyor..."); // Editörde kapanmaz, konsola yazar
        Application.Quit();
    }
}