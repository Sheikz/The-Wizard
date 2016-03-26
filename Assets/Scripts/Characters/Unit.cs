using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

    //public Sprite mysprite;
    public string spritename = "bat";//default
    public float FrameRateMS = 1 / 8.0f;//8 per second default

    //internal
    private Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private int direction;//(0)Front, (1)Back, (2)Left, (3)Right
    private int state;//(0)Idle, (1)Walk, (2)Attack, (3)Death

    //Animation Tracking
    private int Frames = 4;//Our amount of frames per row
    private int ARows = 4;//animation rows
    private int FrameStart = 0;//Starting Frame
    private bool Loop = true;//loop animaton
    private int cFrame = 0;//Current Frame (column)
    private int cFrameRow = 0;//Current Animation (row)
    private float cTime = 0;//Current Time
    private float fTime = 0;//time to switch frames

    //sample move code
    public float speed = 1.5f;
    private Vector3 target;


    // Load Sprite Images based on name
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        for (int i = 0; i < 64; i++)
        {
            sprites = Resources.LoadAll<Sprite>("Units/" + spritename);
        }
        //Debug.Log(sprites.Length);

        //move code
        target = transform.position;
    }

    // Update is called once per frame
    void Update () {
        if (sprites.Length > 0)
        {
            //looping animation
            if (Loop == true)
            {
                if (fTime > FrameRateMS)
                {
                    cFrame += 1;
                    if (cFrame == Frames) { cFrame = FrameStart; }//restart
                    fTime = 0;
                }
            }
            //one shot (stops on last frame)
            else
            {
                if (fTime > FrameRateMS && cFrame < Frames - 1)
                {
                    cFrame += 1;
                    Debug.Log(cFrame);
                    fTime = 0;
                }
            }
            fTime += Time.deltaTime;

            //update
            int index = (cFrameRow * ARows) + cFrame;
            if (index < 64)
                spriteRenderer.sprite = sprites[index];
            //else
                //Debug.Log(cFrameRow + " " + ARows +  " " + cFrame);
        }

        //SAMPLE Change Code
        if (Input.GetMouseButtonDown(0))
        {
            //increment state (idle, walk, attack, death)
            int news = state + 1;
            if (news == 4) { news = 0; }
            ChangeState(news);
 //           ChangeState(1);

            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;
            float deg = GetDegreeToTarget(target, transform.position);
            DegreeToDirection(deg);

            //Debug.Log("DIR: "  + newd);
        }
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        
    }
    public void ChangeState(int s)
    {
        state = s;
        cFrameRow = (direction * ARows) + s;
        cFrame = 0;//reset
        fTime = 0;
        Loop = true;
        if (s == 3) { Loop = false; }//death
    }
    public void ChangeDirection(int d)
    {
        direction = d;
        cFrameRow = (d * ARows) + state;
        cFrame = 0;//reset
        fTime = 0;
        Loop = true;
        if (state == 3) { Loop = false; }//death
    }
    public void DegreeToDirection(float deg)
    {

        int newd = 0;//front
        if (deg >= 0 && deg < 45) { newd = 1; }//back
        if (deg >= 45 && deg < 135) { newd = 3; }//right
        if (deg >= 135 && deg < 181) { newd = 0; }//front

        if (deg < 0 && deg > -45) { newd = 1; }//back
        if (deg < -45 && deg > -135) { newd = 2; }//left
        if (deg < -135 && deg > -181) { newd = 0; }//front

        //only change if different
        if (direction != newd)
        {
            ChangeDirection(newd);
        }

    }
    public float GetDegreeToTarget(Vector3 v1, Vector3 v2)
    {
        float d = Mathf.Atan2(v1.x - v2.x, v1.y - v2.y) * (180 / Mathf.PI);
        //if (d < 0.0) { d += 360.0f; }
        return d;
    }
}

