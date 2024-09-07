using TMPro;
using UnityEngine;

public class InGameUIAniimation : MonoBehaviour
{
    public TextMeshProUGUI gameOverReason;

    
    [Header("Restart Button")]
    public GameObject restartButton;

    Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        EventManager.instance.Set_UI_Filter_Event += Set_UI_Filter;
        EventManager.instance.Game_Over_Event += GameOverAnimation;
        EventManager.instance.ObtainBigItemEvent += Set_BIG_ITEM_UI_Filter;
        EventManager.instance.StageClearEvent += Game_Clear;

    }

    private void OnDisable() {
        EventManager.instance.Set_UI_Filter_Event -= Set_UI_Filter;
        EventManager.instance.Game_Over_Event -= GameOverAnimation;
        EventManager.instance.ObtainBigItemEvent -= Set_BIG_ITEM_UI_Filter;
        EventManager.instance.StageClearEvent -= Game_Clear;
    }

    bool isShowingMinimap = false;
    public void ShowMinimap()
    {
        if(StageManager.isNowInitializing) return;
        if(EventManager.isAnimationPlaying) return;

        if(!isShowingMinimap)
        {
            StageManager.stageInputBlock++;
            isShowingMinimap = true;
            animator.SetTrigger("Minimap Show");
        }else
        {
            StageManager.stageInputBlock--;
            isShowingMinimap = false;
            animator.SetTrigger("Minimap Close");
        }

    }

    bool isShowingMenu = false;
    public void ShowMenu()
    {
        if(StageManager.isNowInitializing) return;
        if(EventManager.isAnimationPlaying) return;

        if(!isShowingMenu)
        {
            StageManager.stageInputBlock++;
            isShowingMenu = true;
            animator.SetTrigger("Menu Show");
        }else
        {
            StageManager.stageInputBlock--;
            isShowingMenu = false;
            animator.SetTrigger("Menu Close");
        }

    }

    
    bool isShowingTutorial = false;
    public void ShowTutorial()
    {
        if(StageManager.isNowInitializing) return;
        if(EventManager.isAnimationPlaying) return;

        if(!isShowingTutorial)
        {
            StageManager.stageInputBlock++;
            isShowingTutorial = true;
            isShowingOption = false;
            animator.SetTrigger("Tutorial Show");
        }else
        {
            StageManager.stageInputBlock--;
            isShowingTutorial = false;
            animator.SetTrigger("Tutorial Close");
        }

    }

    bool isShowingOption = false;
    public void ShowOption()
    {
        if(StageManager.isNowInitializing) return;
        if(EventManager.isAnimationPlaying) return;

        if(!isShowingOption)
        {
            StageManager.stageInputBlock++;
            isShowingOption = true;
            isShowingTutorial = false;
            animator.SetTrigger("Option Show");
        }else
        {
            StageManager.stageInputBlock--;
            isShowingOption = false;
            animator.SetTrigger("Option Close");
        }

    }

    public void GameOverAnimation(bool isGameOver, GameOver_Reason reason){

        restartButton.SetActive(true);
        if(isGameOver)
        {
            switch(reason)
            {
                case GameOver_Reason.Heart0 :
                    gameOverReason.text = "HP ZERO";
                    gameOverReason.color = Color.red;
                    animator.SetFloat("isTimeOver", 0);
                    break;
                case GameOver_Reason.TreasureCrash :
                    gameOverReason.text = "Treasure Crash";
                    gameOverReason.color = Color.yellow;
                    animator.SetFloat("isTimeOver", 0);
                    break;
                case GameOver_Reason.TimeOver :
                    gameOverReason.text = "Time Over";
                    gameOverReason.color = Color.green;
                    animator.SetFloat("isTimeOver", 1);
                    break;
            }
            
            animator.SetTrigger("Game Over");
        }else
        {
            animator.SetTrigger("Restart");
        }
        
    }

    public void SetAnimationPlayingFlag(int flag)
    {
        if(flag == 0)
        {
            EventManager.isAnimationPlaying = false;
        }else
        {
            EventManager.isAnimationPlaying = true;
        }
        
    }

    public void SetItem_Use_Obtain_Flag(Item item, int count)
    {
        animator.SetFloat("itemCount", count);
        animator.SetFloat("ItemNum", (int)item - 1);
    }

    public void Set_Item_Use(bool show)
    {
        if(show)
        {
            animator.SetTrigger("ShowItemPanel");
        }else
        {
            animator.SetTrigger("CloseItemPanel");
        }
        
    }

    private void Set_BIG_ITEM_UI_Filter()
    {
        animator.SetTrigger("Big Treasure Find");
    }

    private void Game_Clear()
    {        
        animator.SetTrigger("Stage Clear");
    }


    private void Set_UI_Filter(EventType eventType)
    {
        if(StageManager.isNowInitializing) return;

        switch(eventType)
        {
            case EventType.MineAppear :
                animator.SetTrigger("Mine Find");
                break;
            case EventType.MineDisappear :
                animator.SetTrigger("Mine Destroy");
                break;
            case EventType.TreasureAppear :
                animator.SetTrigger("Trea Find");
                break;
            case EventType.TreasureDisappear :
                animator.SetTrigger("Trea Destroy");
                break;
        }
    }
}
