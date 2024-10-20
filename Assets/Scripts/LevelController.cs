using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class LevelController : MonoBehaviour
{
    public Tilemap targetTilemap; 

    public List<Vector3Int> redGoalTilePositions = new List<Vector3Int>();  
    public List<Vector3Int> greenGoalTilePositions = new List<Vector3Int>();

    public GameObject redGoalParticlePrefab;  
    public GameObject greenGoalParticlePrefab;

    private bool areAllRedBoxesOnGoals = false;  
    private bool areAllGreenBoxesOnGoals = false;

    private HashSet<Vector3Int> particlesTriggered = new HashSet<Vector3Int>();  

    public PauseMenuController pauseMenuController; 

    private void Update()
    {
        areAllRedBoxesOnGoals = AreAllBoxesOnGoalTiles("RedBox", redGoalTilePositions, redGoalParticlePrefab);
        areAllGreenBoxesOnGoals = AreAllBoxesOnGoalTiles("GreenBox", greenGoalTilePositions, greenGoalParticlePrefab);

        if (areAllRedBoxesOnGoals && areAllGreenBoxesOnGoals)
        {
            StartCoroutine(DelayedLevelComplete());  
        }
    }

    private IEnumerator DelayedLevelComplete()
    {
        yield return new WaitForSeconds(1f);  
        pauseMenuController.ShowLevelComplete();  
    }
    private bool AreAllBoxesOnGoalTiles(string boxTag, List<Vector3Int> goalTilePositions, GameObject particlePrefab)
    {
        GameObject[] boxes = GameObject.FindGameObjectsWithTag(boxTag);  
        HashSet<Vector3Int> boxesOnGoalTiles = new HashSet<Vector3Int>();

        foreach (GameObject box in boxes)
        {
            Vector3Int boxTilePosition = targetTilemap.WorldToCell(box.transform.position);  

            if (goalTilePositions.Contains(boxTilePosition)) 
            {
                boxesOnGoalTiles.Add(boxTilePosition);  

                if (!particlesTriggered.Contains(boxTilePosition))
                {
                    Vector3 particlePosition = targetTilemap.CellToWorld(boxTilePosition) + new Vector3(0.5f, 0.5f, 0f);
                    Instantiate(particlePrefab, particlePosition, Quaternion.identity);  
                    particlesTriggered.Add(boxTilePosition);  
                }
            }
        }

        foreach (Vector3Int goalTilePos in goalTilePositions)
        {
            if (!boxesOnGoalTiles.Contains(goalTilePos) && particlesTriggered.Contains(goalTilePos))
            {
                particlesTriggered.Remove(goalTilePos);  
            }
        }

        return boxesOnGoalTiles.Count == boxes.Length;
    }
}
