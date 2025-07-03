🧠 Project Documentation

🧰 Technologies Used

🔹 Backend

.NET 8 / .NET 7 (ASP.NET Core Web API)

Entity Framework Core (ORM)

SQL Server (Database)

JWT Authentication (HackerPlatform)

Cookies-Based Session (E-Commerce App)

Middleware (Custom validation, error handling)

CORS configuration

Swagger (API testing/documentation)

🏗️ Architecture: Onion Architecture

Presentation Layer: API Controllers

Application Layer: Services and DTOs

Domain Layer: Entities and Core Logic

Infrastructure Layer: EF Core, Database, External services

🎯 Design Patterns Used

✅ HackerPlatform

Repository Pattern: Abstracting data access logic

Unit of Work: Managing transactional consistency

JWT Authentication: Secure, stateless auth for APIs using claims-based identity

Custom Middleware: For error handling, request logging, and validation

✅ E-Commerce App

Cookie-based Sessions: Using secure cookies (SameSite=None, Secure=true) to track users

Custom Middleware: Global Exception Handling, Validation Exception Interceptor

Manual DTO validation (instead of ModelState)

📦 Hacker Platform Documentation

🌐 Authentication

Login/Register with JWT Token

Tokens sent in Authorization: Bearer header

🏆 Features

Modules: Contains vulnerable apps with increasing difficulty

Questions: Related to each module for testing understanding

Achievements: Earned based on completion of challenges

Levels: Progress system based on points

Reports: Per-user reports per module (score, attempt, completion time)

Vulnerabilities: Categorized and explained for each module

User Roles: Admin and Standard Users (RBAC)

📂 Entities Overview

User (with JWT claims)

Module (with Level, Vulnerabilities)

Achievement

Report

Question (with correct answer and options)

Vulnerability (with type, description, and module link)

🛒 E-Commerce Hacker Training App

✅ General Behavior

Uses Cookies to track sessions (EUser token)

Shared Program.cs to handle migrations, middleware, and seeding

Divided into 6 Modules each with security vulnerabilities

👣 Project Features & Endpoints

1. 📦 Products Controller

GET /api/products → List all products

GET /api/products/{id} → Get product by ID

POST /api/products → Add product (Privilege Escalation in Module 6)

PUT /api/products/{id} → Edit product (Privilege Escalation in Module 5)

DELETE /api/products/{id} → (Only in some modules)

2. 🛒 Cart Controller

POST /api/cart → Add to cart (Business Logic flaws in Module 5)

POST /api/cart/checkout → Perform checkout with total validation + coupon

3. 💖 Wishlist Controller

POST /api/wishlist/add → Add to wishlist (CSRF vulnerable in Module 6)

DELETE /api/wishlist/clear → Clear wishlist

4. 👤 Auth Controller

POST /api/auth/register → Register new user

POST /api/auth/login → Login (SQLi vulnerable in some modules)

PUT /api/auth/profile → Update profile (XSS vulnerable in Module 4)

5. 💬 Comments Controller

POST /api/comments → Add comment

DELETE /api/comments/{id} → Delete comment (IDOR vulnerable in Module 4)

6. 📞 Contact Controller

POST /api/contact → Submit contact message

PUT /api/contact/{id} → Edit contact message (IDOR vulnerable in Module 3)

🔐 Vulnerabilities by Module

✅ Module 1

✅ Open Redirect

✅ SQL Injection (Login)

✅ Module 2

✅ XSS in product search

✅ CSRF in profile update

✅ Module 3

✅ SQL Injection on login: Raw SQL without sanitization

✅ IDOR on contact edit (PUT /api/contact/{id}): User can edit messages not theirs

✅ Module 4

✅ XSS in profile update: Allows special characters in fullname, others blocked

✅ IDOR in delete comment: Users can delete others’ comments

❌ No product modification available

✅ Module 5

✅ Business Logic Flaws:

Accepts negative quantities in cart

Rejects zero as invalid

Checkout only if total is positive

✅ Privilege Escalation: Any user can edit any product

✅ Module 6

✅ Privilege Escalation: Any user can add product

✅ CSRF in wishlist add:

Doesn’t check Content-Type

Cookie set to SameSite=None, Secure=true

✅ Coupon functionality used in checkout

🔐 Security Summary

✅ SQL Injection (SQLi)

✅ Cross-Site Scripting (XSS)

✅ Cross-Site Request Forgery (CSRF)

✅ Insecure Direct Object Reference (IDOR)

✅ Business Logic Flaws

✅ Privilege Escalation

📝 Note: كل اندبوينت مصمم يكون قابل للاختراق لغرض التدريب، وموزع بشكل تدريجي في المستويات، مرتبط مباشرة بتقييم المستخدم في منصة الهاكر 👨‍💻

