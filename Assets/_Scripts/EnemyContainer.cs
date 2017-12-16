using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer
{
    public NPCStats stats;
    public GameObject gameObject;
    public LerpBackForth attackAnimator;

    public EnemyContainer(NPCStats stats, GameObject gameObject, LerpBackForth attackAnimator)
    {
        this.stats = stats;
        this.gameObject = gameObject;
        this.attackAnimator = attackAnimator;
    }



}
