echo Copying ConeJSON files...
::xcopy /y "%1ConeJSON\compile.js" "%2"
::xcopy /y "%1ConeJSON\generateConfig.js" "%2"
::xcopy /y "%1ConeJSON\util.js" "%2"
::xcopy /y "%1ConeJSON\config.json5" "%2"