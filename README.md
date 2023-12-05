# EasySave

<p align="center" width="100%">
 <img alt="EasySave logo" src="./assets/easysave-logo@128.png" width="128px" />
</p>


## Table of Contents
**[1. Description](#description)**<br/>
**[1. Project Structure](#project-structure)**

* [1.1. Solution structure](#solution-structure)
* [1.2. CLI](#cli)
* [1.3. Lib](#lib)
* [1.4. Communication between CLI and Lib](#comm-lib-cli)

**[2. Sequence diagrams](#sequence-diagrams)**
* [2.1. New backup job](#new-backup-job)
* [2.2. Start backup job](#start-backup-job)

**[3. User Documentation](#user-documentation)**
* [3.1. CLI](#cli-1)
* [3.2. Examples](#examples)


## Badges

<div id="description"/>

## Description 

EasySave is a software developed on .NET by ProSoft. It allows to create, manage and
execute backup jobs with different modes and settings.

<div id="project-structure"/>

## Project structure

<div id="solution-structure"/>

### Solution structure

The solution contains 3 projects:

- EasyCLI: the command line interface of the software
- EasyLib: the library containing the core of the software
- EasySaveTests: the unit tests of the software

![Project structure](./assets/project-structure.png)

Open interactive diagrams 
[on Figma](https://www.figma.com/file/69B3eZT084VoueoZVX9qXm/ProgSystem?type=whiteboard&node-id=0%3A1&t=XkVQ1kuQdN3ifJBx-1).

<div id="cli"/>

### CLI

This project contains the command line interface of the software. It is the entry point from the CLI.
It handle the arguments and call the library to execute the backup jobs.
Here is the Class Diagram of the CLI

![CLI Class Diagram](./assets/ProgSystemCLI.png)
[Class diagram on Figma](https://www.figma.com/file/69B3eZT084VoueoZVX9qXm/ProgSystem?type=whiteboard&node-id=1-2743&t=XkVQ1kuQdN3ifJBx-4)

<div id="lib"/>

### Lib

This project contains the core of the software. It contains the backup jobs, the backup modes, the backup settings and
the file management.

![Lib Class Diagram](./assets/ProgSystemLib.png)
[Class diagram on Figma](https://www.figma.com/file/69B3eZT084VoueoZVX9qXm/ProgSystem?type=whiteboard&node-id=1-5137&t=XkVQ1kuQdN3ifJBx-4)

<div id="comm-lib-cli"/>

### Communication between CLI and Lib

The `CLI` instantiate `JobManager` from the `Lib`.

The `CLI` subscribe to the `JobStatusListener` observer of the `JobState`.

<div id="sequence-diagrams"/>

## Sequence diagrams

<div id="new-backup-job"/>

### New backup job

This sequence diagram describe the creation of a new backup job.

[Job creation sequence diagram on Figma](https://www.figma.com/file/69B3eZT084VoueoZVX9qXm/ProgSystem?type=whiteboard&node-id=5-5174&t=XkVQ1kuQdN3ifJBx-4)
![Job creation sequence diagram on Figma](./assets/ProgSystemNew.png)

<div id="start-backup-job"/>

### Start backup job

This sequence diagram describe the start of a backup job.

![Job start sequence diagram on Figma](./assets/ProgSystemStart.png)
[Job start sequence diagram on Figma](https://www.figma.com/file/69B3eZT084VoueoZVX9qXm/ProgSystem?type=whiteboard&node-id=5-5323&t=XkVQ1kuQdN3ifJBx-4)

<div id="user-documentation"/>

## User Documentation

<div id="cli-1"/>

### CLI

To use easy save, you need to use the command line interface and run the `easysave.exe` with the command:<br/>
`$ Path/To/File/easysave.exe [command] [options]`

#### Available commands

Display help on other commands: <br/>
`easysave help`

Get the version of the application: <br/>
`easysave version`

List all the jobs in the state file: <br/>
`easysave list`

Checks if the config is valid, and if the state matches all the rules: <br/>
`easysave check [jobs]`

Create a new job in the state file: <br/>
`easysave create <srcPath> <destPath> [--type <{full}|differential>] [-n <name>]` 

Delete a job from the state file: <br/>
`easysave delete <id|name>`

Run selected jobs: <br/>
`easysave run <jobs>`

Discard the state of running jobs: <br/>
`easysave discard <jobs>`

#### Files
Esaysave use 2 files to store the state of the jobs and the logs. They are located in `SystemUserFiles/easySave`.

#### Source and destination paths

The source and destination paths can be either a file or a director located on the local machine or on a network drive or a removable drive.

<div id="examples"/>

### Examples


## Authors

- [Houille Lukas](https://github.com/lukas-houille)
- [Wolff Julien](https://github.com/julien-wff)