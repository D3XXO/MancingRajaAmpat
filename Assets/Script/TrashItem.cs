using UnityEngine;

public class TrashItem : MonoBehaviour
{
    [Header("Settings")]
    public int scoreReward;

    [HideInInspector]
    public TrashSpawner spawnerParent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateManager player = other.GetComponent<PlayerStateManager>();
            if (player != null)
            {
                player.AddValueScore(scoreReward);

                FloatingText floatScript = player.GetComponentInChildren<FloatingText>(true);
                
                if (floatScript != null)
                {
                    floatScript.TriggerText("+" + scoreReward, Color.white);
                }

                if (spawnerParent != null)
                {
                    spawnerParent.OnTrashCollected(gameObject);
                }

                Destroy(gameObject);
            }
        }
    }
}