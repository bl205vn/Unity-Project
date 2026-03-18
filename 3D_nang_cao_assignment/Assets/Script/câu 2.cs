using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class PlayerDeathManager : MonoBehaviour
{
    public GameObject dieTextObject;

    public Volume deathVolume;

    private bool isDead = false;

    private void Start()
    {
        if (dieTextObject != null) dieTextObject.SetActive(false);
        if (deathVolume != null) deathVolume.weight = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && !isDead)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;

        if (dieTextObject != null)
        {
            dieTextObject.SetActive(true);
        }

        if (deathVolume != null)
        {
            deathVolume.weight = 1f;
        }
    }
}