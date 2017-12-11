using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStats {

    private int hitPoints;
    private int magicPoints;
    private int armorClass;
    private int initiative;


    public NPCStats(int hp, int mp, int ac)
    {
        this.hitPoints = hp;
        this.magicPoints = mp;
        this.armorClass = ac;
    }

    public int getHP()
    {
        return hitPoints;
    }

    public int getMP()
    {
        return magicPoints;
    }

    public int getAC()
    {
        return armorClass;
    }



    public void reduceHP(int amount)
    {
        this.hitPoints -= amount;
    }

    public void reduceMP(int amount)
    {
        this.magicPoints -= amount;
    }




}
