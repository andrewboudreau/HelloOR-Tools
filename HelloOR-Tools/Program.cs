
using Google.OrTools.LinearSolver;

Console.WriteLine("Hello, OR-Tools!");
Console.WriteLine("Starting the bin packing example.");

var data = new DataModel();
Solver solver = Solver.CreateSolver("SCIP");

Variable[,] x = new Variable[data.NumItems, data.NumBins];
for (int i = 0; i < data.NumItems; i++)
{
    for (int j = 0; j < data.NumBins; j++)
    {
        x[i, j] = solver.MakeIntVar(0, 1, $"x_{i}_{j}");
    }
}

Variable[] y = new Variable[data.NumBins];
for (int j = 0; j < data.NumBins; j++)
{
    y[j] = solver.MakeIntVar(0, 1, $"y_{j}");
}

for (int i = 0; i < data.NumItems; ++i)
{
    Constraint constraint = solver.MakeConstraint(1, 1, "");
    for (int j = 0; j < data.NumBins; ++j)
    {
        constraint.SetCoefficient(x[i, j], 1);
    }
}

for (int j = 0; j < data.NumBins; ++j)
{
    Constraint constraint = solver.MakeConstraint(0, Double.PositiveInfinity, "");
    constraint.SetCoefficient(y[j], data.BinCapacity);
    for (int i = 0; i < data.NumItems; ++i)
    {
        constraint.SetCoefficient(x[i, j], -DataModel.Weights[i]);
    }
}
Objective objective = solver.Objective();
for (int j = 0; j < data.NumBins; ++j)
{
    objective.SetCoefficient(y[j], 1);
}
objective.SetMinimization();
Solver.ResultStatus resultStatus = solver.Solve();
// Check that the problem has an optimal solution.
if (resultStatus != Solver.ResultStatus.OPTIMAL)
{
    Console.WriteLine("The problem does not have an optimal solution!");
    return;
}

Console.WriteLine($"Number of bins used: {solver.Objective().Value()}");
double TotalWeight = 0.0;

for (int j = 0; j < data.NumBins; ++j)
{
    double BinWeight = 0.0;
    if (y[j].SolutionValue() == 1)
    {
        Console.WriteLine($"Bin {j}");
        for (int i = 0; i < data.NumItems; ++i)
        {
            if (x[i, j].SolutionValue() == 1)
            {
                Console.WriteLine($"Item {i} weight: {DataModel.Weights[i]}");
                BinWeight += DataModel.Weights[i];
            }
        }
        Console.WriteLine($"Packed bin weight: {BinWeight}");
        TotalWeight += BinWeight;
    }
}

Console.WriteLine($"Total packed weight: {TotalWeight}");

class DataModel
{
    public static readonly double[] Weights = { 48, 30, 19, 36, 36, 27, 42, 42, 36, 24, 30 };
    public readonly int NumItems;
    public readonly int NumBins;
    public readonly double BinCapacity = 100.0;
    public DataModel()
    {
        NumBins = Weights.Length;
        NumItems = Weights.Length;
    }
}