using Godot;

namespace Services;

public interface IHelloWorldService
{
    void HelloWorld();
}

public class HelloWorldService : IHelloWorldService
{
    public void HelloWorld()
    {
        GD.Print("Hello World!");
    }
}