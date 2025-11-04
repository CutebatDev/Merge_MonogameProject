using System.Collections.Generic;

namespace Merge_MonogameProject;

public class ObjectPoolManager
{
    private static Dictionary<IPoolable, Stack<IPoolable>> _pools = new();

    /*
     Initialize pool
     Get from pool
     Return to pool
     */
    
}