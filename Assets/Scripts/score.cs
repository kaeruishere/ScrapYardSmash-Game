using UnityEngine;

public class Score : MonoBehaviour
{
    [Header("Topun Tag Adı")]
    public string ballTag = "Ball";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ballTag))
        {
            GameManager.Instance.OnBasketScored();
        }
    }
}
