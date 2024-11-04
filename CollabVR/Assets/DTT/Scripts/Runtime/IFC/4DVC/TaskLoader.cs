using Ashvin._4DVC;
using BimViz.IFC4D;
using UnityEngine;
using UnityEngine.Events;

public class TaskLoader : MonoBehaviour
{
    private const int Model6FileId = 36;

    [SerializeField] private UnityEvent<Task[]> _onTasksLoaded = new();

    [ContextMenu("LoadTasks")]
    public void LoadTasks()
    {
        var taskNeo4jLoader = new IfcTaskLoaderNeo4j(Model6FileId);

        StartCoroutine(taskNeo4jLoader.LoadData(OnTasksLoaded));
    }

    private void OnTasksLoaded(Task[] tasks)
    {
        _onTasksLoaded?.Invoke(tasks);
    }
}