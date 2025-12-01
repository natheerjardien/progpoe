# Contract Monthly Claim System 🗂️

<details>
<summary><strong>PROG6212 POE Part 1 (Click to Expand)</strong></summary>

## Report 📝
## 1.1	Introduction
In this report we’ll be walking through the design choices, how everything's organized, the database setup, and the layout of the user interface for our Contract Monthly Claim System (CMCS) prototype. Right now, this prototype is all about giving you a clear visual idea of how the system works.
CMCS is a web app built with .NET that makes submitting and getting approval for monthly claims a whole lot easier for lecturers. The whole idea was to make things feel consistent, easy to use, and simple to keep up with, so both lecturers and admins can handle claims and all the supporting documents without unnecessary hassle. When I was designing it, I kept best practices in mind by thinking about how users actually interact with a system, keeping data accurate, and making sure it is scalable for Parts 2 and 3.

## 1.2	Design Choices
The CMCS uses the Model-View-Controller (MVC) framework with ASP.NET Core MVC. This approach helps keep things organized, making it easier to maintain, expand, and understand (GeeksForGeeks, 2023).
I’ve implemented various models based off my database design, specifically the UML class diagram, and it included these are the core models like MonthlyClaim which allows the lecturer to submit claim by entering details which then gets sent to the Academic Managers and Programme Coordinator for approval; Lecturer, AcademicManager, ProgrammeCoordinator, along with enums and ViewModels for more flexibility with regards to manipulating which data is used in which views.
When it comes to the Views, they handle what the user sees like the pages and forms, which would all be based on what permissions each user has, which is not implemented as yet but was included as a basis to work and expand on in future Parts of this POE. 
Lecturer Views allows for submission of monthly claims, uploading supporting documents and track their claim status.
Programme Coordinator Views allows for review of claims submitted by lecturers, verify details, amend approval workflow settings and request for resubmissions or approve the claim and send to the Academic Manager.
The Academic Manager Views allows for approval/rejections of claims, amend approval workflow settings and oversee reporting and approval history.
The controllers handle what happens when users interact with the views, taking input, working with models, and deciding which view to show. At this point in the POE, I’ve included dummy data to make the web app independent on data since there is no connection to a database or storage account as yet.

## 1.3	Consistency in Design
I kept everything looking consistent by using a Bootstrap style CSS theme, so all the pages feel uniform. The colours mainly stick to shades of black, white and grey, giving off a professional yet friendly vibe. I've also kept the standard font that comes with the Bootstrap theme, making it easy to read and keeping things modern (Park, 2025). The buttons follow the Bootstrap styles like ‘btn btn-primary’ for main actions and ‘btn btn-outlined-primary’ for secondary ones, so users get clear visual cues and nice hover effects. The navigation is straightforward with a fixed menu bar and clearly labelled links that lead you to the main features. This look and feel across the site helps cut down confusion and makes for a smooth, hassle-free experience.

## 1.4	Systems Analysis and Design Principles
I focused on ideas like breaking things into modules, reusing parts, and making sure everything can grow easily. For example, ViewModels like GenerateReportViewModel helps shape the data shown on the screen to the user, keeping the user interface separate from how the data is actually stored in the database. This setup makes it simpler to change how things look or add new features in the future. I designed the basis of the app with a priority of making sure that future growth will be possible. It includes features like generating reports, claims, adding users and much more, all handled through specific ViewModels and controller actions, but no complete logic at this point in time. This makes it easier to expand and keep the system easy to maintain.

### 1.4.1	Assumptions
During the design phase I assumed that users will have different roles like lecturers, program coordinators, or academic managers, and each role will have different permissions. The system also expects claims to be submitted each month, with supporting files uploaded through the Lecturer ‘SubmitClaim’ interface. Plus, the way I set up the database was to include entities like lecturers, claims, approvals, and documents, all linked in ways that support reporting and tracking. 
### 1.4.2	Constraints
Of course, there are some limits I had to work within. Security and privacy are top priorities, so the system uses login and permission checks to keep sensitive data safe, which will only be applied in future parts, but I included it in the foundational design. Also, the database structure and enums I have created to support the vision of the CMCS like ReportTypesEnum set rules for what kinds of reports and data can be created, keeping things consistent but slightly restricting flexibility. At this stage, there is no functionality so adding all functionality to meet the initial design will be a big  task, ensuring that everything works cohesively.

## 1.5	Database Structure
The main entities are probably Lecturers, Claims (claimId, LecturerId, approvalStatus, etc.), Approvals, and Documents. These are linked together so that the claims are associated with lecturers, and approvals are tied to claims, making it easier to track and generate reports. The setup will allow for filtering and summarizing data in future parts, so the admins will be able to generate reports based on date ranges, specific lecturers, or claim statuses all for admin purposes.
## 1.6	GUI Layout
The user interface is built using empty Razor views through which I could reuse code from previous MVC projects, aiming to be simple and user-friendly (IIE School of Computer Science, 2025). The forms are split into clear sections, with labels to help users fill things out correctly. The dropdown menus and date pickers also make data entry more accurate, and the tables help organize information neatly.
Since the ViewModels control what data is presented to the users, the interface loads faster and feels more responsive for users. In terms of navigation, it is pretty straightforward with buttons and links that lead to important features like user specific tasks.
// According to Rout (2025), a ViewModel contains more than one models data required a particular view, which in this case is suitable because extra properties are needed for the various Views, which cannot be achieved with the main models alone
// I created a ViewModels to include properties from across multiple models
## 1.7	Conclusion
All in all, the CMCS app’s design focuses on being consistent, easy to use, and ready to grow. By following MVC principles and using ViewModels, it kept things organized and also made sure the data display flexible. The clear assumptions and constraints I identified allowed me to make sure I addressed all the needs of the CMCS. The database setup and interface were made to handle claim processing, reporting, and audits efficiently, making CMCS a solid tool for managing administrative tasks in an academic setting.


## UML Class Diagram 🗺️
<img width="1212" height="1430" alt="PROG6212 POE Part 1 UML drawio" src="https://github.com/user-attachments/assets/9f81fcb9-54f1-406f-9164-ca05ecb599c4" />
- (Draw.io, 2025)

## Project Plan 🗓️
<img width="589" height="872" alt="Screenshot 2025-09-17 204041" src="https://github.com/user-attachments/assets/3e03029a-cc4b-4ee4-b585-f389560ac394" />
- (Swartzinger, 2016)

## GUI Screenshots
### Home Index View
<img width="1918" height="990" alt="Screenshot 2025-09-17 205914" src="https://github.com/user-attachments/assets/d59d3ed5-18c4-4859-b4e0-9d3c1fe1fb3a" />

### Nav Bar Expanded - Lecturer Tools
<img width="1919" height="989" alt="Screenshot 2025-09-17 205937" src="https://github.com/user-attachments/assets/d32a9c4d-0e68-4cfa-9a5c-4f6cabc3771c" />

### Lecturer - SubmitClaim View
<img width="1919" height="990" alt="Screenshot 2025-09-17 210027" src="https://github.com/user-attachments/assets/45687d07-698a-4768-839f-13de4f9f0212" />

### Lecturer - ClaimStatusTracker View
<img width="1919" height="989" alt="Screenshot 2025-09-17 210049" src="https://github.com/user-attachments/assets/2a62e6b9-5949-49d7-89a5-4f9ff8cf20be" />

### After clicking 'Claim Details' button, directed to ClaimDetails View
<img width="1918" height="991" alt="Screenshot 2025-09-17 210102" src="https://github.com/user-attachments/assets/3f57a9dd-4097-4859-80b8-5eb59c475953" />

### Lecturer - Profile View
<img width="1919" height="991" alt="Screenshot 2025-09-17 210119" src="https://github.com/user-attachments/assets/4e38c71c-b87b-489e-b4f8-77045805f4f4" />

### Nav Bar Expanded - Programme Cooridnator Tools
<img width="1919" height="991" alt="Screenshot 2025-09-17 210131" src="https://github.com/user-attachments/assets/44964d26-6cc2-4ccb-b46a-38bbf8a8178c" />

### Programme Cooridnator - Approve Claims View
<img width="1919" height="987" alt="Screenshot 2025-09-17 210144" src="https://github.com/user-attachments/assets/f722e849-ce7d-4d13-80c3-fae35fee0d17" />

### After clicking 'Review' button, redirected to Review Claim View
<img width="1592" height="934" alt="Screenshot 2025-09-17 210205" src="https://github.com/user-attachments/assets/74f5934a-0b50-4ba0-be5c-4dbaa546633a" />

### Programme Cooridnator - Processed Claims History View
<img width="1919" height="990" alt="Screenshot 2025-09-17 210221" src="https://github.com/user-attachments/assets/29c45987-999f-4fd6-9e98-6e9303bf92e1" />

### After clicking 'View Details' button, redirected to Review Claim View
<img width="1529" height="924" alt="Screenshot 2025-09-17 210236" src="https://github.com/user-attachments/assets/b4b06279-7399-459a-a926-5e9b5412d40e" />

### Programme Cooridnator - Approval Workflow Settings View
<img width="1919" height="988" alt="Screenshot 2025-09-17 210320" src="https://github.com/user-attachments/assets/11d97339-2155-4ab7-8b21-a2d5e94f4321" />

### Nav Bar Expanded - Academic Manager Tools
<img width="1918" height="985" alt="Screenshot 2025-09-17 210332" src="https://github.com/user-attachments/assets/ed8944f5-1a40-4798-bfc6-86c01bb849cd" />

### Academic Manager - Approve Claims View
<img width="1919" height="989" alt="Screenshot 2025-09-17 210341" src="https://github.com/user-attachments/assets/0a4b7732-51cf-4ac0-8e59-f2b522dc886c" />

### After clicking 'Review' button, redirected to Review Claim View
<img width="1498" height="901" alt="Screenshot 2025-09-17 210354" src="https://github.com/user-attachments/assets/b2b9603a-2ca8-4c6a-8304-7e88c91f36e0" />

### Academic Manager - Processed Claims History View
<img width="1919" height="991" alt="Screenshot 2025-09-17 210408" src="https://github.com/user-attachments/assets/6f3d30fe-e65c-42a8-8a0e-b18a0d97bd67" />

### After clicking 'View Details' button, redirected to Review Claim View
<img width="1524" height="907" alt="Screenshot 2025-09-17 210421" src="https://github.com/user-attachments/assets/f6aa7c25-e35e-4500-98f6-8a1f88004d32" />

### Academic Manager - Approval Workflow Settings View
<img width="1919" height="992" alt="Screenshot 2025-09-17 210441" src="https://github.com/user-attachments/assets/83ad2aa8-514f-4f65-8c3a-4d37f6da6ff0" />

### Nav Bar Expanded - HR/Admin Tools
<img width="1919" height="992" alt="Screenshot 2025-09-17 210451" src="https://github.com/user-attachments/assets/84bf95c7-152c-4189-8953-e3861fc005c7" />

### HR/Admin - Manager Users View
<img width="1919" height="991" alt="Screenshot 2025-09-17 210500" src="https://github.com/user-attachments/assets/473edbeb-19ce-4b57-bd65-0a944f39ff31" />

### After clicking 'Add New System User', redirected to Register View
<img width="1919" height="993" alt="Screenshot 2025-09-17 210523" src="https://github.com/user-attachments/assets/fb05774b-65c0-4fc0-b1ab-0cbe3e3e4872" />

### After clicking 'Add New Lecturer Profile', redirected to LecturerProfile View
<img width="1919" height="989" alt="Screenshot 2025-09-17 210532" src="https://github.com/user-attachments/assets/6444fb4d-b9b0-43b9-99ca-a609203d1733" />

### After clicking 'Edit Lecturer Financials', redirected to Edit Lecturer Finacials View
<img width="1919" height="991" alt="Screenshot 2025-09-17 210605" src="https://github.com/user-attachments/assets/455de051-9c4e-4807-8d19-d9aa31811ca7" />

### After clicking 'Add Lecturer Financials', redirected to Add Lecturer Finacials View
<img width="1918" height="988" alt="Screenshot 2025-09-17 210615" src="https://github.com/user-attachments/assets/e8262881-9c25-4ad4-a4b2-e3129b0545cb" />

### HR/Admin Tools - Display Reports View
<img width="1919" height="992" alt="Screenshot 2025-09-17 210636" src="https://github.com/user-attachments/assets/b141555f-c153-46dd-b6b8-911a613f1c50" />

### HR/Admin Tools - Generate Invoices & Reports View
<img width="1919" height="991" alt="Screenshot 2025-09-17 210721" src="https://github.com/user-attachments/assets/9f58981b-021e-40a0-9987-96f73877d8a1" />

### Nav Bar Expanded - Account Tools
<img width="1919" height="991" alt="Screenshot 2025-09-17 210748" src="https://github.com/user-attachments/assets/e9eb1600-cf5a-473e-abd3-162452880f14" />

### Account - Login
<img width="1919" height="990" alt="Screenshot 2025-09-17 210756" src="https://github.com/user-attachments/assets/9845d110-c1d8-4541-a74a-0234eff2467c" />

### Account - Register
<img width="1919" height="993" alt="Screenshot 2025-09-17 210806" src="https://github.com/user-attachments/assets/c1a1b1ef-ba1d-4be8-8fe7-b3b619b2ba27" />


## Technologies Used ⚙️
- ASP.NET Core MVC
// As demonstrated by IIEVC School of Computer Science (2025), the Controller is responsible for managing related actions of the AcademicManager, Account, Admin, Approval, Lecturer and ProgrammeCoordinator Views
// Ive made the controllers to use the ViewModels, but no logic added to it. Just to make the View visisble in the browser. Used the same concepts adopted from CLDV6212 POE
- Entity Framework Core
- Bootstrap - Lux Theme (Park, 2025).


## Rerefence List 📜

Draw.io. [online] 
Available at: <https://app.diagrams.net/>
[Accessed 09 September 2025].

GeeksForGeeks, 2025. Benefit of using MVC. [online] 
Available at: <https://www.geeksforgeeks.org/software-engineering/benefit-of-using-mvc/>
[Accessed 16 September 2025].

IIEVC School of Computer Science, 2025. CLDV6212 Building a Modern Web App with Azure Table Storage & ASP.NET Core MVC - Part 1. [video online] 
Available at: <https://youtu.be/Txp7VYUMBGQ?si=5sD7TV0vS90-pPHY>
[Accessed 14 September 2025].

Pranaya Rout, 2025. Dot Net Tutorials. ViewModel in ASP.NET MVC. [online] 
Available at: <https://dotnettutorials.net/lesson/view-model-asp-net-mvc/>
[Accessed 14 August 2025].

Satzinger, J.W., Jackson, R.B. and Burd, S.D., 2016. Systems Analysis and Design in a Changing World. 7th edn. Boston, MA: Cengage Learning.
[Accessed on 15 September 2025]

Thomas Park, 2025. Bootswatch - Lux Theme. [online] 
Available at: <https://bootswatch.com/lux/>
[Accessed 16 September 2025].

PROG6212 POE Part 1 = Complete ✅
</details>

<details>
<summary><strong>PROG6212 POE Part 2 (Click to Expand)</strong></summary>

PROG6212 POE Part 2 👇

## Lecturer Feedback: Part 1 ✅
<img width="894" height="293" alt="Screenshot 2025-10-22 211943" src="https://github.com/user-attachments/assets/6b46b2f5-e6ff-4b4b-b938-77e2fab984fd" />
- How I've improved on lecturer feedback:
    Added consistent spacing and layout throughout Views. Though buttons admin buttons such as Review Claim, Verify and Reject are stacked, the look ad feel are more appealing to me, as having them in a sequential format would make the table feel too condensed.

## YouTube Video Link:
https://youtu.be/nG8nMb_Sddc

## MSTests Project - NuGet Packages Installed
<img width="1919" height="1059" alt="Screenshot 2025-10-22 174248" src="https://github.com/user-attachments/assets/8e112ebc-a83d-44a5-8a70-47934e63da23" />


## MSTests - All 5 test passed
<img width="1727" height="1013" alt="Screenshot 2025-10-22 193842" src="https://github.com/user-attachments/assets/129ed669-f1b8-47d8-89f5-11ad799640f0" />

</details>

<details>
<summary><strong>PROG6212 POE Part 3 (Click to Expand)</strong></summary>

# PROG6212 POE Part 3 👇

## PowerPoint
[PROG6212 POE PART 3 - Presentation.pptx](https://github.com/user-attachments/files/23680777/PROG6212.POE.PART.3.-.Presentation.pptx)

## YouTube Video Link (Unlisted):
https://youtu.be/tn8SNMWsO_g

## Lecturer Feedback & Improvements
<img width="928" height="421" alt="Screenshot 2025-11-21 185251" src="https://github.com/user-attachments/assets/cbdddc5f-6038-4685-8be7-7ea9960e2a07" />

Based on the feedback and Part 3 requirements, the following critical updates were implemented:
1.  **Automation:** Lecturers no longer manually input their hourly rate. It is now automatically pulled from the HR-managed database profile to prevent errors or fraud.
2.  **Validation:** Added logic to reject claims where hours worked exceed **180 hours** per month.
3.  **Data Persistence:** Replaced the temporary file-based storage with a robust **SQL Server Database** using **Entity Framework Core**.
4.  **Role Security:** Implemented `ASP.NET Core Identity` to ensure users (Lecturer, HR, Coordinator, Manager) can only access pages relevant to their specific roles.

## New Features Implemented (Part 3)
### 1. Database Integration (Entity Framework Core)
-   Migrated from `claims.json` to a full SQL Server Relational Database.
-   Implemented `ApplicationDbContext` inheriting from `IdentityDbContext` to manage Users, Roles, and MonthlyClaims in a single normalized database structure.
-   Used **Code-First Migrations** to generate database schemas.

### 2. Secure Authentication & Authorization
-   Integrated **ASP.NET Core Identity** for secure user login and logout.
-   Created a role-based system:
    -   **HR:** Manage users, update profiles/passwords, generate reports.
    -   **Lecturer:** Submit claims, view history, track status.
    -   **Programme Coordinator:** Verify claims.
    -   **Academic Manager:** Final approval/rejection.

### 3. HR Reporting Module (PDF Generation)
-   Implemented a reporting feature that generates professional invoices/reports for approved claims.
-   Integrated the **QuestPDF** library to dynamically create downloadable PDF documents with tables, headers, and calculated totals (formatted in ZAR currency).

### 4. Session Management
-   Used `HttpContext.Session` to store and display temporary user feedback messages (e.g., "Claim successfully verified") across page redirects, enhancing the user experience as per the requirements.

### 5. Enhanced File Handling
-   Supporting documents are now encrypted (AES-256) before storage.
-   Added file size tracking to the database to display accurate file sizes (KB/MB) in the UI.

## Technologies Used (Part 3 Updates) ⚙️
-   **ASP.NET Core Identity:** For authentication and role management.
-   **Entity Framework Core (SQL Server):** For persistent data storage.
-   **QuestPDF:** For generating PDF reports.
-   **System.Security.Cryptography:** For AES encryption of uploaded files.

## Web App Screenshots
### Home Page
<img width="1919" height="992" alt="Screenshot 2025-11-21 180621" src="https://github.com/user-attachments/assets/a500b11f-18cc-493e-8597-f8cbe42a83a7" />

### Account - Login
<img width="1919" height="989" alt="Screenshot 2025-11-21 180630" src="https://github.com/user-attachments/assets/a88e14a7-b31a-49d9-afbb-0455adea2ead" />

### HR Logged in - Manage Users View
<img width="1919" height="990" alt="Screenshot 2025-11-21 180647" src="https://github.com/user-attachments/assets/7736ea54-366b-4615-b7ed-ef22d8530802" />

### HR - Edit User
<img width="1919" height="989" alt="Screenshot 2025-11-21 180726" src="https://github.com/user-attachments/assets/a9d32c05-d37d-4a4a-9a0b-bab6f7f5d213" />

### HR - Add New User
<img width="1919" height="989" alt="Screenshot 2025-11-21 180657" src="https://github.com/user-attachments/assets/7f9a6fca-cdd4-4817-8c45-198d4d17f6c6" />

### HR - Generate Report
<img width="1919" height="991" alt="Screenshot 2025-11-21 180712" src="https://github.com/user-attachments/assets/aa4b5dae-4069-43e3-9fda-01f3d6913ed5" />

### Lecturer Logged in - Submit Claim View
<img width="1919" height="990" alt="Screenshot 2025-11-21 180750" src="https://github.com/user-attachments/assets/cc028d75-94fe-4bc1-b72e-b3b8035ab6a6" />

### Lecturer - Track Claims View
<img width="1919" height="989" alt="Screenshot 2025-11-21 180759" src="https://github.com/user-attachments/assets/7ae39945-ddd3-4a76-b851-8c2de4c0b9ed" />

### Lecturer - Claim Details View
<img width="1919" height="991" alt="Screenshot 2025-11-21 180809" src="https://github.com/user-attachments/assets/f9256741-08cc-4b5d-b8b1-7dcf3282fe51" />

### Lecturer - My Profile View
<img width="1919" height="992" alt="Screenshot 2025-11-21 180820" src="https://github.com/user-attachments/assets/828be09b-e204-47e1-a631-b1d97581d5c1" />

### Admin (Programme Coordintor/ Academic Manager - Identical Views - Seperate Logins) - Pending Claims View
<img width="1919" height="993" alt="Screenshot 2025-11-21 180848" src="https://github.com/user-attachments/assets/9e11e6d4-d530-4d1c-a856-f4d5988b9618" />

### Programme Coordinator - Claim History
<img width="1919" height="985" alt="Screenshot 2025-11-21 180856" src="https://github.com/user-attachments/assets/41ff5a60-5c4f-4a55-9eb8-99ede80cd944" />

### Programme Coordinator - Claim Details View (With approval history)
<img width="1919" height="992" alt="Screenshot 2025-11-21 180904" src="https://github.com/user-attachments/assets/4a7f39a5-5811-40f9-9e78-90099475c956" />

### Academic Manager - Pending Claims
<img width="1919" height="992" alt="Screenshot 2025-11-21 181000" src="https://github.com/user-attachments/assets/fed8cabd-29c1-4919-87f2-9b756408ca38" />

### Academic Manager - Claim History
<img width="1917" height="987" alt="Screenshot 2025-11-21 181008" src="https://github.com/user-attachments/assets/c3a47f96-0ebc-440d-92e3-c42b3a3aeca1" />

### Academic Manager - Claim Details (With approval history)
<img width="1919" height="990" alt="Screenshot 2025-11-21 181016" src="https://github.com/user-attachments/assets/a6de9d21-edad-449e-8697-bb006988841c" />

## Updated Reference List 📜
Microsoft, 2025. *Introduction to Identity on ASP.NET Core*. [online] Available at: <https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity> [Accessed 20 November 2025].

Microsoft, 2025. *ASP.NET Core MVC with Entity Framework Core - Tutorial*. [online] Available at: <https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro> [Accessed 20 November 2025].

QuestPDF, 2025. *QuestPDF - Getting Started*. [online] Available at: <https://www.questpdf.com/getting-started.html> [Accessed 21 November 2025].

Microsoft, 2025. *Session and state management in ASP.NET Core*. [online] Available at: <https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state> [Accessed 21 November 2025].

PROG6212 POE Part 3 = Complete ✅

</details>
