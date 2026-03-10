using UnityEngine;
using TMPro;
using System.Collections; 

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    // gameUIPanel kalsın mı? Evet, bazen tüm HUD'u kapatmak istersin.
    public GameObject gameUIPanel; 

    [Header("Popup Score")]
    public GameObject popupPrefab;
    public Transform popupParent;

    [Header("Timer Popup")]
    public GameObject timerPopupPrefab;
    public Transform timerPopupParent;

    private bool isFlashing = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Oyun devam ediyorsa süreyi güncelle
        if (GameManager.Instance != null && GameManager.Instance.gameActive)
        {
            UpdateTimer();
        }
    }

    public void UpdateScore(int score)
    {
        if(scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void UpdateTimer()
    {
        if(timerText != null)
        {
            float t = GameManager.Instance.GetRemainingTime();
            timerText.text = "Time: " + Mathf.Ceil(t);

            if (t <= 10 && t > 0 && !isFlashing)
                StartCoroutine(FlashTimer());
        }
    }

    private IEnumerator FlashTimer()
    {
        isFlashing = true;
        Color normal = Color.yellow; 
        Color red = Color.red;

        while (GameManager.Instance != null && GameManager.Instance.GetRemainingTime() <= 10 && GameManager.Instance.gameActive)
        {
            if(timerText != null) timerText.color = red;
            yield return new WaitForSeconds(0.2f);

            if(timerText != null) timerText.color = normal;
            yield return new WaitForSeconds(0.2f);
        }

        if(timerText != null) timerText.color = normal;
        isFlashing = false;
    }

    public void SpawnScorePopup(int amount)
    {
        if(popupPrefab && popupParent)
        {
            GameObject go = Instantiate(popupPrefab, popupParent);
            go.GetComponent<TextMeshProUGUI>().text = "+" + amount;
            Destroy(go, 1.5f);
        }
    }

    public void SpawnTimerPopup(string text)
    {
        if(timerPopupPrefab && timerPopupParent)
        {
            GameObject go = Instantiate(timerPopupPrefab, timerPopupParent);
            go.GetComponent<TextMeshProUGUI>().text = text;
            Destroy(go, 1.5f);
        }
    }
    
    // ShowGameOver, HideMenu vb. hepsi SİLİNDİ.
    
    public void ShowGameUI() { if(gameUIPanel) gameUIPanel.SetActive(true); }
}