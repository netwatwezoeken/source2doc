Feature: Mediatr

Background:
	Given source code "../../../../MediatrCode" and mediatr libs
	And analyzer is configured for Mediatr

Scenario: Amount of dependencies
	When code is analyzed
	Then 15 dependencies are created
	
Scenario: Amount of types
	When code is analyzed
	Then 17 types are created

Scenario Outline: Classes that throw events are detected
	When code is analyzed
	Then dependency <from> to <to> is listed
	And class <from> is of type <type>

	Examples:
	  | from       | to              | type        |
	  | "SenderA1" | "NotificationA" | "Publisher" |
	  | "SenderA2" | "NotificationA" | "Publisher" |
	  | "SenderB2" | "CommandB"      | "Publisher" |
   
Scenario: Event handlers are detected
	When code is analyzed
	Then dependency "NotificationA" to "NotificationAHandler1" is listed
	And class "NotificationAHandler1" is of type "Handler"
	
Scenario: Event handlers can handle multiple events
	When code is analyzed
	Then dependency "CommandA" to "CommandAHandler2" is listed
	And dependency "CommandB" to "CommandAHandler2" is listed
	
Scenario: Classes that derive from classes that throw events are detected
	When code is analyzed
	Then dependency "DerivedSenderA1" to "SenderA1" is listed
	And class "DerivedSenderA1" is of type "Publisher"

Scenario: Classes that derive from classes that throw events but override the throw are detected
	When code is analyzed
	Then dependency "DerivedSenderA1ThatOverrides" to "NotificationB" is listed
	And class "DerivedSenderA1ThatOverrides" is of type "Publisher"
	
Scenario Outline: Classes that are events are labeled so
	When code is analyzed
	Then <class> is an event

	Examples:
	  | class                  |
	  | "NotificationA"        |
	  | "NotificationB"        |
	  | "DerivedNotificationA" |
	
Scenario Outline: Classes that are publisher are labeled so
	When code is analyzed
	Then <class> is a publisher

	Examples:
	  | class                          |
	  | "SenderA1"                     |
	  | "SenderA2"                     |
	  | "DerivedSenderA1"              |
	  | "DerivedSenderA1ThatOverrides" |

Scenario Outline: Classes that are handlers are labeled so
	When code is analyzed
	Then <class> is a handler
	
	Examples:
	  | class                   |
	  | "NotificationAHandler1" |
	  | "CommandAHandler1"      |
	  | "RequestAHandler1"      |

Scenario: Output mermaid markdown
	When code is analyzed
	Then mermaid markdown can be generated
	
Scenario: Output json
	When code is analyzed
	Then json can be generated