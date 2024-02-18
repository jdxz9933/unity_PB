@echo off
cd %~dp0
set ROOT_PATH=%~dp0
set WORKSPACE=..
echo =================start gen proto code=================
set pb_path=proto
set out_path=../Assets/ProtoGeneratedC#
del /f /s /q %out_path%\*.*
for /f "delims=" %%i in ('dir /b %pb_path%') do (
echo ------------%%i start gen
protoc -I=proto --csharp_out=%out_path% proto\%%i
echo ------------%%i gen success
)
REM echo =================end gen proto code=================
REM set GEN_PROTOBUFRESOLVER=%WORKSPACE%\Tools\ProtobufResolver\ProtobufResolver.exe
REM set INPUT_DATA_DIR=%ROOT_PATH%pb_message
REM set OUTEVENTPATH=%WORKSPACE%/../Assets/Deer/Scripts/HotFix/HotFixCommon/Definition/Constant
REM %GEN_PROTOBUFRESOLVER% --input_data_dir %INPUT_DATA_DIR% --output_proto_dir %OUTEVENTPATH%
REM echo =================end gen proto event=================
pause