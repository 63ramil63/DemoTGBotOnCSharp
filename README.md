# DemoTGBotOnCSharp

An asynchronous Telegram bot built with **.NET / C#** that showcases backend development skills, data persistence, and cross-technology integration. 

## 🚀 Key Features

* **Asynchronous Request Handling:** Leverages the Telegram Bot API to process user updates and interactions concurrently without blocking the main execution thread.
* **Data Persistence:** Integrated with a **MySQL** database for user state management, secure data storage, and application workflow persistence.
* **File System Operations:** Implements structured server-side file management for storing and retrieving user-generated assets.
* **Mini-Game Integration:** Connects core C# backend logic with a lightweight client-side frontend built on **HTML5 & JavaScript**, delivering a seamless hybrid application experience.

## 🛠️ Tech Stack

* **Language:** C#
* **Framework:** .NET Core / .NET 8.0+
* **Database:** MySQL
* **API:** Telegram Bot API (Telegram.Bot)
* **Frontend Components:** HTML5, JavaScript

## 📂 Project Structure

* `DemoTGBotOnCSharp/` — Core backend architecture, command routing, and API interaction handlers.
* `Game/` — Frontend web assets (HTML + JS) for the built-in mini-game interface.

## ⚙️ Requirements 

1. Create .env file or open ConfigExample.env
2. Write next variables ->

   ﻿TG_BOT_TOKEN=bot_token
   
   DB_CONNECTION_STRING=server=localhost;Database=your_database;Uid=root;Pwd=your_pass;
   
   ALLOWED_EXTENSIONS=word,txt
   
   GITHUB_PAGE=https://your_nickname.github.io/repository_name/folder_with_html_game_file 
