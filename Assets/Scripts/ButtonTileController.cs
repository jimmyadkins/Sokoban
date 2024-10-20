using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ButtonTileController : MonoBehaviour
{
    public Tilemap buttonTilemap; 
    public Tilemap ySpikeTilemap; 
    public Tilemap rSpikeTilemap; 

    public Tile yButtonTile;  
    public Tile yButtonPressedTile;  
    public Tile rButtonTile;  
    public Tile rButtonPressedTile;  
    public Tile spikeExtendedTile;
    public Tile spikeRetractedTile;

    public Vector3Int[] ySpikePositions; 
    public Vector3Int[] rSpikePositions; 

    private bool isYButtonPressed = false; 
    private bool isRButtonPressed = false; 
    private Vector3Int yButtonTilePosition;
    private Vector3Int rButtonTilePosition;

    private void Update()
    {
        bool playerOnYButton = IsPlayerOnButton(yButtonTile, yButtonPressedTile);  
        bool crateOnYButton = IsCrateOnButton(yButtonTile, yButtonPressedTile);    
        bool playerOnRButton = IsPlayerOnButton(rButtonTile, rButtonPressedTile);  
        bool crateOnRButton = IsCrateOnButton(rButtonTile, rButtonPressedTile);    

        // Handle Yellow Button
        if ((playerOnYButton || crateOnYButton) && !isYButtonPressed)
        {
            yButtonTilePosition = GetButtonTilePosition(yButtonTile, yButtonPressedTile); 
            buttonTilemap.SetTile(yButtonTilePosition, yButtonPressedTile); 

            // Retract yellow spikes
            foreach (Vector3Int spikePos in ySpikePositions)
            {
                ySpikeTilemap.SetTile(spikePos, spikeRetractedTile);
            }

            ySpikeTilemap.gameObject.layer = LayerMask.NameToLayer("Default"); 
            isYButtonPressed = true;  
        }
        else if (!playerOnYButton && !crateOnYButton && isYButtonPressed)
        {
            buttonTilemap.SetTile(yButtonTilePosition, yButtonTile);  

            // Extend yellow spikes
            foreach (Vector3Int spikePos in ySpikePositions)
            {
                ySpikeTilemap.SetTile(spikePos, spikeExtendedTile);
            }

            ySpikeTilemap.gameObject.layer = LayerMask.NameToLayer("Walls"); 
            isYButtonPressed = false; 
        }

        // Handle Red Button
        if ((playerOnRButton || crateOnRButton) && !isRButtonPressed)
        {
            rButtonTilePosition = GetButtonTilePosition(rButtonTile, rButtonPressedTile); 
            buttonTilemap.SetTile(rButtonTilePosition, rButtonPressedTile); 

            // Retract red spikes
            foreach (Vector3Int spikePos in rSpikePositions)
            {
                rSpikeTilemap.SetTile(spikePos, spikeRetractedTile);
            }

            rSpikeTilemap.gameObject.layer = LayerMask.NameToLayer("Default"); 
            isRButtonPressed = true; 
        }
        else if (!playerOnRButton && !crateOnRButton && isRButtonPressed)
        {
            buttonTilemap.SetTile(rButtonTilePosition, rButtonTile);  

            foreach (Vector3Int spikePos in rSpikePositions)
            {
                rSpikeTilemap.SetTile(spikePos, spikeExtendedTile);
            }

            rSpikeTilemap.gameObject.layer = LayerMask.NameToLayer("Walls");  
            isRButtonPressed = false;  
        }
    }

    private Vector3Int GetTilePosition(Vector3 worldPosition)
    {
        return buttonTilemap.WorldToCell(worldPosition);
    }

    private bool IsPlayerOnButton(Tile buttonTile, Tile buttonPressedTile)
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3Int tilePosition = GetTilePosition(playerPosition);
        TileBase tileAtPlayerPos = buttonTilemap.GetTile(tilePosition);

        return tileAtPlayerPos == buttonTile || tileAtPlayerPos == buttonPressedTile;
    }

    private bool IsCrateOnButton(Tile buttonTile, Tile buttonPressedTile)
    {
        GameObject[] crates = GameObject.FindGameObjectsWithTag("Crate").Concat(GameObject.FindGameObjectsWithTag("GreenBox")).Concat(GameObject.FindGameObjectsWithTag("RedBox")).ToArray();

        foreach (GameObject crate in crates)
        {
            Vector3Int crateTilePosition = GetTilePosition(crate.transform.position);
            TileBase tileAtCratePos = buttonTilemap.GetTile(crateTilePosition);

            if (tileAtCratePos == buttonTile || tileAtCratePos == buttonPressedTile)
            {
                return true;
            }
        }

        return false;
    }

    private Vector3Int GetButtonTilePosition(Tile buttonTile, Tile buttonPressedTile)
    {
        GameObject[] crates = GameObject.FindGameObjectsWithTag("Crate").Concat(GameObject.FindGameObjectsWithTag("GreenBox")).Concat(GameObject.FindGameObjectsWithTag("RedBox")).ToArray();
        foreach (GameObject crate in crates)
        {
            Vector3Int crateTilePosition = GetTilePosition(crate.transform.position);
            TileBase tileAtCratePos = buttonTilemap.GetTile(crateTilePosition);

            if (tileAtCratePos == buttonTile || tileAtCratePos == buttonPressedTile)
            {
                return crateTilePosition;
            }
        }

        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        return GetTilePosition(playerPosition);
    }
}
