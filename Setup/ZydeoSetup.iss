#define MyAppName "Zydeo Chinese dictionary"
#define MyAppNameShort "Zydeo"
#define MyAppVersion "1.0"
#define MyAppPublisher "G�bor L Ugray"
#define MyAppURL "http://zydeo.net/"
#define MyAppExeName "Zydeo.exe"

[Setup]
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
Source: "..\_bin\ZD.AU.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_bin\ZD.CedictEngine.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_bin\ZD.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_bin\ZD.Gui.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_bin\ZD.Gui.Zen.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_bin\ZD.HanziLookup.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_bin\ZD.Texts.dll"; DestDir: "{app}"; Flags: ignoreversion

Source: "..\_bin\ukaitw.ttf"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_bin\hdzb_75.ttf"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_bin\Ubuntu-Bold.ttf"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_bin\Neuton-Regular.ttf"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_bin\cedict-zydeo.bin"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\_bin\strokes-zydeo.bin"; DestDir: "{app}"; Flags: ignoreversion

Source: "..\License.html"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\License-CC-BY-SA-3.0.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\License-GNU-GPL-3.0.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\License-ArphicPublicLicense.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\License-SIL-OpenFont-1.1.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\License-UbuntuFont-1.0.txt"; DestDir: "{app}"; Flags: ignoreversion

Source: "ZydeoSetup.ico"; DestDir: "{app}"; Flags: ignoreversion


[Icons]
Name: "{commonstartmenu}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppNameShort}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: {app}\ZD.AU.exe; Parameters: /install; WorkingDir: {app}; Flags: runascurrentuser
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallRun]
Filename: {app}\ZD.AU.exe; Parameters: /uninstall; WorkingDir: {app}; Flags: runascurrentuser

[CustomMessages]
english.DotNetAdmin=Zydeo needs Microsoft .NET Framework v4.0 to be installed by an Administrator
english.Downloading=Downloading Microsoft .NET Framework v4.0
english.DownloadDetail=Zydeo needs to install the Microsoft .NET Framework v4.0. Please wait while Setup is downloading extra files to your computer.
english.DotNetName=.NET Framework v4.0
english.ToInstall=Dependencies to install:

[Code]
var
  dotnetRedistPath: string;
  downloadNeeded: boolean;
  dotNetNeeded: boolean;
  memoDependenciesNeeded: string;

procedure isxdl_AddFile(URL, Filename: AnsiString);
external 'isxdl_AddFile@files:isxdl.dll stdcall';
function isxdl_DownloadFiles(hWnd: Integer): Integer;
external 'isxdl_DownloadFiles@files:isxdl.dll stdcall';
function isxdl_SetOption(Option, Value: AnsiString): Integer;
external 'isxdl_SetOption@files:isxdl.dll stdcall';

const
  dotnetRedistURL = 'http://download.microsoft.com/download/1/B/E/1BE39E79-7E39-46A3-96FF-047F95396215/dotNetFx40_Full_setup.exe';

function InitializeSetup(): Boolean;
var
  Installed_40 : Cardinal;
  Installed_45: Cardinal;
  Version: String;
  QuerySuccess: Boolean;
begin
  Result := true;
  dotNetNeeded := false;

  // Check for .NET 4.0 or 4.5 full profile
  RegQueryDWordValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Install', Installed_40);
  Installed_45 := 0;

  if (Installed_40 <> 0) then
  begin
    QuerySuccess := RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Version', Version);
    if not QuerySuccess then  // version unavailable
      Installed_40 := 0;
    if QuerySuccess and (Version[3] = '5') then begin // 4.5
      Installed_40 := 0;
      Installed_45 := 1;
    end;
  end;

  if (not ((Installed_45 <> 0) or (Installed_40 <> 0))) then begin
    dotNetNeeded := true;
    if (not IsAdminLoggedOn()) then begin
      MsgBox(CustomMessage('DotNetAdmin'), mbInformation, MB_OK);
      Result := false;
    end else begin
      memoDependenciesNeeded := memoDependenciesNeeded + '      ' + CustomMessage('DotNetName') + '' #13;
      dotnetRedistPath := ExpandConstant('{src}\dotNetFx40_Full_x86_x64.exe.exe');
      if not FileExists(dotnetRedistPath) then begin
        dotnetRedistPath := ExpandConstant('{tmp}\dotNetFx40_Full_x86_x64.exe.exe');
        if not FileExists(dotnetRedistPath) then begin
          isxdl_AddFile(dotnetRedistURL, dotnetRedistPath);
          downloadNeeded := true;
        end;
      end;
      SetIniString('install', 'dotnetRedist', dotnetRedistPath, ExpandConstant('{tmp}\dep.ini'));
    end;
  end;
end;


function NextButtonClick(CurPage: Integer): Boolean;
var
  hWnd: Integer;
  ResultCode: Integer;
  LangFileName: string;

begin
  Result := true;
  
  if CurPage = wpWelcome then begin
    hWnd := StrToInt(ExpandConstant('{wizardhwnd}'));
    // don't try to init isxdl if it's not needed because it will error on < ie 3
    if downloadNeeded then begin
  	  // Download dialog localized
  	  LangFileName := 'isxdl_' + ExpandConstant('{language}') + '.ini';
      ExtractTemporaryFile(LangFileName);
      isxdl_SetOption('language', ExpandConstant('{tmp}\') + LangFileName);
      isxdl_SetOption('label', CustomMessage('Downloading'));
      isxdl_SetOption('description', CustomMessage('DownloadDetail'));
      if isxdl_DownloadFiles(hWnd) = 0 then Result := false;
    end;
    if (Result = true) and (dotNetNeeded = true) then begin
      if Exec(ExpandConstant(dotnetRedistPath), '', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then begin
         // handle success if necessary; ResultCode contains the exit code
         if not (ResultCode = 0) then begin
           Result := false;
         end;
      end else begin
         // handle failure if necessary; ResultCode contains the error code
         Result := false;
      end;
    end;
  end;
end;


function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  s: string;

begin
  if memoDependenciesNeeded <> '' then s := s + CustomMessage('ToInstall') + NewLine + memoDependenciesNeeded + NewLine;
  s := s + MemoDirInfo + NewLine + NewLine;

  Result := s
end;

