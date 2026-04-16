using UnityEngine;
using System.Collections.Generic;

public class CliffLoopManager : MonoBehaviour
{
    [Header("Referensi")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] List<Transform> cliffs;

    [Header("Pengaturan Jarak")]
    [SerializeField] float cliffSpacing;
    [SerializeField] float loopThreshold;

    [Header("Pengaturan Tinggi (Y)")]
    [SerializeField] float baseY;
    [SerializeField] float yStep;

    float totalWidth;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        totalWidth = cliffs.Count * cliffSpacing;
    }

    void Update()
    {
        foreach (Transform cliff in cliffs)
        {
            float deltaX = cliff.position.x - cameraTransform.position.x;

            if (deltaX < -loopThreshold)
            {
                RepositionCliff(cliff, 1);
            }
            else if (deltaX > loopThreshold)
            {
                RepositionCliff(cliff, -1);
            }
        }
    }

    private void RepositionCliff(Transform cliff, int direction)
    {
        Vector3 newPos = cliff.position;

        newPos.x += totalWidth * direction;

        int randStep = Random.Range(-1, 2);
        newPos.y = baseY + (randStep * yStep);

        cliff.position = newPos;
    }
}