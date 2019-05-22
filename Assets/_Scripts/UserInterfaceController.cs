/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This class controls all of the UI elements
/// </summary>
public class UserInterfaceController : MonoBehaviour
{
    #region Variables
    public static UserInterfaceController Instance { get; private set; }

    [Header("General")]
    [SerializeField] private Canvas overlayCanvas;
    [SerializeField] private RectTransform blurFilter;

    [Header("Main Panel")]
    [SerializeField] private RectTransform mainPanel;

    [Header("Statistics")]
    [SerializeField] private RectTransform statisticsPanel;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI enemiesRemainingText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI waveNumberText;

    [Header("Tower Panel")]
    [SerializeField] private RectTransform towerPanel;
    [SerializeField] private TransitionablePanel towerSelection;
    [SerializeField] private RectTransform towerTierSelection;
    [SerializeField] private ScrollRect towerSpecificSelection;
    [SerializeField] private RectTransform towerSelectionBackButton;
    [SerializeField] private RectTransform[] tier1TowerButtonPrefabs;
    [SerializeField] private RectTransform[] tier2TowerButtonPrefabs;
    [SerializeField] private RectTransform[] tier3TowerButtonPrefabs;

    [Header("Upgrades Panel")]
    [SerializeField] private RectTransform upgradesPanel;

    [Header("Other Panel")]
    [SerializeField] private RectTransform otherPanel;

    [Header("Options")]
    [SerializeField] private RectTransform optionsPanel;

    [Header("Miscellaneous")]
    [SerializeField] private GameObject waveTimer;
    [SerializeField] private TextMeshProUGUI waveTimerText;

    public Canvas OverlayCanvas { get { return overlayCanvas; } }
    #endregion

    #region MonoBehaviours

    /// <summary>
    /// Assigns the static instance
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Ensures panel states are correctly set on start, and assigns listeners
    /// </summary>
    private void Start()
    {
        // Listen to the wave ending event
        EnemyManager.OnWaveEnd += EnemyManager_OnWaveEnd;

        // Set initial panel states
        OverlayCanvas.gameObject.SetActive(true);
        ShowPanel(mainPanel);
        ShowPanel(statisticsPanel);
        HidePanel(blurFilter);
        HidePanel(towerPanel);
        HidePanel(upgradesPanel);
        HidePanel(otherPanel);
        HidePanel(optionsPanel);

        // Show the wave timer
        StartCoroutine(DisplayWaveTimer());
    }

    /// <summary>
    /// Run every frame to update UI elements
    /// </summary>
    private void Update()
    {
        UpdateStatisticsText();
    }

    #endregion

    #region Button Functions
    /// <summary>
    /// This function is called to close all opened windows and return to the normal game state
    /// </summary>
    public void Button_ResetOverlay()
    {
        // Disable any opened additional panels
        HidePanel(blurFilter);
        HidePanel(towerPanel);
        HidePanel(upgradesPanel);
        HidePanel(otherPanel);
        HidePanel(optionsPanel);
    }

    /// <summary>
    /// Moves the tower selection panel
    /// </summary>
    public void Button_TransitionTowerSelection()
    {
        if (!towerSelection.IsTransitioning)
        {
            towerSelection.StartTransition();
            StartCoroutine(TowerSelectionTransitionReactor());
        }
    }

    /// <summary>
    /// Updates the list of towers to choose from when a tier is selected
    /// </summary>
    /// <param name="tier">The tier of tower</param>
    public void Button_UpdateTowerSelectionList(int tier)
    {
        // First, destroy any existing buttons (seems the child count is only updated at the end of the frame, so a while loop doesn't work here)
        for (int i = 0; i < towerSpecificSelection.content.childCount; i++)
        {
            Destroy(towerSpecificSelection.content.GetChild(i).gameObject);
        }

        // Then, find the relevant list of prefabs and create buttons for each
        RectTransform[] towerPrefabs;
        Tower.Tier towerTier = (Tower.Tier)tier;
        switch(towerTier)
        {
            case Tower.Tier.Tier1:
                {
                    towerPrefabs = tier1TowerButtonPrefabs;
                    break;
                }
            case Tower.Tier.Tier2:
                {
                    towerPrefabs = tier2TowerButtonPrefabs;
                    break;
                }
            case Tower.Tier.Tier3:
                {
                    towerPrefabs = tier3TowerButtonPrefabs;
                    break;
                }
            default: goto case Tower.Tier.Tier1;
        }

        for (int i = 0; i < towerPrefabs.Length; i++)
        {
            // First, instantiate the button
            RectTransform towerButton = RectTransform.Instantiate(towerPrefabs[i], towerSpecificSelection.content);

            // Now determine the prefab that it can spawn
            GameObject towerPrefab = towerButton.GetComponent<TowerButtonReferencer>().TowerPrefab;

            // Lastly, add a listener so that this button spawns the correct tower
            Button button = towerButton.GetComponent<Button>();
            button.onClick.AddListener(delegate
            {
                // Tell the tower manager to create the tower
                TowerManager.Instance.CreateTower(towerPrefab);

                // Finally, hide the tower panel and turn the blur filter off
                HidePanel(towerPanel);
                ApplyBlurFilter(false);
            });
        }
    }
    #endregion

    #region Functions
    /// <summary>
    /// Displays the specified panel
    /// </summary>
    /// <param name="panel">The panel to be displayed</param>
    public void ShowPanel(RectTransform panel)
    {
        panel.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the specified panel
    /// </summary>
    /// <param name="panel">The panel to be hidden</param>
    public void HidePanel(RectTransform panel)
    {
        panel.gameObject.SetActive(false);

        // Force the tower selection state to true
        if (panel == towerPanel)
        {
            towerSelection.SetState(true);
            towerSelectionBackButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Applies the blur filter to the screen
    /// </summary>
    public void ApplyBlurFilter(bool state)
    {
        blurFilter.gameObject.SetActive(state);
    }

    /// <summary>
    /// Updates the relevant statistics on the top of the screen
    /// </summary>
    private void UpdateStatisticsText()
    {
        goldText.text = PlayerManager.Instance.CurrentGold.ToString();
        enemiesRemainingText.text = EnemyManager.Instance.RemainingEnemiesInWave.ToString() + " / " + EnemyManager.Instance.TotalEnemiesInWave.ToString();
        healthText.text = PlayerManager.Instance.CurrentHealth.ToString();
        waveNumberText.text = EnemyManager.Instance.DisplayWaveNumber.ToString();
    }

    #endregion

    #region Coroutines
    /// <summary>
    /// Displays & updates the wave timer
    /// </summary>
    private IEnumerator DisplayWaveTimer()
    {
        waveTimer.gameObject.SetActive(true);
        float secondsFull = EnemyManager.Instance.WaveTimer;
        while (!EnemyManager.Instance.IsWaveActive)
        {
            waveTimerText.text = "Next wave in:\n" + (int)secondsFull + " seconds";
            secondsFull -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        waveTimer.gameObject.SetActive(false);
    }

    /// <summary>
    /// Waits for the transition to complete before enabling interactivity & the back button
    /// </summary>
    private IEnumerator TowerSelectionTransitionReactor()
    {
        CanvasGroup towerSpecificSelectionCanvasGroup = towerSpecificSelection.GetComponent<CanvasGroup>();
        CanvasGroup towerTierSelectionCanvasGroup = towerTierSelection.GetComponent<CanvasGroup>();

        // Wait until the transition is happening
        yield return new WaitUntil(() => towerSelection.IsTransitioning);

        // Disable the back button if we're going back to the tier selection
        if (!towerSelection.State)
        {
            towerSelectionBackButton.gameObject.SetActive(false);
        }

        // Disable interactivity while transitioning
        towerSpecificSelectionCanvasGroup.interactable = false;
        towerTierSelectionCanvasGroup.interactable = false;

        yield return new WaitUntil(() => !towerSelection.IsTransitioning);

        // Enable interactivity after the transition is over
        towerSpecificSelectionCanvasGroup.interactable = true;
        towerTierSelectionCanvasGroup.interactable = true;

        // Enable the back button if we've ended on the tower selection
        if (!towerSelection.State)
        {
            towerSelectionBackButton.gameObject.SetActive(true);
        }
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Runs various functions when a wave ends, such as showing the wave timer
    /// </summary>
    private void EnemyManager_OnWaveEnd(int waveNumber)
    {
        // Display the wave timer
        StartCoroutine(DisplayWaveTimer());
    }
    #endregion
}
