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

    bool input2Ok = false;

    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);
        
        if (touch.phase == TouchPhase.Began)
        {
            touchStartTime = Time.time; // 터치 시작 시간 기록
            isLongPress = false; // 초기화

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;

            if (StageManager.isNowInputtingItem)
            {
                input2Ok = true;
                StageManager.instance?.ItemPanelShow(false);
                return;
            }

            if (touch.tapCount == 1)
            {
                StageManager.instance?.MoveOrShovelOrInteract(false); // 좌클릭에 해당하는 기능
            }
            else if (touch.tapCount == 2)
            {
                StageManager.instance?.SetFlag(); // 우클릭에 해당하는 기능
            }
        }
        // 터치가 고정된 상태일 때 길게 누르기 감지
        else if (touch.phase == TouchPhase.Stationary)
        {
            if (Time.time - touchStartTime > longPressThreshold)
            {
                if (!isLongPress)
                {
                    isLongPress = true; // 한 번만 실행되도록
                    StageManager.instance?.ItemPanelShow(true); // 길게 누르기로 아이템 패널 표시
                }
            }
        }
        // 터치가 끝났을 때
        else if (touch.phase == TouchPhase.Ended)
        {
            if (input2Ok) return;

            if (!isLongPress)
            {
                StageManager.instance?.ItemPanelShow(true); // 마우스 휠에 해당하는 기능
            }

            isLongPress = false; // 길게 누르기 초기화
            touchStartTime = 0f; // 터치 시간 초기화
        }
    }

    input2Ok = false;

    if (currentInputHardware == IngameInputHardWare.Mouse)
    {
        bool isDownButton0 = Input.GetMouseButtonDown(0); // 좌 클릭
        bool isDownButton1 = Input.GetMouseButtonDown(1); // 우 클릭
        bool isDownButton2 = Input.GetMouseButtonDown(2); // 마우스 휠

        if(isDownButton2)
        {
            if(StageManager.isNowInputtingItem)
            {
                input2Ok = true;
                StageManager.instance?.ItemPanelShow(false);
            }
        }

        if(EventSystem.current.IsPointerOverGameObject()) return;

        if(isDownButton0)
        {
            StageManager.instance?.MoveOrShovelOrInteract(false);
        }

        if(isDownButton1)
        {
            StageManager.instance?.SetFlag();
        }
        else if(isDownButton2)
        {
            if(input2Ok) return;

            StageManager.instance?.ItemPanelShow(true);
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
