using UnityEngine;
using UnityEngine.UI;

public class RhythmNote : MonoBehaviour
{
    public int noteID;
    public float moveSpeed;
    public bool isRedNote;

    void Update()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        if (transform.localPosition.x < -530f)
        {
            if (!isRedNote)
            {
                FindObjectOfType<PlayerStateManager>().FishingState.ChangeProgress(-0.1f);
            }

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        RhythmManager manager = FindObjectOfType<RhythmManager>();
        if (manager != null) manager.allActiveNotes.Remove(this);
    }
}