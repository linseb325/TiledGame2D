using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NoraFight : MonoBehaviour
{
    private LerpBackForth attackAnimator;

    public GameObject fightMenu;
    public GameObject arrow;
    public float offsetBetweenMenuOptions;

    private int selectedIndex = 0;
    private Vector2 arrowPos0;
    private Vector2 arrowPos1;
    private Vector2[] arrowPositions;

    private bool canPressButtons = true;
    private bool isMyTurn = true;

    private AudioSource audioPlayer;
    public AudioClip noraWinsMusic;
    public AudioClip enemyWinsMusic;

    public Text noraHPText;
    public Text noraMPText;
    public Text noraACText;
    public Text enemyHPText;
    public Text enemyMPText;
    public Text enemyACText;

    public Text upperDisplayText;
    public Text lowerDisplayText;

    private string[] enemyAttacks = new string[] { "Punch", "Machine Gun" };
    public float delayBetweenTurns;


	// Use this for initialization
	void Start ()
    {
        GameCore.mainSceneStuff.SetActive(false);

        this.attackAnimator = GetComponent<LerpBackForth>();

        arrowPos0 = this.arrow.transform.position;
        arrowPos1 = arrowPos0 + (offsetBetweenMenuOptions * Vector2.down);
        arrowPositions = new Vector2[2];
        arrowPositions[0] = arrowPos0;
        arrowPositions[1] = arrowPos1;

        this.audioPlayer = this.GetComponent<AudioSource>();

        Invoke("updateStatsUI", 0.5f);
	}

    private void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update ()
    {
        if (canPressButtons)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.selectedIndex++;
                this.arrow.transform.position = arrowPositions[selectedIndex % 2];
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                this.selectedIndex++;
                this.arrow.transform.position = arrowPositions[selectedIndex % 2];
            }
            else if (Input.GetKeyDown(KeyCode.Return) && isMyTurn && !attackAnimator.animationInProgress && !GameCore.currentEnemy.attackAnimator.animationInProgress)
            {
                // Nora selected an attack.

                // Check whether Nora selected taze without any magic points left
                if ((Mathf.Abs((this.arrow.transform.position.y - arrowPos0.y)) <= .1f) && !canUseMagic())
                {
                    displayToScreen("Not enough magic points! Select a different attack.", "upper");
                    displayToScreen("", "lower");
                }
                else
                {
                    // Selected a valid attack. Roll the dice.
                    int diceRoll = GameCore.rollDice(20);
                    displayToScreen("Nora rolled a " + diceRoll, "upper");

                    // Check to see if the roll was high enough to attack
                    if (diceRoll >= GameCore.currentEnemy.stats.getAC())
                    {
                        // At this point, we know Nora can attack.

                        // Disable button presses.
                        canPressButtons = false;

                        // Which attack did we select?
                        if (Mathf.Abs((this.arrow.transform.position.y - arrowPos0.y)) <= .1f)
                        {
                            // Selected taze
                            if (canUseMagic())
                            {
                                // Selected taze and can taze
                                this.attackAnimator.animateOneShot();
                                displayToScreen("Nora tazed!", "lower");

                                GameCore.currentEnemy.stats.reduceHP(2);
                                GameCore.playerStats.reduceMP(1);

                                updateStatsUI();
                                checkForKoolaidDeathAndSwitch();
                            }
                            else
                            {
                                // Selected taze but can't taze
                                displayToScreen("Not enough magic points! Select a different attack.", "upper");
                            }
                        }
                        else if (Mathf.Abs((this.arrow.transform.position.y - arrowPos1.y)) <= .1f)
                        {
                            // Selected ninja
                            this.attackAnimator.animateOneShot();

                            displayToScreen("Nora used ninja!", "lower");
                            GameCore.currentEnemy.stats.reduceHP(1);

                            updateStatsUI();
                            checkForKoolaidDeathAndSwitch();
                        }

                    }
                    else
                    {
                        // Bad dice roll
                        displayToScreen("Nora can't attack - dice roll too low!", "lower");
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

    // Did Nora die? If not, switch to player's turn.
    private void checkForNoraDeathAndSwitch()
    {
        if (GameCore.playerStats.getHP() <= 0)
        {
            // Koolaid wins!
            displayToScreen("KOOL-AID WINS THE FIGHT!", "upper");
            displayToScreen("", "lower");
            canPressButtons = false;

            // Show game over screen
            toGameOverScreen();
        }
        else
        {
            // Back to Nora's turn to attack
            this.isMyTurn = true;
            canPressButtons = true;
        }
    }

    // Did Koolaid die? If not, switch to enemy's turn.
    private void checkForKoolaidDeathAndSwitch()
    {
        if (GameCore.currentEnemy.stats.getHP() <= 0)
        {
            // Nora wins!
            displayToScreen("NORA WINS THE FIGHT!", "upper");
            displayToScreen("", "lower");
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
            StartCoroutine(enemyTakesTurn(enemyAttacks));
        }
    }

    private IEnumerator enemyTakesTurn(string[] attacks)
    {
        // Two-second delay between the end of Nora's attack and the beginning of the enemy's attack
        yield return new WaitForSecondsRealtime(delayBetweenTurns);

        // Select an attack
        string selectedAttack;
        if (GameCore.currentEnemy.stats.getMP() <= 0)
        {
            // Can only punch
            selectedAttack = "Punch";
        }
        else
        {
            // Select a random attack name from the enemyAttacks array
            selectedAttack = enemyAttacks[Random.Range(0, enemyAttacks.Length)];
        }

        // Roll the dice to see if the enemy can attack
        int diceRoll = GameCore.rollDice(20);
        displayToScreen("Enemy rolled a " + diceRoll, "upper");

        if (diceRoll >= GameCore.playerStats.getAC())
        {
            // Enemy can attack
            if (selectedAttack.Equals("Punch"))
            {
                // Punch attack
                displayToScreen("Enemy punched!", "lower");
                GameCore.currentEnemy.attackAnimator.animateOneShot();
                GameCore.playerStats.reduceHP(1);
                this.updateStatsUI();
            }
            else if (selectedAttack.Equals("Machine Gun"))
            {
                // Machine gun attack
                displayToScreen("Enemy used machine gun!", "lower");
                GameCore.currentEnemy.attackAnimator.animateOneShot();
                GameCore.playerStats.reduceHP(2);
                GameCore.currentEnemy.stats.reduceMP(1);
                this.updateStatsUI();
            }
        }
        else
        {
            // Bad dice roll. Enemy can't attack
            displayToScreen("Enemy can't attack - dice roll too low!", "lower");
        }

        checkForNoraDeathAndSwitch();
    }

    private void backToMap()
    {
        GameCore.mainSceneStuff.SetActive(true);
        SceneManager.UnloadSceneAsync("FightScene");
    }

    private void toGameOverScreen()
    {
        SceneManager.LoadSceneAsync("GameOver");
        SceneManager.UnloadSceneAsync("FightScene");
    }

    // Grabs the HP and MP stats from the singleton for both player and enemy.
    private void updateStatsUI()
    {
        this.noraHPText.text = GameCore.playerStats.getHP().ToString();
        this.noraMPText.text = GameCore.playerStats.getMP().ToString();
        this.noraACText.text = GameCore.playerStats.getAC().ToString();
        this.enemyHPText.text = GameCore.currentEnemy.stats.getHP().ToString();
        this.enemyMPText.text = GameCore.currentEnemy.stats.getMP().ToString();
        this.enemyACText.text = GameCore.currentEnemy.stats.getAC().ToString();
    }



    // Displays message on either the upper or lower part of the prompt area, depending on the 'location' parameter.
    // Didn't end up using maxDuration in the implementation.
    private void displayToScreen(string message, string location, float maxDuration = -1f)
    {
        if (location.Equals("upper"))
        {
            this.upperDisplayText.text = message;

            // Only make the text disappear after maxDuration if the user supplies that argument.
            if (!(Mathf.Abs(maxDuration + 1f) < .001f))
            {
                StartCoroutine(clearUpperDisplayText(message, maxDuration));
            }
        }
        else if (location.Equals("lower"))
        {
            this.lowerDisplayText.text = message;

            // Only make the text disappear after maxDuration if the user supplies that argument.
            if (!(Mathf.Abs(maxDuration + 1f) < .001f))
            {
                StartCoroutine(clearLowerDisplayText(message, maxDuration));
            }
        }
        else
        {
            print("Invalid argument passed for location in displayToScreen!");
        }

    }

    /*
    private void clearUpperDisplayText()
    {
        this.upperDisplayText.text = "";
    }

    private void clearLowerDisplayText()
    {
        this.lowerDisplayText.text = "";
    }
    */

    // Clear the upper display after a delay, but only if it's still showing message.
    // Ensures we don't wipe out a later message.
    private IEnumerator clearUpperDisplayText(string message, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (upperDisplayText.text.Equals(message))
        {
            this.upperDisplayText.text = "";
        }
    }

    // Clear the lower display after a delay, but only if it's still showing message.
    // Ensures we don't wipe out a later message.
    private IEnumerator clearLowerDisplayText(string message, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (lowerDisplayText.text.Equals(message))
        {
            this.lowerDisplayText.text = "";
        }
    }










}
