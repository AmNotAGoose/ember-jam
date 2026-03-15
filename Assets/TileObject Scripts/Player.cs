using System.Collections;
using Unity.VisualScripting;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class Player : TileObject
{
    Level level;

    public int xDir = 0;
    public int yDir = 0;
    private float moveTimer = 0f;

    public bool canMove = true;
    public float moveDelay = 0.15f;

    public TileObject heldObject;
    public PlayerAssets playerAssets;

    void Start()
    {
        level = FindFirstObjectByType<Level>();

        type = "player";
        properties.Add(TileObjectProperies.Pushable);

        level.player = this;    
    }

    private void Update()
    {
        moveTimer -= Time.deltaTime;

        xDir = 0;
        yDir = 0;

        if (Input.GetKey(KeyCode.UpArrow)) yDir = 1;
        else if (Input.GetKey(KeyCode.DownArrow)) yDir = -1;
        else if (Input.GetKey(KeyCode.RightArrow)) xDir = 1;
        else if (Input.GetKey(KeyCode.LeftArrow)) xDir = -1;

        if ((xDir != 0 || yDir != 0) && moveTimer <= 0f)
        {
            moveTimer = moveDelay;
            level.TryMovePlayer(xDir, yDir);
        } else if ((xDir == 0 && yDir == 0))
        {
            playerAssets.SetIdleAnimation();
        }

        if (Input.GetKeyDown(KeyCode.Space))
            level.TryPlaceObject();
    }

    public override IEnumerator MoveToLocalOrigin()
    {
        playerAssets.SetActiveAnimation(xDir, yDir);
        yield return StartCoroutine(base.MoveToLocalOrigin());
        if (heldObject != null)
        {
            heldObject.OnPickedFinished();
        }
    }

    public void HoldObject(TileObject objectToHold)
    {
        heldObject = objectToHold;
        objectToHold.OnPicked();
    }

    public void DropObject()
    {
        heldObject.OnDropped(tile);
        heldObject = null;
    }

    public override void OnAffectedTickFinished()
    {
        base.OnAffectedTickFinished();
        level.UpdateLayers();
    }
}
