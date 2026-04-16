using UnityEngine;

public class BackgroundCycle : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private float parallaxEffect;
    private float length;
    private float startPos;

    void Start()
    {
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        startPos = transform.position.x;
    }

    void LateUpdate()
    {
        float distance = cam.transform.position.x * parallaxEffect;
        float temp = cam.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if (temp > startPos + length)
        {
            startPos += length * 2;
        }
        else if (temp < startPos - length)
        {
            startPos -= length * 2;
        }
    }
}