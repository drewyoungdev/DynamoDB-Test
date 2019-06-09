const AWS = require('aws-sdk');
const { createServer } = require('dynamodb-admin');

AWS.config.update({
    region: "us-east-1",
    endpoint: "http://localhost:8000"
});

process.env.AWS_ACCESS_KEY_ID = "xxx";
process.env.AWS_SECRET_ACCESS_KEY = "yyy";

const dynamodb = new AWS.DynamoDB();
const dynClient = new AWS.DynamoDB.DocumentClient({ service: dynamodb });

const app = createServer(dynamodb, dynClient);

const port = 8001;
const server = app.listen(port);
server.on('listening', () => {
    const address = server.address();
    console.log(`  listening on http://0.0.0.0:${address.port}`);
});
