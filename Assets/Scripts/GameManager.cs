using System;
using Beginner2D;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerController player;
    //FixedScript[] enemies;
    BossFightController boss;
    BossFightController2 boss2;
    public UIHandler uiHandler;
    [NonSerialized] public int miniBossesDead = 0;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //enemies = FindObjectsByType<FixedScript>(FindObjectsSortMode.None);
        boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossFightController>();
        
        boss2 = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossFightController2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.health <= 0)
        {
            uiHandler.DisplayLoseScreen();
            Invoke(nameof(ReloadScene), 3f);
        }

        if(boss != null)
        {
            if (boss.isDead)
            {
                Invoke(nameof(BossArenaTransition), 3f);
            }
        }

        if (boss2 != null)
        {
            if (boss2.isDead & miniBossesDead == 2)
            {
                uiHandler.DisplayWinScreen();
                Invoke(nameof(ReloadScene), 3f);
            }
        }
    }
    void ReloadScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    void BossArenaTransition()
    {
        SceneManager.LoadScene("Arena 2");
    }

    //bool AllEnemiesFixed()
    //{
    //    //foreach(FixedScript fixedScript in enemies)
    //    //{
    //    //    if (fixedScript.isBroken) return false;
    //    //}
    //    //return true;
    //    foreach (EnemyController enemy in enemies)
    //    {
    //        if (enemy.isBroken) return false;
    //    }
    //    return true;

    //}

    /*void HandleEnemyFixed()
    {
        enemiesFixed++;

        uiHandler.SetCounter(enemiesFixed, enemies.Length);
    }*/
}
