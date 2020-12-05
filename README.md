# About Line Rider Advanced: Community Edition
Line Rider Advanced: Community Edition, abbreviated as LRA:CE, is a fork of [Line Rider Advanced](https://github.com/jealouscloud/linerider-advanced); "An open source spiritual successor to the flash game Line Rider 6.2 with lots of new features."

This project's goal is to unify the best features from several existing forks of the original Line Rider Advanced all into one version. Once this version is caught up with all others, we, the contributors, will aim to continue development of new features in this repository as opposed to branching off into new forks again.

# Instructions
You can download the latest version from the [releases page](https://github.com/RatherBeLunar/LRA-Community-Edition/releases).

This project requires .NET 4.6 and C# 7 support.

### Windows
If you can't run the application, you most likely need to install [.NET 4.6](https://www.microsoft.com/en-us/download/details.aspx?id=48130).
### Mac/Linux
You will need the [Mono framework](http://www.mono-project.com/download/stable/) installed in order to run LRA:CE.

# LRA:CE Features
* Discord integration (At this point only [rich presence](https://i.ibb.co/8s4NC1X/image.png))
* Customisable scarf ([Custom scarf colors](https://github.com/RatherBeLunar/LRA-Community-Edition/tree/master/Examples/Scarves/README.md) and custom scarf length)
* [Custom riders](https://github.com/RatherBeLunar/LRA-Community-Edition/tree/master/Examples/Riders/README.md)
* [Custom scarves on a custom rider](https://github.com/RatherBeLunar/LRA-Community-Edition/tree/master/Examples/Riders/Bosh-Custom-Scarf-On-Png-Example/README.md)

# Issues
We are tracking issues on [Trello](https://trello.com/b/0RGXoFZQ/lra-community-edition). If the issue you wish to report isn't present there, please submit an issue here on Github, and we will add it to the Trello board.

# Build
### Step 0
Download and extract this and [gwen-lra](https\://github.com/jealouscloud/gwen-lra/tree/dbe3e84568b163f3e20cd876672fc1b3b0e40873)'s source code. Put the extracted gwen-lra source in the */lib/gwen-lra/* folder of this repository.

### Step 1
Open the *src/linerider.sln* file in Visual Studio. (Do not use Visual Studio Code). Click **Project > Manage NuGet Packages**. It should ask you to restore. Click yes.

### Step 2
Still in Visual Studio, click **Build > Build solution** or use the keyboard shortcut Ctrl+Shift+B. If you did everything correctly, you will find a new *build* folder in the root directory. This will contain the .exe file for the application itself.

### Step 3
Download the latest version of the [Discord Game SDK](https://dl-game-sdk.discordapp.net/latest/discord_game_sdk.zip) and copy */lib/x86/discord_game_sdk.dll* to the *build* folder of this repository. (If that doesn't work, try copying */lib/x86_64/discord_game_sdk.dll* instead)

# Libraries
This project uses binaries, sources, or modified sources from the following libraries:

* [ffmpeg](https://ffmpeg.org/)
* [NVorbis](https://github.com/ioctlLR/NVorbis)
* [gwen-dotnet](https://code.google.com/archive/p/gwen-dotnet/)
* [OpenTK](https://github.com/opentk/opentk)
* [Discord Game SDK](https://discord.com/)

You can find their license info in [LICENSES.txt](https://github.com/RatherBeLunar/LRA-Community-Edition/blob/master/LICENSES.txt).

The UI in LRA:CE is a modified version of gwen-dotnet by jealouscloud.

# License
LRA:CE is licensed under GPL3.
