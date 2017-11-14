using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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



	// Use this for initialization
	void Start () {
        this.rb = this.GetComponent<Rigidbody2D>();
        this.anim = this.GetComponent<Animator>();

        this.upMove = new Vector2(0, 1) * speed;
        this.downMove = new Vector2(0, -1) * speed;
        this.leftMove = new Vector2(-1, 0) * speed;
        this.rightMove = new Vector2(1, 0) * speed;
        this.stopMove = Vector2.zero;
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
        }
        else if (Input.GetKey(this.downKey))
        {
            this.anim.SetTrigger("down");
            this.rb.velocity = downMove;
        }
        else if (Input.GetKey(this.leftKey))
        {
            this.anim.SetTrigger("left");
            this.rb.velocity = leftMove;
        }
        else if (Input.GetKey(this.rightKey))
        {
            this.anim.SetTrigger("right");
            this.rb.velocity = rightMove;
        }
        else
        {
            this.rb.velocity = stopMove;
            // TODO: Set trigger for the idle animation when we add it.
        }


        /*
        print("moveHorizontal = " + moveHorizontal);
        print("moveVertical = " + moveVertical);

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        this.rb.AddForce(movement * speed);

        string currDirX = CurrentDirectionX(moveHorizontal);
        string currDirY = CurrentDirectionY(moveVertical);

        if (currDirX.Equals("none"))
        {
            if (!currDirY.Equals("none"))
            {
                // Moving vertically only.
                print("Animating " + currDirY);
                this.anim.SetTrigger(currDirY);
            }
            // Not moving.
        }
        else if (currDirY.Equals("none"))
        {
            if (!currDirX.Equals("none"))
            {
                // Moving horizontally only.
                print("Animating " + currDirX);
                this.anim.SetTrigger(currDirX);
            }
            // Not moving.
        }
        else
        {
            // Moving in more than one direction. What do we do?
        }
        */




    }

    private string CurrentDirectionX(float movementX)
    {
        if (movementX < 0)
        {
            return "left";
        }
        else if (movementX > 0)
        {
            return "right";
        }
        else
        {
            return "none";
        }
    }

    private string CurrentDirectionY(float movementY)
    {
        if (movementY < 0)
        {
            return "down";
        }
        else if (movementY > 0)
        {
            return "up";
        }
        else
        {
            return "none";
        }
    }

}
