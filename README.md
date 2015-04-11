# UrhoAngelscriptIDE
An IDE and Debugger for Angelscript with Urho3D.

# Critical Todo's That May Interfere with usages

* Code editors for the IDE/Debug views need to be consolidated
* Saving remote files (received from debug server) locally, in the case of debugging the local machine, this is unnecessary as the saving will send the file data over the connection where the debug daemon will save it

# IDE Functionality

# Compiling
The executable directory must contain a bin folder into which you place the ScriptCompiler executable.
The IDE will run and feed parameters to the compiler as a slave process. Commandline output from the compiler is piped
into the IDE where it is parsed to scan for error messages.
