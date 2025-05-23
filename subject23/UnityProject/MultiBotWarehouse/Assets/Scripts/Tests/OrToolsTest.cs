using UnityEngine;
using Google.OrTools.Sat;
using System.Collections.Generic;
using System.Linq;
using System;

public class OrToolsTest : MonoBehaviour
{
    private class AssignedTask : IComparable
    {
        public int jobID;
        public int taskID;
        public int start;
        public int duration;

        public AssignedTask(int jobID, int taskID, int start, int duration)
        {
            this.jobID = jobID;
            this.taskID = taskID;
            this.start = start;
            this.duration = duration;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            AssignedTask otherTask = obj as AssignedTask;
            if (otherTask != null)
            {
                if (this.start != otherTask.start)
                    return this.start.CompareTo(otherTask.start);
                else
                    return this.duration.CompareTo(otherTask.duration);
            }
            else
                throw new ArgumentException("Object is not a Temperature");
        }
    }

    public void Start()
    {
        var allJobs =
            new[] {
                new[] {
                    // job0
                    new { machine = 0, duration = 3 }, // task0
                    new { machine = 1, duration = 2 }, // task1
                    new { machine = 2, duration = 2 }, // task2
                }
                    .ToList(),
                new[] {
                    // job1
                    new { machine = 0, duration = 2 }, // task0
                    new { machine = 2, duration = 1 }, // task1
                    new { machine = 1, duration = 4 }, // task2
                }
                    .ToList(),
                new[] {
                    // job2
                    new { machine = 1, duration = 4 }, // task0
                    new { machine = 2, duration = 3 }, // task1
                }
                    .ToList(),
            }
                .ToList();

        int numMachines = 0;
        foreach (var job in allJobs)
        {
            foreach (var task in job)
            {
                numMachines = Math.Max(numMachines, 1 + task.machine);
            }
        }
        int[] allMachines = Enumerable.Range(0, numMachines).ToArray();

        // Computes horizon dynamically as the sum of all durations.
        int horizon = 0;
        foreach (var job in allJobs)
        {
            foreach (var task in job)
            {
                horizon += task.duration;
            }
        }

        // Creates the model.
        CpModel model = new CpModel();

        Dictionary<Tuple<int, int>, Tuple<IntVar, IntVar, IntervalVar>> allTasks =
            new Dictionary<Tuple<int, int>, Tuple<IntVar, IntVar, IntervalVar>>(); // (start, end, duration)
        Dictionary<int, List<IntervalVar>> machineToIntervals = new Dictionary<int, List<IntervalVar>>();
        for (int jobID = 0; jobID < allJobs.Count(); ++jobID)
        {
            var job = allJobs[jobID];
            for (int taskID = 0; taskID < job.Count(); ++taskID)
            {
                var task = job[taskID];
                String suffix = $"_{jobID}_{taskID}";
                IntVar start = model.NewIntVar(0, horizon, "start" + suffix);
                IntVar end = model.NewIntVar(0, horizon, "end" + suffix);
                IntervalVar interval = model.NewIntervalVar(start, task.duration, end, "interval" + suffix);
                var key = Tuple.Create(jobID, taskID);
                allTasks[key] = Tuple.Create(start, end, interval);
                if (!machineToIntervals.ContainsKey(task.machine))
                {
                    machineToIntervals.Add(task.machine, new List<IntervalVar>());
                }
                machineToIntervals[task.machine].Add(interval);
            }
        }

        // Create and add disjunctive constraints.
        foreach (int machine in allMachines)
        {
            model.AddNoOverlap(machineToIntervals[machine]);
        }

        // Precedences inside a job.
        for (int jobID = 0; jobID < allJobs.Count(); ++jobID)
        {
            var job = allJobs[jobID];
            for (int taskID = 0; taskID < job.Count() - 1; ++taskID)
            {
                var key = Tuple.Create(jobID, taskID);
                var nextKey = Tuple.Create(jobID, taskID + 1);
                model.Add(allTasks[nextKey].Item1 >= allTasks[key].Item2);
            }
        }

        // Makespan objective.
        IntVar objVar = model.NewIntVar(0, horizon, "makespan");

        List<IntVar> ends = new List<IntVar>();
        for (int jobID = 0; jobID < allJobs.Count(); ++jobID)
        {
            var job = allJobs[jobID];
            var key = Tuple.Create(jobID, job.Count() - 1);
            ends.Add(allTasks[key].Item2);
        }
        model.AddMaxEquality(objVar, ends);
        model.Minimize(objVar);

        // Solve
        CpSolver solver = new CpSolver();
        CpSolverStatus status = solver.Solve(model);
        Debug.Log($"Solve status: {status}");

        if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
        {
            Debug.Log("Solution:");

            Dictionary<int, List<AssignedTask>> assignedJobs = new Dictionary<int, List<AssignedTask>>();
            for (int jobID = 0; jobID < allJobs.Count(); ++jobID)
            {
                var job = allJobs[jobID];
                for (int taskID = 0; taskID < job.Count(); ++taskID)
                {
                    var task = job[taskID];
                    var key = Tuple.Create(jobID, taskID);
                    int start = (int)solver.Value(allTasks[key].Item1);
                    if (!assignedJobs.ContainsKey(task.machine))
                    {
                        assignedJobs.Add(task.machine, new List<AssignedTask>());
                    }
                    assignedJobs[task.machine].Add(new AssignedTask(jobID, taskID, start, task.duration));
                }
            }

            // Create per machine output lines.
            String output = "";
            foreach (int machine in allMachines)
            {
                // Sort by starting time.
                assignedJobs[machine].Sort();
                String solLineTasks = $"Machine {machine}: ";
                String solLine = "           ";

                foreach (var assignedTask in assignedJobs[machine])
                {
                    String name = $"job_{assignedTask.jobID}_task_{assignedTask.taskID}";
                    // Add spaces to output to align columns.
                    solLineTasks += $"{name,-15}";

                    String solTmp = $"[{assignedTask.start},{assignedTask.start + assignedTask.duration}]";
                    // Add spaces to output to align columns.
                    solLine += $"{solTmp,-15}";
                }
                output += solLineTasks + "\n";
                output += solLine + "\n";
            }
            // Finally print the solution found.
            Debug.Log($"Optimal Schedule Length: {solver.ObjectiveValue}");
            Debug.Log($"\n{output}");
        }
        else
        {
            Debug.Log("No solution found.");
        }

        Debug.Log("Statistics");
        Debug.Log($"  conflicts: {solver.NumConflicts()}");
        Debug.Log($"  branches : {solver.NumBranches()}");
        Debug.Log($"  wall time: {solver.WallTime()}s");
    }
}
