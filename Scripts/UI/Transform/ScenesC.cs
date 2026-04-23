using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ScenesC : Singleton<ScenesC>
{
    public void TransitionToDestination(TransformPoint transformPoint)
    {
        if (transformPoint == null)
            return;

        string sceneName = transformPoint.sceneName;
        var tag = transformPoint.destinationTag;

        switch (transformPoint.transitionType)
        {
            case TransformPoint.TransitionType.SameScene:
                StartCoroutine(TransitionSameScene(tag));
                break;
            case TransformPoint.TransitionType.DifferentScene:
                StartCoroutine(TransitionDifferentScene(sceneName, tag));
                break;
        }
    }

    IEnumerator TransitionSameScene(TransformDestination.DestinationTag destinationTag)
    {
        yield return null;
        TeleportPlayerToDestination(destinationTag);
    }

    IEnumerator TransitionDifferentScene(string sceneName, TransformDestination.DestinationTag destinationTag)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            yield break;

        var op = SceneManager.LoadSceneAsync(sceneName);
        if (op == null)
            yield break;
        while (!op.isDone)
            yield return null;

        TeleportPlayerToDestination(destinationTag);
    }

    void TeleportPlayerToDestination(TransformDestination.DestinationTag destinationTag)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        var dest = GetDestination(destinationTag);
        if (dest == null)
            return;

        player.transform.SetPositionAndRotation(dest.transform.position, dest.transform.rotation);
    }

    private TransformDestination GetDestination(TransformDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsByType<TransformDestination>(FindObjectsSortMode.None);
        for (int i = 0; i < entrances.Length; i++)
            if (entrances[i].destinationTag == destinationTag)
                return entrances[i];
        return null; 
    }
}
