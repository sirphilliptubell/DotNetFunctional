# DotNetFunctional
A Functional / Monadic library for .Net

## About
This is a library of objects and extension method which can help you write functional style programming in .Net (in non-F# projects.) A portion of the code in this project was inspired by some of the videos in the Additional Resources section.

## The Maybe Type
The Maybe Type (sometimes called the Optional Type) exists to help prevent NullReferenceExeptions.

### Background
Whenever you write a function, you create it with an implicit and/or explicit [contract](https://msdn.microsoft.com/en-us/library/dd264808%28v=vs.110%29.aspx) in mind (ie: you have an expectation of what requirements it has and what the end result of it's behavior is.) A well named function should tell you what it does, and the parameters tell you what it needs. If the function requires a byte as one of it's parameters, then you immediately know the parameter must be in the range of 0-255. If it has an int? as a parameter, then you know it can either take an integer, or not. However if you see a function that takes a reference type for a parameter, does it accept null or not? If the function returns a reference type, does it return a null or not? Generally you must hope the developer wrote good documentation (which developers are notorious for being bad at) or you may have to spend time looking at the source code to find out (if you even have access to it.) Adding non-nullable reference types has been proposed for .Net, [but some say it's not coming anytime soon.](http://twistedoakstudios.com/blog/Post330_non-nullable-types-vs-c-fixing-the-billion-dollar-mistake)

### Usage
The Maybe type mirrors Nullable&lt;T&gt; and allows you to define when a reference type may be null. It has properties for HasValue and HasNoValue to indicate whether it is null or not.
```csharp
Maybe<SomeClass> obj = null;  //obj.HasValue is false and obj.HasNoValue is true.
Maybe<SomeClass> obj2 = new SomeClass();  //types can be implicitly converted to their Maybe type.
```
Your functions can now tell it's consumer that it may or may not return a value.
```csharp
Maybe<SomeClass> SomeFunctionThatMayReturnNull() { ... }

void Example()
{
    var obj = SomeFunctionThatMayReturnNull();
    if (obj.HasValue)
    {
        //do something with obj.Value
    }
}
```
You can also access the value within an expression bodied function.
```csharp
void DoSomething(Maybe<SomeClass> paramThatMayBeNull)
    => paramThatMayBeNull
    .IfValue(value => {
        //do something with the value (guaranteed not to be null)
    })
    .Else(() => {
        //do something because there was no value
    });
```
or
```csharp
object DoSomething(Maybe<SomeClass> param1)
    => param1.HasValue
    ? doSomethingElse1(param1.Value)
    : doSomethingElse2();
```
## The Result Type
The Result type exists to help find a middle ground between Exceptions (expensive when they occur) and C-like return codes (non descriptive).

### Background
In C# and VB, any given function has the ability to lie about it's behavior. In this example, the signature "int Divide(int, int)" doesn't tell you that it may throw a DivideByZeroException.
```csharp
int Divide(int dividend, int divisor)
    => dividend / divisor;
```
When functions are nested, it's even less obvious.
```csharp
void Outer() 
{
    try {
        Middle();     
    }
    catch (Exception ex)
    {
        //can we trust the state that Middle() changed is ok???
    }
}

/// <summary>
/// I do lots of work...
/// (no comments here telling I might throw an exception)
/// </summary>
void Middle()
{
    //do stuff...
    Inner(); 
    //do stuff...
}

/// <summary>
/// I may throw an exception but my signature doesn't warn you
/// </summary>
void Inner() 
{ 
    if (new Random.Next() % 2 == 0)
        throw new Exception("Today is just not your day.");
}
```
There are multiple problems with the above code. Does the author of Middle know that Inner may throw an exception? If Middle() changes some state before calling Inner, is that state valid anymore if Inner throws the exception? Can the author of Outer trust that the state of the program is still valid even though the exception was caught? Maybe the author of Middle was lazy and didn't write any comments, or maybe they forgot to mention that it might throw, or was it on purpose because the program's state would still be valid?

The worst thing about exceptions is that they can happen. The second worst thing is that they are essentially a GOTO statement that can cause your program to jump over very large amounts of code and over multiple stack frames. Once they happen it's quite difficult to trust that your application's state is even valid anymore. In C-like programs, some authors would return an integer to indicate whether a function succeeded or not and different values could incidicate why. But a number doesn't tell very much. You may want to return your own value instead so returning an integer for all functions doesn't bode well. In [Midori](http://joeduffyblog.com/2016/02/07/the-error-model/#bugs-abandonment-assertions-and-contracts) Microsoft attempted to resolve this issue by making every function tell what exceptions it could throw and forced you to handle all of those (similar to Java.) Also in Midori, they separated errors into two types of errors. One type of error is the one you expect could happen (eg: a file not being found or a resource is unavailable). The other type of errors are ones where the developer made a mistake (eg: you tried to access an index outside the bounds of an array or a null value was given to a method which shouldn't take one). This would have been a better solution however this feature is not available in the current version of the .Net Framework.

The Result object helps resolve these issues by letting your function's signature indicate that it may fail and optionally at the same time provide a return value.

### Usage
Use Result or Result&lt;T&gt; as a return type
```csharp
Result DoSomethingThatMightFail() 
{
    if (something fails)
        return Result.Fail("something failed");
    else
        return Result.Ok();
}

Result<SomeReturnValueType> DoSomethingThatMightFailAndReturnsAValue() 
{ 
    if (something fails)
        return Result.Fail("something failed");
    else
        return Result.Ok(new SomeReturnValueType());
}

void main() {
    var result = DoSomethingThatMightFail();
    if (result.IsSuccess) // or !result.IsFailure
    {
        //do something
    }
    
    var resultVal = DoSomethingThatMightFailAndReturnsAValue();
    if (resultVal.IsSuccess)
    {
        //do something with resultVal.Value
    }
}
```
Note that a Result object should not replace all exceptions. You should not replace null parameter checks for example.
```csharp
Result Delete(FileInfo file)
{
    //Don't do this, this is a contract error and means the consumer of this method used it incorrectly.
    if (file == null) return Result.Fail("\file' parameter cannot be null");

    //This is ok
    if (!file.Exists) return Result.Fail("File doesn't exist");
    
    /* This may or may not be ok, what was the intention of this function?
	Was it to ensure a file didn't exist (in which the following code block is ok)
	Or was it's intention to ensure a file was really deleted? (in which the following code block is not ok)
    */
    try
    {
        file.Delete();
    }
    catch (IOException ex)
    {
        return Result.Fail(ex);
    }
    return Result.Ok();
}
```




# Additional Resources
* [Brian Beckman: Don't fear the Monad (YouTube)](https://www.youtube.com/watch?v=ZhuHCtR3xq8)
* [Functional programming design patterns by Scott Wlaschin (YouTube)](https://www.youtube.com/watch?v=E8I19uA-wGY) (aka Railway Oriented Programming)
* [Applying Functional Principles in C# (PluralSight)](https://www.pluralsight.com/courses/csharp-applying-functional-principles)
* [Functional Programming with C# (PluralSight)](https://www.pluralsight.com/courses/functional-programming-csharp)
