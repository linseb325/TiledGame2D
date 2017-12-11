using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoraFight : MonoBehaviour
{
    public GameObject fightMenu;
    public GameObject arrow;
    public float offsetBetweenMenuOptions;

    private int selectedIndex = 0;
    private Vector2 arrowPos0;
    private Vector2 arrowPos1;
    private Vector2[] arrowPositions;

    private bool isMoving = false;
    private bool canPressButtons = true;
    private bool isMyTurn = true;

    public GameObject noraIdlePos;
    public GameObject noraFightPos;

    private Transform startMarker;
    private Transform endMarker;
    public float speed;
    private float startTime;
    private float journeyLength;

    private AudioSource audioPlayer;
    public AudioClip noraWinsMusic;
    public AudioClip enemyWinsMusic;

    private string[] enemyAttacks = new string[] { "Punch", "Machine Gun" };


	// Use this for initialization
	void Start () {

        GameCore.mainSceneStuff.SetActive(false);

        arrowPos0 = this.arrow.transform.position;
        arrowPos1 = arrowPos0 + (offsetBetweenMenuOptions * Vector2.down);
        arrowPositions = new Vector2[2];
        arrowPositions[0] = arrowPos0;
        arrowPositions[1] = arrowPos1;

        this.noraIdlePos.transform.position = this.transform.position;
        startMarker = this.noraIdlePos.transform;
        endMarker = this.noraFightPos.transform;

        this.audioPlayer = this.GetComponent<AudioSource>();
	}

    private void FixedUpdate()
    {
        if (isMoving)
        {
            if (Mathf.Abs(this.transform.position.x - endMarker.position.x) < .01f)
            {
                // It's time to stop moving
                stopMoving();
                if (Mathf.Abs(endMarker.position.x - noraFightPos.transform.position.x) < .01f)
                {
                    // If we're in fighting position...
                    // print("Reached fight pos");
                    swapMarkers();
                    Invoke("startMoving", 0.5f);
                }
                else if (Mathf.Abs(endMarker.position.x - noraIdlePos.transform.position.x) < .01f)
                {
                    // If we're back to the original idle position...
                    // print("Reached idle pos");
                    swapMarkers();
                    canPressButtons = true;
                }
            }
            else
            {
                // Move
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
                this.transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
            }
        }
    }

    private void swapMarkers()
    {
        Transform temp = this.startMarker;
        startMarker = endMarker;
        endMarker = temp;
    }

    private void setUpLerpStuff()
    {
        this.journeyLength = Vector3.Distance(endMarker.position, startMarker.position);
        startTime = Time.time;
    }

    private void startMoving()
    {
        setUpLerpStuff();
        canPressButtons = false;
        isMoving = true;
    }

    private void stopMoving()
    {
        isMoving = false;
    }



    // Update is called once per frame
    void Update ()
    {
        if (canPressButtons)
        {
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
            else if (Input.GetKeyDown(KeyCode.Return) && isMyTurn)
            {
                // Nora selected an attack

                // Check whether Nora selected taze without any magic points left
                if ((Mathf.Abs((this.arrow.transform.position.y - arrowPos0.y)) <= .1f) && !canUseMagic())
                {
                    print("Nora doesn't have enough magic points to taze! Select a different attack.");
                }
                else
                {
                    // Roll the dice
                    int diceRoll = GameCore.rollDice(20);
                    print("Rolled a " + diceRoll);

                    // Check to see if the roll was high enough to attack
                    if (diceRoll >= GameCore.currentEnemy.stats.getAC())
                    {
                        // At this point, we know Nora can attack

                        // Which attack did we select?
                        if (Mathf.Abs((this.arrow.transform.position.y - arrowPos0.y)) <= .1f)
                        {
                            // Selected taze
                            if (canUseMagic())
                            {
                                // Selected taze and can taze
                                startMoving();

                                print("TAZE!");
                                GameCore.currentEnemy.stats.reduceHP(2);
                                GameCore.playerStats.reduceMP(1);

                                printCurrentFightStats();
                                checkForKoolaidDeathAndSwitch();
                            }
                            else
                            {
                                // Selected taze but can't taze
                                print("Nora doesn't have enough magic points to taze! Select a different attack.");
                            }
                        }
                        else if (Mathf.Abs((this.arrow.transform.position.y - arrowPos1.y)) <= .1f)
                        {
                            // Selected ninja
                            startMoving();

                            print("NINJA!");
                            GameCore.currentEnemy.stats.reduceHP(1);

                            printCurrentFightStats();
                            checkForKoolaidDeathAndSwitch();
                        }

                    }
                    else
                    {
                        // Bad dice roll
                        print("Nora can't attack - dice roll too low!");
                        checkForKoolaidDeathAndSwitch();
                    }

                }
            }
        }
	}

    private bool canUseMagic()
    {
        return GameCore.playerStats.getMP() > 0;
    }

    private void printCurrentFightStats()
    {
        print("Nora's HP: " + GameCore.playerStats.getHP() + "\n" + "            MP: " + GameCore.playerStats.getMP());
        print("Koolaid's HP: " + GameCore.currentEnemy.stats.getHP() + "\n" + "               MP: " + GameCore.currentEnemy.stats.getMP());
    }

    private void checkForNoraDeathAndSwitch()
    {
        if (GameCore.playerStats.getHP() <= 0)
        {
            // Koolaid wins!
            print("Koolaid wins the fight!");
            canPressButtons = false;

            // Play game over music
            this.audioPlayer.clip = this.enemyWinsMusic;
            this.audioPlayer.Play(); // TODO: Fix looping "game over" sound effect

            // Back to the map
            Invoke("backToMap", 5f);
        }
        else
        {
            // Back to Nora's turn to attack
            this.isMyTurn = true;
        }
    }

    private void checkForKoolaidDeathAndSwitch()
    {
        if (GameCore.currentEnemy.stats.getHP() <= 0)
        {
            // Nora wins!
            print("Nora wins the fight!");
            canPressButtons = false;

            // Play win music
            this.audioPlayer.clip = this.noraWinsMusic;
            this.audioPlayer.Play();

            // Back to the map
            Invoke("backToMap", 5f);
        }
        else
        {
            // Back to Koolaid's turn to attack
            this.isMyTurn = false;
            enemyTakesTurn(enemyAttacks);
        }
    }

    private void enemyTakesTurn(string[] attacks)
    {
        // Select an attack
        string selectedAttack;
        if (GameCore.currentEnemy.stats.getMP() <= 0)
        {
            // Can only punch
            selectedAttack = "Punch";
        }
        else
        {
            // Selects a random attack name from the enemyAttacks array
            selectedAttack = enemyAttacks[Random.Range(0, enemyAttacks.Length)];
        }

        // Roll the dice to see if the enemy can attack
        int diceRoll = GameCore.rollDice(20);

        if (diceRoll >= GameCore.playerStats.getAC())
        {
            // Enemy can attack
            if (selectedAttack.Equals("Punch"))
            {
                // Punch attack
                print("ENEMY PUNCHED");
                GameCore.playerStats.reduceHP(1);
                printCurrentFightStats();
            }
            else if (selectedAttack.Equals("Machine Gun"))
            {
                // Machine gun attack
                print("ENEMY USED MACHINE GUN");
                GameCore.playerStats.reduceHP(2);
                GameCore.currentEnemy.stats.reduceMP(1);
                printCurrentFightStats();
            }
        }
        else
        {
            // Bad dice roll. Enemy can't attack
            print("Enemy can't attack - dice roll too low!");
        }

        checkForNoraDeathAndSwitch();
    }

    private void backToMap()
    {
        GameCore.mainSceneStuff.SetActive(true);
        SceneManager.UnloadSceneAsync("FightScene");
    }



}
