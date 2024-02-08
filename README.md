##### The below readme was written with some AI translations.
##### English: [README.md(English)](https://github.com/minttea25/Chat?tab=readme-ov-file#english)

####  This client (ChatClient) **is not** intended **for** commercial purposes or distribution.

 ---
 ##### Korean

# Chat
채팅 프로그램과 이에 대응하는 서버 리포지토리 입니다. 다음 기본 기능을 포함합니다. 자세한 기능은 `ChatClient`에서 확인해주세요.
- 로그인/회원가입
- 채팅방 생성/입장
- 채팅방 나가기
- 텍스트/이모티콘 메시지 전송

---

# ChatServer
`ChatClient`에 대응하는 채팅 서버입니다. [ServerCoreTCP](https://github.com/minttea25/TCPServerCore)를 사용하였습니다.

## Target Framework 
`.NET 6.0` at `AccountServer`, `ChatServer` and `ChatSharedDb`.

## Common
- 데이터베이스는 기본적으로 `EntityFramework Core`로 관리됩니다.
- 데이터베이스 migration 내용은 포함되지 않습니다.
- `UserToken`은 코드 상에서 해시를 이용한 `HMACSHA256`을 사용하여 간단하게 특정 정보에 대한 토큰을 생성합니다. `Key`는 파일 내에 하드코딩되어 있습니다.
- `ChatClient`에서 비밀번호에 대한 암호화는 `AES` 암호화를 사용하고 있습니다. 이에 대한 `Key`와 `IV`값 또한 파일 내에 하드코딩되어 있습니다.

## Common Directory
서버와 클라이언트에서 공통으로 사용할 proto 파일과 이에 대한 `Google.Protobuf`의 출력 코드와 `ServerCoreTCP`의 `MessageWrapperFactory`의 간편 실행을 포함하고 있습니다. 또한 메시지(패킷)의 정의가 포함되어 있는 파일(`Message Definition.xlsx`)이 있습니다.
* `protoc.exe` 및 실행 시 필요한 파일들은 포함되어 있지 않습니다. 다음을 참고해주세요: [Protobuf 3.21.12](https://github.com/protocolbuffers/protobuf/releases/tag/v3.21.12)

## ChatServer
채팅 서버의 로직을 담당하는 메인 코드입니다. `AccountServer`로 부터 토큰을 발급 받은 후에 이 서버에 연결 할 수 있습니다.

### Features
- Processing : 크게 4개의 기본 스레드와 `Socket`의 IOCP로 실행됩니다.
    - `Main`: 기본 명령어 처리 및 서비스 실행
    - `Room`: 채팅방 관리
    - `Session`: 세션 관리와 세션을 통한 데이터 송수신 관리
    - `Db`: 데이터 베이스 업데이트 처리
    - `IOCP`: `Socket`의 비동기 이벤트 처리
- Sequence Diagrams: [Common/Sequence Diagrams](https://github.com/minttea25/Chat/tree/main/Common/Sequence%20Diagrams)에 주요 처리 시퀀스의 시각화 이미지가 포함되어 있습니다. 이미지는 [PlantUML](https://plantuml.com/)로 생성하였습니다.
- `Traffic Test`: 트래픽 테스트용 브랜치는 `Traffic-Test` 브랜치에 따로 있습니다. 로그인 과정을 생략하고, DummyClient를 이용해 서버 연결 및 채팅방 입장 및 퇴장, 메시지 전송으로 테스트합니다.

### Usage
- `config.json`: 서버 설정 값을 나타내는 파일입니다.
    - `DBConnectionString`: 메인 데이터베이스 ConnectionString을 나타냅니다.
    - `SharedDbConnectionString`: 공유 데이터베이스 ConnectionString을 나타냅니다.
    - `SessionPoolCount`: 세션 풀의 개수를 나타내며, 연결 허용할 클라이언트의 최대 개수를 의미하기도 합니다.
    - `ReuseAddress`: 소켓의 `ReuseAddress` 옵션을 나타냅니다.
    - `RegisterListenCount`: 한번에 listen할 개수를 나타냅니다.
    - `ListenerBacklogCount`: `Listener`의 `backlog` 값을 나타냅니다.
    - `Port`: 서버를 열 포트를 나타냅니다.
    - `HostEntryIndex`: 서버 IP주소를 가져올때의 인덱스를 의미합니다.
    - `LogFile`: 파일 로깅 여부를 나타냅니다.
    - `LogConsole`: 콘솔 로깅 여부를 나타냅니다.
    - `LogDebug`: IDE의 디버그 로깅 여부를 나타냅니다.
- 실행 시 명령어는 다음과 같습니다.
    - `stop`: 서비스를 정지하고 프로그램을 종료합니다.
    - `config`: 현재 적용되고 있는 설정 값을 표시합니다.
    - `pool_count`: 현재 서비스의 풀에 대한 `SocketAsyncEventArgs`와 `Session`개수를 표시합니다.


### External Libraries
- [ServerCoreTCP](https://github.com/minttea25/TCPServerCore)
- [Google.Protobuf](https://protobuf.dev/)
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [Serilog](https://serilog.net/)
- Serilog.Sinks.Console
- Serilog.Sinks.Debug
- Serilog.Sinks.File




## AccountServer
`ASP.Net`을 이용한 간단한 로그인 **웹 서버** 입니다. 클라이언트는 이 서버에 접속하여 로그인과 가입을 할 수 있고 토큰을 발급 받을 수 있습니다. 

### Features
- `AccountController`: **계정 로그인**과 **회원가입**을 담당하는 컨트롤러 입니다.
- `WebPackets`: `https`의 body 부분에 사용될 직렬화 가능한 클래스 정보를 포함합니다. 클라이언트 또한 이것과 유사한 코드를 포함하고 있습니다.

### Usage
- `appsettings.json`: 기본 데이터베이스와 공유 데이터베이스 연결 문자열은 `appsettings.json`에서 확인할 수 있습니다.
- `launchSettings.json`: 실행 기본 설정은 `launchSettings.json`에서 확인할 수 있고 수정할 수 있습니다. (ex - ip address, port ...)

### API (Route)
- `api/accountcontroller/login`: 로그인 요청
- `api/accountcontroller/register`: 회원가입 요청

### External Libraries
- Microsoft.EntityFrameworkCore]
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools



## ChatSharedDb
`ChatServer`와 `AccountServer`가 공용으로 사용하는 데이터베이스 프로젝트입니다. 클라이언트의 `AuthToken`을 관리합니다.

### External Libraries
- Microsoft.EntityFrameworkCore]
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools



---


# ChatClient
**유니티**로 제작된 채팅 어플리케이션 입니다. 마찬가지로 [ServerCoreTCP](https://github.com/minttea25/TCPServerCore)를 사용하였습니다.  [MyUnityPackage](https://github.com/minttea25/MyUnityLibrary)를 기반으로 하여 만들어졌습니다.

## Unity Version
`2021.3.15f1 (LTS)`

## Functions
- **계정 로그인/생성**: 계정 아이디와 비밀번호를 이용해 가입/로그인을 할 수 있습니다.
- **채팅방 생성**: 채팅방 생성 아이콘을 클리하고 채팅방 번호를 입력해 채팅방을 생성하고 참여할 수 있습니다.
- **채팅방 입장**: 채팅방 입장 아이콘을 클릭하고 이미 생성되어 있는 채팅방에 입장할 수 있습니다.
- **채팅방 나가기**: 채팅 방 UI를 길게 클릭하여 채팅방을 나갈 수 있습니다.
- **채팅 전송**: 텍스트를 입력하여 채팅방에 있는 다른 유저에게 채팅을 전송할 수 있습니다.
- **이모티콘 전송**: 이모티콘을 클릭하여 채팅방에 있는 다른 유저에게 이코티콘을 전송할 수 있습니다.
- **유저 이름 변경**: 정보 아이콘을 클릭하여 유저 이름을 변경할 수 있습니다.

## Features
- 입장한 채팅방은 어플리케이션을 종료해도 서버에 데이터가 유지됩니다.
- 채팅 기록은 서버에 유지되지만, 클라이언트에서는 유지되지 않습니다. 어플리케이션 종료 시, 채팅 내용은 사라질 것 입니다.
- 좌측에 서버와의 지연시간(ping)이 표시됩니다.
- 열려있지 않은 채팅방에 대해 아직 읽지 않은 메시지 개수가 표시됩니다.

## External Plugins
Check details: [Dependencies](https://github.com/minttea25/Chat/blob/main/ChatClient/Assets/Plugins/Versions.xml)
- [ServerCoreTCP](https://github.com/minttea25/TCPServerCore)
- Google Protobuf (3.21.12)
- Serilog (3.1.1)
- Compiled files(.dll) that depend on some plugins

## Additional Unity Packages
- TextMeshPro
- Addressables

## Used Resources
- Free Icons: [Flaticon](https://www.flaticon.com/)
- Some UI Images: **Self-made**, [references](https://github.com/minttea25/Chat/blob/main/ChatClient/Resources.txt)
- Font: [Maplestory Font](https://maplestory.nexon.com/Media/Font)

---

##### English



# Chat
This repository corresponds to the chat program and its server. It includes the following basic features. For detailed functionality, please refer to ChatClient.
- Login/Sign up
- Create/Join chat rooms
- Leave chat rooms
- Send text/emoticon messages

---

# ChatServer
This is the chat server corresponding to `ChatClient`, using [ServerCoreTCP](https://github.com/minttea25/TCPServerCore).

## Target Framework 
`.NET 6.0` at `AccountServer`, `ChatServer` and `ChatSharedDb`.

## Common
- The database is managed using `EntityFramework Core`.
- Database migration details are not included.
- `UserToken` is a token generated using simple hashing with `HMACSHA256` for specific information in the code. The Key is hardcoded in the file.
- Encryption for passwords in `ChatClient` is done using `AES` encryption. The `Key` and `IV` values for this encryption are also hardcoded in the file.

## Common Directory
Contains proto files and their corresponding `Google.Protobuf` output code, along with convenient execution of `ServerCoreTCP.MessageWrapperFactory`, to be used both by the server and client. Also includes a file (`Message Definition.xlsx`) containing message (packet) definitions.
* `protoc.exe` and other necessary files for execution are not included. Please refer to: [Protobuf 3.21.12](https://github.com/protocolbuffers/protobuf/releases/tag/v3.21.12)

## ChatServer
This is the main code responsible for the logic of the chat server. After receiving a `AuthToken` from the `AccountServer`, clients can connect to this server.

### Features
- Processing: It consists of 4 main threads and IOCP for Socket handling.
    - `Main`: Basic command processing and service execution.
    - `Room`: Chat room management.
    - `Session`: Session management and data transmission/reception through sessions.
    - `Db`: Database update processing.
    - `IOCP`: Asynchronous event handling for `Socket`.
- Sequence Diagrams: Visualization images of key processing sequences are included in [Common/Sequence Diagrams](https://github.com/minttea25/Chat/tree/main/Common/Sequence%20Diagrams). These images were generated using [PlantUML](https://plantuml.com/).
- `Traffic Test`: There is a separate branch named `Traffic-Test` for traffic testing. It skips the login process and tests server connection, entering and leaving chat rooms, and message transmission using `DummyClient`.

### Usage
- `config.json`: This file represents server configuration values.
    - `DBConnectionString`: Represents the main database ConnectionString.
    - `SharedDbConnectionString`: Represents the shared database ConnectionString.
    - `SessionPoolCount`: Represents the number of session pools, also indicating the maximum number of clients allowed to connect.
    - `ReuseAddress`: Represents the `ReuseAddress` option of the socket.
    - `RegisterListenCount`: Represents the number of connections to listen to at once.
    - `ListenerBacklogCount`: Represents the `backlog` value of the `Listener`.
    - `Port`: Represents the `port` to open for the server.
    - `HostEntryIndex`: Represents the index when retrieving the server IP address.
    - `LogFile`: Indicates whether file logging is enabled.
    - `LogConsole`: Indicates whether console logging is enabled.
    - `LogDebug`: Indicates whether IDE debug logging is enabled.
- Command line options during execution are as follows:
    - `stop`: Stops the service and exits the program.
    - `config`: Displays the currently applied configuration values.
    - `pool_count`: Displays the number of `SocketAsyncEventArgs` and `Session` in the current service pool.


### External Libraries
- [ServerCoreTCP](https://github.com/minttea25/TCPServerCore)
- [Google.Protobuf](https://protobuf.dev/)
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [Serilog](https://serilog.net/)
- Serilog.Sinks.Console
- Serilog.Sinks.Debug
- Serilog.Sinks.File




## AccountServer
This is a simple `web server` for login using `ASP.Net`. Clients can connect to this server to perform login and registration and receive `AuthToken`. 

### Features
- `AccountController`: Controller responsible for **account login** and **registration**.
- `WebPackets`: Contains serializable class information used in the body part of `https`. The client also includes similar code.

### Usage
- `appsettings.json`: Connection String of default database and shared database can be found in `appsettings.json`.
- `launchSettings.json`: Default execution settings can be found and modified in `launchSettings.json`. (e.g., IP address, port)

### API (Route)
- `api/accountcontroller/login`: Login request.
- `api/accountcontroller/register`: Registration request.

### External Libraries
- Microsoft.EntityFrameworkCore]
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools



## ChatSharedDb
This is the database project shared by `ChatServer` and `AccountServer`. It manages the `AuthToken` of clients.

### External Libraries
- Microsoft.EntityFrameworkCore]
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools

---


# ChatClient
This is a chat application developed using **Unity**, also utilizing [ServerCoreTCP](https://github.com/minttea25/TCPServerCore). It is based on [MyUnityPackage](https://github.com/minttea25/MyUnityLibrary).

## Unity Version
`2021.3.15f1 (LTS)`

## Functions
- **Account Login/Creation**: Users can sign up or log in using their account ID and password.
- **Chat Room Creation**: Users can create chat rooms by clicking the create chat room icon and entering the room number.
- **Joining Chat Rooms**: Users can join existing chat rooms by clicking the join chat room icon.
- **Leaving Chat Rooms**: Users can leave chat rooms by long-clicking on the chat room UI.
- **Sending Text Messages**: Users can send text messages to other users in the chat room.
- **Sending Emoticons**: Users can send emoticons to other users in the chat room by clicking on them.
- **Changing Usernames**: Users can change their usernames by clicking on the information icon.

## Features
- Chat rooms that users join will maintain data on the server even after the application is closed.
- Chat history is maintained on the server but not on the client. Chat content will disappear when the application is closed.
- The delay time (ping) with the server is displayed on the left side.
- The number of unread messages for closed chat rooms is displayed.

## External Plugins
Check details: [Dependencies](https://github.com/minttea25/Chat/blob/main/ChatClient/Assets/Plugins/Versions.xml)
- [ServerCoreTCP](https://github.com/minttea25/TCPServerCore)
- Google Protobuf (3.21.12)
- Serilog (3.1.1)
- Compiled files(.dll) that depend on some plugins

## Additional Unity Packages
- TextMeshPro
- Addressables

## Used Resources
- Free Icons: [Flaticon](https://www.flaticon.com/)
- Some UI Images: **Self-made**, [references](https://github.com/minttea25/Chat/blob/main/ChatClient/Resources.txt)
- Font: [Maplestory Font](https://maplestory.nexon.com/Media/Font)





