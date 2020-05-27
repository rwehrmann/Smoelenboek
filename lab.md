# Lab 3.1 - Create an Azure DevOps YAML Pipeline

## Prerequisites

* An Azure DevOps organization. Don't have one? Register one yourself via [link 1](https://app.vsaex.visualstudio.com/) or [link 2](https://azure.microsoft.com/en-us/services/devops/?nav=min) or [link 3](https://dev.azure.com/)

For the sake of simplicity we will import the [code](https://github.com/rwehrmann/Smoelenboek) in your Azure DevOps Git repo allowing you to work independent of others without the hassle of branching.
* In the left hand menu cLick on Repos > import > "https://github.com/rwehrmann/Smoelenboek.git"

## 1. Introduction

In this lab you will learn setting up a continuous integration (CI) pipeline for a dotNet core web application. You will create an Azure DevOps Pipeline in YAML that guarantees the quality of the software by testing the application after each change.

## 2. Create a Pipeline

* Navigate to the the Repos tab and click on "Set up build". The button is located in the upper right corner.
* Choose the starter pipeline.
* For the sake of time choose "commit directly to branch"
* Click save and run

## 3 The YAML schema

You will create a YAML pipeline that makes part of your code allowing you to version control the pipeline.
To learn the basics of YAML, see [Learn YAML in Y Minutes](https://learnxinyminutes.com/docs/yaml/).
Azure Pipelines doesn't support all YAML features. Unsupported features include anchors, complex keys, and sets. Also, unlike standard YAML, Azure Pipelines depends on seeing stage, job, task, or a task shortcut like script as the first key in a mapping.

### Pipeline structure

A pipeline is one or more stages that describe a CI/CD process. Stages are the major divisions in a pipeline. The stages "Build this app," "Run these tests," and "Deploy to preproduction" are good examples.

A stage is one or more jobs, which are units of work assignable to the same machine. You can arrange both stages and jobs into dependency graphs. Examples include "Run this stage before that one" and "This job depends on the output of that job."

A job is a linear series of steps. Steps can be tasks, scripts, or references to external templates.

This hierarchy is reflected in the structure of a YAML file like:

```yaml
Stage A
    Job 1
        Step 1.1
            Task (or script)
            Task (or script)
        Step 1.2
            Task (or script)
        ...
    Job 2
        Step 2.1
            Task (or script)
        Step 2.2
        ...
Stage B
```

See the [YAML schema documentation](https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema) for more info.

In this excersise we will use a single step. So we can omit stage and job.

### 3. 1 Add a task to build

* Click on 'pipeline' in the menu on the left
* Select your new pipeline
* Click on edit
* Remove the 2 script steps
* Use the Task selector on the right side and select "dotnet"
* choose the build task
* click add

### 3.2 Fix the build task

* Notice that the build fails.
* Inspect the logs to find the issue.
* Add a variable that points the right solution file.

```yaml
variables:
  solution: '**/*.sln'
```

In this case we use a file matching pattern allowing us to change the structure of the solution without the need to change the build pipeline.
More info on the file matching in a pipeline can be found [here](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/file-matching-patterns?view=azure-devops)

* Use solution variable as the value of the "path to the project" / "projects"
* Syntax of a variable is:

```yaml
$(variable_name)
```

#### 3.2.1 Debugging (optional)

* Add a variable to the pipeline

```yaml
variables:
  system.debug: true
```

## 4 Add versioning

The version of the software (files on disk) and the build do not correspond with each other. You can change this by change the name of the build pipline. In this case we make use of some predefined variable you can find [here](https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=azure-devops&tabs=yaml)
More details in the [docs](https://docs.microsoft.com/en-us/azure/devops/pipelines/process/run-number?view=azure-devops&tabs=yaml).

* Edit your build pipeline
* On the first line after the comments and before the 'trigger'

```yaml
name: $(date:yyyy).$(date:MM).$(date:dd)$(Rev:.r)
```

* Change the build task. Use the editor or settings of the task.
* Add these arguments

```yaml
/p:Version=$(Build.BuildNumber) /p:AssemblyVersion=$(Build.BuildNumber)
```

* The build task will look like

```yaml
- task: DotNetCoreCLI@2
  inputs:
    command: 'build'
    projects: '$(solution)'
    arguments: '/p:Version=$(Build.BuildNumber) /p:AssemblyVersion=$(Build.BuildNumber)'
```

## 5 Add a new task to run the unit tests

* Use the editor and copy and paste below task

```yaml
- task: DotNetCoreCLI@2
  displayName: 'dotNet test'
  inputs:
    command: 'test'
    projects: '$(build.sourcesDirectory)/**/tst/**/*.csproj'
    publishTestResults: true
    testRunTitle: 'Smoelenboek tests'
    arguments: '--no-build --no-restore'
```

* After the build ran notice the test report!
* Note the @2 suffix in the name of the task. This denotes the version of the task.

## 6 Change the test task to collect code coverage

Notice the code coverage report is empty. In this step you will add the neccassery tasks to collect the code coverage form the tests, create a report an publish the report.

* First, add some new variables

```yaml
  reportGeneratorLocation: '$(build.sourcesDirectory)/tools'
  codeCoverageReportLocation: '$(build.sourcesDirectory)/codecoverage'
```

* Change the test task by adding a new input:

```yaml
arguments: '/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Threshold=100 /p:ThresholdStat=Average --no-build --no-restore'
```

* Use a script task that installs the code coverage [report generator](https://github.com/danielpalme/ReportGenerator)
* Run the generator

```yaml
- script: |
    dotnet tool install dotnet-reportgenerator-globaltool --tool-path $(reportGeneratorLocation)
    $(reportGeneratorLocation)/reportgenerator -reports:$(build.sourcesDirectory)/**/coverage.cobertura.xml -targetdir:$(codeCoverageReportLocation) -reporttypes:Cobertura
  displayName: Create code coverage report
```

* Add a new task to publish code coverage report

```yaml
- task: PublishCodeCoverageResults@1
  displayName: 'Publish code coverage'
  inputs:
    codeCoverageTool: 'Cobertura'
    summaryFileLocation: '$(codeCoverageReportLocation)/Cobertura.xml'
    reportDirectory: '$(codeCoverageReportLocation)'
    failIfCoverageEmpty: true
```

We're using the script keyword here. It's a shorcut for task. The difference between a task (e.g. a powershell, command line or bash task) is.

Notice the usage of the | sign after the script keyword. This enables you to use multiline statements. Meaning that each line is a command.

### 6.1 Fail the build (optional)

A threshold of 100% code coverage is demanded. Let's see if this really works.

* Change the code of a tst by either removing a test or commenting out a test.
* Save the changes and watch the build.

## 7 Add a new task that publishes the web project

In this last step you are going to publish the web project, archive it using a custom name publish the zip to be used in the CD / release pipeline

* First, add some new variables

```yaml
  webProject: '**/Centric.Learning.Smoelenboek.csproj'
```

* Add a task that publishes the web project and define the webproject's location in variable called $(webProject)

```yaml
- script: |
    dotnet publish $(webProject) --output $(build.artifactstagingdirectory) --no-build --no-restore
  displayName: 'dotNet publish'
```

Note the usage of the predifined variable $(build.artifactstagingdirectory) in the dotnet publish argument. We will use this later. And note we use the script shortcut to call the dotnet cli instead of using the DotNetCoreCLI@2 task. Using the CLI gives you more control.

* Add a task to archive the output of the previous task

```yaml
- task: ArchiveFiles@2
  displayName: 'Zip published output'
  inputs:
    rootFolderOrFile: '$(build.artifactstagingdirectory)'
    includeRootFolder: false
    archiveType: 'zip'
    archiveFile: '$(build.artifactstagingdirectory)/$(Build.DefinitionName)-$(Build.BuildNumber).zip'
    replaceExistingArchive: true
    verbose: true
```

Notice the archive file name. The build number is used so u can easily identify it when downloading or viewing it on disk.

* Finally, publish the zip to a container location using the 'PublishBuildArtifacts@1' task.

```yaml
- task: PublishBuildArtifacts@1
  displayName: 'Publish Artifact $(Build.DefinitionName)'
  inputs:
    PathtoPublish: '$(build.artifactstagingdirectory)/$(Build.DefinitionName)-$(Build.BuildNumber).zip'
    ArtifactName: 'SmoelenBoek'
    publishLocation: 'Container'
```

* Once completed you can download the build artifacts form the build details page.

## Further reading and self study material

* https://docs.microsoft.com/en-us/learn/modules/create-a-build-pipeline/
* https://docs.microsoft.com/en-us/learn/browse/?roles=devops-engineer