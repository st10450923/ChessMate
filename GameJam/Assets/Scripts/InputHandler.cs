using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Board board;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private LayerMask boardLayerMask;
    [SerializeField] private float boardZDepth = 0f;

    private void Awake()
    {
        if (gameCamera == null)
            gameCamera = Camera.main;
    }
    private void Update()
    {
        if (gameManager.InputLocked) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
            HandleLeftClick();

        if (mouse.rightButton.wasPressedThisFrame)
            HandleRightClick();
    }

    //Click handling
    private void HandleLeftClick()
    {
        Vector2Int? gridPos = GetBoardGridPosition();
        if (gridPos == null) return;

        board.OnSquareClicked(gridPos.Value.x, gridPos.Value.y);
    }

    private void HandleRightClick()
    {
        board.OnRightClick();
    }
    private Vector2Int? GetBoardGridPosition()
    {
        Vector3 worldPos = GetWorldPosition();
        Vector2Int gridPos = board.WorldToGrid(worldPos);

        if (!board.InBounds(gridPos.x, gridPos.y))
            return null;

        return gridPos;
    }
    private Vector3 GetWorldPosition()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 pos = new Vector3(mouseScreen.x, mouseScreen.y, 0f);
        pos.z = gameCamera.WorldToScreenPoint(
            new Vector3(0f, 0f, boardZDepth)).z;
        return gameCamera.ScreenToWorldPoint(pos);
    }
}
