using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class knightbehavior : MonoBehaviour
{
    // Start is called before the first frame update
    private float timer = 2f;
    public float speed = 1f;
    public int health = 5;
    public int maxHealth = 10;
    private int direction;
    public Animator anim;
    public SpriteRenderer spriteRenderer;
    private Color startColor;
    private bool faceRight = true;
    // public TMP_Text health_ui;
    private bool dead = false;

    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public int attackDamage = 1;

    private float attackTimer = 0f;
    private bool isAttacking = false;


    void Start()
    {
        direction = Random.Range(1, 10);
        startColor =  spriteRenderer.color;
        anim = gameObject.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // health_ui.text = "Knight: " + health + "/" + maxHealth + "HP";

    }

    public int getHealth()
    {
        return health;
    }
    public int getMaxHealth()
    {
        return maxHealth;
    }

    public IEnumerator flashGreen()
    {
        float timer = 0f;
        float fadeDuration = 0.1f;
        // https://docs.unity3d.com/6000.2/Documentation/ScriptReference/Color.html
        Color peakColor = new Color(startColor.r * 0.5f, 1f, startColor.b * 0.5f, startColor.a);
        
        // https://discussions.unity.com/t/how-do-i-fade-a-object-in-out-over-time/601728
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(startColor, peakColor, timer / fadeDuration);
            yield return null;
        }

        timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(peakColor, startColor, timer / fadeDuration);
            yield return null;
        }
        spriteRenderer.color = startColor;
    }
    
    public IEnumerator waitDeath()
    {
        // https://stackoverflow.com/questions/30056471/how-to-make-the-script-wait-sleep-in-a-simple-way-in-unity
        dead = true; // stop 
        health = 0;
        // health_ui.text = "Knight: " + "0" + "/" + maxHealth + "HP";
        anim.SetTrigger("hit");
        anim.SetBool("isMoving", false);
        // https://discussions.unity.com/t/how-to-find-animation-clip-length/661298
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("death", true);
        yield return new WaitForSeconds(5f); 
        Destroy(gameObject);

    }

    public void incrementHealth(int amount)
    {
        if (!dead)
        {
            health += amount;
            if (health > maxHealth)
                health = maxHealth;
            // health_ui.text = "Knight: " + health + "/" + maxHealth + "HP";
            StartCoroutine(flashGreen());
        }

    }

    public void decrementHealth(int amount)
    {
        health -= amount;
        if (health > maxHealth)
            health = maxHealth;
        if (health < 0)
            health = 0;
        if (health == 0)
        {
            StartCoroutine(waitDeath());

        }
        else
        {
            anim.SetTrigger("hit");
        }
        // health_ui.text = "Knight: " + health + "/" + maxHealth + "HP";

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("unwalkable"))
        {
            int prev = direction;
            while (direction == prev)
            {
                direction = Random.Range(1, 10);
            }
        }
    }

    private GameObject FindClosestEnemy()
{
    // https://discussions.unity.com/t/find-nearest-object-with-tag/750830
    GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
    GameObject closest = null;

    float closestDist = Mathf.Infinity;

    foreach (GameObject e in enemies)
    {
        if (e == null) 
            continue;

        Vector3 diff = e.transform.position - transform.position;
        float d = diff.sqrMagnitude;
        if (d < closestDist)
        {
            closestDist = d;
            closest = e;
        }
    }

    return closest;
}

    
    public IEnumerator Attack(GameObject target)
    {
        isAttacking = true;
        attackTimer = attackCooldown;
        
        anim.SetTrigger("attack");
        anim.SetBool("isMoving", false);

        yield return new WaitForSeconds(0.5f);

        if (target != null)
        {
            float dist = Vector2.Distance(transform.position, target.transform.position);
            if (dist <= attackRange)
            {
                var enemy = target.GetComponent<mushroomBehavior>();
                if (enemy != null)
                {
                    enemy.Hurt(attackDamage);
                }
                var enemy1 = target.GetComponent<slimeBehavior>();
                if (enemy1 != null)
                {
                    enemy1.Hurt(attackDamage);
                }
            }
        }
        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        // health_ui.text = "Knight: " + health + "/" + maxHealth + "HP";
        if (!dead)
        {
            timer -= Time.deltaTime;
            bool changeDirection = false;
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


            attackTimer -= Time.deltaTime;
            if (!isAttacking)
            {
                GameObject target = FindClosestEnemy();

                if (target != null)
                {
                    float dist = Vector2.Distance(transform.position, target.transform.position);

                    if (dist <= attackRange && attackTimer <= 0f)
                    {
                        StartCoroutine(Attack(target));
                    }
                }
            }

        }
    }
}
