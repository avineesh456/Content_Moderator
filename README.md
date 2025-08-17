AI-Powered Content Moderator Microservice

This project is a dockerized .NET microservice that provides real-time content moderation using a custom-trained ML.NET model. It analyzes input text to predict its toxicity and is architected for high performance and observability, making it a robust foundation for real-world applications.

Features
Real-time Toxicity Prediction: A RESTful API endpoint for instant text analysis.

Custom ML.NET Model: Utilizes a binary classification model trained to detect toxic content.

High-Performance Caching: Integrated with a Redis cache to store recent predictions, significantly reducing latency for repeated requests.

Containerized Environment: Fully containerized with Docker and managed via Docker Compose for consistent, one-command setup and deployment.

Structured Logging: Implemented with Serilog to provide detailed operational logs, including request tracing, cache performance, and error diagnostics.

Tech Stack
Category	Technology / Library
Backend Framework:	.NET 9, ASP.NET Core Web API

Machine Learning:	ML.NET

Caching;	Redis (via Microsoft.Extensions.Caching.StackExchangeRedis)

Containerization:	Docker, Docker Compose

Logging:	Serilog (Sinks for Console and File)

CSV Data Processing:	CsvHelper Export to Sheets

System Architecture

The service operates on a simple, scalable request-response flow. Incoming requests are first checked against the Redis cache. If the result is found (a cache hit), it's returned immediately. If not (a cache miss), the request is passed to the ML.NET model for prediction, and the result is then cached for future requests before being returned to the client.

Code snippet

sequenceDiagram
    participant Client
    participant API (Docker)
    participant Redis Cache
    participant ML.NET Model

    Client->>+API (Docker): POST /Sentiment/predict with text
    API (Docker)->>+Redis Cache: Check for cached prediction
    alt Cache Hit
        Redis Cache-->>-API (Docker): Return cached result
        API (Docker)-->>-Client: Prediction Response
    else Cache Miss
        Redis Cache-->>-API (Docker): No result found
        API (Docker)->>+ML.NET Model: Get prediction for text
        ML.NET Model-->>-API (Docker): Return ModelOutput
        API (Docker)->>+Redis Cache: Store new prediction
        Redis Cache-->>-API (Docker): Confirm storage
        API (Docker)-->>-Client: Prediction Response
    end
    
Getting Started

Follow these instructions to get a local copy up and running for development and testing.

Prerequisites
.NET 9 SDK

Docker Desktop

Configuration & Setup

Clone the repository:

Bash

git clone <your-repository-url>
cd <repository-name>

Place the Model File:

Ensure your trained ML.NET model, named model.zip, is present in the root of the main web API project folder.

Verify Configuration:

The appsettings.Development.json file should contain the connection string for Redis. This is pre-configured for Docker Compose.

JSON

"ConnectionStrings": {
  "Redis": "redis:6379"
}
Running the Application
The entire application stack (Web API and Redis) is managed by Docker Compose.

Open a terminal in the root directory of the repository (where docker-compose.yml is located).

Run the application:

Bash

docker compose up --build
Access the API:
Once the containers are running, the API will be available. You can access the Swagger UI for testing at:
http://localhost:8080/swagger

API Endpoint
Predict Toxicity
Analyzes a string of text and returns a prediction on whether it is toxic.

URL: /Sentiment/predict

Method: POST

Request Body:

JSON

{
  "sentimentText": "This is some text to analyze."
}
Success Response (200 OK):

JSON

{
  "text": "This is some text to analyze.",
  "isToxic": false,
  "probability": 0.08,
  "score": -2.45
}

Future Improvements

This project serves as a strong foundation. Future work could include:

Periodic training of the model. Have crawler services which crawl actual conversations and append data to the master data-set. Automatically retrain the model on this updated data (monthly).

CI/CD Pipeline: Automate the build, testing, and deployment process using GitHub Actions or Azure DevOps.

Orchestration: Deploy the application to a container orchestrator like Kubernetes (or Azure Container Apps) for better scalability and management.

Managed Cloud Services: Utilize managed cloud services like Azure Cache for Redis and Azure App Service for production environments.

Centralized Logging: Configure Serilog to ship logs to a centralized platform like Application Insights, Seq, or an ELK stack for robust monitoring.
