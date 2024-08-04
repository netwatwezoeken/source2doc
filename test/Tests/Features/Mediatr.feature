Feature: Mediatr

Background:
	Given source code "../../../../MediatrCode" and mediatr libs
	And analyzer is configured for Mediatr
	
Scenario: Notification publishers
	When code is analyzed
	Then 12 dependencies are created

Scenario Outline: Classes that throw events are detected
	When code is analyzed
	Then dependency <from> to <to> is listed

	Examples:
	  | from                           | to                      |
	  | "SenderA1"                     | "NotificationA"         |
	  | "SenderA2"                     | "NotificationA"         |
	  | "NotificationA"                | "NotificationAHandler1" |
   
Scenario Outline: Event handlers are detected
	When code is analyzed
	Then dependency "NotificationA" to "NotificationAHandler1" is listed

Scenario: Classes that derive from classes that throw events are detected
	When code is analyzed
	Then dependency "DerivedSenderA1" to "SenderA1" is listed

Scenario: Classes that derive from classes that throw events but override the throw are detected
	When code is analyzed
	Then dependency "DerivedSenderA1ThatOverrides" to "NotificationB" is listed
	
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
