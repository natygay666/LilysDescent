using UnityEngine;

public class PSXFogSpawner : MonoBehaviour
{
    [Header("Fog Settings")]
    public GameObject fogPrefab;
    public int fogCount = 50;
    public Transform target;
    public bool followTarget = true;
    
    void LateUpdate()
    {
        if (!followTarget) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 pos = cam.transform.position;
        pos.y = transform.position.y;

        transform.position = pos;
    }

    
    [Header("Area")]
    public Vector2 areaSize = new Vector2(50, 50);
    public float minHeight = 0;
    public float maxHeight = 5;

    [Header("Scale")]
    public Vector2 scaleRange = new Vector2(2, 6);

    [Header("Random Movement")]
    public bool randomizeMaterial = true;

    void Start()
    {
        SpawnFog();
    }

    void SpawnFog()
    {
        for (int i = 0; i < fogCount; i++)
        {
            float x = (i % 10) / 10f * areaSize.x - areaSize.x / 2;
            float z = (i / 10) / 10f * areaSize.y - areaSize.y / 2;

            Vector3 pos = new Vector3(
                x + Random.Range(-1f, 1f),
                Random.Range(minHeight, maxHeight),
                z + Random.Range(-1f, 1f)
            );

            Quaternion rot = Quaternion.Euler(90, 0, 0);
            GameObject fog = Instantiate(fogPrefab, pos, rot, transform);
            
            float scale = Random.Range(scaleRange.x, scaleRange.y);
            fog.transform.localScale = new Vector3(scale, scale, scale);
            
            if (fog.GetComponent<BillboardY>() == null)
                fog.AddComponent<BillboardY>();
            
            if (randomizeMaterial)
            {
                Renderer r = fog.GetComponent<Renderer>();
                if (r != null)
                {
                    Material mat = new Material(r.sharedMaterial);

                    float speedX = Random.Range(0.02f, 0.15f);
                    float speedY = Random.Range(0.02f, 0.1f);

                    mat.SetVector("_ScrollSpeed", new Vector4(speedX, speedY, 0, 0));
                    mat.SetFloat("_Intensity", Random.Range(0.5f, 1.2f));

                    r.material = mat;
                }
            }
        }
    }

}