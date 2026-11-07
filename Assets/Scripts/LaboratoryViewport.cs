using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public sealed class LaboratoryViewport : MonoBehaviour
{
    [SerializeField] private int _width = 1920;
    [SerializeField] private int _height = 1080;
    private Camera _camera;

    private void OnEnable()
    {
        _camera = GetComponent<Camera>();
        if (Application.isPlaying && !Application.isEditor)
            Screen.SetResolution(_width, _height, FullScreenMode.Windowed);
        UpdateViewport();
    }

    private void LateUpdate() { UpdateViewport(); }

    private void UpdateViewport()
    {
        if (_camera == null || Screen.height == 0) return;
        float target = (float)_width / _height;
        float actual = (float)Screen.width / Screen.height;
        float relativeHeight = actual / target;
        _camera.rect = relativeHeight < 1f
            ? new Rect(0f, (1f - relativeHeight) * 0.5f, 1f, relativeHeight)
            : new Rect((1f - 1f / relativeHeight) * 0.5f, 0f, 1f / relativeHeight, 1f);
        _camera.aspect = target;
    }
}
