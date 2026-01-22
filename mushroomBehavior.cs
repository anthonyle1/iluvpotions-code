using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mushroomBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject target;
    private float timer = 2f;
    public float speed = 1f;
    public int health = 5;
    public int maxHealth = 10;
    public bool faceRight = false;
    private int direction;
    public float attackRange = 3f;
    public float cooldown = 0.5f;
    public Animator anim;
    public GameObject coin;

    private bool dead = false;
    private bool isAttacking = false;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("friendly_npc") && target == null)
        {
            target = other.gameObject;
        }
        else if (other.CompareTag("friendly_npc") && target != null)
        {
            return;
        }
    }

    public IEnumerator Attack(Vector3 direction)
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
        target.GetComponent<knightbehavior>().decrementHealth(1);
        float ctimer = 0;
        while (ctimer < cooldown)
        {
            ctimer += Time.deltaTime;
            yield return null;
        }
        ctimer = 0;
        while (ctimer < cooldown)
        {
            ctimer += Time.deltaTime;
            transform.Translate(-direction.x * Time.deltaTime * speed, -direction.y * Time.deltaTime * speed, 0);
            yield return null;
        }


        isAttacking = false;
                // target = null;

    }

    public IEnumerator death()
    {
        Instantiate(coin, transform.position, Quaternion.identity);
        Destroy(gameObject);        
        yield return null;
    }
    public void Hurt(int damage)
    {
        health -= damage;

        if (health <= 0)
            dead = true;

        if (dead)
            StartCoroutine(death());
        
        

    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        bool changeDirection = false;
        if (target == null)
        {
            if (timer > 0)
            {
                Vector3 orignalPos = transform.position;
                if (direction == 1)
                {
                    transform.Translate(0, speed * Time.deltaTime, 0);
                }
                if (direction == 2)
                {
                    transform.Translate(0, -speed * Time.deltaTime, 0);
                }
                if (direction == 3)
                {
                    transform.Translate(speed * Time.deltaTime, speed * Time.deltaTime, 0);
                    if (!faceRight)
                        changeDirection = true;
                }
                if (direction == 4)
                {
                    transform.Translate(-speed * Time.deltaTime, speed * Time.deltaTime, 0);
                    if (faceRight)
                        changeDirection = true;
                }
                if (direction == 5)
                {
                    transform.Translate(speed * Time.deltaTime, -speed * Time.deltaTime, 0);
                    if (!faceRight)
                        changeDirection = true;
                }
                if (direction == 6)
                {
                    transform.Translate(-speed * Time.deltaTime, -speed * Time.deltaTime, 0);
                    if (faceRight)
                        changeDirection = true;
                }
                if (direction == 7)
                {
                    transform.Translate(speed * Time.deltaTime, 0, 0);
                    if (!faceRight)
                        changeDirection = true;
                }
                if (direction == 8)
                {
                    transform.Translate(-speed * Time.deltaTime, 0, 0);
                    if (faceRight)
                        changeDirection = true;
                }
                if (direction == 9)
                {
                    transform.Translate(0, 0, 0);
                }
                float distance = Vector3.Distance(orignalPos, transform.position);
                if (distance < 0.001f)
                {
                    anim.SetBool("isMoving", false);
                }
                else
                {
                    anim.SetBool("isMoving", true);
                }
            }
            else
            {
                direction = Random.Range(1, 10);
                timer = Random.Range(2, 5);
            }
            Vector3 characterScale = transform.localScale;
            if (changeDirection)
            {
                faceRight = !faceRight;
                characterScale.x = -characterScale.x;
            }
            transform.localScale = characterScale;

        }
        else
        {
            if (!isAttacking)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                transform.Translate(direction.x * Time.deltaTime * speed, direction.y * Time.deltaTime * speed, 0);
                if (direction.x < 0 && faceRight)
                    changeDirection = true;
                else if (direction.x > 0 && !faceRight)
                    changeDirection = true;
                else
                    changeDirection = false;

                Vector3 characterScale = transform.localScale;
                if (changeDirection)
                {
                    faceRight = !faceRight;
                    characterScale.x = -characterScale.x;
                }
                transform.localScale = characterScale;


                if (Vector2.Distance(transform.position, target.transform.position) < attackRange && !isAttacking)
                {
                    StartCoroutine(Attack(direction));

                }
            }

        }

        
    }
}
