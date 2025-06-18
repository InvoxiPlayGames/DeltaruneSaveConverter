# DeltaruneSaveConverter

A C#.NET command line tool to convert DELTARUNE save files between PC and console editions of the game.

This program is a work in progress and may not work in all situations - this has not been extensively tested. I am not responsible if you lose any progress if using this tool.

## Usage

Requires [.NET 8.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0/runtime) installed.

Download and extract the latest release ZIP from [the releases page](https://github.com/InvoxiPlayGames/DeltaruneSaveConverter/releases).

Open a command prompt to the window containing DeltaruneSaveConverter.exe.

```
DeltaruneSaveConverter [command] [path1] [path2]

Commands:
---------
ConvertFromConsole: extracts a SAV file from path1 into path2 and converts the save files into PC format
ConvertFromPC: converts save files from path1 into console format and packs them into a SAV at path2

Examples:
---------
DeltaruneSaveConverter ConvertFromConsole deltarune_ch1.sav ./pcsave/
- Extracts the save files from deltarune_ch1.sav, converts it to PC format and saves it into ./pcsave/

DeltaruneSaveConverter ConvertFromPC ./pcsave/ deltarune_ch1.sav
- Converts the save files from ./pcsave/ into console format, packs it into deltarune_ch1.sav
```

**Notice:** When converting from Chapter1&2 DEMO, you will still be converting `deltarune_ch1.sav` to get your Chapter 2 saves. **The other .sav files must be ignored.**

## Compiling

Compiling the program requires the [.NET 8.0 SDK](https://dotnet.microsoft.com/download) and can be done by typing `dotnet build` into a command line. Windows users can also use Visual Studio 2022 or later to compile the solution.

## TODO

(in no particular order)

- Properly test Chapter 4
- Test conversion between PS4/PS5 and PC.
- Test making sure "weird route"/"SideB"(?) information is preserved.
- Friendlier user interface (+ GUI/web edition?)

## License

[This code is licensed under the MIT License.](https://github.com/InvoxiPlayGames/DeltaruneSaveConverter/blob/master/LICENSE)
