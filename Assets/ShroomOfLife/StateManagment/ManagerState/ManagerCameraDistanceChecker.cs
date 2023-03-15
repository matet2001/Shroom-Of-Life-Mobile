using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerCameraDistanceChecker : MonoBehaviour
{
    [SerializeField] float treeUIShowAngleDifference;
    
    private List<TreeUIManager> treeUIList;

    public Transform cameraContainerTransform { private get; set; }

    private void Awake()
    {
        ConnectionManager.OnTreeListChange += ConnectionManager_OnTreeListChange;
    }
    private void ConnectionManager_OnTreeListChange(List<TreeController> list)
    {
        treeUIList = new List<TreeUIManager>();
        
        foreach (TreeController treeController in list)
        {
            treeUIList.Add(treeController.GetTreeUIManager());
        }
    }
    public void CheckDistanceFromTrees()
    {        
        float containerToCameraAngle = Vector2.SignedAngle(cameraContainerTransform.up, cameraContainerTransform.up);
        
        foreach (TreeUIManager treeUI in treeUIList)
        {
            Vector2 containerToTreeVector = treeUI.transform.position - cameraContainerTransform.position;
            float containerToTreeAngle = Vector2.SignedAngle(containerToTreeVector, cameraContainerTransform.up);
            bool shouldShowUI = Mathf.Abs(containerToCameraAngle - containerToTreeAngle) < treeUIShowAngleDifference;

            if (treeUI.isUIVisible == shouldShowUI) continue;
            treeUI.SetUIActive(shouldShowUI);
        }
    }
}
