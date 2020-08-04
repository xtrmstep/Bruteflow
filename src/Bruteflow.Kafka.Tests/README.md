# Testing of Bruteflow.Kafka

I use docker-compose stack to deploy one-node Kafka cluster on your local Docker. Install Docker before running code below.

In order to test from IDE, you will need to have deployed Kafka cluster. After each run you'll need to delete created topics. If you don't want to do that, you can use Bash script provided in [tests](/tests) folder in the root of the repository.

Before running Bash script, make sure you're located in _tests_ folder.

```bash
./run.sh
```

It deploys Kafka, build and run tests of Bruteflow.Kafka and shuts down the cluster.

## Test scenario

* Using blocks of Bruteflow.Kafka the test sends 100 event to a Kafka topic.
* The pipeline listens the Kafka topic and pushes to the Bruteflow pipeline which produces event to another topic in Kafka
* The test verifies that all events passed through the pipeline
