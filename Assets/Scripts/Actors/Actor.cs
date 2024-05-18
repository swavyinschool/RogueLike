using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    // Instance of the visibility algorithm used for field of view calculations
    private AdamMilVisibility algorithm;

    // List to store the positions within the actor's field of view
    public List<Vector3Int> FieldOfView = new List<Vector3Int>();

    // Range of the actor's field of view
    public int FieldOfViewRange = 8;

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize the visibility algorithm
        algorithm = new AdamMilVisibility();

        // Calculate the initial field of view
        UpdateFieldOfView();
    }

    // Method to move the actor in a given direction
    public void Move(Vector3 direction)
    {
        // Check if the target position is walkable
        if (MapManager.Get.IsWalkable(transform.position + direction))
        {
            // Update the actor's position
            transform.position += direction;

            // Update the field of view after moving
            UpdateFieldOfView();
        }
    }

    // Method to update the actor's field of view
    public void UpdateFieldOfView()
    {
        // Convert the actor's current world position to grid position
        var pos = MapManager.Get.FloorMap.WorldToCell(transform.position);

        // Clear the current field of view list
        FieldOfView.Clear();

        // Compute the field of view using the visibility algorithm
        algorithm.Compute(pos, FieldOfViewRange, FieldOfView);

        // If the actor is a player, update the fog of war on the map
        if (GetComponent<Player>())
        {
            MapManager.Get.UpdateFogMap(FieldOfView);
        }
    }
}
