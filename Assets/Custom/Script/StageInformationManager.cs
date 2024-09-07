using UnityEngine;

public class StageInformationManager
{
    public static float easyMineRatio = 0.15f;
    public static float normalMineRatio = 0.18f;
    public static float hardMineRatio = 0.23f;
    public static float professionalMineRatio = 0.28f;


    // 이건 stage 모드에서의 기본 제공 돋보기와 시간
    public static int[,] StageMagItemAmount = {
        {10, 12, 14},
        {20, 25, 30},
        {40, 45, 50},
    };
    public static int[,] StageModeTime = {
        {350, 300, 250},
        {400, 350, 300},
        {500, 450, 400},
    };


    public static float[] StageModemineToTreasureRatio = {0.4f, 0.35f, 0.3f};
    public static int[] StageModestageHeight = {10, 18, 23};
    public static int[] StageModestageWidth = {15, 22, 35};
    public static float[] noItemRatio = {0.05f,0.07f,0.1f};


    public static int[] DefaultTrapDamage = {1, 1, 1};


    private static int MaxHeartDefault = 3;
    private static int CurrentHeartDefault = 3;


    public static int currentStagetype = 0;
    public static int currentStageIndex = 0;
    public static Difficulty difficulty = Difficulty.Normal;


    private static int NextmaxHeart = -1;  
    private static int NextcurrentHeart = -1; 
    public static int[] getHearts()
    {
        int[] ints = new int[] {
            NextmaxHeart,
            NextcurrentHeart, 
        };

        return ints;
    }

    public static void setHearts(int maxHeart = -1, int currentHeart = -1)
    {
        if(maxHeart < 0) // 만약 인수 안 준 경우 -> 기본 값으로 초기화
        {
            NextmaxHeart = MaxHeartDefault;  
            NextcurrentHeart = CurrentHeartDefault; 
        }else // 실제 값이 들어오면 -> 그 값으로 초기화
        {
            NextmaxHeart = maxHeart;  
            NextcurrentHeart = currentHeart; 
        }
    }
    private static int NextpotionCount = -1; 
    private static int NextmagGlassCount = -1; 
    private static int NextholyWaterCount = -1;
    public static int[] getUsableItems()
    {
        int[] ints = new int[] {
            NextpotionCount,
            NextmagGlassCount, 
            NextholyWaterCount
        };

        return ints;
    }

    public static void setUsableItems(int potionCount = -1, int magGlassCount = -1 , int holyWaterCount = -1)
    {
        if(potionCount < 0) // 만약 인수 안 준 경우 -> 기본 값으로 초기화
        {
            NextpotionCount = 0;
            NextmagGlassCount = 0;
            NextholyWaterCount = 0;
        }else // 실제 값이 들어오면 -> 그 값으로 초기화
        {
            NextpotionCount = potionCount;
            NextmagGlassCount = magGlassCount;
            NextholyWaterCount = holyWaterCount;
        }
    }

    public static int NexttotalTime = -1;

    public static void SetDataInitialState()
    {
        currentStageIndex = 0;
        NextmaxHeart = -1;  
        NextcurrentHeart = -1; 
        NextpotionCount = -1;
        NextmagGlassCount = -1;
        NextholyWaterCount = -1;
    }
}
