using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[RequireComponent(typeof(Actor))]
[RequireComponent(typeof(Actor), typeof(AStar))]
public class Enemy : MonoBehaviour

{
    public Actor Target { get; set; }

    // Variabele voor het aangeven of er een gevecht aan de gang is (isFighting)
    public bool IsFighting { get; private set; } = false;

    // Variabele voor het algoritme (Algorithm)
    public AStar Algorithm { get; private set; }
    private void Start()

    {

        GameManager.Get.AddEnemy(GetComponent<Actor>());
        Algorithm = GetComponent<AStar>();


    }

    public void MoveAlongPath(Vector3Int targetPosition)

    {

        Vector3Int gridPosition = MapManager.Get.FloorMap.WorldToCell(transform.position);

        Vector2 direction = Algorithm.Compute((Vector2Int)gridPosition, (Vector2Int)targetPosition);

        Action.Move(GetComponent<Actor>(), direction);

    }
    private void Update()
    {
        RunAI();
    }

    public void RunAI()
    {
        // If target is null, set target to player (from gameManager)
        if (Target == null)
        {
            Target = GameManager.Get.Player;
        }

        // Convert the position of the target to a gridPosition
        Vector3Int gridPosition = MapManager.Get.FloorMap.WorldToCell(Target.transform.position);

        // First check if already fighting, because the FieldOfView check costs more CPU
        if (IsFighting || GetComponent<Actor>().FieldOfView.Contains(gridPosition))
        {
            // If the enemy was not fighting, it should be fighting now
            IsFighting = true;

            // Call MoveAlongPath with the gridPosition
            MoveAlongPath(gridPosition);
        }
    }
}


