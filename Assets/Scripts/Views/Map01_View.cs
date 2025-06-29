using System;
using System.Collections;
using System.Collections.Generic;
using Terresquall;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Map01_View : MonoBehaviour
{
    public Action OnChangeWeapon; // Callback for weapon change
    public Action OnReturnToMenu; // Callback for returning to the main menu

    [SerializeField]
    private GameObject mapRoot; // Root object for the map
    [SerializeField]
    private VirtualJoystick joystick; // Joystick for player control
    [SerializeField]
    private Button changeWeaponButton;
    [SerializeField]
    private TextMeshProUGUI weaponText; // Text to display current weapon
    [SerializeField]
    private TextMeshProUGUI hpValueText;
    [Header ("Gameover")]
    [SerializeField]
    private GameObject gameOverPanel; // Panel to display game over screen
    [SerializeField]
    private GameObject gameOverObj; 
    [SerializeField]
    private GameObject youWinObj;
    [SerializeField]
    private Button returnBtn;

    public void Init()
    {
        // Initialize the map view
        mapRoot.SetActive(true);
        changeWeaponButton.onClick.AddListener(ChangeWeapon);
        returnBtn.onClick.AddListener(ReturnToMainMenu);

        gameOverPanel.SetActive(false);
    }

    private void ChangeWeapon()
    {
        OnChangeWeapon?.Invoke(); // Invoke the weapon change callback
    }

    private void ReturnToMainMenu()
    {
       OnReturnToMenu?.Invoke(); // Invoke the return to menu callback
    }

    public void SetChangeWeaponText(string weaponName)
    {
        // Update the weapon text display
        weaponText.text = weaponName;
    }   

    public void SetPlayerHealth(int currentHealth)
    {
        // Update the player's health display
        hpValueText.text = $"{Mathf.Clamp(currentHealth, 0, Mathf.Abs(currentHealth))}";
    }

    public void ShowGameOver(bool isWin)
    {
        // Show the game over panel with win or lose message
        gameOverPanel.SetActive(true);
        gameOverObj.SetActive(!isWin);
        youWinObj.SetActive(isWin);
    }

    public Vector2 GetJoystickInput()
    {
        // Get the joystick input for player movement
        return joystick.GetAxis();
    }
}
