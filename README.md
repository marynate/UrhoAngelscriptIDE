# UrhoAngelscriptIDE
An IDE and Debugger for Angelscript with Urho3D.

## Critical Todo's That May Interfere with usages

* Code editors for the IDE/Debug views need to be consolidated
* Saving remote files (received from debug server) locally, in the case of debugging the local machine, this is unnecessary as the saving will send the file data over the connection where the debug daemon will save it
* Function parameters for intellisense
* Comments/notes on intellisense related items (ie class/function documentation)

# IDE Functionality
The IDE is focused on directories instead of projects/solutions. Select the root directory of your project (in the case of Urho this may be.

Presently only, text, XML, and Angelscript (.as) files may be edited. Angelscript files support intellisense. XML assistance is a WIP pending finishing XSD specs for all Urho3D xml file types.

**Intellisense** requires a successful compilation in order to function, or it will fallback on a default header dump that it will parse for generic Urho3D specific intellisense.

### File Tree

The file tree contains all files and folders below the selected path. .as, .xml, and .txt files can be opened by double clicking. Folder context menu options are to create a new file, new folder, rename, or delete. File context menu options are to rename, edit, or delete.

### Code Editor

* Ctrl + F = Search/Find

Context Menu:

* Compile
* Create a template ScriptObject
* Insert a #include by browsing for the file
* Insert doxygen style comment
* Insert doxygen style property comment
* Insert a snippet
* Common edit operations

### Class Browser

The class information is created by parsing the header dumps generated by the script compiler. A type hierarchy is built that is used both for displaying the class browser and for intellisense functionality.

### Code Snippets

If a folder named "snippets" exists in the directory of the IDE's executable it will be scanned for XML files to be loaded as snippets. Snippets may have "inputs" which are inserted into the snippet code using a {{mustache}} style marking.

### Attributes Browser

The Attributes browser is populated from parsing the Latex output of the scriptcompiler's dump.

The tree consists of entities (Scene/Node/Components) registered to the Urho3D engine and each record contains a list of the attributes registered for it and the attributes type. Right clicking on an attribute will bring up a context menu with options to copy either a "getter" or a "setter" for the attribute selected - which will remove any risk of a typo.

### Events Browser

The events browser is populated from parsing the Latex output of the scriptcompiler's dump.

Events are grouped by Category, Event, Parameters.

Right-clicking on the Event or Parameters will bring a context menu with clipboard options.

In the case of an Event you can copy a template event subcribtion function call, unsubribtion, or generic event handler for the event.

For parameters the only current option is to copy the parameter getter (eventData["ParamName"]).

### Console Log

Contains verbatim output from the last compilation action.

### Error List

If a compilation attempt fails the error log will be parsed and the messages will appear hear. The tab will include a bright red 'X' along with a count of the errors. Each error contains basic summary information and may be double clicked to navigate directly to the location of the error.

Because of how angelscript detects errors, most errors will be reported as being on the line after the error, so investigate everything around the large red squiggly line if in doubt.

### Search

Performs an asynchronous "find in files" that will report the files in which the terms were found and how many times it was found. Double clicking the record will open the file. Later iterations will replace the flat list with a tree and specific items for each result's location in addition to the parent summary for the file.

## Compiling
The executable directory must contain a bin folder into which you place the ScriptCompiler executable.
The IDE will run and feed parameters to the compiler as a slave process. Commandline output from the compiler is piped
into the IDE where it is parsed to scan for error messages.

After a successful compilation the script compiler will be used again in order to generate Latex documentation dumps and a header file for the script types. The IDE will parse these files to generate documentation and class hierarchies.

**It's recommended to modify your scriptcompiler to generate the dump files AFTER first compiling the code.** This way the header dump will contain definitions for the script classes, functions, and variables found in your files.

# Debugger Functionality

## Connections

## Breakpoints

## Execution Control

## Callstack

## Locals, This, and Watches
