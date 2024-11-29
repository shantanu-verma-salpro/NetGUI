
using ImGuiNET;

namespace App
{
    public class Program : UserInterface
    {
        override public void Render(){
            ImGui.ShowDemoWindow();
        }
        public static void Main()
        {
            Program program = new();
            program.Run("hello", 800, 600); 
        }
    }
}
