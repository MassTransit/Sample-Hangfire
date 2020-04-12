# Sample MassTransit Hangfire Scheduler

Solution contains 3 projects:

- *Sample.Hangfire.AspNetCore* - Setup with ASP.NET Core with DI
- *Sample.Hangfire.Console* - Setup with Console application and static configuration
- *Sample.Hangfire.Publisher* - Simple console application to schedule a message

## ASP.NET Core

This project contains 2 endpoints:

- `/health` - to check application is healthy
- `/hangfire` - simple Hangfire Dashboard

## Requirements

`rabbitmq` - up and running and change `Rmq` connection string to use your broker.

