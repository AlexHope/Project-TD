/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    private Vector2 travelDistance = new Vector2();
    [SerializeField] private float transitionTime = 1.0f;
    [SerializeField] private AnimationCurve easingCurve;

    private bool finishTransition;

    // Accessors
    public bool IsTransitioning { get; private set; }
    private Vector2 ScaledTravelDistance { get { return travelDistance * UserInterfaceController.Instance.OverlayCanvas.transform.localScale.x; } }

    /// <summary>
    /// Starts the coroutine to transition the panel
    /// </summary>
    /// <param name="instant">Whether the transition is instant</param>
    public void StartTransition(bool instant = false)
    {
        UserInterfaceController.Instance.StartCoroutine(TransitionElement(instant ? 0.0f : transitionTime));
    }

    /// <summary>
    /// Transitions a transitionable panel to the desired position over time
    /// </summary>
    /// <param name="time">The transition time</param>
    private IEnumerator TransitionElement(float time)
    {
        // Only allow the transition if we are not already transitioning
        if (!IsTransitioning)
        {
            IsTransitioning = true;

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
            IsTransitioning = false;
        }
    }

    /// <summary>
    /// Instantly sets the state of the panel
    /// </summary>
    /// <param name="state">The state to set</param>
    public void SetState(bool state)
    {
        UserInterfaceController.Instance.StartCoroutine(ForceState(state));
    }

    /// <summary>
    /// Forces the panel to a state
    /// </summary>
    /// <param name="state">State to force to</param>
    private IEnumerator ForceState(bool state)
    {
        // If it is transitioning, force it to complete
        if (IsTransitioning)
        {
            finishTransition = true;
            yield return new WaitForEndOfFrame();
        }

        // Once completed, force to the desired state if not already
        if (State != state)
        {
            StartTransition(true);
        }
    }
}
