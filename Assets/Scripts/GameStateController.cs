using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStateController : MonoBehaviour
{
    public static GameStateController Instance { get; private set; }

    public enum GameState { Waiting, Running, Failed, Completed }

    [SerializeField] private Transform carrier;
    [SerializeField] private float finishZ = 22f;
    [SerializeField] private Text messageText;
    [SerializeField] private Text progressText;

    public GameState State { get; private set; } = GameState.Waiting;
    public bool IsRunning => State == GameState.Running;

    private float stateChangedAt;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    private void Start()
    {
        ShowMessage("DRAG TO BALANCE\nTap to start");
        UpdateProgress();
    }

    private void Update()
    {
        UpdateProgress();

        if (State == GameState.Waiting && PointerPressedThisFrame())
        {
            State = GameState.Running;
            stateChangedAt = Time.unscaledTime;
            ShowMessage("Keep it steady!");
        }
        else if ((State == GameState.Failed || State == GameState.Completed) &&
                 Time.unscaledTime - stateChangedAt > 0.45f &&
                 PointerPressedThisFrame())
        {
            Restart();
        }

        if (State == GameState.Running && messageText != null &&
            Time.unscaledTime - stateChangedAt > 1.2f)
        {
            messageText.text = string.Empty;
        }
    }

    private void UpdateProgress()
    {
        if (progressText == null || carrier == null) return;

        float p = Mathf.Clamp01(carrier.position.z / Mathf.Max(0.01f, finishZ));
        progressText.text = $"{Mathf.RoundToInt(p * 100f)}%";
    }

    public void Fail()
    {
        if (State != GameState.Running) return;

        State = GameState.Failed;
        stateChangedAt = Time.unscaledTime;
        ShowMessage("OOPS!\nTap to retry");
    }

    public void Complete()
    {
        if (State != GameState.Running) return;

        State = GameState.Completed;
        stateChangedAt = Time.unscaledTime;
        ShowMessage("PERFECT!\nTap for another run");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ShowMessage(string message)
    {
        if (messageText != null) messageText.text = message;
    }

    private static bool PointerPressedThisFrame()
    {
        bool mouse = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool touch = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
        return mouse || touch;
    }
}
