using Beginner2D;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    //FixedScript[] enemies;
    EnemyController[] enemies;
    public UIHandler uiHandler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //enemies = FindObjectsByType<FixedScript>(FindObjectsSortMode.None);
        enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.health <= 0)
        {
            uiHandler.DisplayLoseScreen();
            Invoke(nameof(ReloadScene), 3f);
        }

        if (AllEnemiesFixed())
        {
            uiHandler.DisplayWinScreen();
            Invoke(nameof(ReloadScene), 3f);
        }
    }
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    bool AllEnemiesFixed()
    {
        //foreach(FixedScript fixedScript in enemies)
        //{
        //    if (fixedScript.isBroken) return false;
        //}
        //return true;
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.isBroken) return false;
        }
        return true;

    }

    /*void HandleEnemyFixed()
    {
        enemiesFixed++;

        uiHandler.SetCounter(enemiesFixed, enemies.Length);
    }*/
}
