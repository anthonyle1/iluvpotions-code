using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class laserBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public bool active;
    private List<knightbehavior> knights;
    public Tilemap tilemap;
    public void OnTriggerStay2D(Collider2D other)
    {
        if (active && other.gameObject.CompareTag("friendly_npc"))
        {
            var knight = other.GetComponent<knightbehavior>();
            if (!knights.Contains(knight)) {
                knight.decrementHealth(1);
                knights.Add(knight);
                Debug.Log("beam -- decreasing knight_health by 1");
            }
        }
    }

    public IEnumerator FadeIn()
    {
        Color current = tilemap.color;
        float timer = 0f;
        float duration = 0.5f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, timer/duration);
            tilemap.color = new Color(current.r, current.g, current.b, a);
            yield return null;
        }
        tilemap.color = new Color(current.r, current.g, current.b, 1f);
        active = true;
        timer = 0;
        duration = 3.5f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null; // not using waitforseconds because it causes issues with ontriggerenter
        }
        active = false;
        duration = 0.5f;
        timer = 0;   
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, timer/duration);
            tilemap.color = new Color(current.r, current.g, current.b, a);
            yield return null;
        }
        tilemap.color = new Color(current.r, current.g, current.b, 0f);
    }

    public void Attack()
    {
        Debug.Log("running");
        knights.Clear();
        StartCoroutine(FadeIn());
    }

    void Start()
    {
        active = false;
        Color current = tilemap.color;
        tilemap.color = new Color(current.r, current.g, current.b, 0f);
        knights = new List<knightbehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
