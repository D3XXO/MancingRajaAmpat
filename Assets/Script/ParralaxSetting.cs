using UnityEngine;

public class ParralaxSetting : MonoBehaviour
{
    [Header("Referensi")]
    [SerializeField] private Transform cameraTransform;

    [Header("Pengaturan Parallax")]
    [SerializeField] private float parallaxFactor;

    private Material mat;
    private Vector2 offset;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        offset.x = cameraTransform.position.x * parallaxFactor;
        mat.mainTextureOffset = offset;
    }
}