using System.Collections.Generic;
using System.Drawing;

namespace MazeSolver
{
    // Клас, що містить результати пошуку шляху
    public class PathfindingResult
    {
        public List<Point> Path { get; }
        public int IterationCount { get; }
        public bool IsPathFound => Path != null && Path.Count > 0;

        public PathfindingResult(List<Point> path, int iterationCount)
        {
            Path = path;
            IterationCount = iterationCount;
        }
    }
}
