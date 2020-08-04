# Bruteflow.Kafka

The list of basic blocks to construct a pipeline based on Bruteflow to consume and produce data from/to Kafka.

## Classes

The Bruteflow.Kafka offers a list abstract classes which are basic building blocks for a data flow pipeline.

### Pipelines

| Classes | Description |
| ------ | ------ |
| AbstractKafkaPipeline |  |
| AbstractKafkaPipelineWithMetrics | |
| AbstractKafkaPipelineCollection |  |

### Consumers

| Classes | Description |
| ------ | ------ |
| AbstractConsumerFactory |  |
| AbstractConsumerWithMetricsFactory | |
| KafkaConsumer |  |
| KafkaConsumerWithMetrics |  |

### Producers

| Classes | Description |
| ------ | ------ |
| AbstractProducerFactory |  |
| AbstractProducerWithMetricsFactory | |
| KafkaProducer |  |
| KafkaProducerWithMetrics |  |

### Serializer/Deserializers

| Classes | Description |
| ------ | ------ |
| ValueDeserializerMessagePackToType |  |
| ValueDeserializerToJObject |  |
| ValueSerializerJObjectToJsonString |  |
| ValueSerializerObjectToJsonString |  |

### Settings

| Classes | Description |
| ------ | ------ |
| KafkaConsumerSettings |  |
| KafkaProducerSettings |  |

