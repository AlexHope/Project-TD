/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserInterfaceController : MonoBehaviour
{
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

    [Header("Turret Panel")]
    [SerializeField] private RectTransform turretPanel;

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
        turretPanel.gameObject.SetActive(false);
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
    /// Applies the blur filter to the screen
    /// </summary>
    public void ApplyBlurFilter(bool state)
    {
        blurFilter.gameObject.SetActive(state);
    }

    /// <summary>
    /// Runs various functions when a wave ends, such as showing the wave timer
    /// </summary>
    private void EnemyManager_OnWaveEnd(int waveNumber)
    {
        // Display the wave timer
        StartCoroutine(DisplayWaveTimer());
    }

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
    /// Updates the relevant statistics on the top of the screen
    /// </summary>
    private void UpdateStatisticsText()
    {
        goldText.text = PlayerManager.Instance.CurrentGold.ToString();
        enemiesRemainingText.text = EnemyManager.Instance.RemainingEnemiesInWave.ToString() + "/" + EnemyManager.Instance.TotalEnemiesInWave.ToString();
        healthText.text = PlayerManager.Instance.CurrentHealth.ToString();
        waveNumberText.text = EnemyManager.Instance.DisplayWaveNumber.ToString();
    }
}
