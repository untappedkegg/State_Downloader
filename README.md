# Template Program
Commandline program to download, process, and update **\<INSERT\>** info, geocoding the address if necessary.

## Steps taken by program
1. Download the files from **\<INSERT\>** (or whatever location is specified in the Config.xml)
2. Process the records
3. Load the records into SQL Server


## Building
This was built with Visual Studio 2019 using .Net Core 3 Preview.

## Running the Project
 1. Double click the executable, or launch from the command line.
 2. The program will attempt to download the **\<INSERT\>**, then process and insert the records into the database.
 3. Config.xml must always have that exact name, and must always appear next to the executable.
 4. 4.  There are different executables to run depending on what operating system
    you would like to run it from. **All versions require a 64-bit operating system**.

    a.  Windows =\> **\<ConsoleTemplate\>**.exe

    b.  Linux =\> **\<ConsoleTemplate\>**_Linux

    c.  MacOS =\> **\<ConsoleTemplate\>**_MacOS


### Config Options
Config.xml contains several parameters for altering the behavior of the program

**showInfo:** Whether or not the program should be verbose about the actions it
is performing, such as which file(s) are being downloaded, etc.

	Default: true

**server:** The fully qualified name of the database server to use.

	Default: **\<INSERT\>**

**user:** The user/account that the program should connect to the database
server as. This could be left blank on Windows as it will use your login
credentials to connect to the database

**password:** The password for the user/account that the program should connect
to the database as. This could be left blank on Windows as it will use your
login credentials to connect to the database

**database:** The database the table is in

	Default: TEMPLATE

**schema:** The schema the table is in

	Default: dbo

**tableName:** The to use

	Default: **\<INSERT\>**

**urlBase:** The URL to download the file from

	Default: **\<INSERT\>**

**deleteDownloadsOnSuccess:** Whether to delete the files that were downloaded when the program finishes executing

	Default: true

**zipFormat:** The file name template to download. 

	Default: **\<INSERT\>**.zip
