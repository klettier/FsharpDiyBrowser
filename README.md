# FsharpDiyBrowser kata

The goal of this fsharp kata is to build a small document renderer

## Solution

![solution](/docs/images/solution2.png)

- FsharpDiyBrowser: contains the domain
- FsharpDiyBrowser.Winform: transformation layer to render documents in a Winform canvas
- LiveCoding folder: contains all live code scripts.

### What do I mean by live coding ?

This kata could be so long to do entierly.
So we need a very fast feedback to check our drawings logic.

I love this kind of technics, but in real production projects, we must do unit tests!

# Let's go !

## Build the project

``` powershell

cd src
./build.cmd

```

## Run the live coding tool

``` powershell

cd src
.\build.cmd LiveCode file="LiveCoding/Test1.fsx"

```

