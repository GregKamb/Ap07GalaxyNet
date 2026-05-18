# 🚀 Chirps (GalaxyNet) - ASP.NET Core MVC Application

## 📝 Project Overview
This is a full-stack web application developed as part of my professional training (Lehrabschluss - Applikationsentwicklung) in Vienna. It demonstrates a modern approach to web development, highlighting my ability to design scalable backend architectures and integrate them with dynamic frontends.


## 🌐 Live Demo
You can test the application live here: [Greg-GalaxyNet on Azure](https://greg-galaxynet-age6d0hshmdueghb.westeurope-01.azurewebsites.net)

## 🛠️ Technologies Used

* **Cloud Infrastructure:** Deployed on **Microsoft Azure** (Azure App Service)
* **File Storage:** **Azure Blob Storage** (for fast, scalable profile picture uploads)
* **Database:** Entity Framework Core with **Azure SQL Database**
* **Backend:** C# / .NET (ASP.NET Core MVC)
* **Authentication:** Custom Cookie-based Auth (Hash & Salt security, no external libraries)
* **Frontend:** HTML5, CSS3, Bootstrap 5 (Responsive UI)

## ✨ Key Features
* **Custom Security:** A hand-built authentication system focusing on security. Passwords are never stored in plain text; they are hashed and salted.
* **Dynamic Peep-Words:** An automated extraction system that finds keywords in posts and tracks their popularity.
* **Smart Filtering:** Users can filter the global feed by clicking on trending "Peep-words" in the sidebar.
* **Interaction System:** Like/Unlike functionality with real-time counter updates.
* **Auto-Seeding:** The application automatically creates the database and populates it with sample data on the first run, making testing effortless.

## 🎮 Try It Yourself (Interactive Features)
I highly recommend testing the custom **Peep-Word** extraction logic! 
1. Log in to the application.
2. Click on **Neuer Beitrag** (New Post).
3. In your content, use the **`<`** symbol before a word to tag it (word must be between 5 and 20 characters). 
   - *Example: "Exploring the new <Nebula today!"*
4. Submit the post. You will instantly see the word magically extracted and appear in the "Letzte Peep-Wörter" sidebar as a clickable filter!

## 🚀 How to Run
1. Clone the repository.
2. Open the solution in **Visual Studio 2022**.
3. Press **F5**. The database will be created and seeded automatically.
4. **Test Credentials (Auto-generated):**
   - **Username:** `TestExplorer`
   - **Password:** `Password123!`
