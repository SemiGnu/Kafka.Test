# Kafka.Test

Demo of Kafka/Debezium.
https://kafka.apache.org/
https://debezium.io/

### Setup
Open powershell to the solution folder, and enter the following:
```
docker-compose up -d
```
The compose file launches four services:

 1. MySql.
 2. Zookeeper. Kafka uses this to store metadata of connections, etc.
 3. Kafka. The event broker. 
 4. Debezium connector. This service listens to changes in the MySql database and sends messages.

When the services are up and running, the connector must be configured to listen to the database. Make following request with Git Bash, or import it to Postman:
```
curl --location --request POST 'localhost:8083/connectors/' \
--header 'Accept: application/json' \
--header 'Content-Type: application/json' \
--data-raw '{
    "name": "mystore-connector",
    "config": {
        "connector.class": "io.debezium.connector.mysql.MySqlConnector",
        "tasks.max": "1",
        "database.hostname": "mysql",
        "database.port": "3306",
        "database.user": "debezium",
        "database.password": "dbz",
        "database.server.id": "223345",
        "database.server.name": "mysql",
        "database.whitelist": "mystore",
        "database.history.kafka.bootstrap.servers": "kafka:9092",
        "database.history.kafka.topic": "dbhistory.mystore",
        "transforms": "unwrap",
        "transforms.unwrap.type": "io.debezium.transforms.UnwrapFromEnvelope",
        "transforms.unwrap.drop.tombstones": "false",
        "key.converter": "org.apache.kafka.connect.json.JsonConverter",
        "key.converter.schemas.enable": "false",
        "value.converter": "org.apache.kafka.connect.json.JsonConverter",
        "value.converter.schemas.enable": "false",
        "include.schema.changes": "false"
    }
}'
```
You can then make sure it is running with this request:
```
curl --location --request GET 'localhost:8083/connectors/mystore-connector/status' \
--header 'Accept: application/json'
```
Finally, you must add the following line to your hosts file (```c:\Windows\System32\Drivers\etc\hosts```):
```
127.0.0.1       kafka
```

### Running
Now you can run the project.
Open the database with your favourite db client.
```
host: localhost
user: root
password: 123456
```
Any changes to mystore.products should automatically propagate to my_other_store.Products.

The events can also be inspected with Conduktor on localhost:9092.
https://www.conduktor.io/
