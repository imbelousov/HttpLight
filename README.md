# HttpLight

HttpLight is extremely lightweight, fast and flexible solution based on `HttpListener`. 
HttpLight is designed to simplify the process of creating self-hosted high-loaded REST API. HttpLight is multi-threaded and it fully supports async/await actions.

#### Key features and advantages:
* Focused on an API development;
* Performant: HttpLight is faster than most popular HTTP frameworks;
* Uses built-in HTTP implementation, so you don't worry about lacks of HTTP protocol support: HttpLight does not implement HTTP, but it simplifies development;
* Supports async/await actions;
* Has an MVC-like architecture: it is simple and habitual.

## Samples

#### Hello world

```c#
class Program
{
    static void Main(string[] args)
    {
        using (var server = new HttpServer())
        {
            server.Controllers.Add<MainController>();
            server.Hosts.Add("localhost", 80);
            server.Start();
            Console.WriteLine("HttpServer started");
            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
        }
    }
}

class MainController : Controller
{
    [Get]
    public string HelloWorld()
    {
        return "Hello, world!";
    }
}
```

#### Action without result
```c#
class MainController : Controller
{
    [Get]
    public void NoResult()
    {
        //do something
    }
}
```

#### Async action
```c#
class MainController : Controller
{
    [Get]
    public async Task<string> AsyncAction()
    {
        //...
    }
}
```

#### Custom URL path
```c#
class MainController : Controller
{
    [Get]
    [Path(/custom)]
    public void Action
    {
        //...
    }
}
```

#### Custom 404 status page (or anything else)
```c#
class MainController : Controller
{
    [StatusCode(HttpStatusCode.NotFound)]
    public string Custom404()
    {
        return "404";
    }
}
```

#### You can specify two or more allowed HTTP  methods
```c#
class MainController : Controller
{
    [Get]
    [Post]
    public void Action
    {
        //...
    }
}
```

#### Passing URL parameters
HttpLight supports most commonly used built-in .NET primitive types including their `Nullable` extensions
```c#
class MainController : Controller
{
    [Get]
    public void Action(int integer, string str, Guid? guid)
    {
        //...
    }
}
```
But you are able to override default parameter binding
```c#
class MainController : Controller
{
    [Get]
    public void Action([Binder(typeof(CustomModelBinder))] CustomModel model, string str, Guid? guid)
    {
        //...
    }
}

class CustomModel
{
    public int Integer { get; set; }
}

class CustomModelBinder : IActionParameterBinder
{
    public object Bind(ActionParameterBinderContext context)
    {
        var model = new CustomModel();
        int integer;
        int.TryParse(context.HttpRequest.UrlParameters.Get("integer"), out integer);
        model.Integer = integer;
        return model;
    }
}
```