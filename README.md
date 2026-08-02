# FilmMate – QA & Testing Project

## Project Overview

FilmMate is a C# console application developed to demonstrate software quality assurance and testing practices. The project focuses on improving software reliability through automated testing, code analysis, and refactoring techniques.

## Testing & QA Methodologies

The project includes different testing approaches and quality assurance activities:

### Test-Driven Development (TDD)

The application logic was developed using the Red-Green-Refactor cycle. Tests were created first in the `TDDProject` and then used to guide the implementation of the required functionality.

### Unit Testing

Unit tests were implemented using NUnit to verify individual components and application logic.

- Test suites are located in the `UnitProject` folder.
- Tests cover film management, user roles, and service functionality.
- Tests can be executed using Visual Studio Test Explorer.

### Structural and Functional Testing

- **White-box testing:** Analysis of internal code structure, including statement and branch coverage.
- **Black-box testing:** Validation of application behavior based on requirements, inputs, and expected outputs.

### Code Tuning and Refactoring

- Refactored existing code by applying SOLID principles to improve readability and maintainability.
- Optimized selected parts of the application to improve performance and reduce unnecessary operations.
