using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomEnemy : MonoBehaviour {

    public int minimumInterval;
    public int maximumInterval;
    private bool timerIsSet;
    private System.Random randGenerator;


	// Use this for initialization
	void Start () {
        this.randGenerator = new System.Random();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    private void FixedUpdate()
    {
        if (!timerIsSet)
        {
            timerIsSet = true;
            int interval = randGenerator.Next(minimumInterval, maximumInterval + 1);
            Invoke("spawnEnemy", interval);
        }
    }

    private void spawnEnemy()
    {
        print("Spawned an enemy");
        timerIsSet = false;
    }

}
