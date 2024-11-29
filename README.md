#  High-Performance Win32 + DirectX11 Backend with ImGui.NET  
A blazing-fast **Win32 + DirectX11 wrapper** designed for efficient GUI development with **ImGui.NET**. Built with modern .NET features, minimal marshaling overhead, and a focus on performance.

---

##  Key Features
- **ImGui.NET Integration**  
  Provides a seamless backend for building GUIs using the popular Dear ImGui framework.  
- **Custom Win32 and DirectX11 Backend**  
  Hand-written and optimized for high performance, enabling smooth rendering and low latency.  
- **Efficient P/Invoke Usage**  
  Minimal marshaling ensures fast interop and reduced garbage collection pressure.  
- **Modern .NET Features**  
  Leverages .NET 6+ for clean, maintainable, and efficient code.  
- **Lightweight and Easy to Use**  
  Simple to integrate into any .NET application with minimal overhead.  

---


##  Quick Start
Initialize the Wrapper

```csharp

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
```

##  Performance Optimizations

Custom Backend: Eliminates bloat for direct access to Win32 and DirectX11 APIs.
Minimal Allocations: Static variables and efficient structs reduce GC pressure in critical loops.
Zero-Cost Abstractions: Hand-written interop layers ensure predictable, high-speed performance.


##  Use Cases
Game Engines
Custom UI Frameworks
Simulation Software
Visualization Tools

##  Contributing
We welcome contributions to improve and extend the library.
Check out our CONTRIBUTING.md for guidelines.

## License
This project is licensed under the MIT License. See the LICENSE file for details.
