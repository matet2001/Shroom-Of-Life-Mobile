using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStageController : MonoBehaviour
{
    [SerializeField] TutorialManager tutorialManager;
    [SerializeField] float timeBeforeNextStage;

    [SerializeField] GameObject firstStage;
    [SerializeField] GameObject secondStage;
    [SerializeField] GameObject thirdStage;
    [SerializeField] GameObject fourthStage;

    private void Start()
    {
        TryToShowTutorial(firstStage);
        YarnMovementController.OnYarnStart += delegate () { TryToShowTutorial(secondStage); };
        ConnectionManager.OnTreeListChange += delegate (TreeController treeController) { TryToShowTutorial(thirdStage); };
        TreeUIManager.OnFirstUIShow += delegate () { TryToShowTutorial(fourthStage); };
    }
    private IEnumerator NextStageTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        tutorialManager.NextStage();
    }
    private void TryToShowTutorial(GameObject stage)
    {
        if (tutorialManager.GetNextStage() == stage)
            StartCoroutine(NextStageTimer(timeBeforeNextStage));
    }
}
