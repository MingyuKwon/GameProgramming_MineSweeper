using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public enum IngameInputHardWare
{
    Mouse = 0,
    JoyStick = 1,
}

public enum InputMode
{
    InGame = 0,
    UI = 1,
}

public enum InputType
{
    Move = 0,
    Shovel = 1,
    Interact = 2,
}

public class InputManager : MonoBehaviour
{
    public static IngameInputHardWare currentInputHardware = IngameInputHardWare.Mouse;

    public class InputEvent
    {
        #region Event
        public static event Action<Vector3Int> MovePressEvent;
        public static void Invoke_Move(Vector3Int position)
        {
            MovePressEvent.Invoke(position);
        }

        #endregion
    }

    public static InputManager instance = null;
    public static Stack<InputMode> inputControlStack = new Stack<InputMode>();
        
    void Awake() {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }else
        {
            Destroy(this.gameObject);
        }
    }

    public static void getInput(InputMode type)
    {
        if(inputControlStack.Count != 0 && inputControlStack.Peek() == type)
        {
            return;
        }

        inputControlStack.Push(type);
    }
    

private float longPressThreshold = 0.5f; 
private float touchStartTime = 0f;
private bool isLongPress = false;

private void Update() {
    if(StageManager.isStageInputBlocked) return;

    if (currentInputHardware == IngameInputHardWare.Mouse)
    {
        bool isDownButton0 = Input.GetMouseButtonDown(0); // 좌 클릭

        if(EventSystem.current.IsPointerOverGameObject()) return;

        if(isDownButton0)
        {
            EventManager.instance.SetFocusEvent?.Invoke(Input.mousePosition);
        }

    }

    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);

        if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            Vector3 touchPosition = touch.position;
            EventManager.instance.SetFocusEvent?.Invoke(touchPosition);
        }
    }
}


    private void OnEnable() {
        delegateInputFunctions();
    }

    private void OnDisable() {
        removeInputFunctions();
    }

    public void delegateInputFunctions()
    {

    }

    public void removeInputFunctions()
    {

    }

    #region moveInputFunctions

    #endregion


}
