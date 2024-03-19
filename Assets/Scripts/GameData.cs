using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public int day = 0;
    public int waveIndex;
    public int photos;
    public int player_weapon;
    public int player_maxHealth;
    public int home_hasOpenedBox; //acts as bool: 0 = false, 1 = true
    public int relationshipLevel;
    public int hasNotification;//acts as bool: 0 = false, 1 = true
    public int showPhoto;//acts as bool: 0 = false, 1 = true
    public int gameFinished;//acts as bool: 0 = false, 1 = true
}
