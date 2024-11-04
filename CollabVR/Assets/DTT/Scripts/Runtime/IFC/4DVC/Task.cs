using System;
using Newtonsoft.Json;

namespace Ashvin._4DVC
{

public class Task
{
    public string Name;

    public Task Parent;
    public Task[] Children;

    public DateTime? ScheduleStart;
    public DateTime? ScheduleFinish;

    public DateTime? EarlyStart;
    public DateTime? EarlyFinish;

    public DateTime? LateStart;
    public DateTime? LateFinish;

    public DateTime? ActualStart;
    public DateTime? ActualFinish;

    [JsonProperty( "Start Time" )]
    public DateTime FirstStartTime;

    [JsonProperty( "End Time" )]
    public DateTime LastFinishTime = DateTime.MaxValue;

    // TODO: Remove or maybe better to remove DateTimes as Timeline has only to handle it as a range from 0 to 1 ===============================================

    public float NormalizedStart { get; set; } = -1f;

    public float NormalizedEnd { get; set; } = -1f;

    public float NormalizedScheduleEnd { get; set; } = -1f;

    public float NormalizedScheduleStart { get; set; } = -1f;

    public bool HasEnd { get; set; } = true;

    #region Public

    public virtual void SetActive( bool enable )
    {
    }

    #endregion
}

}
