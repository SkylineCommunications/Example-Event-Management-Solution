# Example Event Manager

## About

The **Example Event Manager** is a sample application that demonstrates what you can create using the [Event Manager backend](https://catalog.dataminer.services/details/7c402377-ce88-479f-a249-5c086b5d84bd) and the [EventManagement C# Nuget](https://www.nuget.org/packages/Skyline.DataMiner.Learning.EventManagement/).

This backend is build using the Standard Data Model principles.

## Key features

This package allows you to do the following:

- **Creating dashboards and low-code apps** using the provided ad hoc data sources.
- **Integrate with third parties** using the included web API.
- **Integrate with PowerBI** using the included web API.
- **Leverage Document Intelligence** - Analyze a Microsoft Word document and create an event based on its contents.
- **Example of including prompts** - A showcase on how you can leverage the backend to allow an LLM to process prompts specific to events.

## Prerequisites

- [Standard Data Model Registration](https://catalog.dataminer.services/details/52173e49-9185-4772-9b60-c186ee365a81)
- [Example Event Manager Backend](https://catalog.dataminer.services/details/7c402377-ce88-479f-a249-5c086b5d84bd)

## Package contents

### Ad hoc data sources

Ad hoc data sources that allow you to create dashboards and low-code apps that retrieve the events and the languages of each event.

### User-defined API

Make sure to configure the *ExampleEventManager_UDAPI* script with a bearer token, either via Cube or via the *Downloads* page of the low-code app. On that *Downloads* page, you can also download the openapi spec or postman collection.

All information can be retrieved via a web API.

![Postman](./Images/UDAPI_Postman.png)

### Low-code application

### Events page

The *Events* page allows you to upload and analyze a document in order to create an event.

You can enter a prompt to filter out events, or to create, edit, delete events.

![Events page](./Images/eventsPage.png)

### Downloads page

The *Downloads* page allows you to download the event example input form and an openapi spec and postman collection.

![Downloads page](./Images/downloadsPage.png)
