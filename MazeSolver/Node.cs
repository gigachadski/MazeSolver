using System.Drawing;

namespace MazeSolver
{
    // Клас, що описує вузол (клітинку) для алгоритму пошуку шляху  
    public class Node
    {
        // Позиція вузла у лабіринті (координати)  
        public Point Position { get; }

        // g – вартість шляху від старту до цього вузла  
        public double Cost { get; }

        // f = g + h (сумарна оцінка, де h — евристика)  
        public double Priority { get; }

        // Батьківський вузол для відновлення шляху  
        public Node Parent { get; }

        /// <summary>  
        /// Конструктор для створення вузла.  
        /// </summary>  
        /// <param name="pos">Позиція вузла у лабіринті.</param>  
        /// <param name="cost">Вартість шляху від старту до цього вузла (g).</param>  
        /// <param name="priority">Сумарна оцінка вузла (f = g + h).</param>  
        /// <param name="parent">Батьківський вузол для відновлення шляху.</param>  
        public Node(Point pos, double cost, double priority, Node parent)
        {
            Position = pos;
            Cost = cost;
            Priority = priority;
            Parent = parent;
        }
    }
}