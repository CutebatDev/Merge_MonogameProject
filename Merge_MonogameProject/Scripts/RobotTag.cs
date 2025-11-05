// Economy/RobotTag.cs
namespace Merge_MonogameProject
{
    // Purpose: when a robot exists, it registers with the economy; when removed, it unregisters.
    public class RobotTag
    {
        // which level this robot is
        public int Level { get; private set; }

        public RobotTag(int level)
        {
            Level = level;
            // tells the economy system that one robot of this level now exists
            EconomyManager.Instance.Register(Level);
        }

        // call this before destroying the robot
        public void Dispose()
        {
            // tells the economy system that one robot of this level is removed
            EconomyManager.Instance.Unregister(Level);
        }
    }
}
