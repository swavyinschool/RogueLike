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

}


