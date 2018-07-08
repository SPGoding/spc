# SPC

[简体中文版](./README-zh.md) | **Poor English Edition**

## What is This

An online tool to convert Minecraft: Java Edition resource packs from format 3 to format 4.

## How to Use

Select your resource pack at [this page](https://spgoding.github.io/spc) and click [CONVERT].

## How it Works

\* _Need improve._

Before published, SPGoding ran a tool written in C# to collect a mapping of old format file names and new format file names. The tool is located in `/tool/tool.exe`. [Click here](./tool/README.md) to get more information.

And then SPGoding wrote these mappings hardcode.

After users click [CONVERT], SPC will:

1.  Unzip the resource pack.
1.  Upgrade `/pack.mcmeta`. The `format` will be changed to `4`.
1.  Upgrade `/assets/blockstates`.
1.  Upgrade `/assets/lang`. Files will be changed from `.lang` to `.json`.
1.  Upgrade `/assets/models`.
1.  Upgrade `/assets/textures`. Rename files.
1.  Zip the resource pack. Provide a download link.

All these operations **WILL** be done at the broswer.

## How to Contribute

Click [Fork], and then type `git clone <YOUR REPO>` in your command line.

After fixing bugs or/and adding feature, you can send me a _Pull Request_, I will accept it if I am glad.

Have a nice day!
