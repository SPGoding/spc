# SPC Tool

[简体中文版](./README-zh.md) | **Poor English Edition**

## What is This

A program written in C# to get a map of old resource pack file names and new format names.

## How to Use

To run The Tool for Windows, execute this command in your command line:

```cmd
$ F:\fakepath > SpcTool.exe <detail mode> <type> <path 1> <path 2> [path 3]
```

For Linux or Mac, who cares? #run

<!-- Awful table -->

| Parameter          | Type   | Description                                                                                                                                                                                                                                                                                                                           |
| ------------------ | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| detail mode               | Boolean | _Required_. If set to `true`, The Tool will be very slow. |
| type               | String | _Required_. Can be one of these values: `bb`, `ts` and `both`. <br> **bb**: The output file will be written in BBCode style, which can be pasted in [MCBBS](http://www.mcbbs.net) directly and published. <br> **ts**: The output file will be written as Typescript code. <br> **both**: Both `bb` and `ts` files will be generated. |
| path 1 <br> path 2 | String | _Required_. A directory path of extracted Minecraft core jar. The Tool will compare them automaticly.                                                                                                                                                                                                                                    |
| path 3             | String | _Optional_. A directory where the output files will be located. If not defined, files will be put in `./Output`                                                                                                                                                                                                                       |

## How it Works

The Tool will:

1.  Extract two core jars to `%OUTPUT%/Extracts/`.
1.  Push file names and file md5 in `assets` in each jar to two different maps.
1.  Compare the maps. Push the name of files which have the same md5 to a new map.
1.  Format the new map as the specified type (`bb`, `ts` or `both`), and output to `%OUTPUT%/`
1.  Delete `%OUTPUT%/Extracts/`.
