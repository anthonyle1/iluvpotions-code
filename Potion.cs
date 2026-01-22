using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Potion : MonoBehaviour
{
    // public LineRenderer line;

    public float speed = 3f;
    private IEnumerator coroutine;
    private Vector3 target;
    private SpriteRenderer spriteRenderer;
    public Texture2D potion_texture;
    public Texture2D area_texture;
    public float ChargeLevel = 0f;
    public float ChargeSpeed = 1f;
    public float area_radius = 2;
    public AudioClip soundClip;

    public float duration = 2;
    private bool thrown = false;
    private bool effectGiven = false;
    public LineRenderer line;
    public Animator anim;
    private List<knightbehavior> knights;

    void OnTriggerStay2D(Collider2D other)
    {
        if (thrown && other.gameObject.CompareTag("friendly_npc"))
        {
            // https://www.geeksforgeeks.org/c-sharp/list-class-in-c-sharp/
            var knight = other.GetComponent<knightbehavior>();
            if (!knights.Contains(knight)) {
                knight.incrementHealth(1);
                knights.Add(knight);
                Debug.Log("increasing knight_health by 1");
            }
            effectGiven = true;
        }
    }
    public IEnumerator throwPotion()
    {
        
        while (Input.GetMouseButton(0))
        {
            line.SetPosition(0, transform.position);
            // https://discussions.unity.com/t/how-do-i-get-get-mouse-position-in-my-2d-scene-not-the-ui/853651/6
            target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            ChargeLevel += Time.deltaTime * ChargeSpeed;

            Vector3 direction = (target - transform.position);
            line.SetPosition(1, transform.position + ChargeLevel * direction);

            if (ChargeLevel >= 1)
            {
                Debug.Log("charge max");
                ChargeLevel = 1;
            }
            yield return null;
        }
        line.SetPosition(1, transform.position);
        Vector3 direction1 = target - transform.position;
        target = transform.position + direction1 * ChargeLevel;
        transform.SetParent(null);
        Destroy(line);
        anim.SetBool("isThrown", true);
        while (true)
        {
            Vector3 moveDir = (target - transform.position).normalized;
            transform.position += moveDir * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, target) < 1f)
            {
                // https://discussions.unity.com/t/how-to-change-sprite-texture-of-a-sprite/526159/3
                Sprite newSprite = Sprite.Create(area_texture, new Rect(0, 0, area_texture.width, area_texture.height), new Vector2(0.5f, 0.5f));
                spriteRenderer.sprite = newSprite;

                // https://discussions.unity.com/t/how-do-you-change-a-sprites-sorting-layer-in-c/91976
                spriteRenderer.sortingOrder = 0;
                transform.localScale = new Vector3(area_radius, area_radius, area_radius);
                break;
            }
            yield return null;
        }
        anim.SetBool("isThrown", false);
        float timer = 0f;
        float fadeDuration = 0.5f;
        thrown = true;

        // splash sound effect?
        AudioSource.PlayClipAtPoint(soundClip, transform.position);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;

        //https://discussions.unity.com/t/how-do-i-fade-a-object-in-out-over-time/601728
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float lurp_alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            Color current = spriteRenderer.color;
            current.a = lurp_alpha;
            spriteRenderer.color = current;

            yield return null;
        }
        Destroy(gameObject);
    }

    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponentInParent<Animator>();
        knights = new List<knightbehavior>();
    }

    void Update()
    {
        // if (Input.GetMouseButton(0) && !thrown)
        // {
        //     thrown = true;
        //     coroutine = throwPotion();
        //     StartCoroutine(coroutine);
        // }
    }
}
