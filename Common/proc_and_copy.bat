"SimpleProtoc/SimpleProtoc.exe"
pause

"MessageWrapperFactory/MessageWrapperFactory.exe"
pause

XCOPY /Y "server\MessageManager.cs" "../ChatServer/ChatServer/Chat"
XCOPY /Y "server\MessageTypes.cs" "../ChatServer/ChatServer/Chat"
XCOPY /Y "Chat/." "../ChatServer/ChatServer/Chat/proto"
XCOPY /Y "C#/." "../ChatServer/ChatServer/Chat/Messages"

XCOPY /Y "client\MessageManager.cs" "../ChatClient/Assets/Scripts/Network"
XCOPY /Y "client\MessageTypes.cs" "../ChatClient/Assets/Scripts/Network"
XCOPY /Y "Chat/." "../ChatClient/Assets/Scripts/Messages" 
XCOPY /Y "C#/." "../ChatClient/Assets/Scripts/Messages" 

pause


