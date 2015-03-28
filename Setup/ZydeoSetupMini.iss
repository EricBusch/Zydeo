; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Zydeo Chinese dictionary"
#define MyAppNameShort "Zydeo"
#define MyAppVersion "1.0"
#define MyAppPublisher "G�bor L Ugray"
#define MyAppURL "http://zydeo.net/"
#define MyAppExeName "Zydeo.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{46634FB3-D868-44F3-A55C-80852B9FC5AA}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppNameShort}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
Compression=lzma/normal
SolidCompression=yes
ShowLanguageDialog=no
DirExistsWarning=no
OutputDir=..\_install
OutputBaseFilename=ZydeoSetup-v{#MyAppVersion}
ArchitecturesInstallIn64BitMode=x64
FlatComponentsList=False
AlwaysShowComponentsList=False
ShowComponentSizes=False
DisableReadyPage=True
DisableReadyMemo=True
SetupIconFile=ZydeoSetup.ico
UninstallDisplayIcon={app}\ZydeoSetup.ico
WizardImageBackColor=clWhite
WizardImageFile=installer1.bmp
WizardSmallImageFile=installer2.bmp
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription={#MyAppName} setup
VersionInfoCopyright=(C) {#MyAppPublisher} 2015
VersionInfoVersion={#MyAppVersion}.0.0

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "..\_bin\Zydeo.exe"; DestDir: "{app}"; Flags: ignoreversion

Source: "ZydeoSetup.ico"; DestDir: "{app}"; Flags: ignoreversion


[Icons]
Name: "{commonstartmenu}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppNameShort}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
