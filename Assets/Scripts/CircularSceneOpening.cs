using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class CircularSceneOpening : MonoBehaviour
{
    [Header("Apertura circular")]
    [SerializeField, Min(0.1f)] private float _duration = 0.9f;
    [SerializeField, Range(0.01f, 0.3f)] private float _softEdge = 0.08f;
    private static CircularSceneOpening _instance;
    private CircularOpeningGraphic _graphic;
    private Coroutine _opening;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState() { _instance = null; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (_instance != null) return;
        var root = new GameObject("CircularSceneOpening", typeof(RectTransform), typeof(Canvas));
        DontDestroyOnLoad(root);
        root.AddComponent<CircularSceneOpening>();
    }

    private void Awake()
    {
        _instance = this;
        var canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;
        var scaler = gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        // This overlay is purely visual: no raycaster or input system is needed.
        var overlay = new GameObject("SoftCircularReveal", typeof(RectTransform), typeof(CanvasRenderer));
        overlay.transform.SetParent(transform, false);
        var rect = (RectTransform)overlay.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        overlay.AddComponent<UnityEngine.UI.RectMask2D>();
        _graphic = overlay.AddComponent<CircularOpeningGraphic>();
        _graphic.color = Color.black;
        _graphic.raycastTarget = false;
        _graphic.SetOpening(0f, _softEdge);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private Transform _focusTarget;
    private SpriteRenderer _focusRenderer;
    private Camera _focusCamera;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_opening != null) StopCoroutine(_opening);
        _focusTarget = null;
        _focusRenderer = null;
        _focusCamera = null;
        _graphic.SetCenter(new Vector2(0.5f, 0.5f));
        if (scene.name == "SampleScene")
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                var scientist = root.GetComponentInChildren<ScientistMovement>();
                if (scientist != null && _focusTarget == null)
                {
                    _focusTarget = scientist.transform;
                    _focusRenderer = scientist.GetComponent<SpriteRenderer>();
                }
                foreach (var camera in root.GetComponentsInChildren<Camera>())
                {
                    if (camera.isActiveAndEnabled && camera.CompareTag("MainCamera"))
                        _focusCamera = camera;
                }
            }
        }
        _graphic.enabled = true;
        _graphic.SetOpening(0f, _softEdge);
        _opening = StartCoroutine(Open());
    }

    private void LateUpdate()
    {
        if (!_graphic.enabled || _focusTarget == null || _focusCamera == null) return;
        // Use the sprite's visual center rather than its feet/pivot.
        Vector3 position = _focusRenderer != null ? _focusRenderer.bounds.center : _focusTarget.position;
        Vector3 screenPosition = _focusCamera.WorldToScreenPoint(position);
        if (screenPosition.z <= 0f) return;
        Vector2 localPosition;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _graphic.rectTransform, screenPosition, null, out localPosition)) return;
        Rect rect = _graphic.rectTransform.rect;
        if (rect.width <= 0f || rect.height <= 0f) return;
        _graphic.SetCenter(new Vector2(
            Mathf.Clamp01((localPosition.x - rect.xMin) / rect.width),
            Mathf.Clamp01((localPosition.y - rect.yMin) / rect.height)));
    }

    private IEnumerator Open()
    {
        // Keep the first frame covered while scene Start methods place the player.
        yield return null;
        float elapsed = 0f;
        while (elapsed < _duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / _duration));
            _graphic.SetOpening(progress, _softEdge);
            yield return null;
        }
        _graphic.SetOpening(1f, _softEdge);
        _graphic.enabled = false;
        _opening = null;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (_instance == this) _instance = null;
    }
}

// Concentric vertex-alpha rings produce a feathered edge without textures or shaders.
public sealed class CircularOpeningGraphic : UnityEngine.UI.MaskableGraphic
{
    private const int Segments = 128;
    private const int FeatherRings = 8;
    private float _progress;
    private float _softEdge;
    private Vector2 _centerViewport = new Vector2(0.5f, 0.5f);

    public void SetCenter(Vector2 centerViewport)
    {
        _centerViewport = centerViewport;
        SetVerticesDirty();
    }

    public void SetOpening(float progress, float softEdge)
    {
        _progress = progress;
        _softEdge = softEdge;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vertices)
    {
        vertices.Clear();
        Rect rect = rectTransform.rect;
        if (rect.width <= 0f || rect.height <= 0f || _progress >= 1f) return;
        Vector2 center = new Vector2(
            Mathf.Lerp(rect.xMin, rect.xMax, _centerViewport.x),
            Mathf.Lerp(rect.yMin, rect.yMax, _centerViewport.y));
        float farthestX = Mathf.Max(Mathf.Abs(rect.xMin - center.x), Mathf.Abs(rect.xMax - center.x));
        float farthestY = Mathf.Max(Mathf.Abs(rect.yMin - center.y), Mathf.Abs(rect.yMax - center.y));
        float cornerRadius = new Vector2(farthestX, farthestY).magnitude;
        float feather = Mathf.Min(rect.width, rect.height) * _softEdge;
        float innerRadius = Mathf.Lerp(-feather, cornerRadius, _progress);
        float outerRadius = cornerRadius + feather;
        for (int ring = 0; ring <= FeatherRings + 1; ring++)
        {
            float t = Mathf.Min(1f, (float)ring / FeatherRings);
            float radius = ring > FeatherRings ? outerRadius : Mathf.Max(0f, innerRadius + feather * t);
            Color tint = color;
            tint.a *= Mathf.SmoothStep(0f, 1f, t);
            for (int segment = 0; segment <= Segments; segment++)
            {
                float angle = segment * (Mathf.PI * 2f / Segments);
                Vector2 position = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                vertices.AddVert(position, tint, Vector2.zero);
            }
            if (ring == 0) continue;
            int previous = (ring - 1) * (Segments + 1);
            int current = ring * (Segments + 1);
            for (int segment = 0; segment < Segments; segment++)
            {
                vertices.AddTriangle(previous + segment, current + segment, current + segment + 1);
                vertices.AddTriangle(previous + segment, current + segment + 1, previous + segment + 1);
            }
        }
    }
}
