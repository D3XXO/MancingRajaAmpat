using UnityEngine;

public class SimpleParallax : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private float parallaxEffect;
    
    private float startPos;

    void Start()
    {
        startPos = transform.position.x;

        if (cam == null && Camera.main != null) 
        {
            cam = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (cam == null) return;
        float distance = cam.position.x * parallaxEffect;
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}