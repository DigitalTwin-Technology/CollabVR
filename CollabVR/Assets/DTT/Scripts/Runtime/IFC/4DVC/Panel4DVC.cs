using System.Collections;
using System.Collections.Generic;
using Ashvin._4DVC;
using DTT.Scripts.Runtime.IFC._4DVC;
using UnityEngine;
using UnityEngine.UI;

public class Panel4DVC : MonoBehaviour
{
    [SerializeField] private Transform _taskContainer;
    [SerializeField] private ITaskDisplayer _taskPrefab;
    [SerializeField] private Slider _slider;
    public void LoadTasks(IEnumerable<Task> tasks)
    {
        foreach(var task in tasks)
        {
            CreateTask(task);
        }
    }

    private void CreateTask(Task task)
    {
        var taskObject = Instantiate(_taskPrefab, _taskContainer);
        taskObject.Set(TaskToTaskParameters(task));
        Button button = taskObject.GetButton( );
        button.onClick.AddListener( ()=> UpdateSlider( task.NormalizedStart ) );
    }

    private TaskDisplayerParameters TaskToTaskParameters(Task task)
    {
        return new TaskDisplayerParameters()
        {
            Name = task.Name,
            NormalizedStart = task.NormalizedStart,
            NormalizedEnd = task.NormalizedEnd,
        };
    }

    private void UpdateSlider(float time)
    {
        _slider.value = time;
    }
}
