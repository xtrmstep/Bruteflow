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
