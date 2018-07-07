# SPC

[简体中文版](./README-zh.md) | **Poor English Edition**

## What is This?

An online tool to convert Minecraft: Java Edition resource packs from 1.12 to 1.13.

## How to Use?

Upload your 1.12 resource pack at [this page](https://spgoding.github.io/spc), then click [CONVERT].

## How it Works?

\* _Need improve._

After users click [CONVERT], SPC will:

1. Unzip the resource pack.
2. Upgrade `/pack.mcmeta`. The `format` will be changed into `4`.
3. Upgrade `/assets/blockstates`.
4. Upgrade `/assets/lang`. Files will be changed from `.lang` to `.json`.
5. Upgrade `/assets/models`.
6. Upgrade `/assets/textures`. Rename files.
7. Zip the resource pack. Provide download link.

All these action **WILL** be done at the broswer.

## How to Contribute?

Click [Fork], and then type `git clone <YOUR REPO>` in your command line.

After fixing bugs or/and adding feature, you can send me a *Pull Request*, I will accept it if I am glad.

Have a nice day!