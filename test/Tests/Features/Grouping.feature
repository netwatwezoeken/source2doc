Feature: Dependency grouping

Scenario: Simple grouping
	Given these dependencies
	| From             | To              |
	| Dependency.One   | Dependency.Two  |
	| Dependency.Three | Dependency.Four |
	When grouped
	Then 2 groups exist

Scenario: Cyclic dependencies belong in one group
	Given these dependencies
	  | From             | To              |
	  | Dependency.One   | Dependency.Two  |
	  | Dependency.Three | Dependency.Four |
	  | Dependency.Four  | Dependency.One  |
	When grouped
	Then 1 groups exist

Scenario: Dependencies get sorted
	Given these dependencies
	  | From             | To              |
	  | Dependency.One   | Dependency.Two  |
	  | Dependency.Three | Dependency.Four |
	  | Dependency.Four  | Dependency.One  |
	When grouped
	Then group <group> number <index> is <id>
	
	Examples:
	  | group | index | id                 |
	  | 0     | 0     | "Dependency.Four"  |
	  | 0     | 1     | "Dependency.One"   |
	  | 0     | 2     | "Dependency.Three" |
