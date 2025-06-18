# DeltaruneSaveConverter

A C#.NET command line tool to convert DELTARUNE Chapter 1 & 2 save files between PC and console editions of the game. This tool can also, in theory, be used to extract and pack UNDERTALE save files between consoles.

This program is a work in progress and may not work in all situations - this has only been tested converting 1 file from the Nintendo Switch version of DELTARUNE to the PC version and back.

I am not responsible if you lose any progress if using this tool.

## Usage

Requires [.NET 8.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0/runtime) installed.

```
DeltaruneSaveConverter [command] [path1] [path2]

Commands:
---------
ConvertFromConsole: extracts a SAV file from path1 into path2 and converts the save files into PC format
ConvertFromPC: converts save files from path1 into console format and packs them into a SAV at path2
ExtractSAV: extracts a SAV file from path1 into path2 - supports UNDERTALE
PackSAV: packs a SAV file with files from path1 into path2 - supports UNDERTALE
ConvertFileFromConsole: converts the save file at path1 into PC format and saves it at path2
ConvertFileFromPC: converts the save file at path1 into console format and saves it at path2

Examples:
---------
DeltaruneSaveConverter ConvertFromConsole deltarune_ch1.sav ./pcsave/
- Extracts the save files from deltarune_ch1.sav, converts it to PC format and saves it into ./pcsave/

DeltaruneSaveConverter ConvertFromPC ./pcsave/ deltarune_ch1.sav
- Converts the save files from ./pcsave/ into console format, packs it into deltarune_ch1.sav

DeltaruneSaveConverter ExtractSAV deltarune_ch1.sav ./consolesave/
- Extracts the save file from deltarune_ch1.sav and saves it into ./consolesave/
- This should also work for extracting UNDERTALE save files. (No conversion required.)

DeltaruneSaveConverter PackSAV ./consolesave/ deltarune_ch1.sav
- Packs the save file contents from ./consolesave/ into deltarune_ch1.sav
- This should also work for packing UNDERTALE save files. (No conversion required.)

DeltaruneSaveConverter ConvertFileFromConsole ./consolesave/filech1_0 ./pcsave/filech1_0
- Converts the console save file ./consolesave/filech1_0 into PC format and saves it into ./pcsave/filech1_0

DeltaruneSaveConverter ConvertFileFromPC ./pcsave/filech1_0 ./consolesave/filech1_0 
- Converts the PC save file ./pcsave/filech1_0 into console format and saves it into /consolesave/filech1_0
```

**Notice:** When converting from Chapter1&2 DEMO, you will still be converting `deltarune_ch1.sav` to get your Chapter 2 saves. **The other .sav files must be ignored.**

## Compiling

Compiling the program requires the [.NET 8.0 SDK](https://dotnet.microsoft.com/download) and can be done by typing `dotnet build` into a command line. Windows users can also use Visual Studio 2022 or later to compile the solution.

## TODO

(in no particular order)

- Chapter 3/4 support - Chapter 3 seems to be identical to Chapter 2.
- Test conversion between PS4/PS5 and PC.
- Test making sure "weird route"/"SideB"(?) information is preserved.
- macOS/Linux support, NativeAOT stuff?
- Friendlier user interface (+ GUI/web edition?)

## License

[This code is licensed under the MIT License.](https://github.com/InvoxiPlayGames/DeltaruneSaveConverter/blob/master/LICENSE)
