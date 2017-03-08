# HttpLight

HttpLight is extremely lightweight, fast and flexible solution based on `HttpListener`. 
HttpLight is designed to simplify the process of creating self-hosted high-loaded REST API. HttpLight is multi-threaded and it fully supports async/await actions.

## Key features and advantages

* Focused on an API development;
* Performant: HttpLight is faster than most popular HTTP frameworks;
* Uses built-in HTTP implementation, so you don't worry about lacks of HTTP protocol support: HttpLight does not implement HTTP, but it simplifies development;
* Supports async/await actions;
* Has a simple and habitual architecture.

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

#### 1.5. Async pattern

HttpLight fully supports async/await pattern. Just make async action instead of sync and use it without any restrictions:
```c#
[Get]
public async Task<string> Hello()
{
    return await Task.Run(() => "Hello!");
}
```