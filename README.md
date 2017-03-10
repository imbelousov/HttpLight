# HttpLight

HttpLight is extremely lightweight, fast and flexible solution based on `HttpListener`. 
HttpLight is designed to simplify the process of creating self-hosted high-loaded REST API. HttpLight is multi-threaded and it fully supports async/await actions.

## Key features and advantages

* Focused on an API development;
* Performant: HttpLight is faster than most popular HTTP frameworks;
* Uses built-in HTTP implementation, so you don't worry about lacks of HTTP protocol support: HttpLight does not implement HTTP, but it simplifies development;
* Supports async/await actions;
* Has a simple and habitual architecture.

## Performance

All measurements are made with [Apache JMeter](http://jmeter.apache.org/) using 30 threads. Processed requests per second are listed in the table below.

![](https://raw.githubusercontent.com/imbelousov/HttpLight/master/images/performance.png)

|                                                               | Simple GET | Simple POST | Single parameter | 5 parameters | 404   |
|---------------------------------------------------------------|------------|-------------|------------------|--------------|-------|
| [HttpLight](https://github.com/imbelousov/HttpLight)          | 22854      | 23252       | 21105            | 15627        | 22241 |
| [WCF](https://msdn.microsoft.com/en-us/library/bb907578.aspx) | 14679      | 14510       | 11406            | 10190        | 8111  |
| [NancyFx](http://nancyfx.org/)                                | 14072      | 13533       | 12513            | 12206        | 1013  |

## Documentation

### 1. Basics

#### 1.1. Server

All you need to deploy HTTP server is an `HttpServer` instance. Minimal code is represented below:

```c#
using (var server = new HttpServer())
{
    server.Hosts.Add("*", 8080); // Specify host and port here
    server.Start();
    Console.ReadKey();
}
```
Since no action is registered, any request to the server will cause an error 404.

#### 1.2. Controllers and actions

If you want to add some logic to the server, you need to make at least one controller. Controllers are classes inherited from `Controller` which contain request handlers - actions. Actions are just instance methods which can return an HTTP body of response as string, byte array or stream. Actions must be marked with one (or more) `[Method]` attributes to give understanding which types of requests should be handled. You can mark methods with short forms, for instance `[Get]` and `[Post]`. Then you have to add your controller to the server:
```c#
class MainController : Controller
{
    [Get]
    public string Hello()
    {
        return "Hello!";
    }

    [Post]
    public void EmptyBody()
    {
    }
}

server.Controllers.Add<MainController>(); // Or .Add(typeof(MainController))
server.Start();
```
Now your actions are available at http://localhost:8080/Hello and http://localhost:8080/EmptyBody respectively.

#### 1.3. Customizing path

There are different reasons to customize path that is similar to action name by default: resolve collision, simplify it, add prefix, differ GET and POST logic, etc. Mark your action with `[Path]` attribute to achieve this:
```c#
[Get]
[Path("/")]
public string Home()
{
    return "Home page";
}
```
After that a home page is available at http://localhost:8080/.

#### 1.4. Action parameters

HttpLight allows you to make parametrized actions. The most popular standard types are supported: integers, float numbers, guids, strings, booleans, their `Nullable<>` expansions and arrays. For instance, this code:
```c#
[Get]
public string Sum(int a, int b)
{
    return (a + b).ToString();
}
```
Returns "9" for request http://localhost:8080/Sum?a=5&b=4 and "7" for http://localhost:8080/Sum?b=7. You can pass multiple values through URL:
```c#
[Get]
public string Sum(int[] a)
{
    return (a ?? new int[0]).DefaultIfEmpty().Sum().ToString();
}
```
In this case "8" will be response for request http://localhost:8080/Sum?a=5&a=3.

#### 1.5. Parameters source

By default values of parameters are transmitted via URL. You can specify the source of values with `[FromUrl]` and `[FromContent]` attributes. Moreover you can customize names or even create two different parameters with the same names but different types:
```c#
[Get]
public void Action1([FromUrl] int? a, [FromUrl("a")] string aRaw)
{
}

[Post]
public void Action2([FromContent] int a)
{
}
```
`[FromContent]` attribute reads content stream to end, so in case of using this, you are not able to read content as stream. However raw string value is still available:
```c#
[Post]
public void Action([FromContent] int a)
{
    var raw = Request.Content.RawContent;
}
```

#### 1.6. Async pattern

HttpLight fully supports async/await pattern. Just make async action instead of sync and use it without any restrictions:
```c#
[Get]
public async Task<string> Hello()
{
    return await Task.Run(() => "Hello!");
}
```

### 2. Customization

#### 2.1. Parameters binder

Sometimes you can be not satisfied by default binder. Default binder converts URL-encoded value to specified type. You can create your own binder. For example, you can use your model as parameter. The simpliest example is represented below:
```c#
class CustomModel
{
    public string A { get; set; }
    public string B { get; set; }
}

class CustomBinder : IActionParameterBinder
{
    public object Bind(ActionParameterBinderContext context)
    {
        return new CustomModel
        {
            // Extract values from given source
            A = context.Source.GetValues(context.ParameterName + "a").FirstOrDefault(),
            B = context.Source.GetValues(context.ParameterName + "b").FirstOrDefault()
        };
    }
}

[Get]
public void Action([Binder(typeof(CustomBinder))] CustomModel x)
{
}
```
Test this with request to http://localhost:8080/Action?xa=1&xb=2.

#### 2.2. Custom status page

You can customize error pages such as 404 or 500. To achieve this, mark your action with `[StatusCode]` attribute and specify status code as argument:
```c#
[StatusCode(HttpStatusCode.NotFound)]
public string NotFound()
{
    return "404";
}
```

#### 2.3. Common action before any request

It can be useful if you want to check headers and cookies or implement authentication. Mark your action with `[Before]` attribute and return nothing (void or null) to continue processing or something if request must be abandoned:
```c#
[Before]
public string CheckSession()
{
    var sessionCookie = Request.Cookies["SessionId"];
    if (sessionCookie == null)
        return "Session is not started";
    Request.Bag["SessionId"] = sessionCookie.Value;
    return null;
}

[Get]
public void Action()
{
    var sessionId = (string) Request.Bag["SessionId"];
}
```
Action will be invoked only if cookie that contains session identifier is defined.
