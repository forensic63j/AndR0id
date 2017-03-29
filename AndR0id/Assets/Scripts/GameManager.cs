using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public string[] levelNames;
    public int levelIndex;

	// Use this for initialization
	void Start ()
    {
        levelNames = new string[15];
        for(int i = 1; i <= 15; i++)
        {
            levelNames[i - 1] = "level" + i;
        }
        levelIndex = SceneManager.GetActiveScene().buildIndex;
	}

    public void LoadNextLevel(int level)
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("Player"));
        levelIndex = level;
        SceneManager.LoadScene(levelNames[levelIndex]);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
