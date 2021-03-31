using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoadMapDialog : MonoBehaviour
{
    public LoadMapMapNameButton ButtonPF;
    List<LoadMapMapNameButton> ActiveButtons { get; set; } = new List<LoadMapMapNameButton>();
    Coroutine ActiveLoading { get; set; }

    public Transform LoadableMapsList;
    public MapEditorFileManagement MapEditorFileManagementInstance;
    public MapEditorBootup MapEditorBootupInstance;

    Realm SelectedRealm { get; set; }

    public void Open()
    {
        if (gameObject.activeInHierarchy)
        {
            Close();
        }

        gameObject.SetActive(true);
        StartCoroutine(ProcessOpening());
    }

    IEnumerator ProcessOpening()
    {
        Task<List<Realm>> realmsTask = Task.Run(MapEditorFileManagementInstance.GetAllRealms);
        while (!realmsTask.IsCompleted)
        {
            yield return new WaitForEndOfFrame();
        }

        foreach (Realm curRealm in realmsTask.Result)
        {
            LoadMapMapNameButton newButton = Instantiate(ButtonPF, LoadableMapsList);
            newButton.SetRealm(curRealm, SelectRealm);
            ActiveButtons.Add(newButton);
        }
    }

    public void Close()
    {
        if (ActiveLoading != null)
        {
            StopCoroutine(ActiveLoading);
        }

        foreach (LoadMapMapNameButton button in ActiveButtons)
        {
            button.gameObject.SetActive(false);

            // TODO: Get these in an Object pool, properly; it's throwing an error about drawing deleted RectTransforms
            // Destroy(button.gameObject);
        }
        ActiveButtons = new List<LoadMapMapNameButton>();

        gameObject.SetActive(false);
    }

    public void SelectRealm(Realm toSelect)
    {
        SelectedRealm = toSelect;
    }

    public void Load()
    {
        StartCoroutine(ProcessLoading());
    }

    IEnumerator ProcessLoading()
    {
        yield return MapEditorBootupInstance.LoadRealm(SelectedRealm);
        Close();
    }
}
