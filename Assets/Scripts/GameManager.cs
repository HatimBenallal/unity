using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.UI;       //Allows us to use Lists. 
using UnityEngine.SceneManagement;        //Allows us to use SceneManager

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDealy = .1f;
    public static GameManager instance = null;
    
    //Store a reference to our BoardManager which will set up the level.
    public BoardManager boardScript;
    
    public int playerLifePoints = 100;
    public int playerCoinsPoints = 0;
    public int playerkills = 0;
    public float time = 0;
    [HideInInspector] public bool playersTurn = true;
    
    //Current level number, expressed in game as "Day  1".
    public Text levelText;
    private GameObject levelImage;
    private int level = 1;
    public List<Enemy> enemies;
    public List<Boss> boss;
    private bool enemiesMoving;
    private bool bossMoving;
    private bool doingSetup;
    private int cont;
    private int kill = 1;
    public Text killText;
    public Text coinsText;
    public int scene = 0;
    public bool back = false;
    public int damage;



    //Awake is always called before any Start functions
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        //Get a component reference to the attached BoardManager script
        DontDestroyOnLoad(gameObject);
        if (scene==5){
            boss= new List<Boss>();
        }
        else
            enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();

        //Call the InitGame function to initialize the first level 
        InitGame();
        
    }

    void OnLevelWasLoaded (int index)
    {
        if (scene == 0){
            level ++;
            InitGame();            
        }
        else
            InitGame();
        
    }

    //Initializes the game for each level.
    void InitGame()
    {
        
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Level " + level +"\nRoom "+ (scene+1);
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);
        
         
        if (back == false)
            enemies.Clear();
        //Call the SetupScene function of the BoardManager script, pass it current level number.
        
        boardScript.SetupScene(level);
        playerCoinsPoints = GameManager.instance.playerCoinsPoints;
        playerkills = GameManager.instance.playerkills;
        coinsText = GameObject.Find("coinsText").GetComponent<Text>();
        coinsText.text = " coins: " + playerCoinsPoints;
        killText = GameObject.Find("killText").GetComponent<Text>();
        killText.text =  "kills: " + playerkills;
        
    }

    

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        levelText.text = "After " + level + " levels";
        levelImage.SetActive(true);
        enabled = false;
    }

    //Update is called every frame.
    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup || bossMoving)
            return;
        StartCoroutine(MoveEnemies());

    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add (script);
    }

    public void AddBossToList(Boss script)
    {
        boss.Add (script);
    }


    public void RemoveEnemies(Enemy script){
        
        
        enemies.Remove (script);

        playerkills += kill;
        killText = GameObject.Find("killText").GetComponent<Text>();
        killText.text =  "kills: " + playerkills;
        GameManager.instance.playerkills = playerkills;
        
        //playerCoinsPoints = GameManager.instance.playerCoinsPoints;
        playerCoinsPoints += 5;
        coinsText = GameObject.Find("coinsText").GetComponent<Text>();
        coinsText.text = " coins: " + playerCoinsPoints;
        GameManager.instance.playerCoinsPoints= playerCoinsPoints;


        //cont = cont -1;
        if (enemies.Count == 0)
        {
            boardScript.Exxit();
        }
    }

    public void RemoveBoss(Boss script){
        
        
        boss.Remove (script);

        playerkills += kill;
        killText = GameObject.Find("killText").GetComponent<Text>();
        killText.text =  "kills: " + playerkills;
        GameManager.instance.playerkills = playerkills;
        
        //playerCoinsPoints = GameManager.instance.playerCoinsPoints;
        playerCoinsPoints += 20;
        coinsText = GameObject.Find("coinsText").GetComponent<Text>();
        coinsText.text = " coins: " + playerCoinsPoints;
        GameManager.instance.playerCoinsPoints= playerCoinsPoints;


        //cont = cont -1;
        if (boss.Count == 0)
        {
            boardScript.Exxit();
        }
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        bossMoving = true;
        yield return new WaitForSeconds(turnDealy);
        if (enemies.Count == 0 || boss.Count == 0)
        {
            yield return new WaitForSeconds(turnDealy);
        }
        for (int i=0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        for (int i=0; i < boss.Count; i++)
        {
            boss[i].MoveEnemy();
            yield return new WaitForSeconds(boss[i].moveTime);
        }
        
        playersTurn = true;
        enemiesMoving = false;
        bossMoving = false;
    }
}
