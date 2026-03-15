using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class Player : TileObject
{
    Level level;
    int lastLayer = 0;
    public int xDir = 0;
    public int yDir = 0;
    private float moveTimer = 0f;
    public float moveDelay = 0.15f;
    public TileObject heldObject;
    public PlayerAssets playerAssets;

    private Queue<(int x, int y)> moveQueue = new Queue<(int x, int y)>();
    private bool isProcessing = false;

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
            moveQueue.Enqueue((xDir, yDir));
        }
        else if (xDir == 0 && yDir == 0 && !isProcessing && moveQueue.Count == 0)
        {
            playerAssets.SetIdleAnimation();
        }

        if (!isProcessing && moveQueue.Count > 0)
            StartCoroutine(ProcessQueue());

        if (Input.GetKeyDown(KeyCode.Space))
            level.TryPlaceObject();
    }

    private IEnumerator ProcessQueue()
    {
        isProcessing = true;
        while (moveQueue.Count > 0)
        {
            var (x, y) = moveQueue.Dequeue();
            level.TryMovePlayer(x, y);
            yield return new WaitUntil(() => !isMoving);
        }
        isProcessing = false;
    }

    public override IEnumerator MoveToLocalOrigin()
    {
        playerAssets.SetActiveAnimation(xDir, yDir);
        yield return StartCoroutine(base.MoveToLocalOrigin());
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