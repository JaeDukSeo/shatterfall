﻿using UnityEngine;
using System.Collections.Generic;
using System;

public class player : MonoBehaviour
{

    public GameObject floor;
    public GameObject orb;
    orb _orb;
    public bool Active = true;
    public bool Controlled = false;
    int flying = -1;

    //player direction
    float mouseX1;
	float mouseY1;
	float mouseX2;
	float mouseY2;
	float mouseOffsetX;
	float mouseOffsetY;
	float direction;
    bool lockMouseMovement = true;

    private Vector3 knockback = Vector3.zero;
    private int knockback_counter = 0;
    new Rigidbody rigidbody;
    new Renderer renderer;
    Material material;

    static int MAXIMUM_FLOAT_FRAMES = 2; //To be safe
    float MOVE_SPEED = 5f;
    int TURN_SPEED = 1000;
    //turnForceScale;
    Collider thisCollider, floorCollider;
    List<FloorPiece> collisions = new List<FloorPiece>();
	Animation armsUp;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == 8)
        {
            var p = col.gameObject.GetComponent<player>();
            if (p != null)
            {
                p.Push(col.impulse);
                return;
            }
        }
    }

    private class FloorPiece
    {
        public FloorPiece(Collider c)
        {
            collider = c;
            Frames = MAXIMUM_FLOAT_FRAMES;
        }
        public Collider collider;
        public int Frames;
    }

    void OnTriggerStay(Collider other)
    {
        var found = collisions.Find(c => c.collider.GetInstanceID() == other.GetInstanceID());
        if (found == null)
        {
            found = new FloorPiece(other);
            collisions.Add(found);
        }
        found.Frames = MAXIMUM_FLOAT_FRAMES;
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    var fp = (FloorPiece)other;
    //    fp.Frames = 3;
    //    //if (collisions.Find(o => o.GetInstanceID() == fp.GetInstanceID()) == null)
    //        collisions.Add(fp);
    //}

    //void OnTriggerExit(Collider other)
    //{
    //    //Debug.Log("LEFT:" + other.name);
    //    //if (!collisions.Remove(other)) ;
    //    collisions.Remove((FloorPiece)other);
    //        //Debug.LogError("HOLY FLYING FUCK KILL YOURSELF!");
    //}

    public void Die()
    {
        _orb.Die();
        Destroy(gameObject);
    }

    public void Push(Vector3 force)
    {
        knockback_counter = 1;
        knockback = force.normalized * 8;
    }

    // Use this for initialization
    void Start()
    {
        //turnForceScale = 1;
		armsUp = GetComponent<Animation> ();

        rigidbody = GetComponent<Rigidbody>();

        thisCollider = GetComponent<BoxCollider>();
        floorCollider = main.GetFloor().GetComponent<BoxCollider>();

        var clone = Instantiate(orb);
        clone.SetActive(false);
        _orb = clone.GetComponent<orb>();
        _orb.SetHighlightMaterial(material);

		mouseX1 = Input.mousePosition.x;
		mouseY1 = Input.mousePosition.y;
		Cursor.visible = false;

    }

    public class KeyMap
    {
        public KeyMap(string name, string x, KeyCode p, KeyCode? n = null)
        {
            Name = name;
            XBOXkey = x;
            PCkey = p;
            PreviousFrame = 10;
            nPCkey = n;
        }
        public string Name;
        public string XBOXkey;
        public KeyCode PCkey;
        public KeyCode? nPCkey;
        public float PreviousFrame;
    }

    public void Win()
    {
        this.Active = false;
        flying = 0;
    }

    public void InitPlayer(Camera cam, int n, Material m, bool ctrl)
    {
        //n = 5 - n;
        gameObject.name = "Player" + n;
        PC = new PlayerControls(5-n);
        //PC = new PlayerControls(5 - n);
        material = m;
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
            r.material = m;
        this.Controlled = ctrl;
    }

    private PlayerControls PC;

    private class PlayerControls
    {
        public KeyMap Activate;// = new KeyMap("Activate", KeyCode.Mouse1);
        public KeyMap Explode;// = new KeyMap("Explode", KeyCode.Mouse0);
        public KeyMap Jump;// = new KeyMap("Jump", KeyCode.Space);
        public KeyMap Horizontal;// = new KeyMap("Horizontal", KeyCode.Mouse0);
        public KeyMap Vertical;// = new KeyMap("Vertical", KeyCode.Mouse0);
        public KeyMap MoveHorizontal;// = new KeyMap("MoveHorizontal", KeyCode.D, KeyCode.A);
        public KeyMap MoveVertical;// = new KeyMap("MoveVertical", KeyCode.W, KeyCode.S);
        public KeyMap LockMouseMovement;// = new KeyMap("LockMouseMovement", KeyCode.Backslash);

        public PlayerControls(int n)
        {
            Activate = new KeyMap("Activate", "Activate" + n, KeyCode.Mouse1);
            Explode = new KeyMap("Explode", "Explode" + n, KeyCode.Mouse0);
            Jump = new KeyMap("Jump", "Jump" + n, KeyCode.Space);
            Horizontal = new KeyMap("Horizontal", "Horizontal" + n, KeyCode.Mouse0);
            Vertical = new KeyMap("Vertical", "Vertical" + n, KeyCode.Mouse0);
            MoveHorizontal = new KeyMap("MoveHorizontal", "MoveHorizontal" + n, KeyCode.D, KeyCode.A);
            MoveVertical = new KeyMap("MoveVertical", "MoveVertical" + n, KeyCode.W, KeyCode.S);
            LockMouseMovement = new KeyMap("LockMouseMovement", "LockMouseMovement" + n, KeyCode.Backslash);
    }
    };

    private bool GetControlDown(KeyMap control, bool upCheck = false)
    {
        if (this.Controlled)
        {
            float result;
            if ((result = Input.GetAxis(control.XBOXkey)) != control.PreviousFrame)
            {
                if (!upCheck)
                    control.PreviousFrame = result;

                //Debug.Log(result);

                if (result != 0)
                {
                    control.PreviousFrame = result;
                    return true;
                }
            }
            if (Input.GetKeyDown(control.PCkey))
                return true;
            return false;
        }
        else
        {
            return false;
        }
    }

    private bool GetControlUp(KeyMap control, bool downCheck = false)
    {
        float result;
        if ((result = Input.GetAxis(control.XBOXkey)) != control.PreviousFrame)
        {
            if (!downCheck)
                control.PreviousFrame = result;

            if (result == 0)
            {
                control.PreviousFrame = result;
                return true;
            }
                
        }
        if (Input.GetKeyUp(control.PCkey))
            return true;
        return false;
    }

    private float? GetRotationAngle()
    {
        const float TURN_THRESHOLD = 0.6f;
        var x = Input.GetAxis(PC.Horizontal.XBOXkey);
        var y = Input.GetAxis(PC.Vertical.XBOXkey);
        var theta = Mathf.Atan2(y, x) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        if (Mathf.Abs(x) < TURN_THRESHOLD && Mathf.Abs(y) < TURN_THRESHOLD)
            return null;
        return theta;
    }

    public static float GetControlValue(KeyMap control)
    {
        if (Input.GetKey(control.PCkey))
            return 1;
        else if (Input.GetKey((KeyCode)control.nPCkey))
            return -1;
        else
            return Input.GetAxis(control.XBOXkey);
    }

    private bool GetNetworkControlDown(string control)
    {
        return false;
    }

    // Update is called once per frame
    void Update()
    {

        if (this.Active)
        {

            if (GetControlDown(PC.LockMouseMovement))
            {
                lockMouseMovement = !lockMouseMovement;
                Debug.Log("Swapping mouse and gamepad controlls for DIRECTION");
            }

            if (GetControlDown(PC.Explode))// || GetControlDown(PC.Activate))
            {
                if (_orb.gameObject.activeSelf)
                    _orb.Explode();
                else
                {
                    _orb.Activate(transform.position + new Vector3(0, 2, 0), transform.rotation);
                    armsUp["Arms_Up2"].speed = 4.5f;
                    armsUp.Play("Arms_Up2");
                }
                    
            }

            if (GetControlDown(PC.Jump) && transform.position.y > 0.40 && transform.position.y < 0.42)
            {
                rigidbody.velocity = Vector3.up * 0;
                rigidbody.velocity += Vector3.up * 7;
            }



            //Movement:

            var forward = Camera.main.transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward = forward.normalized;
            var right = new Vector3(forward.z, 0, -forward.x);


            var x = GetControlValue(PC.MoveHorizontal);
            var y = rigidbody.velocity.y / MOVE_SPEED;
            var z = GetControlValue(PC.MoveVertical);

            rigidbody.velocity = (x * right + z * forward + y * Vector3.up) * MOVE_SPEED;

            //direction:

            int curDirection, tarDirection;

            if (lockMouseMovement)
            {
                curDirection = (int)gameObject.transform.eulerAngles.y;
                tarDirection = (int)curDirection;

                var conAngle = GetRotationAngle();
                if (conAngle != null)
                    tarDirection = (int)conAngle;
            }
            else
            {
                mouseX2 = Input.mousePosition.x;
                mouseY2 = Input.mousePosition.y;
                mouseOffsetX = mouseX2 - mouseX1;
                mouseOffsetY = mouseY2 - mouseY1;

                if (Mathf.Abs(mouseOffsetX) > 2 || Mathf.Abs(mouseOffsetY) > 2)
                {
                    direction = -Mathf.Atan2(mouseOffsetY, mouseOffsetX) * (180 / Mathf.PI);
                }
                if (direction < 0)
                {
                    direction = 360 - Mathf.Abs(direction);
                }

                curDirection = (int)gameObject.transform.eulerAngles.y;
                tarDirection = (int)direction;

            }

            if (tarDirection < 0)
                tarDirection += 360;

            if (curDirection != tarDirection)
            {
                var delta = 0;
                if (tarDirection - curDirection < -180)
                    delta = 1;
                else if (tarDirection - curDirection < 0)
                    delta = -1;
                else if (tarDirection - curDirection < 180)
                    delta = 1;
                else
                    delta = -1;

                var angle = Time.deltaTime * TURN_SPEED * delta;

                //Debug.Log(tarDirection);
                var a = Mathf.Abs(tarDirection - curDirection);

                if (a < 45 || a > 315)
                    gameObject.transform.eulerAngles = new Vector3(0, tarDirection, 0);
                else
                    gameObject.transform.eulerAngles = new Vector3(0, curDirection + angle, 0);
            }
            else
                rigidbody.angularVelocity = Vector3.zero;
                
        }

        //Too low:
        if (transform.position.y < 0)
        {
            _orb.Explode(true);
            this.Active = false;
            main.PlayerDied(gameObject);
        }

        if (transform.position.y < -25)
            Die();

        if (transform.position.y > 30)
        {
            Die();
            Application.LoadLevel("WinnerMenu");

        }

        //Knockback:
        if (knockback_counter-- > 0)
            rigidbody.velocity += knockback;

        //Flying:
        if (flying >= 0)
        {
            transform.eulerAngles += new Vector3(0, 360 * Time.deltaTime, 0);
            rigidbody.velocity += Vector3.up * 25 * Time.deltaTime;
        }

        //Debug:
        //var s = "";
        //collisions.ForEach(c => s += c.name);

        for (var i=0;i<collisions.Count; i++)
            if (--collisions[i].Frames < 0)
                collisions.Remove(collisions[i--]);

        if (collisions.Count < 1 && transform.position.y < 0.41)
        {
            Physics.IgnoreCollision(thisCollider, floorCollider, true);
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
            //gameObject.layer = 13;
        }
    }

    
}
