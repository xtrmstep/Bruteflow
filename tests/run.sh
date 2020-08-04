#!/bin/bash

trap 'docker-compose down' EXIT

docker-compose up -d && \
dotnet build ./../src/Bruteflow.Kafka.Tests/Bruteflow.Kafka.Tests.csproj && \
dotnet test ./../src/Bruteflow.Kafka.Tests/Bruteflow.Kafka.Tests.csproj
