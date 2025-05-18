using System;
using System.Windows.Forms;
using MazeSolver;

namespace MazeSolver
{
    // Точка входу додатку
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());

            }
            catch (Exception ex)
            {
                MessageBox.Show("Критична помилка додатку: " + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}