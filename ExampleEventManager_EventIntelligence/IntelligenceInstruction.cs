namespace ExampleEventManagerEventIntelligence
{
    public class IntelligenceInstruction
    {

        public static string Instruction = @"EXAMPLE DOCUMENT 1
				<html>
					<prompt>I want to get all events with priority high</prompt>
					<models>
						[
							{
								""Name"": ""Chelsea vs Anderlecht"",
								""Description"": ""Friendly match between football clubs"",
								""Start"": ""2026-02-17T17:55:59.206Z"",
								""End"": ""2026-02-18T13:32:36.414Z"",
								""Type"": ""Basic"",
								""Priority"": ""Low"",
								""Languages"": [
									{
										""Name"": ""Dutch"",
										""AudioType"": ""Surround"",
										""CcSupplierCompanyName"": ""Telenet""
									},
									{
										""Name"": ""English"",
										""AudioType"": ""Stereo"",
										""CcSupplierCompanyName"": ""Starmer""
									}
								],
								""Identifier"": ""b23e6a8e-ed36-4920-bfee-b3313fdb4ef1""
							},
							{
								""Name"": ""Arsenal vs Union"",
								""Description"": ""Friendly match between football clubs"",
								""Start"": ""2026-02-17T18:55:59.206Z"",
								""End"": ""2026-02-18T13:32:36.414Z"",
								""Type"": ""Basic"",
								""Priority"": ""High"",
								""Languages"": [
									{
										""Name"": ""Italian"",
										""AudioType"": ""Surround"",
										""CcSupplierCompanyName"": ""Meloni""
									}
								],
								""Identifier"": ""3362da9a-0bf8-4754-aa55-6f6b9e3d0ecd""
							}
						]
					</models>
				</html>

				EXAMPLE RESPONSE 1

				{
					""ODATAFILTER"" : ""Priority eq 'High'"",
					""HTTPVERB"" : ""GET"",
					""HTTPBODY"" : """",
					""MODELID"": """"
				}

				EXAMPLE DOCUMENT 2

				<html>
					<prompt>I want to update all events priority to Medium</prompt>
					<models>
						[
							{
								""Name"": ""Chelsea vs Anderlecht"",
								""Description"": ""Friendly match between football clubs"",
								""Start"": ""2026-02-17T17:55:59.206Z"",
								""End"": ""2026-02-18T13:32:36.414Z"",
								""Type"": ""Basic"",
								""Priority"": ""Low"",
								""Languages"": [
									{
										""Name"": ""Dutch"",
										""AudioType"": ""Surround"",
										""CcSupplierCompanyName"": ""Telenet""
									},
									{
										""Name"": ""English"",
										""AudioType"": ""Stereo"",
										""CcSupplierCompanyName"": ""Starmer""
									}
								],
								""Identifier"": ""b23e6a8e-ed36-4920-bfee-b3313fdb4ef1""
							},
							{
								""Name"": ""Arsenal vs Union"",
								""Description"": ""Friendly match between football clubs"",
								""Start"": ""2026-02-17T18:55:59.206Z"",
								""End"": ""2026-02-18T13:32:36.414Z"",
								""Type"": ""Basic"",
								""Priority"": ""High"",
								""Languages"": [
									{
										""Name"": ""Italian"",
										""AudioType"": ""Surround"",
										""CcSupplierCompanyName"": ""Meloni""
									}
								],
								""Identifier"": ""3362da9a-0bf8-4754-aa55-6f6b9e3d0ecd""
							}
						]

					</models>
				</html>

				EXAMPLE RESPONSE 2

				{
					""ODATAFILTER"" : """",
					""HTTPVERB"" : ""PUT"",
					""HTTPBODY"" : ""[
							{
								""Name"": ""Chelsea vs Anderlecht"",
								""Description"": ""Friendly match between football clubs"",
								""Start"": ""2026-02-17T17:55:59.206Z"",
								""End"": ""2026-02-18T13:32:36.414Z"",
								""Type"": ""Basic"",
								""Priority"": ""High"",
								""Languages"": [
									{
										""Name"": ""Dutch"",
										""AudioType"": ""Surround"",
										""CcSupplierCompanyName"": ""Telenet""
									},
									{
										""Name"": ""English"",
										""AudioType"": ""Stereo"",
										""CcSupplierCompanyName"": ""Starmer""
									}
								],
								""Identifier"": ""b23e6a8e-ed36-4920-bfee-b3313fdb4ef1""
							},
							{
								""Name"": ""Arsenal vs Union"",
								""Description"": ""Friendly match between football clubs"",
								""Start"": ""2026-02-17T18:55:59.206Z"",
								""End"": ""2026-02-18T13:32:36.414Z"",
								""Type"": ""Basic"",
								""Priority"": ""High"",
								""Languages"": [
									{
										""Name"": ""Italian"",
										""AudioType"": ""Surround"",
										""CcSupplierCompanyName"": ""Meloni""
									}
								],
								""Identifier"": ""3362da9a-0bf8-4754-aa55-6f6b9e3d0ecd""
							}
						]"",
					""MODELID"": """"
				}


				REQUIRED FORMATS

				- the http verb in the respionse can be GET,PUT, POST or DELETE
				- when the prompt tag refers to retrieving data, the OData filter in the response should be filled in and the http body should be empty and the HTTPverb should be get
				- when the prompt tag refers to updating or creating data, the OData filter in the response should be empty and the http body should be filled in with the new or updated data and the HTTPverb should be PUT for update and POST for create
				- when the prompt tag refers to deleting data, the MODELID in the response should be filled in with the if of the items to delete and the http body should be empty and the HTTPverb should be DELETE
				- below is the open api spec in yaml that describes the models

				OPENAPI SPEC

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
                Priority:
                  enum:
                    - Low
                    - Medium
                    - High
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
                  Priority:
                    enum:
                      - Low
                      - Medium
                      - High
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
                Priority:
                  enum:
                    - Low
                    - Medium
                    - High
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
                  Priority:
                    enum:
                      - Low
                      - Medium
                      - High
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
                  Priority:
                    enum:
                      - Low
                      - Medium
                      - High
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
        Priority:
          enum:
            - Low
            - Medium
            - High
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
  - BearerAuth: [ ]
			";
    }
}
