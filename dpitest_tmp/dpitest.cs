using System;
using System.Drawing;
using System.Windows.Forms;

class DpiTest
{
    [STAThread]
    private static void Main()
    {
        // ★ 不调用 SetProcessDPIAware：模拟真实 Launcher 进程的未感知上下文
        Application.EnableVisualStyles();
        try { Application.SetCompatibleTextRenderingDefault(false); }
        catch (InvalidOperationException) { }

        using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
        {
            Console.WriteLine("System DpiX=" + g.DpiX + "  scale=" + (g.DpiX / 96f));
        }
        Console.WriteLine("PrimaryScreen.Bounds=" + Screen.PrimaryScreen.Bounds
            + " WorkingArea=" + Screen.PrimaryScreen.WorkingArea);

        XyzController.MainForm m = new XyzController.MainForm();
        Console.WriteLine("MainForm after ctor: Size=" + m.Size + " ClientSize=" + m.ClientSize
            + " AutoScaleDims=" + m.AutoScaleDimensions + " Min=" + m.MinimumSize + " IsHandleCreated=" + m.IsHandleCreated);

        XyzController.PointJumpForm j = new XyzController.PointJumpForm();
        Console.WriteLine("PointJumpForm after ctor: Size=" + j.Size + " ClientSize=" + j.ClientSize
            + " AutoScaleDims=" + j.AutoScaleDimensions + " Min=" + j.MinimumSize + " IsHandleCreated=" + j.IsHandleCreated);

        XyzController.TrajectoryViewForm t = new XyzController.TrajectoryViewForm();
        Console.WriteLine("TrajectoryViewForm after ctor: Size=" + t.Size + " ClientSize=" + t.ClientSize
            + " AutoScaleDims=" + t.AutoScaleDimensions + " Min=" + t.MinimumSize);
    }
}
