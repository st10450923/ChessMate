using UnityEngine;
using System.Collections;
public enum ChessIdentity
{
    King,
    Queen,
    Bishop,
    Knight,
    Rook,
    Pawn
}
public enum PlayerTeam
{
    White,
    Black
}

public enum GamePhase
{
    Chess,
    Checkers
}
public class Piece : MonoBehaviour
{
    [SerializeField] private ChessIdentity chessIdentity;
    [SerializeField] private PlayerTeam team;

    [SerializeField] private Sprite chessSprite;
    [SerializeField] private Sprite checkersSprite;

    [SerializeField] private Sprite checkersKingSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private GameObject selectionIndicator;

    [SerializeField] private float moveSpeed = 0.15f;
    [SerializeField] private float captureAnimDuration = 0.2f;


    public ChessIdentity ChessIdentity => chessIdentity;
    public PlayerTeam Team => team;
    public int Col { get; private set; }
    public int Row { get; private set; }
    public Vector2Int GridPosition => new Vector2Int(Col, Row);
    public bool IsCheckersKing { get; private set; }
    public bool IsPromotedQueen { get; private set; }


    public void Initialise(ChessIdentity type, PlayerTeam playerTeam, int col, int row)
    {
        chessIdentity = type;
        team = playerTeam;
        Col = col;
        Row = row;
        IsCheckersKing = false;
        IsPromotedQueen = false;
        RefreshSprite(GamePhase.Chess);
    }

    public void SetGridPosition(int col, int row)
    {
        Col = col;
        Row = row;
    }
    public void AnimateTo(Vector3 worldTarget)
    {
        StopAllCoroutines();
        StartCoroutine(SlideToPosition(worldTarget));
    }

    private IEnumerator SlideToPosition(Vector3 target)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < moveSpeed)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / moveSpeed);
            yield return null;
        }

        transform.position = target;
    }

    public void SetSelected(bool selected)
    {
        if (selectionIndicator != null)
            selectionIndicator.SetActive(selected);
    }

    public void OnPhaseChanged(GamePhase newPhase)
    {
        RefreshSprite(newPhase);
    }

    private void RefreshSprite(GamePhase phase)
    {
        if (spriteRenderer == null) return;

        if (phase == GamePhase.Chess)
        {
            // During Chess phase always show the chess piece sprite,
            // even if this piece was a checkers king last phase.
            spriteRenderer.sprite = chessSprite;
        }
        else
        {
            // During Checkers phase: king sprite if kinged, disc otherwise
            spriteRenderer.sprite = IsCheckersKing ? checkersKingSprite : checkersSprite;
        }
    }

    public void SetCheckersKing(bool value)
    {
        IsCheckersKing = value;

        // Only update the sprite if we're currently in the Checkers phase.
        // If this is called during a phase reset (before the switch animation),
        // the sprite will be refreshed by OnPhaseChanged anyway.
        if (spriteRenderer != null && IsInCheckersPhase())
            spriteRenderer.sprite = value ? checkersKingSprite : checkersSprite;
    }

    private bool IsInCheckersPhase()
    {
        return spriteRenderer.sprite == checkersSprite
            || spriteRenderer.sprite == checkersKingSprite;
    }

    public void PromoteToQueen()
    {
        chessIdentity = ChessIdentity.Queen;
        IsPromotedQueen = true;
        RefreshSprite(GamePhase.Chess);

        Debug.Log($"{team} pawn promoted to Queen at ({Col},{Row})");
    }

    public void OnCaptured()
    {
        SetSelected(false);
        StopAllCoroutines();
        StartCoroutine(CaptureAnimation());
    }
    private IEnumerator CaptureAnimation()
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Color startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        while (elapsed < captureAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / captureAnimDuration;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            if (spriteRenderer != null)
                spriteRenderer.color = Color.Lerp(startColor, Color.clear, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}
