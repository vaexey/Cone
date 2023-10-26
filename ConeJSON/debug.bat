@REM echo Copying ConeJSON files into main Cone project directory
@REM xcopy /y .\compile.js .\..\Cone\bin\x64\Debug\net6.0\compiler
@REM xcopy /y .\util.js .\..\Cone\bin\x64\Debug\net6.0\compiler
@REM xcopy /y .\generateConfig.js .\..\Cone\bin\x64\Debug\net6.0\compiler
@REM xcopy /y .\package.json .\..\Cone\bin\x64\Debug\net6.0\compiler
@REM cp ".\package.json" ".\..\Cone\bin\x64\Debug\net6.0\compiler"
@REM set PREV_DIR=%CD%
@REM cd .\..\Cone\bin\x64\Debug\net6.0\compiler
@REM npm install
@REM cd %PREV_DIR%

echo Generating ncc...
call ncc build src/generateConfig.js -o dist
echo Copying dist into debug folder...
cp ".\dist\index.js" ".\..\Cone\bin\x64\Debug\net6.0\compiler"
echo Done.