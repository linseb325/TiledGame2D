using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoraFight : MonoBehaviour
{
    public GameObject fightMenu;
    public GameObject arrow;
    public float offsetBetweenMenuOptions;

    private Vector2 noraIdlePos;
    private Vector2 noraFightPos;

    private int selectedIndex = 0;
    private Vector2 arrowPos0;
    private Vector2 arrowPos1;
    private Vector2[] arrowPositions;



	// Use this for initialization
	void Start () {
        // GameCore.mainSceneStuff.SetActive(false);
        arrowPos0 = this.arrow.transform.position;
        arrowPos1 = arrowPos0 + (offsetBetweenMenuOptions * Vector2.down);
        arrowPositions = new Vector2[2];
        arrowPositions[0] = arrowPos0;
        arrowPositions[1] = arrowPos1;

        this.noraIdlePos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Exit the scene
            GameCore.mainSceneStuff.SetActive(true);
            SceneManager.UnloadSceneAsync("FightScene");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.selectedIndex++;
            this.arrow.transform.position = arrowPositions[selectedIndex % 2];
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.selectedIndex++;
            this.arrow.transform.position = arrowPositions[selectedIndex % 2];
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            // Selected an attack
            this.fightMenu.SetActive(false);
        }
	}



}
