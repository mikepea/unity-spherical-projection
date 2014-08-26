﻿
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlayerCollision1 : MonoBehaviour
{

    public int numPills;
    public int playerMaxLives = 3;
    public string gameOverScene;
    public bool superPlayer;

    private int score;
    private int playerLivesRemaining;

    public AudioClip munch;
    private float lastPillMunchTime;
    private float pillMunchDelay;

    private GameObject scoreboard;

    void Start ()
    {
        pillMunchDelay = munch.length;
        lastPillMunchTime = - pillMunchDelay;
        numPills = GameObject.FindGameObjectsWithTag ("Pill").Length +
                   GameObject.FindGameObjectsWithTag ("Power Pill").Length;
        playerLivesRemaining = playerMaxLives;
        score = 0;

        GameObject[] scoreboards = GameObject.FindGameObjectsWithTag("Scoreboard");
        scoreboard = scoreboards[0];
    }

    int Score ()
    {
        return score;
    }

    GameObject GlobalState() {
        GameObject[] states = GameObject.FindGameObjectsWithTag ("PersistedState");
        return states[0];
    }

    bool AudioEnabled ()
    {
        return GlobalState().GetComponent<GlobalGameDetails>().AudioEnabled();
    }

    void MapIsCleared ()
    {
        Debug.Log ("MAP COMPLETE!");
        GlobalState().SendMessage("NextMap");
    }

    void DisableAllBaddies() {
        GameObject[] baddies = GameObject.FindGameObjectsWithTag ("Baddy");
        foreach (GameObject baddy in baddies) {
            baddy.renderer.enabled = false; // dont SetActive(false), as cannot then find it.
        }
    }

    void PlayerHasDied ()
    {
        DisableAllBaddies();
        if (playerLivesRemaining == 0) {
            //Application.LoadLevel (gameOverScene);
            //playerLivesRemaining = playerMaxLives;
            this.SendMessage ("HasDied");
            this.SendMessage ("GameOver");
        } else {
            playerLivesRemaining--;
            this.SendMessage ("HasDied");
            //ResetPlayerPositions ();
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.tag == "Baddy") {
            if ( other.gameObject.GetComponent<PlayerSphericalMovement>().IsScared() == true ) {
                score += 200;
                other.gameObject.SendMessage ("EnterDeadMode");
            } else if ( other.gameObject.GetComponent<PlayerSphericalMovement>().IsDead() == true ) {
              // ignore the dead
            } else {
                if ( ! superPlayer ) {
                  PlayerHasDied ();
                }
            }
        } else if (other.gameObject.tag == "Pill" || other.gameObject.tag == "Power Pill" ) {
            score += 10;
            lastPillMunchTime = Time.time;
            other.renderer.enabled = false;
            Destroy(other.gameObject, 0.5f);
            if (other.gameObject.tag == "Power Pill") {
                other.gameObject.SetActive (false);
                GameObject[] baddies = GameObject.FindGameObjectsWithTag ("Baddy");
                foreach (GameObject baddy in baddies) {
                    baddy.SendMessage ("EnterScaredMode");
                }
            }
        }
    }

    void FixedUpdate ()
    {
        DisplayScore();
        numPills = GameObject.FindGameObjectsWithTag ("Pill").Length + 
                   GameObject.FindGameObjectsWithTag ("Power Pill").Length;
        if (numPills == 0) {
          MapIsCleared ();
        }
        if ( AudioEnabled() ) {
          if ( lastPillMunchTime + pillMunchDelay > Time.time ) {
            if ( ! audio.isPlaying ) {
              audio.clip = munch;
              audio.loop = true;
              audio.Play();
            }
          } else {
            audio.loop = false;
          }
        }
    }

    void DisplayScore() 
    {
        scoreboard.GetComponent<TextMesh>().text = score.ToString();
    }

}
