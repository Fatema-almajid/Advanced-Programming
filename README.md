
# 🎓 Training & Certification Platform

## 📌 Overview

This project is a web-based system designed to manage the full training lifecycle for a professional training provider.

---

## 👥 Roles

* **Trainee**
* **Instructor**
* **Training Coordinator**

---

# 👥 Test Users

## 🎓 Trainee
- 📧 Email: `ali@mail.com`  
  🔑 Password: `123456`

- 📧 Email: `omar@mail.com`  
  🔑 Password: `123456`

## 👩‍🏫 Instructor
- 📧 Email: `sara@mail.com`  
  🔑 Password: `123456`

## 🗂️ Training Coordinator
- 📧 Email: `dana@mail.com`  
  🔑 Password: `123456`

---

## ⚙️ Core Features

### 📚 Course Management

* Create and manage courses
* Assign category, duration, capacity, and fees
* Support for course prerequisites

### 👨‍🏫 Instructor Management

* Assign instructors to sessions
* Manage instructor availability
* Prevent scheduling conflicts

### 🏫 Classroom & Equipment

* Manage classrooms and seating capacity
* Assign equipment (e.g., projectors, computers)

### 📅 Course Scheduling

* Create sessions with:

  * Instructor
  * Room
  * Date & Time
* Validate availability before scheduling

### 🧾 Enrollment Lifecycle

* Status flow:

  ```
  Enrolled → Confirmed → Attending → Completed / Dropped
  ```
* Validate:

  * Capacity
  * Prerequisites

### 💳 Payment & Balance Tracking

* Record payments (Partial / Full)
* Track outstanding balances
* Manage due dates

### 📝 Assessment

* Record trainee results (Pass / Fail)
* Link results to certification progress

### 🏆 Certification Tracks

* Group courses into tracks
* Track trainee progress
* Generate certification eligibility

### 🔔 Notifications

* Notify users about important events

---

## 🧩 Main Entities

* User
* Course
* Session
* Enrollment
* Payment
* Balance
* Assessment
* Track
* TraineeCertification
* Classroom
* Equipment
* Notification

---

## 🚀 Technologies Used

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* GitHub