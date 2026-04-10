using UnityEngine;

public class Gamedata //Game data used across the game
{
    private static Gamedata instance;
    public int playerHealth { get; set; } = 6;
    public int playerPowerbar { get; set; } = 0;
    public Vector2 playerPosition { get; set; } = new Vector2(0, 0);
    public bool dataExists { get; set; }

    private Gamedata() { }

    public static Gamedata Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Gamedata();
            }
            return instance;
        }
    }
}

