# Hotel Booking API

## Author 
Stephen Edward Ng

## Overview
This is a technical task by Stephen Edward Ng for Waracle. This is a hotel room booking API using ASP.NET Core and Entity Framework (EF) Core, written in C# following RESTful principles.

## Planning

### Jira Board

Project tasks can be seen on Jira 
https://spacetourer.atlassian.net/jira/software/projects/SCRUM/boards/1?filter=&groupBy=none

### Project Tasks

* Create the VS solution

* Models
  * Hotel: Id, Name, NoOfRooms (all are 6 but want to make extendible) 
  * Room: Id, HotelId, RoomTypeId 
  * RoomType: Id, Name, Capacity (Single 1, Double 2, Deluxe 4) 
  * Booking: Id, GuestId, RoomId, StartDate, EndDate, NoOfGuests, Reference
  * Guest: Id, Name 

* Controllers
  * Find a hotel based on its name
  * Find available rooms between two dates for a given number of people
  * Book a room
  * Find booking details based on a booking reference 

* Logic
  * No double bookings
  * A room cannot be occupied by more people than its capacity
  * Services

* Use the database of your choosing
  * SQLite

* API must be testable
  * Swagger documentation should be made available for testing
  * Expose functionality to allow for seeding and resetting the data

* Error handling 
  * Error log
  * Custom Business Exceptions

* XML documentation

* GitHub

* Validate date cannot be in the past, end date cannot be before start date
* Refactor to DTOs so Models are not exposed to api
* Change GUID reference to more realistic booking reference

* Automated testing

## AI, Microsoft Docs, StackOverflow
No AI agents (Claude Code, Github Copilot etc) were used. Copilot chat, Microsoft documentation and StackOverflow were used for things like troubleshooting a corrupted IIS Express install, LINQ syntax, setting up SQLlite and speeding up the unit tests.

## Swagger

Swagger documentation is available at https://localhost:xxxxx/swagger

Seed data /api/TestData/Seed

Reset data /api/TestData/Reset

Get hotel by name /api/HotelBooking/hotel

Get available rooms /api/HotelBooking/available-rooms

Book room /api/HotelBooking/book

Get booking by reference /api/HotelBooking/booking

## Automated Testing
Testing is located in associated project HotelBooking.Tests