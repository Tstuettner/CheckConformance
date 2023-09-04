echo off

del /F/S/Q *.user

FOR /D /R %%X IN ("My Project"*) DO RD /S /Q "%%X"

FOR /F "delims=*" %%G IN ('DIR /B /AD /S .\*bin') DO rmdir /S/Q "%%G"
FOR /F "delims=*" %%G IN ('DIR /B /AD /S .\*obj') DO rmdir /S/Q "%%G"
FOR /F "delims=*" %%G IN ('DIR /B /AD /S .\*x64') DO rmdir /S/Q "%%G"

