# 🌍 WasteConnect -- Smart Illegal Dumping Management System

> **A cloud-based waste management platform built with ASP.NET Core MVC
> and Microsoft Azure.**
>
> 

# Overview

WasteConnect is a modern platform designed to help communities, waste
management companies, and municipalities work together to report,
monitor, and resolve illegal dumping.

The system allows residents to report illegal dumping with photos and
location information while enabling municipalities and registered waste
management companies to manage clean-up operations through dedicated
dashboards.

 
# Problem Statement

Illegal dumping negatively affects communities, public health, tourism,
and municipal resources.

WasteConnect aims to:

-   Reduce illegal dumping
-   Improve response times
-   Increase community participation
-   Provide municipalities with real-time analytics
-   Support informed environmental decision-making



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



## Company Features

-   Company Dashboard
-   Register Company
-   View Assigned Jobs
-   Update Job Status
-   View Startup and DueDate cleanups
-   Find Nearest Disposal Sites
-   Track and Locate cleanup Area
-   Show Job Completion



## Admin Features

-   View All Reports
-   Manage Users
-   Manage Companies
-   Assign job to Companies
-   Analytics Dashboard



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
-   
# System Architecture

``` text
Residents
      │
      ▼
ASP.NET Core MVC Application
      │
 ┌────┴───────────────┐
 │                    │
 ▼                    ▼
Azure SQL        Azure Cosmos DB
(Authentication) (Reports)
 │
 ▼
Azure Blob Storage
(Images)
 │
 ▼
Azure Maps
(Location & Directions)
 │
 ▼
Azure App Service
```




#  Project Structure

``` text
Controllers/
Models/
Views/
Services/
Data/
ViewModels/
wwwroot/
```


# System Screenshots

## Home Page

<img width="1335" height="634" alt="image" src="https://github.com/user-attachments/assets/71acfb61-e55a-4e21-8c30-63d7bec4dc1e" />

## User Dashboard

<img width="1319" height="636" alt="image" src="https://github.com/user-attachments/assets/6ca2a9d1-c5c2-43ce-a325-9eab83bbb6c6" />

## User Dashboard (Mobile Version)

<img width="283" height="624" alt="image" src="https://github.com/user-attachments/assets/29fc3a8e-b54d-47e8-96b8-74d8269f9ea3" />


## Report Illegal Dumping

<img width="1328" height="639" alt="image" src="https://github.com/user-attachments/assets/cc581451-ca4c-4014-a410-d5617366d304" />



## Report Illegal Dumping( Mobile Version)

<img width="285" height="619" alt="image" src="https://github.com/user-attachments/assets/83800ac3-2aab-41d2-977d-a16cccc523c7" />



## Number Verification Before Report Submission

<img width="1329" height="632" alt="image" src="https://github.com/user-attachments/assets/3af2d5e3-cb3e-42d2-b81b-97cd65afc2a8" />


## My Reports

<img width="1321" height="637" alt="image" src="https://github.com/user-attachments/assets/a8c9acfe-0d83-460e-86cd-1615f1464109" />


## My Reports (Mobile Version)

<img width="286" height="622" alt="image" src="https://github.com/user-attachments/assets/f3c1ced5-f086-4dee-83ab-4c16b83d5ffb" />

## Admin Dashboard

<img width="1335" height="632" alt="image" src="https://github.com/user-attachments/assets/1b6e1aae-ec68-4246-9dce-501c6b299fa6" />

## Admin Manage Reports

<img width="1333" height="643" alt="image" src="https://github.com/user-attachments/assets/2794a346-a69e-44d2-89dd-a05c9b61a077" />


## Admin Assign Job To Companies

<img width="1264" height="636" alt="image" src="https://github.com/user-attachments/assets/28345514-610b-4768-a8f0-49b05122958a" />


## Analytics Dashboard

<img width="1327" height="633" alt="image" src="https://github.com/user-attachments/assets/8bb61ffc-d010-42b6-a26b-0057eca0989f" />

<img width="1317" height="608" alt="image" src="https://github.com/user-attachments/assets/3158daf4-b93d-4fca-a50a-1030c5f39f8b" />


## Company Dashboard

<img width="1326" height="634" alt="image" src="https://github.com/user-attachments/assets/cff1ae07-021b-4275-ac88-aa075856eb9a" />



## Assigned Jobs

<img width="1323" height="631" alt="image" src="https://github.com/user-attachments/assets/6d66b214-d766-45fe-b69e-7676ca7fe2c8" />

## Assigned Job Completed

<img width="1324" height="635" alt="image" src="https://github.com/user-attachments/assets/0ecc74b3-cb31-4072-a6e3-ca6451a3af40" />

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

## Planned

-   AI Waste Detection
-   AI Cleanup Recommendations
-   Mobile Application
-   Notifications
-   Automated Job Assignment
-   Predictive Analytics
-   Counsilor Login
-   Civic Reports( e.g potholes, damaged roads, overflowing bins etc.)














