using UnityEngine;

public class FogFollowCamera : MonoBehaviour
{
    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // seguimos EXACTAMENTE la cámara
        transform.position = new Vector3(
            cam.position.x,
            cam.position.y,
            cam.position.z
        );
    }

}