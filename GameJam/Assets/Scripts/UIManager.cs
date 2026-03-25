using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text turnLabel;
    [SerializeField] private TMP_Text phaseLabel;
    [SerializeField] private TMP_Text switchCountdownLabel;
    [SerializeField] private GameObject switchWarningPanel;
    [SerializeField] private int switchWarningThreshold = 1;
    [SerializeField] private Color chessPhaseColour = new Color(0.9f, 0.85f, 0.6f);
    [SerializeField] private Color checkersPhaseColour = new Color(0.6f, 0.85f, 0.9f);
    [SerializeField] private Color whiteTeamColour = new Color(0.95f, 0.95f, 0.95f);
    [SerializeField] private Color blackTeamColour = new Color(0.3f, 0.3f, 0.3f);
    [SerializeField] private GameObject winScreenPanel;
    [SerializeField] private TMP_Text winnerLabel;
    [SerializeField] private TMP_Text winReasonLabel;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button quitButton;
    private void Awake()
    {
        if (winScreenPanel != null)
            winScreenPanel.SetActive(false);

        if (switchWarningPanel != null)
            switchWarningPanel.SetActive(false);

        if (replayButton != null)
            replayButton.onClick.AddListener(OnReplayClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }
    public void RefreshAll(PlayerTeam currentTurn, GamePhase currentPhase, int movesUntilSwitch)
    {
        UpdateTurn(currentTurn);
        UpdatePhase(currentPhase);

        if (winScreenPanel != null)
            winScreenPanel.SetActive(false);
    }
    public void UpdateTurn(PlayerTeam currentTurn)
    {
        if (turnLabel != null)
        {
            turnLabel.text = currentTurn == PlayerTeam.White ? "WHITE" : "BLACK";
            turnLabel.color = currentTurn == PlayerTeam.White
                ? whiteTeamColour
                : blackTeamColour;
        }

        UpdateSwitchCountdown(gameManager.MovesUntilSwitch);
    }
    public void UpdatePhase(GamePhase currentPhase)
    {
        if (phaseLabel != null)
        {
            phaseLabel.text = currentPhase == GamePhase.Chess
                ? "CHESS PHASE"
                : "CHECKERS PHASE";

            phaseLabel.color = currentPhase == GamePhase.Chess
                ? chessPhaseColour
                : checkersPhaseColour;
        }
    }
    public void ShowWinScreen(PlayerTeam winner, string reason)
    {
        if (winScreenPanel != null)
            winScreenPanel.SetActive(true);

        if (winnerLabel != null)
        {
            winnerLabel.text = winner == PlayerTeam.White
                ? "WHITE WINS!"
                : "BLACK WINS!";

            winnerLabel.color = winner == PlayerTeam.White
                ? whiteTeamColour
                : blackTeamColour;
        }

        if (winReasonLabel != null)
            winReasonLabel.text = reason;
    }
    private void UpdateSwitchCountdown(int movesUntilSwitch)
    {
        if (switchCountdownLabel != null)
        {
            switchCountdownLabel.text = movesUntilSwitch == 1
                ? "Switch next turn!"
                : $"Switch in {movesUntilSwitch}";
        }

        if (switchWarningPanel != null && switchWarningThreshold > 0)
        {
            switchWarningPanel.SetActive(movesUntilSwitch <= switchWarningThreshold);
        }
    }
    private void OnReplayClicked()
    {
        if (winScreenPanel != null)
            winScreenPanel.SetActive(false);

        gameManager.RestartGame();
    }
    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
