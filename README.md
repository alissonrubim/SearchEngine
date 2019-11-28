# SearchEngine

Official website: http://www.alissonrubim.com/projects/searchengine

In my development life, I always end up doing some string-searching work in files, or even directories, to fix something or find a problem.
So, one day I decided to create a libary to help with that, and them I did! I prevent to you SearchEngine, a simple library to find patters inside a file or directory.

## Releases, please


## How to install

### Using NugetRepository
Has a better way to share code tham using a repository? I don't think so. Them you can find the SearchEngine in the Nuget and use it.
Link to the Nuget Gallery:
https://www.nuget.org/packages/SearchEngine

If you prefer the command line, here it's:
```powersheel
Install-Package SearchEngine -Version 1.0.1
```

### Importing as .dll
You can download the release and import the SearchEngine.dll to your project. 

### Building by my own
Or, if you prefer, you can insert the file SearchEngine.cs inside your project and build it by your own. :)

## How this work?
Very simple, I must to tell you. 

#### 1. First step is create the SearchEngine object, like this:
```C#
 SearchEngine.SearchEngine seachEngine = new SearchEngine.SearchEngine(new SearchEngineConfig()
{
    StartSearchPattern = "[assembly: AssemblyVersion(",
    EndSearchPattern = ")]"
});
```
At SearchEngineConfig, you will set the properties to satisfy your search.

#### 2. Now, you decide if going to serach inside a File or inside a Directory.
By File:
```C#
IEnumerable<SearchEngineMatch> searchResult = seachEngine.SearchInFile("AssemblyInfo.cs");
```
By Directory:

#### 3. With the results in hands, we can see some properties that the object SearchEngineMatch have.
```C#
result.FilePath; //The file where the match was found it
result.FoundSetence; //The founded string, the value between StartSearchPattern and EndSearchPattern
result.FoundSetenceStartIndex; //Where the founded string starts at the file
result.FoundSetenceEndIndex; //Where the founded string ends at the file
result.FoundFullSetence; //The full setence founded (StartSearchPattern + FoundSetence + EndSearchPattern)
result.FoundFullSetenceStartIndex; //Where the full setence starts at the file
result.FoundFullSetenceEndIndex; //Where the full setence ends at the file
```

Very easy, hm?

If you need help or want to improve the SearchEngine, please, send me a message. I'll be glade to chat with you.
