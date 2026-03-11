using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    //FixedScript[] enemies;
    public UIHandler uiHandler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //enemies = FindObjectsByType<FixedScript>(FindObjectsSortMode.None);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (player.health <= 0)
        {
            uiHandler.DisplayLoseScreen();
            Invoke(nameof(ReloadScene), 3f);
        }

        if (AllEnemiesFixed())
        {
            uiHandler.DisplayWinScreen();
            Invoke(nameof(ReloadScene), 3f);
        }*/
    }
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /*foreachbool AllEnemiesFixed()
    {
         (FixedScript fixedScript in enemies)
        {
            if (fixedScript.isBroken) return false;
        }
        return true;
    }*/

    /*void HandleEnemyFixed()
    {
        enemiesFixed++;

        uiHandler.SetCounter(enemiesFixed, enemies.Length);
    }*/
}
