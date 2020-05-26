# Lab 3.1 - Create an Azure DevOps YAML Pipeline

## Prerequisites
* An Azure DevOps organization. Register an Azure DevOps Organization via https://app.vsaex.visualstudio.com/ or https://azure.microsoft.com/en-us/services/devops/?nav=min or https://dev.azure.com/
* Use the "start free" option.

The project we are using https://github.com/rwehrmann/Smoelenboek
To make sure you use your own copy import the code in your Azure DevOps Git repo. 
* In the left hand menu cLick on Repos > import > "https://github.com/rwehrmann/Smoelenboek.git"

## 1. Introduction
In this lab you will learn setting up a continuous integration (CI) pipeline for the application. You will create an Azure DevOps Pipeline in YAML that guarantees the quality of the software by testing the application after each change.

## 2. Create a Pipeline
* Navigate to the the Repos tab and click on "Set up build". The button is located in the upper right corner.
* Choose the starter pipeline.
* For the sake of time choose "commit directly to branch"
* Click save and run

## 3 The YAML schema 
### Pipeline structure
A pipeline is one or more stages that describe a CI/CD process. Stages are the major divisions in a pipeline. The stages "Build this app," "Run these tests," and "Deploy to preproduction" are good examples.

A stage is one or more jobs, which are units of work assignable to the same machine. You can arrange both stages and jobs into dependency graphs. Examples include "Run this stage before that one" and "This job depends on the output of that job."

A job is a linear series of steps. Steps can be tasks, scripts, or references to external templates.

This hierarchy is reflected in the structure of a YAML file like:

```Pipeline
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
See: https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema

### 3. 1 Add a task to build
* Click on 'pipeline' in the menu on the left
* Select your new pipeline
* Click on edit
* Remove the 2 script steps
* Use the Task selector on the right side and select "dotnet"
* choose the build task
* click add

### 3.2 Fix the build task
* Notice that is fails.
* Inspect the logs
* Add a variable that point the right solution file
```
variables:
  solution: '**/*.sln'
```
* Use solution variable as the value of the "path to the project" / "projects"
* Syntax of a variable is 
```
$(variable_name)
```

#### 3.2.1 Debugging (optional)
* Add a variable to the pipeline

```
variables:
  system.debug: true
```

## Add versioning
The version of the software (files on disk) and the build do not correspond with each other. You can change this by change the name of the build pipline. In this case we make use of some predefined variable you can find [here](https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=azure-devops&tabs=yaml)
* Edit your build pipeline
* On the first line after the comments and before the 'trigger'
```
name: $(date:yyyy).$(date:MM).$(date:dd)$(Rev:.r)
```
* Change the build task. Use the editor or settings of the task.
* Add these arguments
```
/p:Version=$(Build.BuildNumber) /p:AssemblyVersion=$(Build.BuildNumber)
```
* The build task will look like
```
- task: DotNetCoreCLI@2
  inputs:
    command: 'build'
    projects: '$(solution)'
    arguments: '/p:Version=$(Build.BuildNumber) /p:AssemblyVersion=$(Build.BuildNumber)'
```
## 4 Add a new task to run the unit tests
* Use the editor and copy and paste below task
```
- task: DotNetCoreCLI@2
    displayName: 'dotNet test'
    inputs:
      command: 'test'
      projects: '$(build.sourcesDirectory)/**/tst/**/*.csproj'
      publishTestResults: true
      testRunTitle: 'Smoelenboek tests'
```
* After the build ran notice the test report!

## 5 Change the test task to collect code coverage 

* Notice the code coverage report is empty
* Change the test task by adding a new input:
```
arguments: '/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Threshold=40 /p:ThresholdStat=Average'
```
* Use a script task that installs the code coverage [report generator](https://github.com/danielpalme/ReportGenerator)
* Run the generator
```
- script: |
    dotnet tool install dotnet-reportgenerator-globaltool --tool-path $(reportGeneratorLocation)
    $(reportGeneratorLocation)/reportgenerator -reports:$(build.sourcesDirectory)/**/coverage.cobertura.xml -targetdir:$(codeCoverageReportLocation) -reporttypes:Cobertura
    displayName: Create code coverage report
```

* Add a new task to publish code coverage report
```
- task: PublishCodeCoverageResults@1
  displayName: 'Publish code coverage'
  inputs:
    codeCoverageTool: 'Cobertura'
    summaryFileLocation: '$(codeCoverageReportLocation)/Cobertura.xml'
    reportDirectory: '$(codeCoverageReportLocation)'
    failIfCoverageEmpty: true
```
## 7 Add a new job with a new task to publish the artifacts but only when test job is ok


## Further reading and self study:
https://docs.microsoft.com/en-us/learn/modules/create-a-build-pipeline/ and https://docs.microsoft.com/en-us/learn/browse/?roles=devops-engineer
Jobs can run in parallel but cost money unless you have a public project (https://centric-sp.visualstudio.com/_settings/buildqueue?_a=concurrentJobs or https://docs.microsoft.com/en-us/azure/devops/pipelines/licensing/concurrent-jobs?view=azure-devops)