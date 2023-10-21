echo Copying ConeJSON files into main Cone project directory
xcopy /y .\compile.js .\..\Cone\bin\x64\Debug\net6.0\compiler
xcopy /y .\util.js .\..\Cone\bin\x64\Debug\net6.0\compiler
xcopy /y .\generateConfig.js .\..\Cone\bin\x64\Debug\net6.0\compiler
xcopy /y .\package.json .\..\Cone\bin\x64\Debug\net6.0\compiler
set PREV_DIR=%CD%
cd .\..\Cone\bin\x64\Debug\net6.0\compiler
npm install
cd %PREV_DIR%
echo Done.