# UrhoAngelscriptIDE
An IDE and Debugger for Angelscript with Urho3D.

## Critical Todo's That May Interfere with usages

* Code editors for the IDE/Debug views need to be consolidated
* Saving remote files (received from debug server) locally, in the case of debugging the local machine, this is unnecessary as the saving will send the file data over the connection where the debug daemon will save it

## IDE Functionality
The IDE is focused on directories instead of projects/solutions. Select the root directory of your project (in the case of Urho this may be.

Presently only, text, XML, and Angelscript (.as) files may be edited. Angelscript files support intellisense. XML assistance is a WIP pending finishing XSD specs for all Urho3D xml file types.

**Intellisense** requires a successful compilation in order to function, or it will fallback on a default header dump that it will parse for generic Urho3D specific intellisense.

## Compiling
The executable directory must contain a bin folder into which you place the ScriptCompiler executable.
The IDE will run and feed parameters to the compiler as a slave process. Commandline output from the compiler is piped
into the IDE where it is parsed to scan for error messages.

After a successful compilation the script compiler will be used again in order to generate Latex documentation dumps and a header file for the script types. The IDE will parse these files to generate documentation and class hierarchies.

**It's recommended to modify your scriptcompiler to generate the dump files AFTER first compiling the code.** This way the header dump will contain definitions for the script classes, functions, and variables found in your files.
