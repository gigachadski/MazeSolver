namespace MazeSolver
{
    // граф у вигляді матриці суміжності
    public class MazeGraph
    {
        private readonly int[,] _maze;
        private readonly int _rows;
        private readonly int _cols;

        // Ключ — вершина (Point), значення — список сусідів
        public Dictionary<Point, List<Point>> AdjacencyList { get; } = new();

        public MazeGraph(int[,] maze)
        {
            if (maze == null) throw new ArgumentNullException(nameof(maze));
            _maze = maze;
            _rows = maze.GetLength(0);
            _cols = maze.GetLength(1);
            BuildGraph();
        }

        /// <summary>  
        /// Створює граф лабіринту у вигляді списку суміжності.  
        /// Для кожної прохідної клітинки (0) визначає її сусідів,  
        /// які також є прохідними, і додає їх до списку суміжності.  
        /// </summary>  
        /// <remarks>  
        /// Граф будується на основі масиву лабіринту (_maze),  
        /// де 1 — стіна, а 0 — прохідна клітинка.  
        /// Сусіди визначаються за чотирма напрямками:  
        /// вгору, вниз, вліво, вправо.  
        /// </remarks>  
        /// <exception cref="ArgumentNullException">  
        /// Викидається, якщо масив лабіринту (_maze) дорівнює null.  
        /// </exception>  
        private void BuildGraph()
        {

            int[] dx = { -1, 1, 0, 0 };

            int[] dy = { 0, 0, -1, 1 };



            for (int x = 0; x < _rows; x++)

            {

                for (int y = 0; y < _cols; y++)

                {

                    if (_maze[x, y] != 0)

                        continue;  // стіна



                    var current = new Point(x, y);

                    var neighbors = new List<Point>();



                    for (int i = 0; i < 4; i++)

                    {

                        int nx = x + dx[i];

                        int ny = y + dy[i];

                        if (nx >= 0 && nx < _rows && ny >= 0 && ny < _cols && _maze[nx, ny] == 0)

                        {

                            neighbors.Add(new Point(nx, ny));

                        }

                    }



                    AdjacencyList[current] = neighbors;

                }

            }

        }

    }
}