﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    public TMP_InputField scroleSpeed;
    public Toggle diceInflate;

    public void LoadSceen()
    {
        //todo, se
        if (PlayerPrefs.HasKey("cameraSpeed"))
        {
            scroleSpeed.GetComponent<TMP_InputField>().text = PlayerPrefs.GetInt("cameraSpeed").ToString();
            diceInflate.isOn = bool.Parse(PlayerPrefs.GetString("diceInflate"));
        } else
        {
            scroleSpeed.GetComponent<TMP_InputField>().text = GlobalVariables.data.CAMERA_ROTATION_SPEED.ToString();
            diceInflate.isOn = GlobalVariables.data.SHOW_DICE_INFLATE_ANIMATION;
        }
    }

    public void ApplySettings()
    {
        //take the settings on the screen and apply them to the Global Variables and player prefs
        int speed = int.Parse( scroleSpeed.GetComponent<TMP_InputField>().text);
        if(ValidScroleSpeed(speed))
        {
            PlayerPrefs.SetInt("cameraSpeed", speed);
            GlobalVariables.data.CAMERA_ROTATION_SPEED = speed;
        }
        PlayerPrefs.SetString("diceInflate", diceInflate.isOn.ToString());
        GlobalVariables.data.SHOW_DICE_INFLATE_ANIMATION = diceInflate.isOn;
        PlayerPrefs.Save();
    }

    public bool ValidScroleSpeed(int speed)
    {
        if (speed > 0 && speed < 300)
        {
            return true;
        } else
        {
            return false;
        } 
    }
}
