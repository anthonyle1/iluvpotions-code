using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public bool faceRight = false;
    public float health = 10f;
    public float speed = 10f;
    public Animator anim;

    public Potion potion;

    private Vector3 originalPos;

    public GameObject potionPrefab;
    private float timer = 0f;
    public float reload = 0.5f;
    private bool isThrown = false;

    private bool walkable = true;

    // Start is called before the first frame update
    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.gameObject.CompareTag("walkable"))
    //     {
    //         walkable = true;
    //     }
       
    // }

    // void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.gameObject.CompareTag("walkable"))
    //     {
    //         transform.Translate(-Input.GetAxis("Horizontal") * speed * Time.deltaTime, -Input.GetAxis("Vertical") * speed * Time.deltaTime, 0);
    //         walkable = false;
    //     }
    // }


    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //https://docs.unity3d.com/6000.2/Documentation/ScriptReference/Input.GetAxis.html
        //https://discussions.unity.com/t/wasd-arrow-key-movement/189003/2
        bool changeDirection = false;
        bool isMoving = false;
        if (Input.GetMouseButton(0) && !isThrown)
        {
            anim.SetBool("windUp", true);
            StartCoroutine(potion.throwPotion());
            potion = null;
            isThrown = true;

        }


        if (isThrown)
        {
            timer += Time.deltaTime;
            if (timer > reload)
            {
                if (potion == null)
                {
                    //https://docs.unity3d.com/6000.2/Documentation/ScriptReference/Object.Instantiate.html
                    potion = Instantiate(potionPrefab).GetComponent<Potion>();
                    potion.transform.SetParent(transform, false);
                    isThrown = false;
                    anim.SetBool("windUp", false);

                    anim.SetBool("isThrown", isThrown);
                    timer = 0;

                }
            }

        }

        if (walkable && !isMoving)
        {
            if (Input.GetKey("w") || Input.GetKey("up"))
            {
                transform.Translate(0, Input.GetAxis("Vertical") * speed * Time.deltaTime, 0);
                isMoving = true;
            }
            if (Input.GetKey("a") || Input.GetKey("left"))
            {
                transform.Translate(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0, 0);
                if (!faceRight)
                    changeDirection = true;
                isMoving = true;
            }
            if (Input.GetKey("s") || Input.GetKey("down"))
            {
                transform.Translate(0, Input.GetAxis("Vertical") * speed * Time.deltaTime, 0);
                isMoving = true;
            }
            if (Input.GetKey("d") || Input.GetKey("right"))
            {
                transform.Translate(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0, 0);
                if (faceRight)
                    changeDirection = true;
                isMoving = true;
            }

            anim.SetBool("isMoving", isMoving);
        }


        Vector3 characterScale = transform.localScale;
        if (changeDirection)
        {
            faceRight = !faceRight;
            characterScale.x = -characterScale.x;
        }
        transform.localScale = characterScale;
    }
}
