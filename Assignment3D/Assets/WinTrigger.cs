using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private string playerTag = "Player";

    [Header("Objects to show when player wins")]
    [SerializeField] private GameObject[] objectsToShow;

    private bool _hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered)
            return;

        if (!other.CompareTag(playerTag))
            return;

        _hasTriggered = true;
        ShowAllObjects();
    }

    private void ShowAllObjects()
    {
        if (objectsToShow == null)
            return;

        foreach (var obj in objectsToShow)
        {
            if (obj == null)
                continue;

            // Bật GameObject
            obj.SetActive(true);

            // Bật mọi ParticleSystem bên trong (kể cả đang bị tắt)
            var particles = obj.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var ps in particles)
            {
                ps.gameObject.SetActive(true);
                ps.Play(true);
            }
        }
    }
}


