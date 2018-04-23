# Xamzor
### _E**x**tensible **A**pplication **M**arkup for Bla**zor**_

Xamzor is an experimental project in which I try to prototype a set of reusable Blazor components that are familiar to XAML developers and make it easy to compose web views that feel like native apps.

The list of [Xamzor components](https://github.com/SvenEV/Xamzor/tree/master/Xamzor/UI/Components) currently includes the most common layout controls (Grid, StackPanel, Border, ScrollViewer) as well as TextBlock, Image and Button.

👉 **[Try the Live Demo](http://xamzor.azurewebsites.net/)**  
*(this always runs the latest commit and may therefore be broken from time to time)*

## Goals
* Provide WPF-style layout primitives including `Grid`, `StackPanel` and `Border`, supporting properties like  `Width`, `Height`, `HorizontalAlignment`, `VerticalAlignment`, `Margin` and more

* Render to HTML, mapping the layout properties to CSS (using grid/flexbox).

* Be a library, not a framework

* Allow mixing-and-matching Xamzor components and "normal" HTML.

## Non-Goals
* I don't want Xamzor markup to be 100% syntax-compatible to XAML. In many ways, Razor syntax is more flexible than XAML.
* I don't want to replicate WPF features for scenarios that can be solved more elegantly in Razor (e.g. we don't need `{Binding}` or `ICommand`)

# About Blazor
[Blazor](https://github.com/aspnet/Blazor) is "an experimental web UI framework using C#/Razor and HTML, running in the browser via WebAssembly". It is a project from the ASP.NET team at Microsoft. Even if you are not a fan of my work, you should definitely [check out Blazor](https://github.com/aspnet/Blazor).
