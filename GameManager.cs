using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] enemies;
    public GameObject[] party;
    public TMP_Text remaining_enemies_display;
    public TMP_Text end_title;
    public TMP_Text money_text;
    public TMP_Text health_ui;

    public AudioClip soundClip;
    public AudioClip coinPickup;

    public Button next;
    public Button restart;
    public Button addKnight;


    public bool isPlaying;
    public Camera level1;
    public int money = 0;
    private GameObject group2;
    private GameObject group3;

    private GameObject horizontal;
    private laserBehavior[] laser;
    public GameObject knightPrefab;


    public int stage = 0;
    public float timer = 0;
    public float cooldown = 5f;

    void Start()
    {
        // https://docs.unity3d.com/ScriptReference/GameObject.FindGameObjectsWithTag.html
        enemies = GameObject.FindGameObjectsWithTag("enemy");
        party = GameObject.FindGameObjectsWithTag("friendly_npc");
        end_title.gameObject.SetActive(false);
        next.gameObject.SetActive(false);
        restart.gameObject.SetActive(false);
        addKnight.gameObject.SetActive(false);

        level1.enabled = true;
        money_text.text = money + " moolahs";
        group2 = GameObject.Find("group_2");
        group2.gameObject.SetActive(false);
        group3 = GameObject.Find("group_3");
        group3.gameObject.SetActive(false);

        horizontal = GameObject.Find("horizontal");
        laser = horizontal.GetComponentsInChildren<laserBehavior>();
        horizontal.gameObject.SetActive(false);
    }

    public void addCoin(int value)
    {
        AudioSource.PlayClipAtPoint(coinPickup, level1.gameObject.transform.position, 2.0f);
        money += value;
        money_text.text = money + " moolahs";
    }
    
    public void OnClick()
    {
        AudioSource.PlayClipAtPoint(soundClip, level1.gameObject.transform.position);
        stage = stage + 1;
        if (stage == 1)
        {
            Camera.main.transform.position = new Vector3(0f, -15.37f, -10f);
            end_title.gameObject.SetActive(false);
            next.gameObject.SetActive(false);
            restart.gameObject.SetActive(false);
            
            var player = GameObject.FindWithTag("Player");
            player.GetComponent<PlayerBehavior>().enabled = false;
            player.transform.position = new Vector3(-0.37f, -17.65f, 0f);
            player.GetComponent<PlayerBehavior>().enabled = true;
            addKnight.gameObject.SetActive(false);
            group2.gameObject.SetActive(true);

            foreach (GameObject p in party) {
                p.transform.position = new Vector3(0f, -13f, 0f);
            }
            isPlaying = true;
        }
        if (stage > 2)
            stage = 2;
        if (stage == 2)
        {
            Camera.main.transform.position = new Vector3(-0.75f, -32.25f, -10f);
            end_title.gameObject.SetActive(false);
            next.gameObject.SetActive(false);
            horizontal.SetActive(true);
            isPlaying = true;
            group3.gameObject.SetActive(true);
            restart.gameObject.SetActive(false);
            addKnight.gameObject.SetActive(false);


            var player = GameObject.FindWithTag("Player");
            player.GetComponent<PlayerBehavior>().enabled = false;
            player.transform.position = new Vector3(-0.37f, -35f, 0f);
            player.GetComponent<PlayerBehavior>().enabled = true;

            foreach (GameObject p in party) {
                p.transform.position = new Vector3(0f, -32f, 0f);
            }
        }
        
            
    }

    public void RestartGame()
    {
        //  https://discussions.unity.com/t/how-to-restart-scene-properly/118396
        AudioSource.PlayClipAtPoint(soundClip, level1.gameObject.transform.position);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        stage = 0;

    }

    public void Revive()
    {
        AudioSource.PlayClipAtPoint(soundClip, level1.gameObject.transform.position);
        Debug.Log("new knight");
        if (money < 5)
            return;
        money -= 5;
        money_text.text = money + " moolahs";
        var player = GameObject.FindWithTag("Player");
        Instantiate(knightPrefab, player.gameObject.transform.position, Quaternion.identity);
        party = GameObject.FindGameObjectsWithTag("friendly_npc");
    }

    // Update is called once per frame
    void Update()
    {
        health_ui.text = "";
        party = GameObject.FindGameObjectsWithTag("friendly_npc");
        foreach (GameObject p in party) {
            if (p == null)
                continue;

            var kn = p.GetComponent<knightbehavior>();
            health_ui.text += "Knight: " + kn.getHealth() + "/" + kn.getMaxHealth() + "\n";
        }
        if (isPlaying) {

            if (stage == 2)
            {
                timer += Time.deltaTime;
                if (timer > cooldown)
                    timer = 0;
                if (timer == 0)
                {
                    int num_laser = Random.Range(1, 5);
                    HashSet<int> picked_lasers = new HashSet<int>();
                    while (picked_lasers.Count < num_laser)
                    {
                        int picked = Random.Range(0, laser.Length);
                        picked_lasers.Add(picked);
                    }

                    foreach (int p in picked_lasers)
                    {
                        laser[p].Attack();
                    }
                    

                }
            }

            enemies = GameObject.FindGameObjectsWithTag("enemy");
            remaining_enemies_display.text = "remaining enemies: " + enemies.Length;

            party = GameObject.FindGameObjectsWithTag("friendly_npc");
            if (party.Length == 0)
            {
                // game ends because all knights are dead
                end_title.gameObject.SetActive(true);
                restart.gameObject.SetActive(true);

                end_title.text = "Your party all died :(";
                isPlaying = false;
            }
            else if (enemies.Length == 0 && stage >= 2)
            {
                // game ends because all enemies are dead
                // move to next 
                end_title.gameObject.SetActive(true);
                restart.gameObject.SetActive(true);

                end_title.text = "You Beat the Game (yay)!";
                isPlaying = false;
            }
            else if (enemies.Length == 0 )
            {
                // game ends because all enemies are dead
                // move to next 
                end_title.gameObject.SetActive(true);
                next.gameObject.SetActive(true);
                restart.gameObject.SetActive(true);
                addKnight.gameObject.SetActive(true);

                end_title.text = "You Win!";
                isPlaying = false;
            }
        }
    }
}
