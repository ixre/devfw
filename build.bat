@echo off



color 66



echo =======================================



echo = 代码合并工具 github.com/atnet/devfw =



echo =======================================



set dir=%~dp0

set megdir=%dir%\dll\



if exist "%megdir%merge.exe" (

  

  echo 生成中,请稍等...
  
  cd %dir%dist\dll\



echo  /keyfile:%dir%ops.cms.snk>nul



"%megdir%merge.exe" /closed /log:%dir%dist\build_log.txt /ndebug /targetplatform:v4 /target:dll /out:%dir%dist\atnet.devfw.dll^
 AtNet.DevFw.Core.dll AtNet.DevFw.PluginKernel.dll AtNet.DevFw.Data.dll AtNet.DevFw.Template.dll AtNet.DevFw.Web.dll AtNet.DevFw.Toolkit.Data.dll
  


  

  echo 完成!输出到:%dir%dist\atnet.devfw.dll



)




pause
