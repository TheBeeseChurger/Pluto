using UnityEngine;
using UnityEngine.UI;

public class CallIndicatorScript : MonoBehaviour
{
    private enum IndicatorState { Edge, Fill, Off }

    private Transform currentSignalSource;
    private Transform player;

    Camera cam;

    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;

    [Header("Screen Bounds")]
    [SerializeField] private float left = 50;
    [SerializeField] private float right = 50;
    [SerializeField] private float top = 50;
    [SerializeField] private float bottom = 50;

    [Header("Indicators")]
    [SerializeField] private Sprite[] edgeIndicators;
    [SerializeField] private Sprite[] fillIndicators;
    [SerializeField] private Sprite emptyIndicator;

    private RectTransform parentRect;
    private Canvas canvas;

    private Image image;
    private Timer anim_timer;

    private IndicatorState currentState = IndicatorState.Off;
    private IndicatorState wouldBeState = IndicatorState.Off;
    private int currentIndex = 0;

    private bool isOff = true;
    private bool isInitialized = false;

    public void Init(Transform player1, Transform player2)
    {
        cam = Camera.main;
        player = player1;
        currentSignalSource = player2;

        parentRect = transform.parent.GetComponent<RectTransform>();
        canvas = parentRect.GetComponent<Canvas>();
        while (canvas != null && !canvas.isRootCanvas)
        {
            canvas = canvas.transform.parent.GetComponent<Canvas>();
        }

        image = GetComponent<Image>();

        anim_timer = gameObject.AddComponent<Timer>();

        anim_timer.timer_spd = 1f;
        anim_timer.timer_time = 0.5f;

        isInitialized = true;
    }

    public void TurnOff()
    {
        currentState = IndicatorState.Off;
        isOff = true;
    }

    public void TurnOn()
    {
        currentState = wouldBeState;
        wouldBeState = IndicatorState.Off;
        isOff = false;
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (anim_timer.End)
        {
            switch (currentState)
            {
                case IndicatorState.Off:
                    currentIndex = 0;
                    image.sprite = emptyIndicator;
                    break;
                case IndicatorState.Edge:
                    currentIndex = (currentIndex + 1) % edgeIndicators.Length;
                    image.sprite = edgeIndicators[currentIndex];
                    break;
                case IndicatorState.Fill:
                    currentIndex = (currentIndex + 1) % fillIndicators.Length;
                    image.sprite = fillIndicators[currentIndex];
                    break;
            }
        }
    }

    void LateUpdate()
    {
        if (!isInitialized) return;

        Vector3 projScreenPos = cam.WorldToScreenPoint(currentSignalSource.position);

        float minX = left;
        float maxX = Screen.width - right;
        float minY = bottom;
        float maxY = Screen.height - top;

        bool inside =
            projScreenPos.x >= minX && projScreenPos.x <= maxX &&
            projScreenPos.y >= minY && projScreenPos.y <= maxY &&
            projScreenPos.z > 0f;

        if (inside && (currentState != IndicatorState.Fill || wouldBeState != IndicatorState.Fill))
        {
            ChangeState(IndicatorState.Fill);
        }
        else if (!inside && (currentState != IndicatorState.Edge || wouldBeState != IndicatorState.Edge))
        {
            ChangeState(IndicatorState.Edge);
        }

        float clampedX = Mathf.Clamp(projScreenPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(projScreenPos.y, minY, maxY);

        Vector2 targetScreenPos = new(clampedX - xOffset, clampedY - yOffset);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            targetScreenPos,
            cam,
            out Vector2 localPoint
        );

        Vector3 dir = player.position - currentSignalSource.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.SetLocalPositionAndRotation(localPoint, Quaternion.Euler(0, 0, angle + 45f));
    }

    private void ChangeState(IndicatorState state)
    {
        if (isOff)
        {
            wouldBeState = state;
        }
        else
        {
            currentState = state;
        }
    }
}
