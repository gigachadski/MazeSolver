using System;
using System.Windows.Forms;
using MazeSolver;

namespace MazeSolver
{
    // ����� ����� �������
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
                MessageBox.Show("�������� ������� �������: " + ex.Message, "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}