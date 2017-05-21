# FsharpDiyBrowser kata

The goal of this fsharp kata is to build a small document renderer

## Solution

![solution](/docs/images/solution2.png)

- FsharpDiyBrowser: contains the domain
- FsharpDiyBrowser.Winform: transformation layer to render documents in a Winform canvas
- FsharpDiyBrowser.HtmlInput: transsform HTML to domain models
- LiveCoding folder: contains all live code scripts.

### What do I mean by live coding ?

This kata could be so long to do entierly.
So we need a very fast feedback to check our drawings logic.

Each time we change a file, then we have an immediate feedback.

![Livecode2](/docs/images/Livecode2.gif)


I love this kind of technics, but in real production projects, we must do unit tests!

# Let's go !

## Build the project

``` powershell

cd src
.\build.cmd

```

## Try to complete Test1

- Discover and extend the domain.
- Implement new rendering features. (look at comments in the script)

``` powershell

cd src
.\build.cmd LiveCode file="LiveCoding/Test1.fsx"

```

## Try to complete Test2

- render the body with correct children positions
- parse percents, etc ...
- try to implement images and hyper links

``` powershell

cd src
.\build.cmd LiveCode file="LiveCoding/Test2.fsx"

```
![Livecode3](/docs/images/Livecode3.gif)

