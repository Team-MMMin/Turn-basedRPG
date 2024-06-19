using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public HashSet<PlayerUnitController> Player { get; } = new HashSet<PlayerUnitController>();
    public HashSet<MonsterController> MonsterController { get; } = new HashSet<MonsterController>();

    #region Roots
    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject() { name = name };

        return root.transform;
    }

    public Transform PlayerUnitRoot { get { return GetRootTransform("@PlayerUnits"); } }
    public Transform MonsterRoot { get { return GetRootTransform("@Monsters"); } }
    #endregion

    // TODO
    // 오브젝트 스폰
    // 디스폰
}
