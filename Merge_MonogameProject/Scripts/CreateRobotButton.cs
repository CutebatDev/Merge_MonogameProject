namespace Merge_MonogameProject;

public class CreateRobotButton : UiButton
{
    public override void OnClick()
    {
        base.OnClick();
        RobotSpawnManager.SpawnRobot();
        
    }
}