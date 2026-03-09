namespace ExampleEventManager_DocumentIntelligence
{
    /// <summary>
    /// Provides instructions and an example for generating a human-readable event creation prompt from a Word document,
    /// formatted as JSON with a single Prompt property.
    /// </summary>
    /// <remarks>Use the provided example and guidelines to ensure the output matches the required format and
    /// includes all necessary event fields. The Status field must always be set to "Requested".</remarks>
    public class DocumentInstruction
    {
        /// <summary>
        /// Provides instructions and an example output for generating a human-readable event creation prompt in JSON
        /// format, based on a Word document and the specified OpenAPI event schema.
        /// </summary>
        /// <remarks>The prompt must follow the example output format, include all required event fields,
        /// and set the Status field to 'Requested' regardless of the document's value.</remarks>
        public static string Instruction = @"EXAMPLE OUTPUT:
{
 ""Prompt"": ""
I want to create an event with
 Name : PSG vs AJAX
 Description : A friendly match between the French and Dutch football team
 Start: 20/04/2026 18:00
 End: 202/04/2026 20:00
 Type: Pro
 Status: Requested

And following languages:
  Dutch as name, Surround Audio Type and European Subtitles as CC Supplier
  French as name, Surround Audio Type and European Subtitles as CC Supplier""
}

ADDITIONAL INFORMATIN

- You will be given a word document you need to translate it to a prompt as in the example output to create an event.
- The prompt needs to be human readable and contain carriage returns
- Make sure that the json returned is as in the format of the example output, with a single property Prompt which contains the human readable prompt.
- The fields of the event you can find I the below openapi spec YAML.
- The field Status should always have as value requested regardless of the value in the document.

OPENAPI SPEC YAML
openapi: 3.0.4
info:
  title: ExampleEventManager_UDAPI
  version: 1.0.0
servers:
  - url: 'https://{DataMinerSystemName}-{Organization}.on.dataminer.services/api/custom'
    description: User Defined API endpoint via cloud connection
    variables:
      DataMinerSystemName:
        default: ''
        description: The name of the DataMiner System
      Organization:
        default: ''
        description: The name of the organization
  - url: '{Protocol}://{BaseUrl}/api/custom'
    description: Local endpoint
    variables:
      Protocol:
        default: http
        description: 'The protocol to use, either http or https'
        enum:
          - http
          - https
      BaseUrl:
        default: localhost
        description: The base URL of the DataMiner System
paths:
  /eventmanager/events:
    get:
      tags:
        - Events
      summary: Retrieves a collection of Event objects based on the specified filter and order.
      description: ""Retrieve all Events ordered by start ascending (default)\nGET /eventmanager/events\n\nRetrieve Events with a filter and custom order\nGET  /eventmanager/events?filter=contains(Name,'AB')&orderby=Identifier asc\n\nRetrieve Events with a filter only\nGET /eventmanager/events?filter=Identifier eq '123e4567-e89b-12d3-a456-426614174000'""
      parameters:
        - name: filter
          in: query
          description: 'An OData filter string used to filter the results. If not specified, all items are returned.'
          schema:
            type: string
            default: ''
        - name: orderby
          in: query
          description: 'An OData orderby string used to order the results. You can specify multiple properties, e.g. ""Start desc, Name asc"". Default is ""Start"".'
          schema:
            type: string
            default: Start
      responses:
        '200':
          description: ''
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/Event'
        '400':
          description: ''
          content:
            application/json:
              schema:
                type: object
                items:
                  $ref: '#/components/schemas/Error'
                properties:
                  Title:
                    type: string
                  Details:
                    type: string
                  Code:
                    type: integer
                    format: int32
                  FaultingNode:
                    type: integer
                    format: int32
        '500':
          description: ''
          content:
            application/json:
              schema:
                type: object
                items:
                  $ref: '#/components/schemas/Error'
                properties:
                  Title:
                    type: string
                  Details:
                    type: string
                  Code:
                    type: integer
                    format: int32
                  FaultingNode:
                    type: integer
                    format: int32
    post:
      tags:
        - Events
      summary: Creates a new Event object in the repository.
      description: ""Create a new Event\nPOST /eventmanager/events\nContent-Type: application/json\n\n{\n\""Identifier\"": \""123e4567-e89b-12d3-a456-426614174000\"",\n\""Name\"": \""Empower 2026\"",\n...\n}""
      requestBody:
        content:
          application/json:
            schema:
              type: object
              items:
                $ref: '#/components/schemas/Event'
              properties:
                Name:
                  type: string
                Description:
                  type: string
                Start:
                  type: string
                  format: date-time
                End:
                  type: string
                  format: date-time
                Type:
                  enum:
                    - Basic
                    - Pro
                    - Advanced
                  type: string
                Status:
                  enum:
                    - Requested
                    - Processing
                    - Done
                  type: string
                Languages:
                  type: array
                  items:
                    type: object
                    properties:
                      Name:
                        type: string
                      AudioType:
                        enum:
                          - Stereo
                          - Surround
                          - Mono
                        type: string
                      CcSupplierCompanyName:
                        type: string
                Identifier:
                  type: string
        required: true
      responses:
        '201':
          description: ''
          content:
            application/json:
              schema:
                type: object
                items:
                  $ref: '#/components/schemas/Event'
                properties:
                  Name:
                    type: string
                  Description:
                    type: string
                  Start:
                    type: string
                    format: date-time
                  End:
                    type: string
                    format: date-time
                  Type:
                    enum:
                      - Basic
                      - Pro
                      - Advanced
                    type: string
                  Status:
                    enum:
                      - Requested
                      - Processing
                      - Done
                    type: string
                  Languages:
                    type: array
                    items:
                      type: object
                      properties:
                        Name:
                          type: string
                        AudioType:
                          enum:
                            - Stereo
                            - Surround
                            - Mono
                          type: string
                        CcSupplierCompanyName:
                          type: string
                  Identifier:
                    type: string
        '500':
          description: ''
          content:
            application/json:
              schema:
                type: object
                items:
                  $ref: '#/components/schemas/Error'
                properties:
                  Title:
                    type: string
                  Details:
                    type: string
                  Code:
                    type: integer
                    format: int32
                  FaultingNode:
                    type: integer
                    format: int32
    put:
      tags:
        - Events
      summary: Updates an existing Event object or creates it if it does not exist.
      description: ""Update an existing event or create if it does not exist\nPUT /eventmanager/events\nContent-Type: application/json\n\n{\n\""Identifier\"": \""123e4567-e89b-12d3-a456-426614174000\"",\n\""Name\"": \""Empower 2026\"",\n...\n}""
      requestBody:
        content:
          application/json:
            schema:
              type: object
              items:
                $ref: '#/components/schemas/Event'
              properties:
                Name:
                  type: string
                Description:
                  type: string
                Start:
                  type: string
                  format: date-time
                End:
                  type: string
                  format: date-time
                Type:
                  enum:
                    - Basic
                    - Pro
                    - Advanced
                  type: string
                Status:
                  enum:
                    - Requested
                    - Processing
                    - Done
                  type: string
                Languages:
                  type: array
                  items:
                    type: object
                    properties:
                      Name:
                        type: string
                      AudioType:
                        enum:
                          - Stereo
                          - Surround
                          - Mono
                        type: string
                      CcSupplierCompanyName:
                        type: string
                Identifier:
                  type: string
        required: true
      responses:
        '200':
          description: ''
          content:
            application/json:
              schema:
                type: object
                items:
                  $ref: '#/components/schemas/Event'
                properties:
                  Name:
                    type: string
                  Description:
                    type: string
                  Start:
                    type: string
                    format: date-time
                  End:
                    type: string
                    format: date-time
                  Type:
                    enum:
                      - Basic
                      - Pro
                      - Advanced
                    type: string
                  Status:
                    enum:
                      - Requested
                      - Processing
                      - Done
                    type: string
                  Languages:
                    type: array
                    items:
                      type: object
                      properties:
                        Name:
                          type: string
                        AudioType:
                          enum:
                            - Stereo
                            - Surround
                            - Mono
                          type: string
                        CcSupplierCompanyName:
                          type: string
                  Identifier:
                    type: string
        '201':
          description: ''
          content:
            application/json:
              schema:
                type: object
                items:
                  $ref: '#/components/schemas/Event'
                properties:
                  Name:
                    type: string
                  Description:
                    type: string
                  Start:
                    type: string
                    format: date-time
                  End:
                    type: string
                    format: date-time
                  Type:
                    enum:
                      - Basic
                      - Pro
                      - Advanced
                    type: string
                  Status:
                    enum:
                      - Requested
                      - Processing
                      - Done
                    type: string
                  Languages:
                    type: array
                    items:
                      type: object
                      properties:
                        Name:
                          type: string
                        AudioType:
                          enum:
                            - Stereo
                            - Surround
                            - Mono
                          type: string
                        CcSupplierCompanyName:
                          type: string
                  Identifier:
                    type: string
        '500':
          description: ''
          content:
            application/json:
              schema:
                type: object
                items:
                  $ref: '#/components/schemas/Error'
                properties:
                  Title:
                    type: string
                  Details:
                    type: string
                  Code:
                    type: integer
                    format: int32
                  FaultingNode:
                    type: integer
                    format: int32
    delete:
      tags:
        - Events
      summary: Deletes one or more Event objects based on the specified model identifier.
      description: ""Delete an event by identifier\nDELETE /eventmanager/events?modelId=123e4567-e89b-12d3-a456-426614174000""
      parameters:
        - name: modelId
          in: query
          description: The identifier of the model to delete.
          required: true
          schema:
            type: string
      responses:
        '500':
          description: ''
          content:
            application/json:
              schema:
                type: object
                items:
                  $ref: '#/components/schemas/Error'
                properties:
                  Title:
                    type: string
                  Details:
                    type: string
                  Code:
                    type: integer
                    format: int32
                  FaultingNode:
                    type: integer
                    format: int32
components:
  schemas:
    Event:
      type: object
      properties:
        Name:
          type: string
        Description:
          type: string
        Start:
          type: string
          format: date-time
        End:
          type: string
          format: date-time
        Type:
          enum:
            - Basic
            - Pro
            - Advanced
          type: string
        Status:
          enum:
            - Requested
            - Processing
            - Done
          type: string
        Languages:
          type: array
          items:
            type: object
            properties:
              Name:
                type: string
              AudioType:
                enum:
                  - Stereo
                  - Surround
                  - Mono
                type: string
              CcSupplierCompanyName:
                type: string
        Identifier:
          type: string
    Error:
      type: object
      properties:
        Title:
          type: string
        Details:
          type: string
        Code:
          type: integer
          format: int32
        FaultingNode:
          type: integer
          format: int32
  securitySchemes:
    BearerAuth:
      type: http
      description: The API key you created in DataMiner.
      scheme: bearer
security:
  - BearerAuth: [ ]";

    }
}
