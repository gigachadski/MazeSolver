namespace MazeSolver
{
    // Перелік режимів редагування лабіринту
    public enum EditMode { ToggleWall, SetStart, SetEnd }

    // Перелік алгоритмів пошуку шляху
    public enum PathfindingAlgorithm
    {
        Dijkstra,
        AStarManhattan,
        AStarEuclidean
    }
}
