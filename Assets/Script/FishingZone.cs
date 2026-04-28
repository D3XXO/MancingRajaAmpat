using UnityEngine;

public class FishingZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateManager player = other.GetComponent<PlayerStateManager>();
            if (player != null && player.fishingButton != null)
            {
                player.IsInFishingZone = true;
                player.fishingButton.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateManager player = other.GetComponent<PlayerStateManager>();
            if (player != null && player.fishingButton != null)
            {
                player.IsInFishingZone = false;
                player.fishingButton.SetActive(false);
            }
        }
    }
}