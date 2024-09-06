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

    #region InputCheck
    public class InputCheck
    {


    }
    #endregion

    public class InputEvent
    {
        static bool isCurrentInput(InputMode type)
        {
            bool flag = false;

            if(inputControlStack.Count == 0)
            {
                return flag;
            }

            if(inputControlStack.Peek() == type)
            {
                flag = true;
            }

            return flag;
        }

        #region Event
        public static event Action<Vector3Int> MovePressEvent;
        public static void Invoke_Move(Vector3Int position)
        {
            MovePressEvent.Invoke(position);
        }

        #endregion
    }

    #region static Field
    public static InputManager instance = null;
    public static Stack<InputMode> inputControlStack = new Stack<InputMode>();
    public static int currentInputRule = 0;
    public static InputCheck inputCheck;
    #endregion
    
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
        changePlayerInputRule();
    }

    private static void changePlayerInputRule(int ruleNum)
    {

    }

    private static void changePlayerInputRule()
    {
        if(inputControlStack.Count == 0)
        {
            return;
        }

        switch(inputControlStack.Peek())
        {
            case InputMode.InGame :
                changePlayerInputRule(0);
                break;
            case InputMode.UI :
                changePlayerInputRule(1);
                break;
        }
    }

    public static bool itemLock = false;
    public static bool flagLock = false;
    public static bool shovelLock = false;
    
    private void Update() {
        if(StageManager.isStageInputBlocked) return;

        bool input2Ok = false;

        bool isDownButton0 = Input.GetMouseButtonDown(0);
        bool isDownButton1 = Input.GetMouseButtonDown(1);
        bool isDownButton2 = Input.GetMouseButtonDown(2);
        bool isDownButton3 = Input.GetMouseButtonDown(3);
        bool isDownButton4 = Input.GetMouseButtonDown(4);


        if(isDownButton2)
        {
            if(StageManager.isNowInputtingItem)
            {
                input2Ok = true;
                StageManager.instance?.ItemPanelShow(false);
            }
            
        }

        if(isDownButton4)
        {
            String str = EquippedItem.playerEquippedItem[0].ToString() + "\n" +
            EquippedItem.playerEquippedItem[1].ToString() + "\n" +
            EquippedItem.playerEquippedItem[2].ToString() + "\n" +
            EquippedItem.playerEquippedItem[3].ToString() + "\n" +
            EquippedItem.playerEquippedItem[4].ToString() + "\n";
            Debug.Log(str);
        
        }

        if(isDownButton3)
        {
            EventManager.instance.StairOpen_Invoke_Event();
        }

        if(EventSystem.current.IsPointerOverGameObject()) return;

        if(isDownButton0)
        {
            StageManager.instance?.MoveOrShovelOrInteract(shovelLock);
        }

        if(isDownButton1 && !flagLock)
        {
            StageManager.instance?.SetFlag();
        }else if(isDownButton2 && !itemLock)
        {
            if(input2Ok) return;

            StageManager.instance?.ItemPanelShow(true);
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
