using UnityEngine;

public class OptimizeCameraRatio : MonoBehaviour
{
    public Vector2 targetAspect = new Vector2(16, 9);
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        UpdateCameraScale();
    }

    private void Update() {
        UpdateCameraScale();
    }

    void UpdateCameraScale()
    {
        // Calculate the current aspect ratio
        float currentAspect = (float)Screen.width / Screen.height;

        // Calculate the scale factor we need to apply to maintain the target aspect ratio
        float scaleHeight = currentAspect / (targetAspect.x / targetAspect.y);

        // If we're too wide, we scale down the height of the camera; otherwise, we scale up the width
        if (scaleHeight < 1)
        {
            Rect rect = cam.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            cam.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = cam.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            cam.rect = rect;
        }
    }

    // You may want to call this whenever the screen resolution changes.
    void OnResolutionChanged()
    {
        UpdateCameraScale();
    }
}
