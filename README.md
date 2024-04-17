# Milles Bank
This readme includes a short description of the project, its purpose and main functions.

## Content
1. Description
2. Patterns/Principles
3. Library, Common Classes & Interfaces

## 1. Description

This project is made in ASP.NET Razor pages and is made to be a bank application.
The project features a money laundering console to check for suspicious activity,
an API that gets account and customer details aswell as the bank web application itself.

I have used services, library, automapper and more.

## 2. Patterns/Principles

### Principles i have used in my project:
#### SOC (Separation of Concern):
I achieved this by making sure to divide my classes and interfaces into different maps and more classes. This makes it easier to for example debug, scale and develop.

#### Single Responsibility Principle (SRP):
Since i have divided my classes and interfaces it also makes my project follow SRP by only being responsible for one thing. 
An example of this is my Strategy classes, they are only responsible of calculating either a shape or a numbered calculation.

#### Open/Closed Principle (OCP):
My classes have been designed to not require any adjustments but they can be developed to include new features without needing to change code that's already written.

#### Interface Segregation Principle (ISP):
Most of my classes have a interface and the interfaces only have methods that the classes require, the methods that arent required for the interface were made private and thus my project
is following ISP.

#### Dependency Inversion Principle (DIP):
Almost all my classes depend on interfaces that are registered by Autofac, thus my project is not very dependent on different parts of my code.

## 3. 

## 4. 

## 5. 

## 6. Dependency Injection, Generic Classes & Interfaces

The bank application uses dependency injection for easier testing aswell as generic classes to follow the DRY principles. Interfaces are ofcourse included to streamline the process of relieving dependencies.

### Common Classes & Interfaces (that Shape, Calculator and Rock Paper Scissors share)
- App
- Program
- DataSeeding
- DbConfiguration
- ProjectDbContext
- Message
- Autofac
- IMenu
- IMenuFactory
- IDataSeeding
