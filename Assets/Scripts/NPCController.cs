using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class NPCController : MonoBehaviour
{
    public GameObject[] boxPrefabs;  
    private GameObject currentBox;   
    public float moveSpeed = 5.0f;   
    public float tileSize = 1.0f;    
    public Animator animator;        
    public LayerMask impassableLayer;
    public LayerMask boxLayer;       

    public Vector3Int[] waypoints;   
    public Vector3[] directions;     

    private int currentWaypointIndex = 0;
    private bool isMoving = false;       
    private Vector3 startPos;            
    private Vector3 targetPos;           
    private float moveTime;              
    private Vector3 moveDirection;       
    private Vector3 lastMoveDirection;   
    private Tilemap tilemap;             
    private Vector3 boxStartPos;         
    private Vector3 boxTargetPos;        
    private bool isBoxMoving = false;    

    void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();  
        TeleportToRandomWaypoint();  
        StartCoroutine(MoveToNextTile());
    }

    IEnumerator MoveToNextTile()
    {
        while (true)
        {
            if (!isMoving)
            {
                Vector3 directionToMove = directions[currentWaypointIndex];
                Vector3 nextPos = transform.position + directionToMove * tileSize;

                if (IsPassable(nextPos))
                {
                    Move(directionToMove); 
                    MoveBox(directionToMove);  
                }
                else
                {
                    RemoveBox();
                    TeleportToRandomWaypoint();
                }
            }

            yield return null;
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

        UpdateAnimator(direction, true);  

        StartCoroutine(MoveNPCSmoothly());
    }

    IEnumerator MoveNPCSmoothly()
    {
        while (moveTime < 1.0f)
        {
            moveTime += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, moveTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;

        UpdateAnimator(moveDirection, false);
    }

    void MoveBox(Vector3 direction)
    {
        if (currentBox == null)
        {
            Vector3Int npcTilePosition = waypoints[currentWaypointIndex];

            Vector3 boxSpawnPos = npcTilePosition + direction;

            boxSpawnPos.x += -0.5f;
            boxSpawnPos.y += 0.5f;

            int randomIndex = Random.Range(0, boxPrefabs.Length);
            currentBox = Instantiate(boxPrefabs[randomIndex], boxSpawnPos, Quaternion.identity);

            boxStartPos = currentBox.transform.position;
            boxTargetPos = boxStartPos + direction;
            isBoxMoving = true;
            StartCoroutine(MoveBoxSmoothly());  
        }
        else
        {
            boxStartPos = currentBox.transform.position;
            boxTargetPos = currentBox.transform.position + direction * tileSize;

            StartCoroutine(MoveBoxSmoothly());
        }
    }

    IEnumerator MoveBoxSmoothly()
    {
        float boxMoveTime = 0f;

        while (boxMoveTime < 1.0f)
        {
            boxMoveTime += Time.deltaTime * moveSpeed;
            currentBox.transform.position = Vector3.Lerp(boxStartPos, boxTargetPos, boxMoveTime);
            yield return null;
        }

        currentBox.transform.position = boxTargetPos;
        isBoxMoving = false;
    }


    void RemoveBox()
    {
        if (currentBox != null)
        {
            Destroy(currentBox); 
            currentBox = null;   
        }
    }

    void UpdateAnimator(Vector3 direction, bool isMoving)
    {
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
        animator.SetFloat("Speed", 1f);  
    }

    void TeleportToRandomWaypoint()
    {
        currentWaypointIndex = Random.Range(0, waypoints.Length);

        Vector3Int randomTile = waypoints[currentWaypointIndex];
        Vector3 worldPos = tilemap.CellToWorld(randomTile);

        worldPos.x += -0.5f; 
        worldPos.y += 0.813f; 

        transform.position = worldPos;
        lastMoveDirection = directions[currentWaypointIndex];
        UpdateAnimator(lastMoveDirection, true);
    }
}
