# EasySave

## Badges

## Description

EasySave is a software developed on .NET by ProSoft. It allows to create, manage and
execute backup jobs with different modes and settings.

## Project structure

### Solution structure
This image describe the solution structure of the project. It contains 3 projects:
- EasyCLI: the command line interface of the software
- EasyLib: the library containing the core of the software
- EasySaveTests: the unit tests of the software

![Project structure](./assets/project-structure.png)

Open interactive diagrams [open figma online](https://www.figma.com/file/69B3eZT084VoueoZVX9qXm/ProgSystem?type=whiteboard&node-id=0%3A1&t=XkVQ1kuQdN3ifJBx-1)

### CLI
This project contains the command line interface of the software. It is the entry point from the CLI.
It handle the arguments and call the library to execute the backup jobs.
Here is the Class Diagram of the CLI

[Open class diagram online](https://www.figma.com/file/69B3eZT084VoueoZVX9qXm/ProgSystem?type=whiteboard&node-id=1-2743&t=XkVQ1kuQdN3ifJBx-4)
![CLI Class Diagram](./assets/ProgSystemCLI.png)


### Lib
This project contains the core of the software. It contains the backup jobs, the backup modes, the backup settings and the file managment.

[Open class diagram online](https://www.figma.com/file/69B3eZT084VoueoZVX9qXm/ProgSystem?type=whiteboard&node-id=1-5137&t=XkVQ1kuQdN3ifJBx-4)
![Lib Class Diagram](./assets/ProgSystemLib.png)

### Communication between CLI and Lib
The `CLI` instantiate `JobManager` from the `LIB`.

The `CLI` subscribe to the `JobStatusListener` observer of the `JobState`.

## Sequence diagrams

### New backup job
This sequence diagram describe the creation of a new backup job.

[Open sequence diagram online](https://www.figma.com/file/69B3eZT084VoueoZVX9qXm/ProgSystem?type=whiteboard&node-id=5-5174&t=XkVQ1kuQdN3ifJBx-4)
![New backup job sequence diagram](./assets/ProgSystemNew.png)

### Start backup job
This sequence diagram describe the start of a backup job.

[Open sequence diagram online](https://www.figma.com/file/69B3eZT084VoueoZVX9qXm/ProgSystem?type=whiteboard&node-id=5-5323&t=XkVQ1kuQdN3ifJBx-4)
![New backup job sequence diagram](./assets/ProgSystemStart.png)
