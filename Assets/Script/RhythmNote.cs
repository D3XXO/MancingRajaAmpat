using UnityEngine;

public class RhythmNote : MonoBehaviour
{
    public int noteID;
    public float moveSpeed;

    void Update()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        if (transform.localPosition.x < -530f)
        {
            FindObjectOfType<PlayerStateManager>().FishingState.ChangeProgress(-0.1f);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        RhythmManager manager = FindObjectOfType<RhythmManager>();
        if (manager != null) manager.allActiveNotes.Remove(this);
    }
}