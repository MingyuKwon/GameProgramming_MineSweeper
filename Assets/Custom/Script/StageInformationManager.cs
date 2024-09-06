using UnityEngine;

public class StageInformationManager
{
    public static float easyMineRatio = 0.15f;
    public static float normalMineRatio = 0.18f;
    public static float hardMineRatio = 0.23f;
    public static float professionalMineRatio = 0.28f;
    public static float mineToTreasureRatio = 0.5f;

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
    

    public static float[] noItemRatio = {0.1f,0.2f,0.3f};

    public static int[] tutorialWidth = {5,5,7,9};
    public static int[] tutorialHeight = {5,5,7,9};

    public static int[,] tutorial1Stageinform = {
                        {0,0,0,-1,-2},
                        {0,1,1,1,0},
                        {0,1,1,1,0},
                        {-1,1,1,1,0},
                        {-2,0,0,-2,0}, 
                    };

    public static int[,] tutorial2Stageinform = {
                        {-2,-1,0,-1,-1},
                        {-2,1,1,1,-1},
                        {-1,1,1,1,-1},
                        {-1,1,1,1,-1},
                        {-2,0,0,-2,-2}, 
                    };
    public static int[,] tutorial3Stageinform = {
                        {-1,-2,-1,0,0,0,-1},
                        {-2,0,0,0,-2,-2,0},
                        {-2,-1,1,1,1,-2,0},
                        {0,0,1,1,1,0,0},
                        {-1,0,1,1,1,0,-1},
                        {-1,0,0,0,-2,0,0},
                        {-1,-2,0,0,0,-2,0},
                    };

    public static int[] tutorialmineCount = {2,8,8};
    public static int[] tutorialtreasureCount = {3,5,9};

    public static int[,] stageWidthMin = 
    {
        {8 , 9, 10, 11, 12,13},
        {15, 16, 17, 18, 19,20},
        {28 , 29, 30, 31, 32,33},
    };
    public static int[,] stageWidthMax = 
    {
        {10 , 11, 12, 13, 14,15},
        {17 , 18, 19, 20, 21,22},
        {30 , 31, 32, 33, 34,35},
    };
    
    public static int[,] stageHeightMin = {
        {8 , 8, 8, 8, 8,8},
        {14 , 14, 15, 15, 16,16},
        {16 , 16, 16, 16, 17,17},
    };
    public static int[,] stageHeightMax = {
        {9 , 9, 9, 10, 10,10},
        {16 , 16, 16, 17, 17,18},
        {18 , 19, 20, 21, 22,23},
    };


    public static int DefaultTimeforEntireGame = 100;
    
    public static int[] DefaultTrapDamage = {1, 1, 2};

    public static int[] plusPotion_byItem_perStage = {3,2,2};
    public static int[] plusMag_byItem_perStage = {4,4,4};
    public static int[] plusHoly_byItem_perStage = {4,3,3};
    public static int[] plusTime_byItem_perStage = {60,50,40};

    public static int[] plusPotion_Default_perStage = {4,2,0};
    public static int[] plusMag_Default_perStage = {6,7,8};
    public static int[] plusHoly_Default_perStage = {4,2,0};
    public static int[] DefaultTimeperStage = {150, 200, 250};

    public static float[] item_obtain_Up_Percentage = {0.3f,0.3f,0.4f};

    public static int Potion_Default = 2;
    public static int Mag_Default = 2;
    public static int Holy_Default = 5;

    private static int MaxHeartDefault = 3;
    private static int CurrentHeartDefault = 3;

    private static GameModeType gameMode = GameModeType.adventure;
    public static void changeGameMode(GameModeType gM)
    {
        gameMode = gM;
    }

    public static GameModeType getGameMode()
    {
        return gameMode;
    }

    public static void setPlayerData(int[] paras)
    {
        if(paras == null || paras[2] == -1) return;

        currentStagetype = paras[0];
        currentStageIndex = paras[1];
        setHearts(paras[2], paras[3]);
        setUsableItems(paras[4],paras[5],paras[6]);
        EquippedItem.playerEquippedItem[0] = (Item)paras[7]; 
        EquippedItem.playerEquippedItem[1] = (Item)paras[8]; 
        EquippedItem.playerEquippedItem[2] = (Item)paras[9]; 
        EquippedItem.playerEquippedItem[3] = (Item)paras[10]; 
        EquippedItem.playerEquippedItem[4] = (Item)paras[11];
        difficulty = (Difficulty)paras[12];

        NexttotalTime = paras[13];
        NextWidth = paras[14];
        NextHeight = paras[15];
    }

    public static int currentStagetype = 0;
    public static int currentStageIndex = 0;
    public static bool isnextStageDungeon = true;
    public static Difficulty difficulty = Difficulty.Normal;
    public static Vector3Int treasurePosition = new Vector3Int(-4,-1,0);

    public static int NextWidth = -1;  
    public static int NextHeight = -1;  

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
            NextpotionCount = Potion_Default;
            NextmagGlassCount = Mag_Default;
            NextholyWaterCount = Holy_Default;
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
        EquippedItem.ClearEquippedItem();
        currentStageIndex = 0;
        isnextStageDungeon = true;
        NextWidth = -1;  
        NextHeight = -1;  
        NextmaxHeart = -1;  
        NextcurrentHeart = -1; 
        NextpotionCount = -1;
        NextmagGlassCount = -1;
        NextholyWaterCount = -1;

        InputManager.itemLock =false;
        InputManager.shovelLock =false;
        InputManager.flagLock =false;
    }
}
