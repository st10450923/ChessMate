using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SwitchController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform boardVisualRoot;
    [SerializeField] private float flipDuration = 0.8f;
    [SerializeField][Range(0f, 1f)] private float pieceHidePoint = 0.45f;
    [SerializeField][Range(0f, 1f)] private float pieceShowPoint = 0.55f;
    [SerializeField] private Sprite chessBoardSprite;
    [SerializeField] private Sprite checkersBoardSprite;
    [SerializeField] private SpriteRenderer boardSpriteRenderer;
    [SerializeField] private AudioClip flipSound;
    [SerializeField] private SoundManager soundManager;


    public UnityEvent OnFlipStarted = new UnityEvent();
    public UnityEvent OnFlipComplete = new UnityEvent();


    private bool isFlipping = false;
    private Piece[] allPieces;

    public void BeginSwitch()
    {
        if (isFlipping)
        {
            Debug.LogWarning("SwitchController.BeginSwitch called while already flipping.");
            return;
        }

        StartCoroutine(FlipSequence());
    }
    private IEnumerator FlipSequence()
    {
        isFlipping = true;

        allPieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);

        OnFlipStarted.Invoke();

        if (soundManager != null && flipSound != null)
            soundManager.PlaySFXClip(flipSound,boardVisualRoot, 1f);

        yield return StartCoroutine(AnimateBoardFlip());

        gameManager.OnSwitchComplete();

        ApplyBoardTheme(gameManager.CurrentPhase);

        OnFlipComplete.Invoke();

        isFlipping = false;
    }
    private IEnumerator AnimateBoardFlip()
    {
        if (boardVisualRoot == null)
        {
            Debug.LogWarning("SwitchController: boardVisualRoot is not assigned. " +
                             "Skipping flip animation.");
            yield return new WaitForSeconds(0.1f);
            yield break;
        }

        float halfDuration = flipDuration * 0.5f;
        bool piecesHidden = false;
        bool piecesShown = false;
        bool spriteSwapped = false;

        Vector3 originalScale = boardVisualRoot.localScale;

        // --- First half: squeeze X scale from 1 to 0 ---
        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            boardVisualRoot.localScale = new Vector3(
                Mathf.Lerp(originalScale.x, 0f, smoothT),
                originalScale.y,
                originalScale.z);

            // Hide pieces just before the board goes flat
            float globalT = elapsed / flipDuration;
            if (!piecesHidden && globalT >= pieceHidePoint && pieceHidePoint > 0f)
            {
                SetPiecesVisible(false);
                piecesHidden = true;
            }

            yield return null;
        }

        // Snap to exactly flat
        boardVisualRoot.localScale = new Vector3(0f, originalScale.y, originalScale.z);

        // Swap the board sprite at the midpoint while it's invisible
        if (!spriteSwapped)
        {
            SwapBoardSpriteForIncomingPhase();
            spriteSwapped = true;
        }

        // --- Second half: expand X scale from 0 back to 1 ---
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            boardVisualRoot.localScale = new Vector3(
                Mathf.Lerp(0f, originalScale.x, smoothT),
                originalScale.y,
                originalScale.z);

            // Reveal pieces as the board expands back out
            float globalT = 0.5f + (elapsed / flipDuration);
            if (!piecesShown && globalT >= pieceShowPoint && pieceHidePoint > 0f)
            {
                SetPiecesVisible(true);
                piecesShown = true;
            }

            yield return null;
        }

        // Snap back to original scale
        boardVisualRoot.localScale = originalScale;

        if (pieceHidePoint > 0f)
            SetPiecesVisible(true);
    }
    private void SwapBoardSpriteForIncomingPhase()
    {
        if (boardSpriteRenderer == null) return;

        bool incomingIsChess = gameManager.CurrentPhase == GamePhase.Checkers;

        Sprite incoming = incomingIsChess ? chessBoardSprite : checkersBoardSprite;
        if (incoming != null)
            boardSpriteRenderer.sprite = incoming;
    }
    private void SetPiecesVisible(bool visible)
    {
        if (allPieces == null) return;

        foreach (Piece piece in allPieces)
        {
            if (piece == null) continue;

            // Toggle all renderers on the piece and its children
            Renderer[] renderers = piece.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
                r.enabled = visible;
        }
    }
    private void ApplyBoardTheme(GamePhase newPhase)
    {
        if (boardSpriteRenderer == null) return;

        Sprite target = newPhase == GamePhase.Chess
            ? chessBoardSprite
            : checkersBoardSprite;

        if (target != null)
            boardSpriteRenderer.sprite = target;
    }
#if UNITY_EDITOR
    /// <summary>
    /// Test the flip animation from the Unity Editor without entering Play Mode.
    /// Call via the Inspector context menu (right-click the component header).
    /// </summary>
    [ContextMenu("Preview Flip (Play Mode Only)")]
    private void PreviewFlip()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("PreviewFlip only works in Play Mode.");
            return;
        }
        BeginSwitch();
    }
#endif
}
