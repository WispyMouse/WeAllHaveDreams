using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapEditorBootup : MonoBehaviour
{
    private async Task Start()
    {
        DebugTextLog.AddTextToLog("Press P to enter Play mode", DebugTextLogChannel.DebugOperationInputInstructions);

        AsyncOperation libraryHandle = SceneManager.LoadSceneAsync("Library", LoadSceneMode.Additive);

        while (!libraryHandle.isDone)
        {
            await Task.Delay(1);
        }

        AsyncOperation contextHandle = SceneManager.LoadSceneAsync("WorldContext", LoadSceneMode.Additive);

        while (!contextHandle.isDone)
        {
            await Task.Delay(1);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TransitionToGameplay();
        }
    }

    public void TransitionToGameplay()
    {
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }
}
