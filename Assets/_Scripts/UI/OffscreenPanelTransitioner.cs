/*==========================================================
Created by Alex Hope. For use in the Project-TD app.
==========================================================*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script will transition any panel off the screen, ensuring the button used to transition this remains on screen
/// </summary>
public class OffscreenPanelTransitioner : MonoBehaviour
{
    public enum TransitionDirection
    {
        Left,
        Right,
        Up,
        Down,
    }

    [SerializeField] private RectTransform button;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private bool initialState;
    [SerializeField] private TransitionDirection transitionDirection;
    [SerializeField] private float transitionTime = 1.0f;
    [SerializeField] private AnimationCurve easingCurve = new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 0, 0));

    private Vector2 distanceToScreenEdge = new Vector2();
    private bool isTransitioning = false;
    private bool state = true;

    /// <summary>
    /// Determines the distance to the edge of the screen
    /// </summary>
    private void Start()
    {
        // Calculate the distance to the edge of the screen
        float scaleMultiplier = UserInterfaceController.Instance.OverlayCanvas.transform.localScale.x;
        switch (transitionDirection)
        {
            case TransitionDirection.Left:
                {
                    distanceToScreenEdge.x = ((button.rect.width / 2) * scaleMultiplier) - (button.position.x);
                    break;
                }
            case TransitionDirection.Right:
                {
                    distanceToScreenEdge.x = (Screen.width - button.position.x) - ((button.rect.width / 2) * scaleMultiplier);
                    break;
                }
            case TransitionDirection.Up:
                {
                    distanceToScreenEdge.y = (Screen.height - button.position.y) - ((button.rect.height / 2) * scaleMultiplier);
                    break;
                }
            case TransitionDirection.Down:
                {
                    distanceToScreenEdge.y = ((button.rect.height / 2) * scaleMultiplier) - (button.position.y);
                    break;
                }
            default: goto case TransitionDirection.Left;
        }

        if (!initialState)
        {
            StartCoroutine(TransitionPanel(0.0f));
        }
    }

    /// <summary>
    /// Called to start the transition
    /// </summary>
    public void Button_StartTransition()
    {
        StartCoroutine(TransitionPanel(transitionTime));
    }

    /// <summary>
    /// Transitions the panel
    /// </summary>
    /// <param name="endPosition">The position to transition the panel to</param>
    private IEnumerator TransitionPanel(float time)
    {
        if (!isTransitioning)
        {
            isTransitioning = true;

            // Update the sprite
            button.GetComponent<Button>().image.sprite = state ? inactiveSprite : activeSprite;

            Vector2 startPosition = transform.position;
            Vector2 endPosition = startPosition + (distanceToScreenEdge * (state ? 1 : -1));

            if (time > 0.0f)
            {
                float progress = 0.0f;
                while (progress < 1.0f)
                {
                    // Transition the panel
                    transform.position = Vector2.Lerp(startPosition, endPosition, easingCurve.Evaluate(progress));

                    // Ensure the progress is clamped
                    progress += Time.deltaTime / time;
                    progress = Mathf.Clamp01(progress);

                    yield return new WaitForEndOfFrame();
                }
            }

            // Forcibly snap to the end position to prevent any floating point issues
            transform.position = endPosition;

            // Update the state
            state = !state;

            isTransitioning = false;
        }
    }
}
