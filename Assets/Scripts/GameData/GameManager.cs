using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public int playerHealth = 5;
    public int playerPowerbar = 0;
    public Vector2 playerPosition = new Vector2(0, 0);
    public int checkPointNum;

    [SerializeField] Button continueButton;

    private void Start()
    {
        //Activates 'Continue' button on main menu if data exists
        if (continueButton && File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            continueButton.interactable = true;
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject);

        }
    }
    //Saves the game locally
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData(Gamedata.Instance.playerHealth, VectorConversion(Gamedata.Instance.playerPosition),
        Gamedata.Instance.playerPowerbar, Gamedata.Instance.checkPointNum);

        bf.Serialize(file, data);
        file.Close();

    }

    public void Load(int scene)
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();
            
            playerHealth = data.playerHealth;
            playerPosition = ConvertBackToVector(data.playerPosition);
            playerPowerbar = data.playerPowerbar;
            checkPointNum = data.checkPointNum;
            Gamedata.Instance.playerHealth = playerHealth;
            Gamedata.Instance.playerPosition = playerPosition;
            Gamedata.Instance.playerPowerbar = playerPowerbar;
            Gamedata.Instance.checkPointNum = checkPointNum;
            SceneManager.LoadScene(scene);
        }
    }

    //Creates a new game and deletes the old save file
    public void NewGame(int scene)
    {
        string path = Application.persistentDataPath + "/playerInfo.dat";

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        playerHealth = 6;
        playerPowerbar = 0;
        playerPosition = Vector2.zero;
        checkPointNum = -1;
        Gamedata.Instance.dataExists = false;

        Gamedata.Instance.playerHealth = playerHealth;
        Gamedata.Instance.playerPowerbar = playerPowerbar;
        Gamedata.Instance.playerPosition = playerPosition;
        Gamedata.Instance.checkPointNum = checkPointNum;

        SceneManager.LoadScene(scene);
    }

    //Converts player position into a serializable format 
    private (float, float) VectorConversion(Vector2 playerPos)
    {
        return (playerPos.x, playerPos.y);

    }

    private Vector2 ConvertBackToVector((float, float) playerPos)
    {
        return new Vector2(playerPos.Item1, playerPos.Item2);

    }
}

[Serializable]
class PlayerData
{
    public int playerHealth;
    public (float, float) playerPosition;
    public int playerPowerbar;
    public int checkPointNum;

    public PlayerData(int health, (float, float) position, int powerbar, int checkpoint)
    {
        playerHealth = health;
        playerPosition = position;
        playerPowerbar = powerbar;
        checkPointNum = checkpoint;
    }
}
