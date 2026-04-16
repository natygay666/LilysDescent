using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using Unity.VisualScripting; 

public class GameUIController : MonoBehaviour
{
    [Header("esc pause")]
    public GameObject pauseMenuUI;
    public static bool GameIsPaused = false;

    [Header("inventory xd")]
    public GameObject inventoryMenuUI;

    [Header("controls png")]
    public GameObject controlsPopupUI;
    public Button closePopupButton;

    [Header("saveref")]
    public GameObject player; 
    
    private bool isInventoryOpen = false;
    
    private string filePath;

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerData.dat");
    }

    void Start()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (inventoryMenuUI != null) inventoryMenuUI.SetActive(false);
        if (controlsPopupUI != null) controlsPopupUI.SetActive(false);
        
        if (closePopupButton != null)
            closePopupButton.onClick.AddListener(CloseControlsPopup);
        
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isInventoryOpen)
            {
                CloseInventoryMenu();
            }
            else
            {
                if (GameIsPaused)
                    Resume();
                else
                    Pause();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (GameIsPaused)
            {
                Resume();
            }

            if (isInventoryOpen)
                CloseInventoryMenu();
            else
                OpenInventoryMenu();
        }
    }

    // ==================== pause menu ====================
    public void Resume()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
            
        Time.timeScale = 1f;
        GameIsPaused = false;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
            
        Time.timeScale = 0f;
        GameIsPaused = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ==================== inventory ====================
    void OpenInventoryMenu()
    {
        if (inventoryMenuUI != null)
            inventoryMenuUI.SetActive(true);
            
        Time.timeScale = 0f;
        isInventoryOpen = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseInventoryMenu()
    {
        if (inventoryMenuUI != null)
            inventoryMenuUI.SetActive(false);
        
        if (controlsPopupUI != null)
            controlsPopupUI.SetActive(false);
            
        Time.timeScale = 1f;
        isInventoryOpen = false;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ==================== controls png menu ====================
    public void ShowControlsPopup()
    {
        if (controlsPopupUI != null)
            controlsPopupUI.SetActive(true);
    }

    public void CloseControlsPopup()
    {
        if (controlsPopupUI != null)
            controlsPopupUI.SetActive(false);
    }

    // ==================== save system====================
    public void SaveGame()
    {
        if (player == null)
        {
            Debug.LogError("GameObject del jugador no asignado en GameUIController");
            return;
        }
        Variables playerVars = player.GetComponent<Variables>();
        if (playerVars == null)
        {
            Debug.LogError("No se encontró el componente Variables en el jugador");
            return;
        }
        
        int currentHp = (int)playerVars.declarations.Get("PlayerHp");
        int currentUlti = (int)playerVars.declarations.Get("UltiValue");
        
        PlayerSaveData data = new PlayerSaveData();
        data.PlayerHp = currentHp;
        data.UltiValue = currentUlti;
        
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        
        Debug.Log($"saved! - HP: {currentHp}, Ulti: {currentUlti}");
    }

    public void LoadGame()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("no save data");
            return;
        }

        if (player == null)
        {
            Debug.LogError("GameObject del jugador no asignado en GameUIController");
            return;
        }

        Variables playerVars = player.GetComponent<Variables>();
        if (playerVars == null)
        {
            Debug.LogError("No se encontró el componente Variables en el jugador");
            return;
        }
        
        string json = File.ReadAllText(filePath);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
        
        playerVars.declarations.Set("PlayerHp", data.PlayerHp);
        playerVars.declarations.Set("UltiValue", data.UltiValue);
        
        Debug.Log($"Partida cargada - HP: {data.PlayerHp}, Ulti: {data.UltiValue}");
    }

    // ==================== cosas ui  ====================
    
    public void OnSaveButton()
    {
        SaveGame();
        Debug.Log("saved!");
    }

    public void OnLoadButton()
    {
        LoadGame();
        Debug.Log("game loaded!");
    }

    public void OnQuitToMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        isInventoryOpen = false;
        SceneManager.LoadScene("MainMenu");
    }
    public void OnPlayButton()
    {
        CloseInventoryMenu();
    }

    public void OnControlsButton()
    {
        ShowControlsPopup();
    }

    public void UpdateHealthSlider(int playerHp, int playerMaxHp)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateUltiSlider(int ultiValue)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateInventoryUI(List<string> inventory)
    {
        throw new System.NotImplementedException();
    }
}
[System.Serializable]
public class PlayerSaveData
{
    public int PlayerHp;
    public int UltiValue;
}


