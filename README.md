# 🌍 WasteConnect -- Smart Illegal Dumping Management System

> **A cloud-based waste management platform built with ASP.NET Core MVC
> and Microsoft Azure.**
>
> 

---

# Overview

WasteConnect is a modern platform designed to help communities, waste
management companies, and municipalities work together to report,
monitor, and resolve illegal dumping.

The system allows residents to report illegal dumping with photos and
location information while enabling municipalities and registered waste
management companies to manage clean-up operations through dedicated
dashboards.

---
 
# Problem Statement

Illegal dumping negatively affects communities, public health, tourism,
and municipal resources.

WasteConnect aims to:

-   Reduce illegal dumping
-   Improve response times
-   Increase community participation
-   Provide municipalities with real-time analytics
-   Support informed environmental decision-making

---

#  Features

## User Features

-   User Registration & Login
-   Report Illegal Dumping
-   Upload Images
-   View My Reports
-   Edit Reports
-   Delete Reports
-   Mobile Responsive Design
-   Find Nearest Disposal Site

---

## Company Features

-   Company Dashboard
-   Register Company
-   View Assigned Jobs
-   Update Job Status
-   View Startup and DueDate cleanups
-   Find Nearest Disposal Sites
-   Track and Locate cleanup Area
-   Show Job Completion

---

## Admin Features

-   View All Reports
-   Manage Users
-   Manage Companies
-   Assign job to Companies
-   Analytics Dashboard

---

#  Cloud Technologies

-   ASP.NET Core MVC (.NET 8)
-   Microsoft Azure App Service
-   Azure SQL Database
-   Azure Cosmos DB
-   Azure Blob Storage
-   Azure Maps
-   Azure Email Communication
-   Twilio
-   Git & GitHub
-   Report Status Management

---

  # ☁️ Microsoft Azure Cloud Services

WasteConnect leverages multiple Microsoft Azure cloud services to provide a scalable, secure, and reliable platform for illegal dumping management.



## Azure App Service

Azure App Service hosts the WasteConnect web application in the cloud, allowing users to access the platform from anywhere without requiring local installation.

**Responsibilities**

- Hosts the ASP.NET Core MVC application
- Provides secure HTTPS access
- Automatic scaling
- High availability
- Continuous deployment support

---

## Azure SQL Database

Azure SQL Database stores structured relational data required by the application.

**Stored Information**

- User Accounts
- Authentication Data
- Roles
- Company Information
- Administrator Accounts

**Benefits**

- Secure
- Reliable
- Fully managed
- Automatic backups
- High availability

---

## Azure Cosmos DB

Azure Cosmos DB stores application data that benefits from a NoSQL document database.

**Stored Information**

- Illegal Dumping Reports
- Report Status
- GPS Coordinates
- Report Metadata
- Analytics Data

**Benefits**

- Extremely fast read/write operations
- Global scalability
- Flexible document structure
- High performance

---

## Azure Blob Storage

Blob Storage stores uploaded images submitted by residents.

**Stored Files**

- Illegal dumping photographs
- Company documentation
- Future report attachments

**Benefits**

- Unlimited storage
- Secure cloud access
- Optimized for large files
- Cost-effective

---

## Azure Maps

Azure Maps provides intelligent location services throughout WasteConnect.

**Features**

- Display dumping locations
- Route navigation
- Find nearest disposal sites
- Company navigation
- Distance calculations

---

## Azure Communication Services

Azure Communication Services provides cloud-based communication capabilities.

Current and planned usage includes:

- Email notifications
- OTP verification
- Future municipality notifications
- Company communication

---

## Twilio SMS

Twilio is integrated for mobile phone verification.

Current functionality:

- One-Time Password (OTP)
- Phone number verification
- Secure report submission

---

# 🏗️ System Architecture

The following diagram illustrates the overall architecture of the WasteConnect platform.

```text
                        Residents / Companies / Admin
                                      │
                                      ▼
                          ASP.NET Core MVC Application
                                      │
      ┌───────────────────────────────┼───────────────────────────────┐
      ▼                               ▼                               ▼
Azure SQL Database           Azure Cosmos DB             Azure Blob Storage
(Authentication)              (Reports)                 (Images/Documents)
      │                               │                               │
      └───────────────┬───────────────┴───────────────┬───────────────┘
                      ▼
                 Azure Maps
          (Location Services)
                      │
                      ▼
        Azure Communication Services
        (Email Notifications)
                      │
                      ▼
              Twilio SMS Service
             (OTP Verification)
                      │
                      ▼
             Azure App Service
             (Cloud Hosting)
```

---





# 📂 Project Structure

```
WasteConnect
│
├── Controllers
│      ├── AccountController
│      ├── UserController
│      ├── CompanyController
│      ├── AdminController
│      └── HomeController
│
├── Models
│
├── ViewModels
│
├── Services
│      ├── BlobService
│      ├── CosmosService
│      ├── EmailService
│      ├── SmsService
│      └── AzureMapsService
│
├── Data
│
├── Views
│
├── wwwroot
│      ├── css
│      ├── js
│      ├── images
│      └── uploads
│
├── Program.cs
├── appsettings.json
└── WasteConnect.sln
```

---


# System Screenshots

## Home Page

<img width="1335" height="634" alt="image" src="https://github.com/user-attachments/assets/71acfb61-e55a-4e21-8c30-63d7bec4dc1e" />

---

# 👤 Resident Experience
## User Dashboard

<img width="1319" height="636" alt="image" src="https://github.com/user-attachments/assets/6ca2a9d1-c5c2-43ce-a325-9eab83bbb6c6" />

---

## User Dashboard (Mobile Version)

<img width="283" height="624" alt="image" src="https://github.com/user-attachments/assets/29fc3a8e-b54d-47e8-96b8-74d8269f9ea3" />

---
## Report Illegal Dumping

<img width="1328" height="639" alt="image" src="https://github.com/user-attachments/assets/cc581451-ca4c-4014-a410-d5617366d304" />

---

## Report Illegal Dumping( Mobile Version)

<img width="285" height="619" alt="image" src="https://github.com/user-attachments/assets/83800ac3-2aab-41d2-977d-a16cccc523c7" />

---

## Number Verification Before Report Submission

<img width="1329" height="632" alt="image" src="https://github.com/user-attachments/assets/3af2d5e3-cb3e-42d2-b81b-97cd65afc2a8" />

---
## My Reports

<img width="1321" height="637" alt="image" src="https://github.com/user-attachments/assets/a8c9acfe-0d83-460e-86cd-1615f1464109" />

---
## My Reports (Mobile Version)

<img width="286" height="622" alt="image" src="https://github.com/user-attachments/assets/f3c1ced5-f086-4dee-83ab-4c16b83d5ffb" />

---

# 🛡️ Administrator Experience
## Admin Dashboard


<img width="1335" height="632" alt="image" src="https://github.com/user-attachments/assets/1b6e1aae-ec68-4246-9dce-501c6b299fa6" />

--

## Admin Manage Reports

<img width="1333" height="643" alt="image" src="https://github.com/user-attachments/assets/2794a346-a69e-44d2-89dd-a05c9b61a077" />

---

## Admin Assign Job To Companies

<img width="1264" height="636" alt="image" src="https://github.com/user-attachments/assets/28345514-610b-4768-a8f0-49b05122958a" />

---

## Analytics Dashboard

<img width="1327" height="633" alt="image" src="https://github.com/user-attachments/assets/8bb61ffc-d010-42b6-a26b-0057eca0989f" />

<img width="1317" height="608" alt="image" src="https://github.com/user-attachments/assets/3158daf4-b93d-4fca-a50a-1030c5f39f8b" />

---

# 🏢 Company Experience
## Company Dashboard


<img width="1326" height="634" alt="image" src="https://github.com/user-attachments/assets/cff1ae07-021b-4275-ac88-aa075856eb9a" />

---

## Assigned Jobs

<img width="1323" height="631" alt="image" src="https://github.com/user-attachments/assets/6d66b214-d766-45fe-b69e-7676ca7fe2c8" />

---

## Assigned Job Completed

<img width="1324" height="635" alt="image" src="https://github.com/user-attachments/assets/0ecc74b3-cb31-4072-a6e3-ca6451a3af40" />

---

# Roadmap

## Completed

-   Authentication
-   Role-based Authorization
-   Azure SQL Integration
-   Azure Cosmos DB Integration
-   Azure Blob Storage
-   Azure Maps
-   Company Management
-   Analytics Dashboard
-   Responsive User Interface

---

## Planned

-   AI Waste Detection
-   AI Cleanup Recommendations
-   Mobile Application
-   Notifications
-   Automated Job Assignment
-   Predictive Analytics
-   Counsilor Login
-   Civic Reports( e.g potholes, damaged roads, overflowing bins etc.)














