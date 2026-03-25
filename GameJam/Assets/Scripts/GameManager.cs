using UnityEngine;
using UnityEngine.Events;
public enum WinReason
{
    KingCaptured,
    AllPiecesCaptured,
    NoValidMoves
}
public class GameManager : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private SwitchController switchController;
    [SerializeField] private UIManager uiManager;

    [SerializeField] private int movesPerPhase = 2;

    // Events — subscribe in UIManager, SwitchController, etc.

    /// <summary>Fired when the active player changes. Passes the new turn's team.</summary>
    public UnityEvent<PlayerTeam> OnTurnChanged = new UnityEvent<PlayerTeam>();

    /// <summary>Fired when the phase switches. Passes the new phase.</summary>
    public UnityEvent<GamePhase> OnPhaseChanged = new UnityEvent<GamePhase>();

    /// <summary>Fired when the game ends. Passes the winning team.</summary>
    public UnityEvent<PlayerTeam> OnGameOver = new UnityEvent<PlayerTeam>();

    // Public read-only state

    /// <summary>Whose turn it currently is.</summary>
    public PlayerTeam CurrentTurn { get; private set; } = PlayerTeam.White;

    /// <summary>Current game phase (Chess or Checkers).</summary>
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Chess;

    /// <summary>
    /// When true, all player input is ignored.
    /// Set during switch animations and win sequences.
    /// </summary>
    public bool InputLocked { get; private set; } = false;

    /// <summary>How many moves have been made in the current phase so far.</summary>
    public int MovesThisPhase { get; private set; } = 0;

    /// <summary>How many moves remain before the next Switch.</summary>
    public int MovesUntilSwitch => movesPerPhase - MovesThisPhase;

    void Start()
    {
        CurrentTurn = PlayerTeam.White;
        CurrentPhase = GamePhase.Chess;
        InputLocked = false;
        MovesThisPhase = 0;

        OnTurnChanged.Invoke(CurrentTurn);
        OnPhaseChanged.Invoke(CurrentPhase);

        uiManager?.RefreshAll(CurrentTurn, CurrentPhase, MovesUntilSwitch);
    }
    public void OnMoveComplete()
    {
        MovesThisPhase++;

        // Check phase-specific win conditions before advancing the turn
        if (CurrentPhase == GamePhase.Checkers)
        {
            PlayerTeam opponent = Opponent(CurrentTurn);

            if (board.GetPieceCount(opponent) == 0)
            {
                TriggerWin(CurrentTurn, WinReason.AllPiecesCaptured);
                return;
            }

            // Advance turn temporarily to check if the opponent has moves
            PlayerTeam nextPlayer = Opponent(CurrentTurn);
            if (board.HasNoValidMoves(nextPlayer))
            {
                TriggerWin(CurrentTurn, WinReason.NoValidMoves);
                return;
            }

            //Check if phases must switch
            if (MovesThisPhase >= movesPerPhase)
            {
                TriggerSwitch();
                return;
            }

            // Otherwise just advance the turn
            AdvanceTurn();
        }
    }

    public void OnKingCaptured(PlayerTeam capturingTeam)
    {
        TriggerWin(capturingTeam, WinReason.KingCaptured);
    }

    private void AdvanceTurn()
    {
        CurrentTurn = Opponent(CurrentTurn);
        OnTurnChanged.Invoke(CurrentTurn);
        //uiManager?.UpdateTurn(CurrentTurn, MovesUntilSwitch);

        Debug.Log($"Turn: {CurrentTurn} | Phase: {CurrentPhase} | " +
                  $"Moves this phase: {MovesThisPhase}/{movesPerPhase}");
    }
    private void TriggerSwitch()
    {
        InputLocked = true;

        // Let the board clean up selection and checkers state before the flip
        board.OnPhaseWillSwitch();

        // SwitchController runs the animation, then calls OnSwitchComplete
        switchController.BeginSwitch();
    }

    public void OnSwitchComplete()
    {
        // Toggle phase
        CurrentPhase = CurrentPhase == GamePhase.Chess
            ? GamePhase.Checkers
            : GamePhase.Chess;

        MovesThisPhase = 0;

        // Notify pieces so they can swap sprites
        NotifyPiecesOfPhaseChange();

        OnPhaseChanged.Invoke(CurrentPhase);

        // Advance to the next player's turn
        AdvanceTurn();

        InputLocked = false;

        Debug.Log($"Phase switched to: {CurrentPhase}");
    }

    private void NotifyPiecesOfPhaseChange()
    {
        // Find all pieces in the scene and tell them the phase changed.
        // This is a simple broadcast — for larger projects consider an event bus.
        Piece[] allPieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);
        foreach (Piece p in allPieces)
            p.OnPhaseChanged(CurrentPhase);
    }

    public void SetInputLocked(bool locked)
    {
        InputLocked = locked;
    }

    private void TriggerWin(PlayerTeam winner, WinReason reason)
    {
        InputLocked = true;
        string reasonText = reason switch
        {
            WinReason.KingCaptured => "King captured",
            WinReason.AllPiecesCaptured => "All pieces captured",
            WinReason.NoValidMoves => "Opponent has no valid moves",
            _ => "Unknown"
        };

        Debug.Log($"Game over — {winner} wins! Reason: {reasonText}");
        OnGameOver.Invoke(winner);
        //uiManager?.ShowWinScreen(winner, reasonText);
    }

    public void RestartGame()
    {
        CurrentTurn = PlayerTeam.White;
        CurrentPhase = GamePhase.Chess;
        InputLocked = false;
        MovesThisPhase = 0;

        OnTurnChanged.Invoke(CurrentTurn);
        OnPhaseChanged.Invoke(CurrentPhase);

        uiManager?.RefreshAll(CurrentTurn, CurrentPhase, MovesUntilSwitch);

        // BoardSetup should listen to OnGameOver or have its own restart method
        // to respawn pieces. Hook that up in the Inspector via UnityEvent or
        // call it directly here if you have a reference.
        // boardSetup.Restart();
    }

    //Utility
    public static PlayerTeam Opponent(PlayerTeam team) =>
       team == PlayerTeam.White ? PlayerTeam.Black : PlayerTeam.White;
}
