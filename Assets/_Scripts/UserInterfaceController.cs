/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Turret Panel")]
    [SerializeField] private RectTransform turretPanel;

    [Header("Upgrades Panel")]
    [SerializeField] private RectTransform upgradesPanel;

    [Header("Other Panel")]
    [SerializeField] private RectTransform otherPanel;

    [Header("Options")]
    [SerializeField] private RectTransform optionsPanel;

    [Header("Miscellaneous")]
    [SerializeField] private GameObject waveTimerText;

    public Canvas OverlayCanvas { get { return overlayCanvas; } }

    /// <summary>
    /// Assigns the static instance
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Ensures panel states are correctly set on start
    /// </summary>
    private void Start()
    {
        mainPanel.gameObject.SetActive(true);
        statisticsPanel.gameObject.SetActive(true);

        blurFilter.gameObject.SetActive(false);
        turretPanel.gameObject.SetActive(false);
        upgradesPanel.gameObject.SetActive(false);
        otherPanel.gameObject.SetActive(false);
        optionsPanel.gameObject.SetActive(false);
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
}
