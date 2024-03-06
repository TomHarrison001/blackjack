using UnityEngine;

public class MobileInputs : MonoBehaviour
{
    private enum InputMode { Touch, Accel, Swipe }
    private InputMode inpMode = InputMode.Touch;
    private Vector2 fingerDown, fingerUp;
    private float moveInput;

    private void Update()
    {
        if (inpMode == InputMode.Touch)
            TouchMove();
        else if (inpMode == InputMode.Accel)
            AccelMove();
        else if (inpMode == InputMode.Swipe)
            SwipeMove();
        else
            moveInput = 0;
        Move();
    }

    private void TouchMove()
    {
        if (Input.touchCount > 0)
        {
            if (Input.mousePosition.x > (Screen.width * 0.75f))
                moveInput = 1f;
            else if (Input.mousePosition.x <= (Screen.width * 0.25f))
                moveInput = -1f;
            else
                moveInput = 0f;
        }
        else moveInput = 0f;
    }

    private void AccelMove()
    {
        moveInput = Mathf.Clamp(Input.acceleration.x, -1f, 1f);
    }

    private void SwipeMove()
    {
        if (Input.touchCount == 1)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                fingerDown = Input.touches[0].position;
            }
            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                fingerUp = Input.touches[0].position;
                moveInput = CheckSwipe();
            }
        }
        else moveInput = 0f;
    }

    private float CheckSwipe()
    {
        if (fingerDown.x - fingerUp.x < -5f)
            return 1f;
        if (fingerDown.x - fingerUp.x > 5f)
            return -1f;
        return 0f;
    }

    private void Move()
    {
        Debug.Log(moveInput);
    }
}
