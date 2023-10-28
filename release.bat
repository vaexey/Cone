echo Release Cone + ConeEngine...
echo Clean...
rmdir /s /q "dist"
rmdir /s /q "Cone\bin\x64\Release"
mkdir dist
echo Build...
msbuild Cone.sln /property:Configuration=Release /property:Platform=x64
echo Copying...
xcopy "Cone\bin\x64\Release\net6.0" "dist" /h /i /c /k /e /r /y

echo Release ConeJSON
cd ConeJSON
call ncc build src/generateConfig.js -o dist
echo Copying...
mkdir ".\..\dist\compiler"
copy ".\dist\index.js" ".\..\dist\compiler"
cd ..
echo Cone release finished.