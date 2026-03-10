using UnityEngine;
using TMPro;

public class PopupScore : MonoBehaviour
{
    public float duration = 1f;
    public float moveUp = 50f;

    private TextMeshProUGUI txt;
    private Vector3 startPos;

    void Awake()
    {
        txt = GetComponent<TextMeshProUGUI>();
        startPos = transform.localPosition;
    }

    void Update()
    {
        duration -= Time.deltaTime;

        transform.localPosition = startPos + Vector3.up * (1f - duration) * moveUp;
        txt.alpha = duration;

        if (duration <= 0)
            Destroy(gameObject);
    }
}
