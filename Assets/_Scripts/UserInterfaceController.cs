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
/// This class is used to simplify the transitionability of panels
/// </summary>
[Serializable]
public class TransitionablePanel
{
    [Header("Panel")]
    public RectTransform Panel;
    public bool State = true;

    [Header("Transition Details")]
    [SerializeField] private Vector2 travelDistance = new Vector2();
    [SerializeField] private float transitionTime = 1.0f;
    [SerializeField] private AnimationCurve easingCurve;

    private bool isTransitioning;
    private bool finishTransition;

    // Accessors
    private Vector2 ScaledTravelDistance { get { return travelDistance * UserInterfaceController.Instance.OverlayCanvas.transform.localScale.x; } }

    /// <summary>
    /// Starts the coroutine to transition the panel
    /// </summary>
    public void StartTransition(bool instant = false, int frameDelay = 0)
    {
        UserInterfaceController.Instance.StartCoroutine(TransitionElement(instant ? 0.0f : transitionTime, frameDelay));
    }

    /// <summary>
    /// Instantly completes the transition
    /// </summary>
    public void CompleteTransition()
    {
        if (isTransitioning)
        {
            finishTransition = true;
        }
    }

    /// <summary>
    /// Transitions a transitionable panel to the desired position over time
    /// </summary>
    public IEnumerator TransitionElement(float time, int frameDelay = 0)
    {
        // Wait for the specified delay
        for (int i = 0; i < frameDelay; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        // Only allow the transition if we are not already transitioning
        if (!isTransitioning)
        {
            isTransitioning = true;

            // Determine our start and end positions
            Vector2 startPosition = Panel.position;
            Vector2 endPosition = (Vector2)Panel.position + ((State ? 1 : -1) * ScaledTravelDistance);

            // We only need to do the transition if the provided time is greater than zero
            if (time > 0.0f)
            {
                float progress = 0.0f;
                while (progress < 1.0f)
                {
                    // Break out early if needed
                    if (finishTransition)
                    {
                        finishTransition = false;
                        break;
                    }

                    Panel.position = Vector2.Lerp(startPosition, endPosition, easingCurve.Evaluate(progress));

                    // Ensure the progress is clamped
                    progress += Time.deltaTime / time;
                    progress = Mathf.Clamp01(progress);

                    yield return new WaitForEndOfFrame();
                }
            }

            // Update the element
            Panel.position = endPosition;
            State = !State;
            isTransitioning = false;
        }
    }
}

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
        mainPanel.gameObject.SetActive(true);
        statisticsPanel.gameObject.SetActive(true);
        blurFilter.gameObject.SetActive(false);
        towerPanel.gameObject.SetActive(false);
        upgradesPanel.gameObject.SetActive(false);
        otherPanel.gameObject.SetActive(false);
        optionsPanel.gameObject.SetActive(false);
        waveTimer.gameObject.SetActive(false);

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
    /// Displays the specified panel
    /// </summary>
    /// <param name="panel">The panel to be displayed</param>
    public void Button_DisplayPanel(RectTransform panel)
    {
        panel.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the specified panel
    /// </summary>
    /// <param name="panel">The panel to be hidden</param>
    public void Button_HidePanel(RectTransform panel)
    {
        panel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Moves the tower selection panel
    /// </summary>
    public void Button_TransitionTowerSelection()
    {
        towerSelection.StartTransition();
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
                Button_HidePanel(towerPanel);
                ApplyBlurFilter(false);

                // Reset the tower selection panel
                towerSelection.CompleteTransition();
                towerSelection.StartTransition(true, 1);
            });
        }
    }
    #endregion

    #region Functions
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
        enemiesRemainingText.text = EnemyManager.Instance.RemainingEnemiesInWave.ToString() + "/" + EnemyManager.Instance.TotalEnemiesInWave.ToString();
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
