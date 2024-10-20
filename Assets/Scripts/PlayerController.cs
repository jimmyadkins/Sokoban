using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;  
    public float tileSize = 1.0f;   
    public Animator animator;       
    public LayerMask impassableLayer; 
    public LayerMask boxLayer; 

    private bool isMoving = false;  
    private Vector3 startPos;       
    private Vector3 targetPos;      
    private float moveTime;         
    private Vector3 moveDirection;  
    private Vector3 lastMoveDirection;

    void Update()
    {
        if (!isMoving)
        {
            HandleInput();  
        }
        else
        {
            MovePlayerSmoothly();  
        }
    }

    void HandleInput()
    {
        Vector3 inputDirection = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) inputDirection = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) inputDirection = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) inputDirection = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) inputDirection = Vector3.right;

        if (inputDirection != Vector3.zero && !isMoving)
        {
            Vector3 targetPos = transform.position + inputDirection * tileSize;

            UpdateAnimator(inputDirection, true); 

            Collider2D boxHit = Physics2D.OverlapCircle(targetPos, 0.1f, boxLayer);
            if (boxHit != null)
            {
                Vector3 boxTargetPos = boxHit.transform.position + inputDirection * tileSize;

                if (IsPassable(boxTargetPos) && !IsBoxOnTile(boxTargetPos))
                {
                    StartCoroutine(MoveBox(boxHit.gameObject, inputDirection));  
                    Move(inputDirection);  
                    lastMoveDirection = inputDirection;  
                }
                else
                {
                    Debug.Log("Can't push the box there, tile is impassable or another box is there!");
                    UpdateAnimator(inputDirection, false); 
                    lastMoveDirection = inputDirection; 
                }
            }
            else if (IsPassable(targetPos))
            {
                Move(inputDirection);  
                lastMoveDirection = inputDirection;  
            }
            else
            {
                Debug.Log("Can't move there, tile is impassable!");
                UpdateAnimator(inputDirection, false); 
                lastMoveDirection = inputDirection;  
            }
        }
    }
    bool IsPassable(Vector3 targetPos)
    {
        Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.1f, impassableLayer);
        return hit == null;  
    }
    bool IsBoxOnTile(Vector3 targetPos)
    {
        Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.1f, boxLayer);
        return hit != null;  
    }
    void Move(Vector3 direction)
    {
        startPos = transform.position;
        targetPos = startPos + direction * tileSize;

        moveDirection = direction; 
        isMoving = true;           
        moveTime = 0f;             
    }

    void MovePlayerSmoothly()
    {
        moveTime += Time.deltaTime * moveSpeed;
        transform.position = Vector3.Lerp(startPos, targetPos, moveTime);

        if (moveTime >= 1.0f)
        {
            transform.position = targetPos; 
            isMoving = false;              
            UpdateAnimator(lastMoveDirection, false);  
        }
    }

    void UpdateAnimator(Vector3 direction, bool isMoving)
    {
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);

        animator.SetFloat("Speed", isMoving ? 1f : 0f);
    }

    IEnumerator MoveBox(GameObject box, Vector3 direction)
    {
        Vector3 boxStartPos = box.transform.position;
        Vector3 boxTargetPos = boxStartPos + direction * tileSize;
        float boxMoveTime = 0f;

        while (boxMoveTime < 1.0f)
        {
            boxMoveTime += Time.deltaTime * moveSpeed;
            box.transform.position = Vector3.Lerp(boxStartPos, boxTargetPos, boxMoveTime);
            yield return null;
        }

        box.transform.position = boxTargetPos; 
    }
}
