using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer
{
    public NPCStats stats;
    public GameObject gameObject;

    public EnemyContainer(NPCStats stats, GameObject gameObject)
    {
        this.stats = stats;
        this.gameObject = gameObject;
    }



}
