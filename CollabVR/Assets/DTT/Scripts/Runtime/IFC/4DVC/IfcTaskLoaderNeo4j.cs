using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ashvin._4DVC;
using Ashvin.Neo4j;
using DTT.Scripts.Runtime.Neo4j;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace BimViz.IFC4D
{

public class IfcTaskLoaderNeo4j
{
    private const string Query = @"MATCH (t:IfcTask {file_id: $fileId})
OPTIONAL MATCH (t)-[:IsNestedBy]->(nt)
OPTIONAL MATCH (t)-[:hasTaskTime]->(tt:IfcTaskTime)
OPTIONAL MATCH (t)-[:OperatesOn]->(obj)
WITH t, properties(t) AS TaskProperties, properties(tt) AS TaskTime, ID(t) AS TaskId, COLLECT(ID(nt)) AS Childs, COLLECT(obj.GlobalId) as Objects
RETURN COLLECT(DISTINCT t {TaskProperties, TaskId, TaskTime: TaskTime, Childs: Childs, OperatesOn: Objects}) AS Result";

    private readonly int _fileId;
    
    public IfcTaskLoaderNeo4j(int fileId)
    {
        _fileId = fileId;
    }

    public IEnumerator LoadData( UnityAction < Task[] > onFinish )
    {
        using UnityWebRequest request = Neo4jManager.Instance.GetQueryRequest(
            new[]
            {
                new Statement
                {
                    statement = Query,
                    parameters = new Dictionary < string, dynamic > { { "fileId", _fileId } }
                }
            } );

        yield return request.SendWebRequest();

        if ( request.result != UnityWebRequest.Result.Success )
        {
            Debug.LogError( request.url + " " + request.error );

            yield break;
        }

        Neo4jResult response = Neo4jManager.ParseResult( request.downloadHandler.text );

        request.Dispose();

        if ( response.Errors.Length > 0 )
        {
            Debug.LogError(
                "Response contains errors: " +
                string.Join( ", ", response.Errors.Select( e => $"Code: {e.Code} | Message: {e.Message}" ) ) );

            yield break;
        }

        Neo4jTask[] tasks = JsonConvert.DeserializeObject < Neo4jTask[] >( response.Results[0].Data[0].Row[0] );

        Dictionary < int, Neo4jTask > taskDictionary = tasks.ToDictionary( t => t.TaskId );

        foreach ( Neo4jTask task in tasks )
        {
            task.NestedTasks = new List < Neo4jTask >();

            foreach ( int childId in task.Childs )
            {
                Neo4jTask childTask = taskDictionary[childId];
                task.NestedTasks.Add( childTask );
                childTask.Parent = task;
            }
        }

        // Only parent tasks
        tasks = tasks.Where( t => t.Parent == null ).ToArray();

        UpdateTaskTimes( tasks );

        Task[] timelineTasks = Neo4jTaskToTimelineTask( tasks ).OrderBy( t => t.FirstStartTime ).ToArray();

        onFinish?.Invoke( timelineTasks );
    }

    // Some child tasks does not contain the TaskTime so we need to update them with their parent's TaskTime
    private static void UpdateTaskTimes( Neo4jTask[] tasks )
    {
        void CheckTaskTime( Neo4jTask task )
        {
            if ( task.TaskTime == null )
            {
                task.TaskTime = task.Parent.TaskTime;
            }

            foreach ( Neo4jTask childTask in task.NestedTasks )
            {
                CheckTaskTime( childTask );
            }
        }

        foreach ( Neo4jTask task in tasks )
        {
            CheckTaskTime( task );
        }
    }

    private static IEnumerable < Task > Neo4jTaskToTimelineTask( IReadOnlyList < Neo4jTask > tasks )
    {
        if ( tasks.Count == 0 )
        {
            return null;
        }

        List < Task > timelineTasks = new();
        timelineTasks.AddRange( from task in tasks select Neo4JTaskToTimelineData( task, null ) );

        return timelineTasks.ToArray();
    }

    private static Task Neo4JTaskToTimelineData(Neo4jTask task, Task parent)
    {
        ObjectTask data = new()
        {
            Name = task.Properties.Name,
            Ids = task.OperatesOn.Select( Guid.MergeAndCompress ).ToArray(),
            Parent = parent
        };

        data.Children = task.NestedTasks.Select<Neo4jTask, Task>( t => Neo4JTaskToTimelineData( t, data ) ).ToArray();

        if ( task.TaskTime == null )
        {
            return data;
        }

        List < DateTime > firstStartTime = new();
        List < DateTime > lastStartTime = new();

        if ( task.TaskTime.ActualStart.HasValue )
        {
            data.ActualStart = task.TaskTime.ActualStart.Value;
            firstStartTime.Add( data.ActualStart.Value );
        }

        if ( task.TaskTime.ActualFinish.HasValue )
        {
            data.ActualFinish = task.TaskTime.ActualFinish.Value;
            lastStartTime.Add( data.ActualFinish.Value );
        }

        if ( task.TaskTime.ScheduleStart.HasValue )
        {
            data.ScheduleStart = task.TaskTime.ScheduleStart.Value;
            firstStartTime.Add( data.ScheduleStart.Value );
        }

        if ( task.TaskTime.ScheduleFinish.HasValue )
        {
            data.ScheduleFinish = task.TaskTime.ScheduleFinish.Value;
            lastStartTime.Add( data.ScheduleFinish.Value );
        }

        if ( task.TaskTime.LateStart.HasValue )
        {
            data.LateStart = task.TaskTime.LateStart.Value;
            firstStartTime.Add( data.LateStart.Value );
        }

        if ( task.TaskTime.LateFinish.HasValue )
        {
            data.LateFinish = task.TaskTime.LateFinish.Value;
            lastStartTime.Add( data.LateFinish.Value );
        }

        if ( task.TaskTime.EarlyStart.HasValue )
        {
            data.EarlyStart = task.TaskTime.EarlyStart.Value;
            firstStartTime.Add( data.EarlyStart.Value );
        }

        if ( task.TaskTime.EarlyFinish.HasValue )
        {
            data.EarlyFinish = task.TaskTime.EarlyFinish.Value;
            lastStartTime.Add( data.EarlyFinish.Value );
        }

        if ( firstStartTime.Count > 0 )
        {
            data.FirstStartTime = firstStartTime.Min();
        }

        if ( lastStartTime.Count > 0 )
        {
            data.LastFinishTime = lastStartTime.Max();
        }

        return data;
    }

    public class Neo4jTask
    {
        public class IfcTaskTime
        {
            public DateTime? ScheduleStart;
            public DateTime? ScheduleFinish;
            public DateTime? ActualStart;
            public DateTime? ActualFinish;
            public DateTime? EarlyStart;
            public DateTime? EarlyFinish;
            public DateTime? LateStart;
            public DateTime? LateFinish;

            public string DataOrigin;
            public string ScheduleDuration;
            public string ActualDuration;
            public string GlobalId;

            public DateTime? Finish
            {
                get
                {
                    DateTime? result = null;

                    if ( ActualFinish.HasValue )
                    {
                        result = ActualFinish;
                    }

                    if ( !ScheduleFinish.HasValue )
                    {
                        return result;
                    }

                    if ( result.HasValue )
                    {
                        if ( ScheduleFinish.Value > result.Value )
                        {
                            result = ScheduleFinish;
                        }
                    }
                    else
                    {
                        result = ScheduleFinish;
                    }

                    return result;
                }
            }

            #region Unity Event Functions

            public DateTime? Start
            {
                get
                {
                    DateTime? result = null;

                    if ( ActualStart.HasValue )
                    {
                        result = ActualStart;
                    }

                    if ( !ScheduleStart.HasValue )
                    {
                        return result;
                    }

                    if ( result.HasValue )
                    {
                        if ( ScheduleStart.Value < result.Value )
                        {
                            result = ScheduleStart;
                        }
                    }
                    else
                    {
                        result = ScheduleStart;
                    }

                    return result;
                }
            }

            #endregion
        }

        public class TaskProperties
        {
            public float StandardWork;
            public string Identification;
            public bool IsMilstone;
            public string PredefinedType;
            public string Description;
            public string WorkMethod;
            public string ObjectType;
            public string Status;
            public string Name;
            public string GlobalId;
        }

        public int TaskId;
        public int[] Childs;

        [JsonProperty( "TaskProperties" )]
        public TaskProperties Properties;

        public IfcTaskTime TaskTime;

        public string[] OperatesOn;

        public List < Neo4jTask > NestedTasks;
        public Neo4jTask Parent;
    }
}

}
