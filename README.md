# 🎮 Real-Time Game Chat System

A real-time game chat app built in **C# .NET** and **Windows Forms**. It uses asynchronous, multi-threaded TCP/IP communication to ensure a fast, lag-free player experience.

## ✨ Features
* ⚡ **Lag-Free UI:** Uses `async/await` so the chat window never freezes during network delays.
* 🧵 **Multi-Threaded:** Sends and receives messages instantly on a separate background track.
* 📦 **Modular Design:** Keeps network logic packaged in a reusable library separate from the screen.
* 🛠️ **Reliable:** Gracefully handles sudden drops or disconnections without crashing.

## 🛠️ Tech Stack
* **Language/Framework:** C# & .NET Framework
* **Environment:** Visual Studio
* **UI:** Windows Forms

## 🚀 How It Works
1. **Architecture:** Built an independent library to isolate network logic from the visual screen.
2. **Connection:** Connected the UI and upgraded the system to run asynchronously.
3. **Thread Safety:** Added background listeners and safe updates to keep the screen smooth.
