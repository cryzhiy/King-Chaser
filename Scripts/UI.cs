using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Pause Menu 
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Resume button
    public void resumeButton() {
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        gameManager.paused = false;
        gameManager.updateHUD();
        gameManager.updatePauseMenu();
    }

    // Return to Main Menu
    public void mainmenuButton() {
        SceneManager.LoadScene("MainMenu");
    }
    // Quit button that quits application
    public void quitButton() {
        Application.Quit();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Main Menu
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    // Launches the Standard Map
    public void standardMapButton() {
        SceneManager.LoadScene("GrasslandsAlphaMap");
    }
    // Launches the Randomly Generated Map
    public void randomGenButton() {
        SceneManager.LoadScene("RandomMap");
    }
}
