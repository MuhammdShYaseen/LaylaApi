🏗️ Layla Api — Backend API
ASP.NET Core REST API for Apartment & Chalet Rental Platform

The Layla App Backend provides all server-side logic for a mobile application designed to help users search for furnished apartments or chalets, book them for days or hours, communicate with owners, sign rental contracts, and receive real-time notifications.

This backend is built with ASP.NET Core, Entity Framework Core, JWT Authentication, and integrates with Push Notification Services and real-time map interactions.

🚀 Core Features
🔐 Authentication & User Management

Unified login system (owners & renters)

Real identity verification

JWT-based authentication

User profile management

First-time Terms of Use agreement

🏠 Apartment Management

Add new apartments with all required details

Update availability (Available / Unavailable)

Track next available date

Attach pricing, photos, and location info

Owner information and in-app communication

🔍 Apartment Search & Filtering

The backend supports multiple search modes:

Filter by distance (nearest → farthest)

Filter by price (cheapest → most expensive)

Filter by area/space

Filter by region name

List apartments around user’s real-time location

Full map browsing endpoints

🗺️ Map & Location System

Store latitude/longitude of apartments

Track real-time locations of users interested in renting

Provide endpoints for displaying all apartments on the map

Click-on-Map → fetch apartment details

📅 Booking System

Conflict-free reservation system

API endpoints carefully designed to avoid booking race conditions

Validation for overlapping times

Booking history and reservation status

📄 Contract Generation & Storage

Generate rental contracts through the API

Send a copy to owner, renter, and store one in the database

Ready for later electronic verification integration

🔔 Push Notifications

A fully modular notification system that supports:

New apartment alerts

Booking status updates

Messaging and contract updates

Nearby apartment alerts

Admin broadcasts

Backend includes:

Notification controllers

Token registration

Queued push delivery

Owner–renter notification channels

💳 Payments & Commission Logic # not implemented yet

(Currently planned for next development phase)

App commission:

$1 from the owner

$0.5 from the renter

Future support for online payment gateways

Transaction logging endpoints

🧪 Booking Conflict Protection

The backend ensures zero booking conflicts through:

EF Core concurrency checks

Date range validation

Transaction-safe operations

Single-entry time-slot locking

This prevents any overlapping reservations even if multiple users try booking at the same time.

🛠️ Technologies Used

ASP.NET Core 8

Entity Framework Core

SQL Server

SignalR (optional for real-time updates)

Firebase / FCM for push notifications

JWT Authentication

AutoMapper

📜 Future Enhancements

Complete online payment integration

Advanced analytics (popular areas, pricing predictions)

Subscription system for owners

Real-time chat via SignalR

Smart recommendation engine
