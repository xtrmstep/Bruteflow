# Bruteflow.Kafka

The list of basic blocks to construct a pipeline based on Bruteflow to consume and produce data from/to Kafka.

## Methodology

The library clearly defines building blocks and their relationship. In order to create a pipeline you will need to follow a set of simple steps:

- Inherit from one of the abstract pipelines
  - One pipeline has only one consumer
  - One pipeline has only one Bruteflow pipeline
  - One pipeline can have several producers (because Bruteflow pipeline can have multiple sinks)
- Inherit consumer and producer factory classes
  - One factory class can create only one type of consumer/producer
  - If you need different consumers/producers, then inherit them from the corresponding abstract classes
  - Serialization and deserialization can be configured via factory classes
- Configure dependencies using `.AddBruteflowKafkaPipelines()` extension of `IServiceCollection`
- Execute the pipeline

The Bruteflow.Kafka supposed to be used inside worker process. In order to stop pipeline execution the method `ReadNextEntity()` of the pipeline class should return `False`. Another option is to use cancellation token to stop the process.

## Classes

The Bruteflow.Kafka offers a list abstract classes which are basic building blocks for a data flow pipeline.

### Pipelines

| Classes | Description |
| ------ | ------ |
| AbstractKafkaPipeline | Base Kafka pipeline class |
| AbstractKafkaPipelineCollection | Base class for a collection of pipelines to simplify controlling of a set of pipelines |

### Consumers

| Classes | Description |
| ------ | ------ |
| AbstractConsumerFactory | Base class for a consumer factory |
| KafkaConsumer | Basic class for consumer. You may use it in simple scenarios |

### Producers

| Classes | Description |
| ------ | ------ |
| AbstractProducerFactory | Base class for a producer factory |
| KafkaProducer | Basic class for producer. You may use it in simple scenarios |

### Serializer/Deserializers

The library should support all standard classes and was specifically tested for `JObject` and `MessagePack`. You can inject your own logic using DI.

The type which is used as a destination one for `MessagePack` deserializer should be declared with `System.Runtime.Serialization` attributes, such as `DataContract` for a class and `DataMember(Order = #)` for properties.

Json serialization/deserialization is implemented using `Newtonsoft.Json`, because it's more flexible.

| Classes | Description |
| ------ | ------ |
| ValueDeserializerMessagePackToType | Used in consumer. Deserialize value from `MessagePack` to a type  |
| ValueDeserializerToJObject | Used in consumer. Deserializes string value to a `JObject` object |
| ValueSerializerJObjectToJsonString | Used in producer. Serialize a `JObject` to a Json string representation |
| ValueSerializerObjectToJsonString | Used in producer. Serialize a C# object to a Json string representation |

### Settings

| Classes | Description |
| ------ | ------ |
| KafkaConsumerSettings | Consumer settings |
| KafkaProducerSettings | Producer settings |

## Metrics

There are classes with `WithMetrics` in their names. Such classes sends metrics on specific events. By default, the a silent implementation of metric publisher is used.

## Example

Check the test of Bruteflow.Kafka [here](/src/Bruteflow.Kafka.Tests/README.md)