# PixelWorldsServer2
First ever Pixel Worlds Private Server (Can be validated by the date this repo was created),
[Also this is the second version of PWPS, refer to github.com/playingoDEERUX/PixelWorldsServer for V1 which was released way earlier and has less features than this]

Features:

- Multiplayer
- Breaking/Placing Blocks/Water/Gates + Damaging them
- Registration System
- Chatting
- Wearing Clothes
- Collecting/Collectables & Dropped items
- Worlds
- Broadcasting Global Message
- Saving/Loading players/worlds from SQLite & file (You will need 'maps' folder in location of server executable, world files are saved as .map)
- NOTE: The Player & World session managers keeps players in memory before flushing them to DB, even if a player goes offline - it takes some time for a player session to be unloaded.
- Shop working with almost all packs
- Basic Lock System with ownership
- Proper Ticking System and message handling just like in real PW so that the PW Client never dies or gets disconnected randomly
- Getting free items via /give and /find
- Proper TCP Server with Timeout Handling (within my own FeatherNet TCP wrapper)
- Easy management
- Respawning to Spawn Entrance
- Multithreading (server itself is single-threaded, but sending packets, player sessions and performing most operations on the server itself is thread-safe)
- Proper Admin Console (can type in commands into the console and do something, for example 'getinfo (username)' to also grab userID and additional infos about the user or e.g 'givegems (userid) (amount)')
- Logger Thread
- Integrated/Embedded Discord Bot
- Proper Session Management
- Proper message framing to handle packets beyond 64KB if ever needed (within my own FeatherNet TCP wrapper)
- Many more
- Stable: This server is able to handle hundreds of players and hasn't unexpectedly crashed or was ever frozen at all.

Fast & Reliable Code: Uses SQLite as backend for Database Management.
Have fun with this, I won't maintain this anymore since I've quit.

NOTE: **You need player.dat and items.txt in the same location as server executable to host this.**
This project uses .NET Core so it is crossplatform for both Windows & Linux.
