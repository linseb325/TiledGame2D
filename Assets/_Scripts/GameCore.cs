using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore {

    public static GameObject mainSceneStuff;

    public static NPCStats playerStats;

    public static EnemyContainer currentEnemy;

    public static int rollDice(int numSides)
    {
        return Random.Range(1, numSides+1);
    }








}
