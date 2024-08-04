Feature: Mediatr

Background:
	Given a compiler with path "../../../../MediatrCode" and mediatr libs
	And analyzer is configured for Mediatr
	
Scenario: Notification publishers
	When code is analyzed
	Then 12 dependencies are created

Scenario Outline: Classes that throw events
	When code is analyzed
	Then dependency <from> to <to> is listed

	Examples:
	  | from                           | to                      |
	  | "SenderA1"                     | "NotificationA"         |
	  | "SenderA2"                     | "NotificationA"         |
	  | "NotificationA"                | "NotificationAHandler1" |
   
Scenario Outline: Classes that handle events
	When code is analyzed
	Then dependency "NotificationA" to "NotificationAHandler1" is listed

Scenario: Classes that derive from classes that throw events
	When code is analyzed
	Then dependency "DerivedSenderA1" to "SenderA1" is listed

Scenario: Classes that derive from classes that throw events but override the sending
	When code is analyzed
	Then dependency "DerivedSenderA1ThatOverrides" to "NotificationB" is listed