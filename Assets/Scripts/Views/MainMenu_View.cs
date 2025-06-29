using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu_View : MonoBehaviour
{
    [SerializeField]
    private Button playgameBtn;

    // Start is called before the first frame update
    void Start()
    {
        playgameBtn.onClick.AddListener(LoadMap01);
    }

    private void LoadMap01()
    {
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Map_01");
    }
}
