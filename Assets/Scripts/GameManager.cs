using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        //Reset save
        PlayerPrefs.DeleteAll();
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject);
            Destroy(hud);
            Destroy(menu);
            Destroy(mainMenu);
           
            return;
        }

        instance = this;
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += OnSceneLoaded;


    }

    private void Update()
    {
        OpenMainMenu();
        FinishGame();
    }


    //Ressources
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;
    public List<int> xpTable;

    //References
    public Player player;
    public Weapon weapon;
    public FloatingTextManager floatingTextManager;
    public RectTransform hitpointBar;
    public Animator deathMenuAnim;
    public Animator finishMenuAnim;
    public GameObject hud;
    public GameObject menu;
    public GameObject mainMenu;
    public GameObject finishMenu;
   


    // Logic
    public int pesos;
    public int experience;

    //Floating text
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }

    //Upgrade Weapon
    public bool TryUpgradeWeapon()
    {
        //is the weapon max level?
        if (weaponPrices.Count <= weapon.weaponLevel)
            return false;

        if(pesos >= weaponPrices[weapon.weaponLevel])
        {
            pesos -= weaponPrices[weapon.weaponLevel];
            weapon.UpgradeWeapon();
            return true;
        }

        return false;
    }

    //Hitpoint Bar
    public void OnHitpointChange()
    {
        float ratio = (float)player.hitpoint / (float)player.maxHitpoint;
        hitpointBar.localScale = new Vector3(1, ratio, 1);
    }

    //Experience System
    public int GetCurrentLevel()
    {
        int r = 0;
        int add = 0;

        while (experience >= add)
        {
            add += xpTable[r];
            r++;

            if (r == xpTable.Count) //Max level
                return r;
        }

        return r;

    }
    public int GetXpToLevel(int level)
    {
        int r = 0;
        int xp = 0;

        while (r < level)
        {
            xp += xpTable[r];
            r++;
        }
        return xp;
    }
    public void GrantXp(int xp)
    {
        int currLevel = GetCurrentLevel();
        experience += xp;
        if (currLevel < GetCurrentLevel())
            OnLevelUp();
    }

    private void OnLevelUp()
    {
     
        player.OnLevelUp();
        OnHitpointChange();
    }

    //On Scene Loaded
    public void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;
    }

    //Death menu and Respawn
    public void Respawn()
    {
        deathMenuAnim.SetTrigger("Hide");
        SceneManager.LoadScene("Main");
        player.Respawn();
    }

    public void WinRespawn()
    {
        finishMenuAnim.SetTrigger("hide");
        SceneManager.LoadScene("Main");
        player.Respawn();
    }

    //Save state
    public void SaveState()
    {
        string s = "";

        s += "0" + "|";
        s += pesos.ToString() + "|";
        s += experience.ToString() + "|";
        s += weapon.weaponLevel.ToString();

        PlayerPrefs.SetString("SaveState", s);
      
    }

    //Load state
    public void LoadState(Scene s, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= LoadState;

        if (!PlayerPrefs.HasKey("SaveState"))
            return;

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        //Change player skin
        pesos = int.Parse(data[1]);

        //Experience
        experience = int.Parse(data[2]);
        if(GetCurrentLevel()!=1)
            player.SetLevel(GetCurrentLevel());

        //Change the weapon Level
        weapon.SetWeaponLevel(int.Parse(data[3]));

       

    }

 public void QuitGame()
    {
        Application.Quit();
    }
     
    public void OpenMainMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mainMenu.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("MainMenuClose"))
                mainMenu.GetComponent<Animator>().SetTrigger("show");
        }
    }


    public void FinishGame()
    {
        if (GameObject.Find("Enemy") != null)
        {
            if (GameObject.Find("Enemy").transform.childCount == 0)
            {
                finishMenu.GetComponent<Animator>().SetTrigger("show");
                
            }
        }
       
    }

}
