using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoraMovement : MonoBehaviour {

    public float speed;
    private Rigidbody2D rb;
    private Animator anim;

    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;

    private Vector2 upMove;
    private Vector2 downMove;
    private Vector2 leftMove;
    private Vector2 rightMove;
    private Vector2 stopMove;

    private int stepsLeft;
    public int minimumInterval;
    public int maximumInterval;

    private bool canDecrementSteps = true;

    public GameObject sceneStuff;


	// Use this for initialization
	void Start () {

        GameCore.mainSceneStuff = this.sceneStuff;
        GameCore.playerStats = new NPCStats(15, 3, 12);

        this.rb = this.GetComponent<Rigidbody2D>();
        this.anim = this.GetComponent<Animator>();

        this.resetStepTimer();

        this.upMove = new Vector2(0, 1) * speed;
        this.downMove = new Vector2(0, -1) * speed;
        this.leftMove = new Vector2(-1, 0) * speed;
        this.rightMove = new Vector2(1, 0) * speed;
        this.stopMove = Vector2.zero;

        SceneManager.sceneUnloaded += delegate {
            resetStepTimer();
            this.canDecrementSteps = true;
        };

	}
	
	// Update is called once per frame
	void Update () {
        
	}

    private void FixedUpdate()
    {
        // float moveHorizontal = Input.GetAxis("Horizontal");
        // float moveVertical = Input.GetAxis("Vertical");

        if (Input.GetKey(this.upKey))
        {
            this.anim.SetTrigger("up");
            this.rb.velocity = upMove;
            subtractStep();
        }
        else if (Input.GetKey(this.downKey))
        {
            this.anim.SetTrigger("down");
            this.rb.velocity = downMove;
            subtractStep();
        }
        else if (Input.GetKey(this.leftKey))
        {
            this.anim.SetTrigger("left");
            this.rb.velocity = leftMove;
            subtractStep();
        }
        else if (Input.GetKey(this.rightKey))
        {
            this.anim.SetTrigger("right");
            this.rb.velocity = rightMove;
            subtractStep();
        }
        else
        {
            this.rb.velocity = stopMove;
            // TODO: Set trigger for the idle animation when we add it.
        }

    }

    // Unnecessary?
    void setFightSceneActive()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("FightScene"));
    }


    private void subtractStep()
    {
        if (canDecrementSteps)
        {
            this.stepsLeft--;
            print(stepsLeft);
            if (stepsLeft <= 0)
            {
                this.canDecrementSteps = false;
                print("Start fight!");
                SceneManager.LoadScene("FightScene", LoadSceneMode.Additive);
            }
        }
    }


    private void resetStepTimer()
    {
        this.stepsLeft = Random.Range(minimumInterval, maximumInterval);
    }

}
