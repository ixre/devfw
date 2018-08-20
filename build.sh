#!/usr/bin/env sh


echo =======================================

echo = project url: github.com/jsix/devfw =

echo =======================================



dir=$(pwd)
target="${dir}/dist/jrdev.dll"
cd dist/dll

echo "生成到:${dir}/dist/jrdev.dll"
	
../../dll/merge.exe -closed -ndebug \
	 /targetplatform:v4 /target:dll /out:../jrdev.dll \
 	 JR.DevFw.Core.dll JR.DevFw.PluginKernel.dll JR.DevFw.Data.dll \
	 JR.DevFw.Template.dll JR.DevFw.Web.dll JR.DevFw.Toolkit.Data.dll
  





