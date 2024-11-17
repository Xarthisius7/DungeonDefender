using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MinimapGenerator : MonoBehaviour
{
    public List<Sprite> miniMapSprite; // Editable list of all directional sprites
    public List<Sprite> roomIcons;     // Editable list of special room icons

    public GameObject startLoc;        // Starting location of the minimap
    public GameObject mapPieceSample;  // Prefab for each room piece on the minimap
    public GameObject Frame;

    public int xMapOffset = 10;        // Horizontal offset for minimap pieces
    public int yMapOffset = 10;        // Vertical offset for minimap pieces

    public Vector3 mapPieceScale = new Vector3(1, 1, 1);
    public Transform playerTransform;  // Player transform to track position
    public float minimapScale = 0.1f;  // Scale factor to convert player movement to minimap movement

    private GameObject[,] mapPiecesGenerated;


    private void Update()
    {
        UpdateMinimap();
    }

    public void UpdateMinimap()
    {
        if (playerTransform != null && Frame != null)
        {
            // Update the position of the minimap frame based on the player's position
            Frame.transform.localPosition = new Vector3(-playerTransform.position.x * minimapScale, -playerTransform.position.y * minimapScale, Frame.transform.localPosition.z);
        }
    }

    public GameObject[,] GenerateMinimap(GridCell[,] grid, int gridSize)
    {
        int halfGridSize = gridSize / 2;

        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);
        mapPiecesGenerated = new GameObject[rows, cols];

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y].hasRoom)
                {
                    // Retrieve the connected directions for this room
                    string actualDir = string.Join("", grid[x, y].connectedDir);
                    actualDir = string.Concat(new string(actualDir.Distinct().ToArray()).OrderBy(c => c));

                    // Instantiate the map piece
                    GameObject mapPiece = Instantiate(mapPieceSample, Frame.transform);
                    mapPiece.transform.localPosition = new Vector3((x - halfGridSize) * xMapOffset, (y - halfGridSize) * yMapOffset, 0);
                    mapPiece.transform.localScale = mapPieceScale;

                    // Find the "Base" child object and update its image component
                    Transform baseChild = mapPiece.transform.Find("Base");
                    if (baseChild != null)
                    {
                        Image baseImage = baseChild.GetComponent<Image>();
                        if (baseImage != null)
                        {
                            // Set the sprite for the base based on actualDir
                            int spriteIndex = GetSpriteIndex(actualDir);
                            if (spriteIndex >= 0 && spriteIndex < miniMapSprite.Count)
                            {
                                baseImage.sprite = miniMapSprite[spriteIndex];
                            }
                        }
                    }

                    // If the room has a special feature, render the special room icon
                    if (!string.IsNullOrEmpty(grid[x, y].roomFeature))
                    {
                        baseChild = mapPiece.transform.Find("Icon");
                        int featureIndex = GetFeatureIndex(grid[x, y].roomFeature);
                        if (featureIndex >= 0 && featureIndex < roomIcons.Count)
                        {
                            Image featureImage = baseChild.GetComponent<Image>();
                            if (featureImage != null)
                            {
                                featureImage.sprite = roomIcons[featureIndex];
                            }
                        }
                    }

                    mapPiecesGenerated[x,y] = mapPiece;


                }
            }
        }
        return mapPiecesGenerated;
    }

    private int GetSpriteIndex(string actualDir)
    {
        // List of all possible direction combinations
        List<string> directionCombinations = new List<string>
        {
            "U", "B", "L", "R", "BU", "LU", "RU", "BL", "BR", "LR",
            "BLU", "BRU", "LRU", "BLR", "BLRU"
        };

        return directionCombinations.IndexOf(actualDir);
    }

    private int GetFeatureIndex(string roomFeature)
    {
        // List of all special room features
        List<string> features = new List<string> { "TREASURE", "HIGHDIFF", "TRAP", "FINALEXIT", "BASEROOM" };
        return features.IndexOf(roomFeature);
    }
}
