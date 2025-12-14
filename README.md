# 🎬 Cinema Seat Reservation System

## 📌 Project Overview

The **Cinema Seat Reservation System** is a complete desktop-based application designed to manage and control seat reservations in a cinema environment. The system combines a **Windows Forms graphical user interface (GUI)** with a backend implemented in **F#** to simulate real-world cinema booking behavior.

The application focuses on **real-time seat availability**, **safe concurrent booking**, and **clear separation of concerns**, while providing a user-friendly visual interface for interaction.

---

## 🎯 Objectives

* Manage cinema halls and seat layouts efficiently
* Allow users to view available seats in real time
* Prevent multiple users from booking the same seat
* Ensure data consistency during concurrent operations
* Practice clean architecture and separation of concerns

---

## 🧠 System Concept

Each cinema hall contains a fixed seat layout represented by rows and columns. Every seat has a **status** that reflects its current state:

* **Available** → Seat is free and can be booked
* **Booked** → Seat is already booked and unavailable

Seat statuses are managed strictly through the database to ensure reliability, consistency, and protection against double booking.

---

## ⚙️ Core Features

### 1️⃣ Graphical User Interface (GUI)

* Windows Forms interface for seat selection and booking
* Visual seat layout using buttons or grid controls
* Seat colors represent current status (Available / Booked)

### 2️⃣ Seat Layout Management

* Seats are represented by row and column coordinates
* Seat layout is loaded dynamically from the database
* Only two seat states are supported: Available and Booked

### 3️⃣ Seat Reservation Logic (F# Backend)

* All business logic is implemented in F#
* Seat availability is validated before booking
* Booking operations are executed inside database transactions

### 4️⃣ Concurrency Handling

* Prevents double booking during simultaneous user requests
* SQL locking and transactional logic ensure data integrity

### 5️⃣ Repository-Based Architecture

* Repositories handle all database access using Dapper
* Service layer contains booking and validation logic
* UI layer communicates only with services

## 🗄️ Database Design

The system relies on a SQL database to store:

* Cinema halls
* Seats and their statuses
* Reservations and bookings

The database ensures:

* Referential integrity
* Atomic operations during booking
* Safe rollback in case of errors

---

## 🧩 Technologies Used

* **Language (Backend):** F#
* **UI Framework:** Windows Forms (WinForms)
* **Database:** SQL Server
* **Data Access:** Dapper
* **Architecture:** Layered Architecture / Repository Pattern
* **Testing:** xUnit (Unit Testing)
* **Tools:** SQL Server Management Studio (SSMS), Git, GitHub, Visual Studio

---

## 🔄 Booking Workflow

1. User selects a cinema hall
2. System loads all available seats from the database
3. User selects one or more seats
4. System checks seat availability inside a transaction
5. Selected seats are marked as booked
6. Database updates are committed safely

---

## 🧪 Unit Testing

The project includes **unit tests implemented using xUnit** to ensure correctness and reliability of the business logic.

### Testing Focus:

* Seat availability validation
* Booking and reservation rules
* Service-layer logic independent from the UI
* Edge cases and error handling

Unit tests help guarantee system stability and make future refactoring safer.

---

## 🚀 Future Enhancements

* User authentication and roles
* Movie scheduling and showtimes
* Ticket pricing and payments
* Seat expiration for temporary reservations
* Admin dashboard and reports
* UI enhancements and animations

---

## 📂 Project Purpose

This project was created as a **practice and learning project** to strengthen understanding of:

* Database-driven systems.
* Concurrency problems and solutions.
* Clean code structure.
* Real-world booking system logic.

---

## 👩‍💻 Author

Developed as part of a hands-on learning journey in backend development and system design.

---

✅ *This project demonstrates how real-world reservation systems manage data consistency, concurrency, and scalability.*
