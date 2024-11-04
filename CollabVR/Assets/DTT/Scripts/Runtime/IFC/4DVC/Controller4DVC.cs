// Copyright (c) 2024  DigitalTwin Technology GmbH
// https://www.digitaltwin.technology/

using System;
using System.Collections.Generic;
using System.Linq;
using Ashvin._4DVC;
using BimViz.Highlight;
using UnityEngine;
using Guid = BimViz.Guid;

namespace DTT.Scripts.Runtime.IFC._4DVC
{
    public class Controller4DVC : MonoBehaviour
    {
        [SerializeField] private HighlightGameObjectManager highlightManager;
        [SerializeField] private HighlightData highlightData;
        private readonly Dictionary<Task, List<MeshRenderer>> MeshRenderersOfObjects = new();
        private GameObject _model;

        private Dictionary<string, GameObject> _modelDictionary;

        private Task[] _tasks;

        public void SetModelRoot(GameObject root)
        {
            _model = root;
            _modelDictionary = new Dictionary<string, GameObject>();
            foreach(Transform child in _model.transform.GetChild(0))
            {
                try
                {
                    var id = Guid.MergeAndCompress(child.name);
                    _modelDictionary.TryAdd(id, child.gameObject);
                }
                catch(Exception e)
                {
                    _modelDictionary.TryAdd(child.name, child.gameObject);
                }
            }
        }

        public void LoadTasks(Task[] tasks)
        {
            _tasks = tasks;
            Debug.Log($"{tasks.Length} tasks loaded in 4DVC");

            CalculateMinMaxTasksTime(tasks, out var min, out var max);

            foreach(var task in _tasks)
            {
                ConfigureTask(task, min, max);
            }
        }

        private void CalculateMinMaxTasksTime(Task[] tasks, out long min, out long max)
        {
            min = tasks.Min(x => x.FirstStartTime.Ticks);

            max = 0;

            {
                List<Task> tasksNoLastTime = new();

                for(var i = 0; i < tasks.Length; ++i)
                {
                    var task = tasks[i];

                    //TODO: DateTime?
                    if(task.LastFinishTime == DateTime.MaxValue)
                    {
                        tasksNoLastTime.Add(task);
                    }
                    else
                    {
                        if(max < task.LastFinishTime.Ticks)
                        {
                            max = task.LastFinishTime.Ticks;
                        }
                    }

                    if(max < task.FirstStartTime.Ticks)
                    {
                        //TODO: Así no, imagine that the timeline is in just one day
                        max = new DateTime(task.FirstStartTime.Ticks).AddDays(1).Ticks;
                    }
                }

                foreach(var task in tasksNoLastTime)
                {
                    task.LastFinishTime = new DateTime(max);
                }
            }

            if(min == 0)
            {
                Debug.LogWarning("No start time found");
            }

            if(max == 0)
            {
                Debug.LogWarning("No end time found");
            }
        }

        public void UpdateTime(float t)
        {
            foreach(var data in _tasks)
            {
                HandleTask(t, data);
            }
        }

        private void HandleTask(float t, Task task)
        {
            if(task == null || MeshRenderersOfObjects == null)
            {
                return;
            }

            if(MeshRenderersOfObjects.TryGetValue(task, out var objects))
            {
                HandleObjectsTask(t, task, objects);
            }

            if(task.Children != null)
            {
                foreach(var child in task.Children)
                {
                    HandleTask(t, child);
                }
            }
        }

        private void HandleObjectsTask(float t, Task data, List<MeshRenderer> objects)
        {
            if(objects.Count != 0)
            {
                HandleObjects(t, data.NormalizedStart, data.NormalizedEnd, objects);
            }
        }

        protected virtual void HandleObjects(float t, float start, float finish, List<MeshRenderer> objects)
        {
            if(t < start)
            {
                foreach(var meshRenderer in objects)
                {
                    meshRenderer.enabled = false;
                    highlightManager.ResetSelection(meshRenderer.gameObject);
                }
            }
            else if(start < t && t < finish)
            {
                foreach(var meshRenderer in objects)
                {
                    meshRenderer.enabled = true;

                    highlightManager.AddToMultipleSelection(
                        meshRenderer.gameObject,
                        highlightData);
                }
            }
            else if(finish <= t)
            {
                foreach(var meshRenderer in objects)
                {
                    meshRenderer.enabled = true;
                    highlightManager.ResetSelection(meshRenderer.gameObject);
                }
            }
        }

        private void ConfigureTask(Task d, long min, long max)
        {
            if(d.ActualStart.HasValue)
            {
                d.NormalizedStart = (float)Normalize(min, max, d.ActualStart.Value.Ticks);
            }
            else if(d.FirstStartTime.Ticks != 0)
            {
                d.NormalizedStart = (float)Normalize(min, max, d.FirstStartTime.Ticks);
            }

            if(d.ActualFinish.HasValue)
            {
                d.NormalizedEnd = (float)Normalize(min, max, d.ActualFinish.Value.Ticks);
            }
            else if(d.LastFinishTime.Ticks != 0)
            {
                d.NormalizedEnd = (float)Normalize(min, max, d.LastFinishTime.Ticks);
            }

            if(d.ScheduleStart.HasValue)
            {
                d.NormalizedScheduleStart =
                    (float)Normalize(min, max, d.ScheduleStart.Value.Ticks);
            }

            if(d.ScheduleFinish.HasValue)
            {
                d.NormalizedScheduleEnd =
                    (float)Normalize(min, max, d.ScheduleFinish.Value.Ticks);
            }

            if(d is ObjectTask task)
            {
                LoadIdObjects(task);
            }

            if(d.Children == null)
            {
                return;
            }

            foreach(var child in d.Children)
            {
                ConfigureTask(child, min, max);
            }
        }

        protected virtual void LoadIdObjects(ObjectTask d)
        {
            if(d.Ids == null || d.Ids.Length == 0)
            {
                return;
            }

            List<MeshRenderer> meshRenderers = new();

            foreach(var id in d.Ids)
            {
                var taskGameObject = SearchById(id);

                if(taskGameObject != null &&
                   taskGameObject.TryGetComponent(out MeshRenderer meshRenderer))
                {
                    meshRenderers.Add(meshRenderer);
                }
            }

            MeshRenderersOfObjects.Add(d, meshRenderers);
        }

        private GameObject SearchById(string id)
        {
            return _modelDictionary.GetValueOrDefault(id);
        }

        public static double Normalize(long min, long max, long value)
        {
            return (double)(value - min) / (max - min);
        }
    }
}