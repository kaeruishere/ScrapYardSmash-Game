using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>();
        
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
        else
        {
            Debug.LogError("HATA: GameManager sahnede bulunamadı!");
        }
    }
}