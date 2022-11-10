# PixelWorldsServer2
First ever Pixel Worlds Private Server made in C# (Can be validated by the date this repo was created)

Features:

- Multiplayer
- Breaking/Placing Blocks/Water/Gates
- Registration System
- Chatting
- Wearing Clothes
- Collecting
- Worlds
- Broadcasting Message
- Shop working with almost all packs
- Basic Lock System
- Proper Ticking System and message handling just like in real PW so that the PW Client never dies or gets disconnected randomly
- Getting free items
- Proper TCP Server with Timeout Handling (within my own FeatherNet TCP wrapper)
- Easy management
- Proper Session Management
- Proper message framing to handle packets beyond 64KB if ever needed (within my own FeatherNet TCP wrapper)
- Many more

Fast & Reliable Code: Uses SQLite as backend for Database Management.
Have fun with this, I won't maintain this anymore since I've quit.

NOTE: **You need player.dat and items.txt in the same location as server executable to host this.**
This project uses .NET Core so it is crossplatform for both Windows & Linux.
