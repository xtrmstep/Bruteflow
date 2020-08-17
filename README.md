![Publish Bruteflow NuGet Package](https://github.com/xtrmstep/Bruteflow/workflows/Publish%20Bruteflow%20NuGet%20Package/badge.svg) 
![Publish Bruteflow.Kafka NuGet Package](https://github.com/xtrmstep/Bruteflow/workflows/Publish%20Bruteflow.Kafka%20NuGet%20Package/badge.svg) 
![Publish Bruteflow.Kafka.Stats NuGet Package](https://github.com/xtrmstep/Bruteflow/workflows/Publish%20Bruteflow.Kafka.Stats%20NuGet%20Package/badge.svg)

# Bruteflow libraries

Simple synchronous pipeline builder which supports well-known flowchart blocks and offers basic bulding blocks to construct Kafka-based DAG pipeline.

This is no way a replcement for Dataflow (Task Parallel Library). It uses another approach for building a pipeline.

## Libraries

| Library | Description |
| ------ | ------ |
| [Bruteflow](/src/Bruteflow/README.md) | Basic building blocks to contruct a dataflow pipeline. |
| [Bruteflow.Kafka](/src/Bruteflow.Kafka/README.md) | Implementation of basic blocks to contruct pipeline which consumes and produces data from Kafka topic as a stream. |
| [Bruteflow.Kafka.Stats](/src/Bruteflow.Kafka.Stats/README.md) | Implementation of StatsD metric capturing. Uses [JustEat.StatsD](https://github.com/justeat/JustEat.StatsD). |

## Alternatives

* [elsa-workflows](https://github.com/elsa-workflows/elsa-core) [![Stars](https://img.shields.io/github/stars/elsa-workflows/elsa-core.svg)](https://github.com/elsa-workflows/elsa-core) - A .NET Standard 2.0 Workflows Library.
* [Workflow Engine](https://workflowengine.io) - A lightweight .NET and Java workflow engine.
* [CoreWF](https://github.com/UiPath/corewf) [![Stars](https://img.shields.io/github/stars/UiPath/corewf.svg)](https://github.com/UiPath/corewf) - WF runtime ported to work on .NET Core
* [Workflow Core](https://github.com/danielgerlag/workflow-core) [![Stars](https://img.shields.io/github/stars/danielgerlag/workflow-core.svg)](https://github.com/danielgerlag/workflow-core) - Lightweight workflow engine for .NET Standard
* [WorkflowEngine.NET](https://github.com/optimajet/WorkflowEngine.NET) [![Stars](https://img.shields.io/github/stars/optimajet/WorkflowEngine.NET.svg)](https://github.com/optimajet/WorkflowEngine.NET) - WorkflowEngine.NET - component that adds workflow in your application. It can be fully integrated into your application, or be in the form of a specific service (such as a web service)
