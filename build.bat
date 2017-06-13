@echo off

color 66

echo =======================================

echo = 代码合并工具 github.com/jsix/devfw =

echo =======================================

set dir=%~dp0

set megdir=%dir%\dll\

if exist "%megdir%merge.exe" (

  echo 生成中,请稍等...
  cd %dir%dist\dll\

echo  /keyfile:%dir%j6.devfw.snk>nul

"%megdir%merge.exe" /closed /keyfile:%dir%/src/core/J6.DevFw.Core/j6.devfw.snk /ndebug /targetplatform:v4 /target:dll /out:%dir%dist\jrdev.dll^
 JR.DevFw.Core.dll JR.DevFw.PluginKernel.dll JR.DevFw.Data.dll JR.DevFw.Template.dll JR.DevFw.Web.dll JR.DevFw.Toolkit.Data.dll
  


  echo 完成!输出到:%dir%dist\jrdev.dll

)


pause