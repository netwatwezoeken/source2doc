Feature: SourceSelection
	
Selection of source files to include into the analysis

Scenario: Single folder
	Given a compiler with path "../../../../TestCode"
    And analyzer is configured
	When code is analyzed
	Then the result should be 2 symbols
	And "TestCode.Class1" is a listed symbol
	And "TestCode.Sub.Class1" is a listed symbol
