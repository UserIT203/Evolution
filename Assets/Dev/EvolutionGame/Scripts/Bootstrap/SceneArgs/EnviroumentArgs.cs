using System.Collections.Generic;

public class EnviroumentArgs : ISceneArgs
{
    public int LevelIndex;
    public List<LevelSpawnConfig> LevelConfigs;
    public GlobalManager GlobalManager;
    public GameManager GameManager;
    public LevelSetting LevelSetting;

    public EnviroumentArgs(
        int index, 
        List<LevelSpawnConfig> configs, 
        GlobalManager globalManager,
        GameManager gameManager,
        LevelSetting levelSetting
        )
    {
        LevelIndex = index; 
        LevelConfigs = configs;
        GlobalManager = globalManager;
        GameManager = gameManager;
        LevelSetting = levelSetting;
    }
}
